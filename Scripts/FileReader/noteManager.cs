using UnityEngine;
using System.Collections.Generic;
using System;
using GameMain;

namespace vvproj
{
  public class NoteForRequestToEngine
  {
    public int? key;
    public int frameLength;
    public string lyric;
    public NoteForRequestToEngine(int? Key, int FrameLength, string Lyric)
    {
      key = Key;
      frameLength = FrameLength;
      lyric = Lyric;
    }
    public string ToJson()
    {
      string keyStr = key == null ? "null" : key.ToString();
      return $"{{\"key\":{keyStr},\"frame_length\":{frameLength},\"lyric\":\"{lyric}\"}}";
    }
  }
  public class NoteManager
  {
    private static float frameRate = 93.75f;// フレームレート。現時点のVoicevoxの仕様では変えられないはず

    public static string FetchQuery(
        vvprojNoteType[] notes,
        vvprojTempoType[] tempos,
        int tpqn,
        int keyRangeAdjustment,
        float restDurationSeconds)
    {
      if (notes.Length == 0)
      {
        return "{\"notes\":[]}";
      }

      // ソート
      Array.Sort(notes);
      List<NoteForRequestToEngine> notesForRequestToEngine = new List<NoteForRequestToEngine>();

      // ノートを変換
      float headNoteDurationSeconds = TickToSecond(notes[0].position, tempos, tpqn);
      int headRestFrameLength = (int)Math.Round((restDurationSeconds + headNoteDurationSeconds) * frameRate);
      float firstNoteOnTime = TickToSecond(headRestFrameLength, tempos, tpqn);
      // 先頭に休符を追加
      notesForRequestToEngine.Add(new NoteForRequestToEngine(null, headRestFrameLength, ""));

      int frame = 0;
      int index = 0;
      foreach (vvprojNoteType note in notes)
      {
        // 休符
        float noteOnTime = TickToSecond(note.position, tempos, tpqn);
        int noteOnFrame = (int)Math.Round(noteOnTime * frameRate);
        if (noteOnFrame > frame)
        {
          if (index > 0)// index==0の時は初めに追加済
          {
            notesForRequestToEngine.Add(new NoteForRequestToEngine(null, noteOnFrame - frame, ""));
          }
          frame = noteOnFrame;
        }

        // 音符
        float noteOffTime = TickToSecond(note.position + note.duration, tempos, tpqn);
        int noteOffFrame = (int)Math.Round(noteOffTime * frameRate);
        int noteFrameLength = Math.Max(1, noteOffFrame - frame);

        string lyric = note.lyric
            .Replace("じょ", "ジョ")
            .Replace("うぉ", "ウォ")
            .Replace("は", "ハ")
            .Replace("へ", "ヘ");

        // トランスポーズする
        int? key = note.noteNumber - keyRangeAdjustment;
        notesForRequestToEngine.Add(new NoteForRequestToEngine(key, noteFrameLength, lyric));
        frame += noteFrameLength;
        index++;
      }

      // 末尾に休符を追加
      int restFrameLength = (int)Math.Round((restDurationSeconds) * frameRate);
      notesForRequestToEngine.Add(new NoteForRequestToEngine(null, restFrameLength, ""));

      string result = "";
      foreach (NoteForRequestToEngine noteForRequestToEngine in notesForRequestToEngine)
      {
        result += noteForRequestToEngine.ToJson() + ",";
      }
      // 最後の","を削る
      result = result.Remove(result.Length - 1);

      return "{\"notes\":[" + result + "]}";
    }

    public List<GameNote> makeGameNotes(
      vvprojNoteType[] notes,
      vvprojTempoType[] tempos,
      int tpqn,
      float restDurationSeconds
    )
    {
      Array.Sort(notes);
      List<GameNote> result = new List<GameNote>();

      short formerNoteDirection = -1;// 0~3
      foreach (vvprojNoteType note in notes)
      {
        // typeについて
        short type = (short)(note.noteNumber % 4);
        if (formerNoteDirection == type)
        {
          type = (short)((type + 1) % 4);
        }
        formerNoteDirection = type;

        // 開始と終了の時間について
        float noteOnTime = TickToSecond(note.position, tempos, tpqn) + restDurationSeconds;

        float noteOffTime;
        if (TickToSecond(note.duration, tempos, tpqn) > GameMain.Setting.minLongNoteSecond)// OffTime-OnTimeよりこっちの方が思っていた通りになると思う
        {
          // longNote化
          noteOffTime = TickToSecond(note.position + note.duration, tempos, tpqn) + restDurationSeconds;
          type += 10;
        }
        else{
          // shortNoteなら
          noteOffTime = noteOnTime;
        }

        // 追加
        if(note.lyric!="っ"&&note.lyric!="ッ")// 小文字の「っ」は息を止めるのに使われるらしい
        {
          result.Add(new GameNote(noteOnTime, noteOffTime, type));
        }
      }
      return result;
    }
    public List<GameNote> adjustGameNotes(// ノーツが被らないようにする
      List<GameNote> firstCharaNotes,
      List<GameNote> secondCharaNotes
    )
    {
      foreach (GameNote note in secondCharaNotes)
      {
        note.type += 20;
      }

      List<GameNote> notes = firstCharaNotes;
      notes.AddRange(secondCharaNotes);

      notes.Sort();

      GameNote lastLongNote = new GameNote(-1, -1, -1);
      GameNote lastShortNote = new GameNote(-1, -1, -1);
      foreach (GameNote note in notes)
      {
        while (
          (lastLongNote.EndTime >= note.StartTime && lastLongNote.type % 10 == note.type % 10) ||
          (lastShortNote.StartTime == note.StartTime && lastShortNote.type % 10 == note.type % 10))
        {
          short typeWithoutDirection = (short)(note.type - (note.type % 10));
          note.type = (short)((note.type + 1) % 4 + typeWithoutDirection);
        }
        if (note.type % 20 < 10) // shortNoteなら
        {
          lastShortNote = note;
        }
        else
        {
          lastLongNote = note;
        }
      }
      return notes;
    }

    public static float TickToSecond(int ticks, vvprojTempoType[] tempos, int tpqn)
    {
      float timeOfTempo = 0;
      vvprojTempoType tempo = tempos[tempos.Length - 1];

      for (int i = 0; i < tempos.Length; i++)
      {
        if (i == tempos.Length - 1)
          break;

        if (tempos[i + 1].position > ticks)
        {
          tempo = tempos[i];
          break;
        }

        timeOfTempo += TickToSecondForConstantBpm(tempos[i + 1].position - tempos[i].position, tempos[i].bpm, tpqn);
      }

      return timeOfTempo + TickToSecondForConstantBpm(ticks - tempo.position, tempo.bpm, tpqn);
    }

    public static float TickToSecondForConstantBpm(int ticks, float bpm, int tpqn)
    {
      float quarterNotesPerMinute = bpm;
      float quarterNotesPerSecond = quarterNotesPerMinute / 60;
      return ticks / (tpqn * quarterNotesPerSecond);
    }
  }
}
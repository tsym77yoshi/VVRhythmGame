using System;
using vvproj;
using UnityEngine;

namespace vvrojIsValid
{
  public class ScoreValidator
  {
    private static readonly int[] BEAT_TYPES = { 1, 2, 4, 8, 16 };

    private const int MIN_BPM = 30;

    public static bool IsValidTpqn(int tpqn)
    {
      return tpqn % 3 == 0 && Array.TrueForAll(BEAT_TYPES, value => tpqn % value == 0);
    }

    public static bool IsValidBpm(float bpm)
    {
      return !float.IsNaN(bpm) && bpm >= MIN_BPM;
    }

    public static bool IsValidTempo(vvprojTempoType tempo)
    {
      return tempo.position >= 0 && IsValidBpm(tempo.bpm);
    }

    public static bool IsValidBeats(int beats)
    {
      return beats >= 1;
    }

    public static bool IsValidBeatType(int beatType)
    {
      return Array.Exists(BEAT_TYPES, value => value == beatType);
    }

    public static bool IsValidTimeSignature(vvprojTimeSignaturesType timeSignature)
    {
      return timeSignature.measureNumber >= 1 && IsValidBeats(timeSignature.beats) && IsValidBeatType(timeSignature.beatType);
    }

    public static bool IsValidNote(vvprojNoteType note)
    {
      return note.position >= 0 && note.duration >= 1 && note.noteNumber >= 0 && note.noteNumber <= 127;
    }

    public static bool IsValidTempos(vvprojTempoType[] tempos)
    {
      return tempos.Length >= 1 && tempos[0].position == 0 && Array.TrueForAll(tempos, IsValidTempo);
    }

    public static bool IsValidTimeSignatures(vvprojTimeSignaturesType[] timeSignatures)
    {
      return timeSignatures.Length >= 1 && timeSignatures[0].measureNumber == 1 && Array.TrueForAll(timeSignatures, IsValidTimeSignature);
    }

    public static bool IsValidNotes(vvprojNoteType[] notes)
    {
      return Array.TrueForAll(notes, IsValidNote);
    }

    public static bool IsValidScore(vvprojSongType score)
    {
      return IsValidTpqn(score.tpqn) && IsValidTempos(score.tempos) && IsValidTimeSignatures(score.timeSignatures) && Array.TrueForAll(score.tracks, value => IsValidNotes(value.notes));
    }

    public static bool IsValidKeyRangeAdjustment(int keyRangeAdjustment){
      return -28 <= keyRangeAdjustment && keyRangeAdjustment <= 28;
    }
    public static bool IsValidVoiceShiftKey(int voiceShiftKey)
    {
      return -24 <= voiceShiftKey && voiceShiftKey <= 24;
    }
  }
}
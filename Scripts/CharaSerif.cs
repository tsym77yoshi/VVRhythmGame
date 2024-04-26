using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class uuidAndName
{
  public string uuid;
  public string name;
  public uuidAndName(string uuid_, string name_)
  {
    uuid = uuid_;
    name = name_;
  }
}
class serifType
{
  public string serifScene;
  // start01, start02, start03, (allexce, fullcom), special, great01, great02, notgood, mystery

  public string scopeType;
  // common, preset, name, styleid

  public string scope;
  // preset:
  // name/styleid:そのまま

  public string text;
  // 変更するセリフ

  public bool isNeta;

  public serifType(string serifScene_, string scopeType_, string scope_, string text_, bool isNeta_ = false)
  {
    serifScene = serifScene_;
    scopeType = scopeType_;
    scope = scope_;
    text = text_;
    isNeta = isNeta_;
  }
}
class charaPresetType
{
  public string preset;
  public string name;
  public charaPresetType(string preset_, string name_)
  {
    preset = preset_;
    name = name_;
  }
}
public class CharaSerif
{
  private List<serifType> defaultSerifSets = new List<serifType>{
    new serifType("start01","common","","はじまります"),
    new serifType("start02","common","","はじまりです"),
    new serifType("start03","common","","開始です"),
    new serifType("special","common","","おめでとう"),
    new serifType("great01","common","","さすが"),
    new serifType("great02","common","","すごい"),
    new serifType("notgood","common","","いい感じ"),
    new serifType("mystery","common","","ファイトです"),
    new serifType("","","",""),
    new serifType("","","",""),
    new serifType("","","",""),
    new serifType("","","",""),
    new serifType("","","",""),
    new serifType("","","",""),
    new serifType("start01","name","ずんだもん","はじまりますのだ"),// ずんだもん
    new serifType("start02","name","ずんだもん","はじまるのだ"),
    new serifType("start03","name","ずんだもん","開始なのだ"),
    new serifType("special","name","ずんだもん","おめでとうなのだ"),
    new serifType("great01","name","ずんだもん","さすがなのだ"),
    new serifType("great02","name","ずんだもん","すごいのだ"),
    new serifType("notgood","name","ずんだもん","いいなのだ"),
    new serifType("mystery","name","ずんだもん","ファイトなのだ")
  };
  private List<serifType> defaultNetaSerifSets = new List<serifType>{
  };
  private string charaUuidAndNamesStr =
  @"7ffcb7ce-00ec-4bdc-82cd-45a8889e43ff,四国めたん
388f246b-8c41-4ac1-8e2d-5d79f3ff56d9,ずんだもん
35b2c544-660e-401e-b503-0e14c635303a,春日部つむぎ
3474ee95-c274-47f9-aa1a-8322163d96f1,雨晴はう
b1a81618-b27b-40d2-b0ea-27a9ad408c4b,波音リツ
c30dc15a-0992-4f8d-8bb8-ad3b314e6a6f,玄野武宏
e5020595-5c5d-4e87-b849-270a518d0dcf,白上虎太郎
4f51116a-d9ee-4516-925d-21f183e2afad,青山龍星
8eaad775-3119-417e-8cf4-2a10bfd592c8,冥鳴ひまり
481fb609-6446-4870-9f46-90c4dd623403,九州そら
9f3ee141-26ad-437e-97bd-d22298d02ad2,もち子さん
1a17ca16-7ee5-4ea5-b191-2f02ace24d21,剣崎雌雄
67d5d8da-acd7-4207-bb10-b5542d3a663b,WhiteCUL
0f56c2f2-644c-49c9-8989-94e11f7129d0,後鬼
044830d2-f23b-44d6-ac0d-b5d733caa900,No.7
468b8e94-9da4-4f7a-8715-a22a48844f9e,ちび式じい
0693554c-338e-4790-8982-b9c6d476dc69,櫻歌ミコ
a8cc6d22-aad0-4ab8-bf1e-2f843924164a,小夜/SAYO
882a636f-3bac-431a-966d-c5e6bba9f949,ナースロボ＿タイプＴ
471e39d2-fb11-4c8c-8d89-4b322d2498e0,†聖騎士 紅桜†
0acebdee-a4a5-4e12-a695-e19609728e30,雀松朱司
7d1e7ba7-f957-40e5-a3fc-da49f769ab65,麒ヶ島宗麟
ba5d2428-f7e0-4c20-ac41-9dd56e9178b4,春歌ナナ
00a5c10c-d3bd-459f-83fd-43180b521a44,猫使アル
c20a2254-0349-4470-9fc8-e5c0f8cf3404,猫使ビィ
1f18ffc3-47ea-4ce0-9829-0576d03a7ec8,中国うさぎ
04dbd989-32d0-40b4-9e71-17c920f2a8a9,栗田まろん
dda44ade-5f9c-4a3a-9d2c-2a976c7476d9,あいえるたん
287aa49f-e56b-4530-a469-855776c84a8d,満別花丸
97a4af4b-086e-4efd-b125-7ae2da85e697,琴詠ニア";
  private string charaPresetsStr =
  @"F1四国めたん
M2ずんだもん
F2春日部つむぎ
F3雨晴はう
F3波音リツ
M1玄野武宏
M2白上虎太郎
M1青山龍星
F3冥鳴ひまり
F3九州そら
F3もち子さん
M1剣崎雌雄
F3WhiteCUL
F3後鬼
F3No.7
M3ちび式じい
F4櫻歌ミコ
F4小夜/SAYO
F3ナースロボ＿タイプＴ
M1†聖騎士 紅桜†
M1雀松朱司
M1麒ヶ島宗麟
F4春歌ナナ
F4猫使アル猫使ビィ
F5中国うさぎ
F3栗田まろん
F3あいえるたん
M2満別花丸
F3琴詠ニア";

  public string[] getSpeechTexts(string[] uuids, string[] serifScenes, int[] styleIds, bool isNeta = false)
  {
    if (uuids.Length==1)
    {
      return new string[] { getSpeechText(uuids[0], serifScenes, styleIds[0],"", isNeta) };
    }
    else
    {
      // コンビの物を作るなら

      string headText = "";
      if (serifScenes[0] == "allexce")
      {
        headText = "オールエクセレント、";
        serifScenes[0] = "special";
      }
      else if (serifScenes[0] == "fullcom")
      {
        headText = "フルコンボ、";
        serifScenes[0] = "special";
      }
      string[] result = new string[2];
      result[0] = getSpeechText(uuids[0], serifScenes, styleIds[0], "", isNeta);
      result[1] = getSpeechText(uuids[1], serifScenes, styleIds[1], result[0], isNeta);
      result[0] = headText + result[0];
      return result;
    }
  }
  public string getSpeechText(string uuid, string[] serifScenes, int styleId, string doubletext = "", bool isNeta = false)
  {
    string headText = "";
    if (serifScenes[0] == "allexce")
    {
      headText = "オールエクセレント、";
      serifScenes[0] = "special";
    }
    else if (serifScenes[0] == "fullcom")
    {
      headText = "フルコンボ、";
      serifScenes[0] = "special";
    }
    List<serifType> serifSets = new List<serifType>();
    serifSets.AddRange(defaultSerifSets);
    if (isNeta)
    {
      serifSets.AddRange(defaultNetaSerifSets);
    }
    // ここでプレイヤーが設定したserifsetsを呼び出す

    List<charaPresetType> charaPresets = new List<charaPresetType>();
    foreach (string charaPresetStr in charaPresetsStr.Split("\r\n"))
    {
      charaPresets.Add(new charaPresetType(charaPresetStr.Substring(0, 2), charaPresetStr.Substring(2)));
    }

    List<uuidAndName> charaUuidAndNames = new List<uuidAndName>();
    foreach (string charaUuidAndNameStr in charaUuidAndNamesStr.Split("\r\n"))
    {
      charaUuidAndNames.Add(new uuidAndName(charaUuidAndNameStr.Split(",")[0], charaUuidAndNameStr.Split(",")[1]));
    }
    string charaname = "";
    foreach (uuidAndName charaUuidAndName in charaUuidAndNames)
    {
      if (uuid == charaUuidAndName.uuid)
      {
        charaname = charaUuidAndName.name;
        break;
      }
    }

    // 処理開始
    string[] texts = new string[serifScenes.Length];

    // common
    foreach (serifType serifset in serifSets)
    {
      for (short i = 0; i < serifScenes.Length; i++)
      {
        if (serifset.serifScene == serifScenes[i] && serifset.scopeType == "common")
        {
          texts[i] = serifset.text;
        }
      }
    }
    // preset
    string charaPreset = "";
    foreach (charaPresetType charaPresetOne in charaPresets)
    {
      if (charaPresetOne.name == charaname)
      {
        charaPreset = charaPresetOne.preset;
        break;
      }
    }
    if (charaPreset != "")
    {
      foreach (serifType serifset in serifSets)
      {
        for (short i = 0; i < serifScenes.Length; i++)
        {
          if (serifset.serifScene == serifScenes[i] && serifset.scopeType == "preset" && serifset.scope == charaPreset)
          {
            texts[i] = serifset.text;
          }
        }
      }
    }
    // uuid
    foreach (serifType serifset in serifSets)
    {
      for (short i = 0; i < serifScenes.Length; i++)
      {
        if (serifset.serifScene == serifScenes[i] && serifset.scopeType == "name" && serifset.scope == charaname)
        {
          texts[i] = serifset.text;
        }
      }
    }
    // styleid
    foreach (serifType serifset in serifSets)
    {
      for (short i = 0; i < serifScenes.Length; i++)
      {
        if (serifset.serifScene == serifScenes[i] && serifset.scopeType == "styleid" && serifset.scope == styleId.ToString())
        {
          texts[i] = serifset.text;
        }
      }
    }

    // 抽選
    // 重複確認
    bool isDoubled = false;
    bool isDoubleRemove = false;
    foreach (string text in texts)
    {
      if (text == doubletext)
      {
        isDoubled = true;
      }
      else
      {
        isDoubleRemove = true;
      }
    }

    System.Random rand = new System.Random();
    int randomNumber = Math.Min(rand.Next(0, serifScenes.Length), serifScenes.Length - 1);

    if (isDoubled && isDoubleRemove)
    {
      while (texts[randomNumber] == doubletext)
      {
        randomNumber = Math.Min(rand.Next(0, serifScenes.Length), serifScenes.Length - 1);
      }
    }

    return headText + texts[randomNumber];
  }
}
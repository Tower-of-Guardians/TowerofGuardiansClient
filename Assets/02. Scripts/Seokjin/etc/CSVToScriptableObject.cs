
#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
#endif

enum CSVData
{
    CardData,
    ResultPercentData,
    MonsterData,
    MonsterEncounterData,
    StatusEffectData,
    SynergyData,
    EffectData
}

#if UNITY_EDITOR
// 에디터 폴더에 위치해야 함
public class CSVToScriptableObject
{
#if UNITY_EDITOR
    static string csv_name;
    static CSVData csv_data;
    // Unity 에디터 메뉴에 항목 추가 (예: Tools/Create Item SOs)
    [MenuItem("Tools/Generate Data/CardDataCreate")]
    public static void CardDataCreate()
    {
        csv_data = CSVData.CardData;
        csv_name = "CardData";
        soFolderPath = "Assets/Datas/" + csv_name;
        imageResourcesPath = "Assets/04. Images/";
        GenerateItemSOs();
    }

    [MenuItem("Tools/Generate Data/ResultPercentDataCreate")]
    public static void ResultDataCreate()
    {
        csv_data = CSVData.ResultPercentData;
        csv_name = "ResultPercentData";
        soFolderPath = "Assets/Datas/" + csv_name;
        GenerateItemSOs();
    }

    [MenuItem("Tools/Generate Data/MonsterDataCreate")]
    public static void MonsterDataCreate()
    {
        csv_data = CSVData.MonsterData;
        csv_name = "MonsterData";
        soFolderPath = "Assets/Datas/" + csv_name;
        GenerateItemSOs();
    }

    [MenuItem("Tools/Generate Data/MonsterEncounterDataCreate")]
    public static void MonsterEncounterDataCreate()
    {
        csv_data = CSVData.MonsterEncounterData;
        csv_name = "MonsterEncounterData";
        soFolderPath = "Assets/Datas/" + csv_name;
        GenerateItemSOs();
    }

    [MenuItem("Tools/Generate Data/StatusEffectDataCreate")]
    public static void StatusEffectDataCreate()
    {
        csv_data = CSVData.StatusEffectData;
        csv_name = "StatusEffectData";
        soFolderPath = "Assets/Datas/" + csv_name;
        GenerateItemSOs();
    }

    [MenuItem("Tools/Generate Data/SynergyDataCreate")]
    public static void SynergyDataCreate()
    {
        csv_data = CSVData.SynergyData;
        csv_name = "SynergyData";
        soFolderPath = "Assets/Datas/" + csv_name;
        GenerateItemSOs();
    }
    [MenuItem("Tools/Generate Data/EffectData")]
    public static void EffectDataCreate()
    {
        csv_data = CSVData.EffectData;
        csv_name = "EffectData";
        soFolderPath = "Assets/Datas/" + csv_name;
        GenerateItemSOs();
    }
#endif

#if UNITY_EDITOR
    static string soFolderPath; // ScriptableObject를 저장할 폴더
    static string imageResourcesPath;// Resources 폴더 내의 이미지 폴더 경로 (Resources를 제외한 상대 경로)

    public static void GenerateItemSOs()
    {
        string csvFilePath = Application.dataPath + "/Datas/CSV/" + csv_name + ".csv"; // 예시 경로
        // 1. CSV 파일 경로 설정 (Resources 폴더에 두거나, Assets 내의 특정 경로 지정)
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvFilePath);
            return;
        }

        // 저장 폴더가 없으면 생성
        if (!AssetDatabase.IsValidFolder(soFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Datas", csv_name);
        }

        if (!AssetDatabase.IsValidFolder(imageResourcesPath))
        {
            Debug.LogWarning(imageResourcesPath + " 폴더가 없습니다. 이미지 로드에 실패할 수 있습니다.");
        }

        // 2. 파일 읽기
        string[] allLines = File.ReadAllLines(csvFilePath);

        // 첫 번째 줄은 헤더(제목)이므로 건너뜁니다.
        switch (csv_data)
        {
            case CSVData.CardData:
                SetCardData(allLines);
                break;
            case CSVData.ResultPercentData:
                SetResultData(allLines);
                break;
            case CSVData.MonsterData:
                SetMonsterData(allLines);
                break;
            case CSVData.MonsterEncounterData:
                SetMonsterEncounterData(allLines);
                break;
            case CSVData.StatusEffectData:
                SetStatusEffectData(allLines);
                break;
            case CSVData.SynergyData:
                SynergyDataCreate(allLines);
                break;
            case CSVData.EffectData:
                EffectDataCreate(allLines);
                break;
        }

        // 에셋 데이터베이스를 새로고침하여 Unity 에디터에 반영
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(csv_name + "CSV 데이터로부터 " + (allLines.Length - 1) + "개의 ScriptableObject 생성이 완료되었습니다.");
    }
    private static void SetCardData(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            // 3. ScriptableObject 인스턴스 생성 및 데이터 할당
            CardData newItem = ScriptableObject.CreateInstance<CardData>();
            int n = 0;

            // CSV 열 순서에 맞게 데이터 파싱 및 할당
            // 오류 처리는 생략, 실제 사용 시에는 예외 처리 필요
            newItem.id = values[n++].Trim();
            newItem.itemName = values[n++].Trim();

            // ✨ 스프라이트 시트 파일 이름과 개별 스프라이트 이름 읽기
            string spriteNameInSheet = values[n++].Trim();   // 예: Sword
            newItem.iconimage = FindSprite(spriteNameInSheet, "Icons/", "ItemIcon.png");

            if (int.TryParse(values[n++].Trim(), out int garde)) newItem.grade = garde;
            if (int.TryParse(values[n++].Trim(), out int star)) newItem.star = star;
            if (int.TryParse(values[n++].Trim(), out int price)) newItem.price = price;
            newItem.synergy1Icon = FindSprite(values[n++].Trim(), "Icons/", "ItemIcon.png");
            newItem.synergy1ID = values[n++].Trim();
            newItem.synergy2Icon = FindSprite(values[n++].Trim(), "Icons/", "ItemIcon.png");
            newItem.synergy2ID = values[n++].Trim();
            newItem.synergy3Icon = FindSprite(values[n++].Trim(), "Icons/", "ItemIcon.png");
            newItem.synergy3ID = values[n++].Trim();
            string newdescription = values[n++].Replace("\"\"", "<br>").Replace("\"", "").Replace("<br>", "\"").Trim();
            newItem.effectDescription = newdescription;
            if (int.TryParse(values[n++].Trim(), out int ATK)) newItem.ATK = ATK;
            if (int.TryParse(values[n++].Trim(), out int DEF)) newItem.DEF = DEF;
            if (int.TryParse(values[n++].Trim(), out int When)) newItem.when = When;
            if (int.TryParse(values[n++].Trim(), out int Trigger)) newItem.trigger = Trigger;
            newItem.effect1ID = values[n++].Trim(); ;
            newItem.effect1Value = values[n++].Trim();
            newItem.effect2ID = values[n++].Trim(); ;
            newItem.effect2Value = values[n++].Trim();

            if (!string.IsNullOrEmpty("Card"))
            {
                string cardname = string.Format("CardFrame{0}{1}", newItem.grade, newItem.star);
                string synergyname = string.Format("SynergyFrame{0}", newItem.grade);
                string fullSpriteSheetPath = Path.Combine(imageResourcesPath + "UI/Jongmin/Associate With Card").Replace('\\', '/');
                var file_carframe = Directory.GetFiles(fullSpriteSheetPath, "*.png", SearchOption.AllDirectories).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == cardname);
                var file_synergyframe = Directory.GetFiles(fullSpriteSheetPath, "*.png", SearchOption.AllDirectories).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == synergyname);

                if (file_carframe != null)
                {
                    newItem.cardimage = AssetDatabase.LoadAssetAtPath<Sprite>(file_carframe);
                }
                else
                {
                    Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{cardname}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                }

                if (file_synergyframe != null)
                {
                    newItem.synergyFrameImage = AssetDatabase.LoadAssetAtPath<Sprite>(file_synergyframe);
                }
                else
                {
                    Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{synergyname}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                }

                // 4. 에셋 파일로 저장
                string fileName = newItem.id + ".asset";
                //string fullPath = Path.Combine(soFolderPath, fileName);

                AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
            }
        }
    }
    private static void SetResultData(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            ResultPercentData newItem = ScriptableObject.CreateInstance<ResultPercentData>();

            if (int.TryParse(values[0].Trim(), out int level)) newItem.level = level;
            for (int i = 1; i < values.Length; i++)
            {
                if (int.TryParse(values[i].Trim(), out int num)) newItem.percent.Add(num);
            }

            string fileName = "ResultPercentData" + newItem.level + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }
    }
    private static void SetMonsterData(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            MonsterData newItem = ScriptableObject.CreateInstance<MonsterData>();
            int n = 0;

            // ID,Name,Image,HP,Kind,PatternType,Passive1ID,Passive1Value,Passive2ID,Passive2Value,Passive3ID,Passive3Value,Action1ID,Action1Min,Action1Max...Action7Max
            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();
            n++; // Image 컬럼(현재 미사용)

            if (int.TryParse(values[n++].Trim(), out int hp)) newItem.HP = hp;
            if (int.TryParse(values[n++].Trim(), out int kind)) newItem.Kind = kind;
            if (int.TryParse(values[n++].Trim(), out int patternType)) newItem.PatternType = patternType;

            newItem.Passive1ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int passive1Value)) newItem.Passive1Value = passive1Value;
            newItem.Passive2ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int passive2Value)) newItem.Passive2Value = passive2Value;
            newItem.Passive3ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int passive3Value)) newItem.Passive3Value = passive3Value;

            newItem.Action1ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action1Min)) newItem.Action1Min = action1Min;
            if (int.TryParse(values[n++].Trim(), out int action1Max)) newItem.Action1Max = action1Max;

            newItem.Action2ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action2Min)) newItem.Action2Min = action2Min;
            if (int.TryParse(values[n++].Trim(), out int action2Max)) newItem.Action2Max = action2Max;

            newItem.Action3ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action3Min)) newItem.Action3Min = action3Min;
            if (int.TryParse(values[n++].Trim(), out int action3Max)) newItem.Action3Max = action3Max;

            newItem.Action4ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action4Min)) newItem.Action4Min = action4Min;
            if (int.TryParse(values[n++].Trim(), out int action4Max)) newItem.Action4Max = action4Max;

            newItem.Action5ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action5Min)) newItem.Action5Min = action5Min;
            if (int.TryParse(values[n++].Trim(), out int action5Max)) newItem.Action5Max = action5Max;

            newItem.Action6ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action6Min)) newItem.Action6Min = action6Min;
            if (int.TryParse(values[n++].Trim(), out int action6Max)) newItem.Action6Max = action6Max;

            newItem.Action7ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int action7Min)) newItem.Action7Min = action7Min;
            if (int.TryParse(values[n++].Trim(), out int action7Max)) newItem.Action7Max = action7Max;

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);

            /*if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            MonsterData newItem = ScriptableObject.CreateInstance<MonsterData>();
            int n = 0;

            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();

            if (int.TryParse(values[n++].Trim(), out int hp)) newItem.Hp = hp;
            if (int.TryParse(values[n++].Trim(), out int atkmin)) newItem.ATKMin = atkmin;
            if (int.TryParse(values[n++].Trim(), out int atkmax)) newItem.ATKMax = atkmax;
            if (int.TryParse(values[n++].Trim(), out int defmin)) newItem.DEFMin = defmin;
            if (int.TryParse(values[n++].Trim(), out int defmax)) newItem.DEFMax = defmax;
            if (int.TryParse(values[n++].Trim(), out int kind)) newItem.Kind = kind;
            if (int.TryParse(values[n++].Trim(), out int patterntype)) newItem.PatternType = patterntype;
            newItem.PassiveID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int passval)) newItem.PassiveValue = passval;
            newItem.StatusEffect1ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int tar1)) newItem.Target1 = tar1;
            if (int.TryParse(values[n++].Trim(), out int val1)) newItem.Value1 = val1;
            newItem.StatusEffect2ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int tar2)) newItem.Target2 = tar2;
            if (int.TryParse(values[n++].Trim(), out int val2)) newItem.Value3 = val2;
            newItem.StatusEffect3ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int tar3)) newItem.Target2 = tar3;
            if (int.TryParse(values[n++].Trim(), out int val3)) newItem.Value3 = val3;

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);*/
        }
    }
    private static void SetMonsterEncounterData(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            MonsterEncounterData newItem = ScriptableObject.CreateInstance<MonsterEncounterData>();
            int n = 0;

            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();

            if (int.TryParse(values[n++].Trim(), out int section)) newItem.Section = section;
            if (int.TryParse(values[n++].Trim(), out int gold)) newItem.Gold = gold;
            if (int.TryParse(values[n++].Trim(), out int exp)) newItem.Exp = exp;

            newItem.Mon1ID = values[n++].Trim();
            newItem.Mon2ID = values[n++].Trim();
            newItem.Mon3ID = values[n++].Trim();
            newItem.Mon4ID = values[n++].Trim();

            if (float.TryParse(values[n++].Trim(), out float pos1)) newItem.Mon1Position = pos1;
            if (float.TryParse(values[n++].Trim(), out float pos2)) newItem.Mon2Position = pos2;
            if (float.TryParse(values[n++].Trim(), out float pos3)) newItem.Mon3Position = pos3;
            if (float.TryParse(values[n++].Trim(), out float pos4)) newItem.Mon4Position = pos4;

            if (float.TryParse(values[n++].Trim(), out float bar1)) newItem.Mon1BarLength = bar1;
            if (float.TryParse(values[n++].Trim(), out float bar2)) newItem.Mon2BarLength = bar2;
            if (float.TryParse(values[n++].Trim(), out float bar3)) newItem.Mon3BarLength = bar3;
            if (float.TryParse(values[n++].Trim(), out float bar4)) newItem.Mon4BarLength = bar4;

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }
    }
    private static void SetStatusEffectData(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            StatusEffectData newItem = ScriptableObject.CreateInstance<StatusEffectData>();
            int n = 0;

            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();

            if (int.TryParse(values[n++].Trim(), out int apply)) newItem.Apply = apply;
            if (int.TryParse(values[n++].Trim(), out int buff)) newItem.BuffType = buff;
            if (int.TryParse(values[n++].Trim(), out int num)) newItem.NumType = num;
            if (int.TryParse(values[n++].Trim(), out int duration)) newItem.DurationType = duration;
            if (int.TryParse(values[n++].Trim(), out int releas)) newItem.ReleaseCondition = releas;
            newItem.Description = values[n++].Trim();

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }
    }
    private static void SynergyDataCreate(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            SynergyData newItem = ScriptableObject.CreateInstance<SynergyData>();
            int n = 0;

            newItem.ID = values[n++].Trim();
            newItem.Name = values[n++].Trim();
            newItem.Description = values[n++].Trim();

            if (int.TryParse(values[n++].Trim(), out int tier)) newItem.Tier = tier;

            newItem.Effect1ID = values[n++].Trim();
            newItem.Effect1Synergys = new List<int>();
            if (int.TryParse(values[n++].Trim(), out int es1_1)) newItem.Effect1Synergys.Add(es1_1); else newItem.Effect1Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es1_2)) newItem.Effect1Synergys.Add(es1_2); else newItem.Effect1Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es1_3)) newItem.Effect1Synergys.Add(es1_3); else newItem.Effect1Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es1_4)) newItem.Effect1Synergys.Add(es1_4); else newItem.Effect1Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es1_5)) newItem.Effect1Synergys.Add(es1_5); else newItem.Effect1Synergys.Add(0);
            newItem.EffectSynergys[newItem.Effect1ID] = newItem.Effect1Synergys;

            newItem.Effect2ID = values[n++].Trim();
            newItem.Effect2Synergys = new List<int>();
            if (int.TryParse(values[n++].Trim(), out int es2_1)) newItem.Effect2Synergys.Add(es2_1); else newItem.Effect2Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es2_2)) newItem.Effect2Synergys.Add(es2_2); else newItem.Effect2Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es2_3)) newItem.Effect2Synergys.Add(es2_3); else newItem.Effect2Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es2_4)) newItem.Effect2Synergys.Add(es2_4); else newItem.Effect2Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es2_5)) newItem.Effect2Synergys.Add(es2_5); else newItem.Effect2Synergys.Add(0);
            newItem.EffectSynergys[newItem.Effect2ID] = newItem.Effect2Synergys;

            newItem.Effect3ID = values[n++].Trim();
            newItem.Effect3Synergys = new List<int>();
            if (int.TryParse(values[n++].Trim(), out int es3_1)) newItem.Effect3Synergys.Add(es3_1); else newItem.Effect3Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es3_2)) newItem.Effect3Synergys.Add(es3_2); else newItem.Effect3Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es3_3)) newItem.Effect3Synergys.Add(es3_3); else newItem.Effect3Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es3_4)) newItem.Effect3Synergys.Add(es3_4); else newItem.Effect3Synergys.Add(0);
            if (int.TryParse(values[n++].Trim(), out int es3_5)) newItem.Effect3Synergys.Add(es3_5); else newItem.Effect3Synergys.Add(0);
            newItem.EffectSynergys[newItem.Effect3ID] = newItem.Effect3Synergys;

            string fileName = newItem.ID + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }
    }
    private static void EffectDataCreate(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            EffectData newItem = ScriptableObject.CreateInstance<EffectData>();
            int n = 0;

            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();
            newItem.Effect = values[n++].Trim();

            if (int.TryParse(values[n++].Trim(), out int target)) newItem.Target = target;
            if (int.TryParse(values[n++].Trim(), out int choice)) newItem.Choice = choice;

            newItem.CreateMagic = values[n++].Trim();
            newItem.StatusEffect = values[n++].Trim();
            newItem.NumType = values[n++].Trim();

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }
    }

    private static Sprite FindSprite(string spriteNameInSheet, string address, string spritename)
    {
        if (!string.IsNullOrEmpty(spriteNameInSheet))
        {
            string fullSpriteSheetPath = Path.Combine(imageResourcesPath + address, spritename).Replace('\\', '/');

            // AssetDatabase.LoadAllAssetsAtPath를 사용하여 스프라이트 시트 내의 모든 스프라이트를 불러옵니다.
            // 이 함수는 주 에셋(Texture2D)과 그 하위 에셋(Sprite)들을 모두 불러옵니다.
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(fullSpriteSheetPath);
            Sprite foundSprite = null;

            foreach (Object asset in allAssets)
            {
                // 불러온 에셋 중 Sprite 타입이고 이름이 일치하는 것을 찾습니다.
                if (asset is Sprite sprite && sprite.name == spriteNameInSheet)
                {
                    foundSprite = sprite;
                    break;
                }
            }

            if (foundSprite != null)
            {
                return foundSprite;
            }
            else
            {
                Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{spriteNameInSheet}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                return null;
            }
        }
        else return null;
    }
#endif 
}
#endif
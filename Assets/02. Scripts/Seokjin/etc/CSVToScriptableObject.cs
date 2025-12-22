using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

enum CSVData
{
    CardData,
    ResultPercentData,
    PlayerData,
    MonsterData,
    MonsterActionData,
    MonsterEncounterData,
    StatusEffectData
}

// 에디터 폴더에 위치해야 함
public class CSVToScriptableObject
{
    static string csv_name;
    static CSVData csv_data;
    // Unity 에디터 메뉴에 항목 추가 (예: Tools/Create Item SOs)
    [MenuItem("Tools/Generate Data/CardDataCreate")]
    public static void CardDataCreate()
    {
        csv_data = CSVData.CardData;
        csv_name = "CardData";
        soFolderPath = "Assets/Datas/" + csv_name;
        imageResourcesPath = "Assets/04. Images/Test/";
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

    [MenuItem("Tools/Generate Data/MonsterActionDataCreate")]
    public static void MonsterActionDataCreate()
    {
        csv_data = CSVData.MonsterActionData;
        csv_name = "MonsterActionData";
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
            case CSVData.MonsterActionData:
                SetMonsterActionData(allLines);
                break;
            case CSVData.MonsterEncounterData:
                SetMonsterEncounterData(allLines);
                break;
            case CSVData.StatusEffectData:
                SetStatusEffectData(allLines);
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

            if (!string.IsNullOrEmpty(spriteNameInSheet))
            {
                string fullSpriteSheetPath = Path.Combine(imageResourcesPath+ "Icons/", "ItemIcon.png").Replace('\\', '/');

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
                    newItem.iconimage = foundSprite;
                }
                else
                {
                    Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{spriteNameInSheet}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                }
            }

            if (int.TryParse(values[n++].Trim(), out int garde)) newItem.grade = garde;
            if (int.TryParse(values[n++].Trim(), out int star)) newItem.star = star;
            if (int.TryParse(values[n++].Trim(), out int price)) newItem.price = price;
            newItem.synergy1ID = values[n++].Trim();
            newItem.synergy2ID = values[n++].Trim();
            newItem.synergy3ID = values[n++].Trim();
            newItem.effectDescription = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int ATK)) newItem.ATK = ATK;
            if (int.TryParse(values[n++].Trim(), out int DEF)) newItem.DEF = DEF;
            newItem.effect1ID = values[n++].Trim(); ;
            newItem.effect1Value = values[n++].Trim();
            newItem.effect2ID = values[n++].Trim(); ;
            newItem.effect2Value = values[n++].Trim();

            if (!string.IsNullOrEmpty("Card"))
            {
                string fullSpriteSheetPath = Path.Combine(imageResourcesPath +"Card/", "CardFrame.png").Replace('\\', '/');

                // AssetDatabase.LoadAllAssetsAtPath를 사용하여 스프라이트 시트 내의 모든 스프라이트를 불러옵니다.
                // 이 함수는 주 에셋(Texture2D)과 그 하위 에셋(Sprite)들을 모두 불러옵니다.
                Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(fullSpriteSheetPath);
                Sprite foundSprite = null;

                foreach (Object asset in allAssets)
                {
                    // 불러온 에셋 중 Sprite 타입이고 이름이 일치하는 것을 찾습니다.
                    if (asset is Sprite sprite && sprite.name == string.Format("CardFrame{0}{1}", newItem.grade, newItem.star))
                    {
                        foundSprite = sprite;
                        break;
                    }
                }

                if (foundSprite != null)
                {
                    newItem.cardimage = foundSprite;
                }
                else
                {
                    Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{spriteNameInSheet}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                }
            }

            //ID,Name,Image,Grade,Star,Price,Synergy1ID,Synergy2ID,Synergy3ID,EffectDescription,ATK,DEF,Effect1ID,Effect1Value,Effect2ID,Effect2Value

            // ... 나머지 필드 할당

            // 4. 에셋 파일로 저장
            string fileName = newItem.id + ".asset";
            //string fullPath = Path.Combine(soFolderPath, fileName);

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
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

            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();

            // ✨ 스프라이트 시트 파일 이름과 개별 스프라이트 이름 읽기
            string spriteNameInSheet = values[n++].Trim();   // 예: Sword

            if (!string.IsNullOrEmpty(spriteNameInSheet))
            {
                string fullSpriteSheetPath = Path.Combine(imageResourcesPath + "Icons/", "ItemIcon.png").Replace('\\', '/');

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
                    newItem.Image = foundSprite;
                }
                else
                {
                    Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{spriteNameInSheet}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                }
            }


            if (int.TryParse(values[n++].Trim(), out int hp)) newItem.HP = hp;
            if (int.TryParse(values[n++].Trim(), out int kind)) newItem.Kind = kind;
            if (int.TryParse(values[n++].Trim(), out int pattern)) newItem.PatternType = pattern;

            newItem.Passive1ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int Passive1)) newItem.Passive1Value = Passive1;
            newItem.Passive2ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int Passive2)) newItem.Passive2Value = Passive2;
            newItem.Passive3ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int Passive3)) newItem.Passive3Value = Passive3;

            newItem.Action1ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min1)) newItem.Action1Min = min1;
            if (int.TryParse(values[n++].Trim(), out int max1)) newItem.Action1Max = max1;

            newItem.Action2ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min2)) newItem.Action2Min = min2;
            if (int.TryParse(values[n++].Trim(), out int max2)) newItem.Action2Max = max2;

            newItem.Action3ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min3)) newItem.Action3Min = min3;
            if (int.TryParse(values[n++].Trim(), out int max3)) newItem.Action3Max = max3;

            newItem.Action4ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min4)) newItem.Action4Min = min4;
            if (int.TryParse(values[n++].Trim(), out int max4)) newItem.Action4Max = max4;

            newItem.Action5ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min5)) newItem.Action5Min = min5;
            if (int.TryParse(values[n++].Trim(), out int max5)) newItem.Action5Max = max5;

            newItem.Action6ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min6)) newItem.Action6Min = min6;
            if (int.TryParse(values[n++].Trim(), out int max6)) newItem.Action6Max = max6;

            newItem.Action7ID = values[n++].Trim();
            if (int.TryParse(values[n++].Trim(), out int min7)) newItem.Action7Min = min7;
            if (int.TryParse(values[n++].Trim(), out int max7)) newItem.Action7Max = max7;

            //ID,Name,Image,HP,Kind,PatternType,Passive1ID,Passive1Value,Passive2ID,Passive2Value,Passive3ID,Passive3Value,
            //Action1ID,Action1Min,Action1Max,Action2ID,Action2Min,Action2Max,Action3ID,Action3Min,Action3Max,Action4ID,Action4Min,Action4Max,Action5ID,Action5Min,Action5Max,Action6ID,Action6Min,Action6Max,Action7ID,Action7Min,Action7Max

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }
    }
    private static void SetMonsterActionData(string[] allLines)
    {
        foreach (string line in allLines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            MonsterActionData newItem = ScriptableObject.CreateInstance<MonsterActionData>();
            int n = 0;

            newItem.Id = values[n++].Trim();
            newItem.Name = values[n++].Trim();            

            if (int.TryParse(values[n++].Trim(), out int garde)) newItem.Target = garde;
            newItem.StatusEffect = values[n++].Trim();
            newItem.Description = values[n++].Trim();

            string fileName = newItem.Id + ".asset";

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
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
}
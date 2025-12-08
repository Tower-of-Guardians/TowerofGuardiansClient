using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

enum CSVData
{
    CardData,
    ResultPercentData
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

            // CSV 열 순서에 맞게 데이터 파싱 및 할당
            // 오류 처리는 생략, 실제 사용 시에는 예외 처리 필요
            newItem.id = values[0].Trim();
            newItem.itemName = values[1].Trim();

            // ✨ 스프라이트 시트 파일 이름과 개별 스프라이트 이름 읽기
            string spriteNameInSheet = values[2].Trim();   // 예: Sword

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

            if (int.TryParse(values[3].Trim(), out int garde)) newItem.grade = garde;
            if (int.TryParse(values[4].Trim(), out int star)) newItem.star = star;
            if (int.TryParse(values[5].Trim(), out int price)) newItem.price = price;
            newItem.synergy1ID = values[6].Trim();
            newItem.synergy2ID = values[7].Trim();
            newItem.synergy3ID = values[8].Trim();
            newItem.effectDescription = values[9].Trim();
            if (int.TryParse(values[10].Trim(), out int ATK)) newItem.ATK = ATK;
            if (int.TryParse(values[11].Trim(), out int DEF)) newItem.DEF = DEF;
            newItem.effect1ID = values[12].Trim(); ;
            newItem.effect1Value = values[13].Trim();
            newItem.effect2ID = values[14].Trim(); ;
            newItem.effect2Value = values[15].Trim();

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
}
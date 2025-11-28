using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// 에디터 폴더에 위치해야 함
public class CSVToScriptableObject
{
    static string csv_name;
    // Unity 에디터 메뉴에 항목 추가 (예: Tools/Create Item SOs)
    [MenuItem("Tools/Generate Data/CardDataCreate")]
    public static void CardDataCreate()
    {
        csv_name = "CardData";
        GenerateItemSOs();
    }

    [MenuItem("Tools/Generate Data/SkilDataCreate")]
    public static void SkilDataCreate()
    {
        csv_name = "SkillData";
        GenerateItemSOs();
    }

    public static void GenerateItemSOs()
    {
        // 1. CSV 파일 경로 설정 (Resources 폴더에 두거나, Assets 내의 특정 경로 지정)
        string csvFilePath = Application.dataPath + "/Datas/CSV/" + csv_name + ".csv"; // 예시 경로
        string soFolderPath = "Assets/Datas/" + csv_name; // ScriptableObject를 저장할 폴더
        string imageResourcesPath = "Assets/04. Images/Test/Icons/"; // Resources 폴더 내의 이미지 폴더 경로 (Resources를 제외한 상대 경로)
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
                string fullSpriteSheetPath = Path.Combine(imageResourcesPath, "ItemIcon.png").Replace('\\', '/');

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
                    newItem.image = foundSprite;
                }
                else
                {
                    Debug.LogWarning($"스프라이트 시트 '{fullSpriteSheetPath}'에서 스프라이트 '{spriteNameInSheet}'를 찾을 수 없습니다. (이름 또는 슬라이싱 확인 필요)");
                }
            }

            if (int.TryParse(values[3].Trim(), out int garde)) newItem.grade = garde;
            if (int.TryParse(values[4].Trim(), out int star)) newItem.star = star;
            newItem.synergy1 = values[5].Trim();
            newItem.synergy1ID = values[6].Trim();
            newItem.synergy2 = values[7].Trim();
            newItem.synergy2ID = values[8].Trim();
            newItem.synergy3 = values[9].Trim();
            newItem.synergy3ID = values[10].Trim();
            newItem.effectDescription = values[11].Trim();
            if (int.TryParse(values[12].Trim(), out int ATK)) newItem.ATK = ATK;
            if (int.TryParse(values[13].Trim(), out int DEF)) newItem.DEF = DEF;
            newItem.effect1ID = values[14].Trim(); ;
            newItem.effect1Value = values[15].Trim();
            newItem.effect2ID = values[16].Trim(); ;
            newItem.effect2Value = values[17].Trim();
            //ID,Name,Image,Grade,Star,Synergy1,Synergy1ID,Synergy2,Synergy2ID,Synergy3,Synergy3ID,EffectDescription,ATK,DEF,Effect1ID,Effect1Value,Effect2ID,Effect2Value

            // ... 나머지 필드 할당

            // 4. 에셋 파일로 저장
            string fileName = newItem.id + ".asset";
            //string fullPath = Path.Combine(soFolderPath, fileName);

            AssetDatabase.CreateAsset(newItem, soFolderPath + "/" + fileName);
        }

        // 에셋 데이터베이스를 새로고침하여 Unity 에디터에 반영
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(csv_name + "CSV 데이터로부터 " + (allLines.Length - 1) + "개의 ScriptableObject 생성이 완료되었습니다.");
    }
}
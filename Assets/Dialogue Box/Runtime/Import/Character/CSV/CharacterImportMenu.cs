#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DialogueBox.Editor
{
    public static class CharacterImportMenu
    {
        [MenuItem("Tools/Dialogue Box/Import Characters")]
        private static void ImportDebug()
        {
            // Specify your custom file path.
            var character_csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Dialogue Box/Demo/CSV/Character.csv");
            if(character_csv == null)
            {
                Debug.LogError("CharacterImportMenu: Can't find .csv file.");
                return;
            }

            var db = Selection.activeObject as CharacterDatabase;
            if (db == null)
            {
                Debug.LogWarning("DialogueBox: Run it with the CharacterDatabase asset selected.");
                return;
            }

            // Specify your own raw source.
            var source = new CSVCharacterRawSource(character_csv);
            var writer = new CharacterDatabaseWriter();

            CharacterImportPipeline.Import(source, writer, db);
            Debug.Log("Import completed. Check the CharacterDatabase in the Inspector.");
        }
    }
}
#endif
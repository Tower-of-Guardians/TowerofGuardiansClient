#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DialogueBox.Editor
{
    public static class DialogueImportMenu
    {
        [MenuItem("Tools/Dialogue Box/Import Dialogues")]
        private static void ImportDebug()
        {
            // Specify your custom file path.
            var node_csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Dialogue Box/Demo/CSV/DialogueNode.csv");
            var choice_csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Dialogue Box/Demo/CSV/DialogueChoice.csv");

            if(node_csv == null || choice_csv == null)
            {
                Debug.LogError("DialogueImportMenu: Can't find .csv files.");
                return;
            }

            var db = Selection.activeObject as DialogueDatabase;
            if (db == null)
            {
                Debug.LogWarning("DialogueBox: Run it with the DialogueDatabase asset selected.");
                return;
            }

            // Specify your own raw source.
            var source = new CSVDialogueRawSource(node_csv, choice_csv);
            var writer = new DialogueDatabaseWriter();

            DialogueImportPipeline.Import(source, writer, db);

            Debug.Log("Import completed. Check the DialogueDatabase in the Inspector.");
        }
    }
}
#endif
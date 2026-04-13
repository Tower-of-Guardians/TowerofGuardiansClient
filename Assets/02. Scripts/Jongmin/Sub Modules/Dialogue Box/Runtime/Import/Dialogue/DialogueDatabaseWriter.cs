#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;

namespace DialogueBox.Editor
{
    public sealed class DialogueDatabaseWriter : IDialogueDatabaseWriter
    {
        public void Overwrite(DialogueDatabase db, 
                              IEnumerable<DialogueRawEntry> entries, 
                              IEnumerable<DialogueRawNode> nodes)
        {
            if(db == null)
                return;

            var m_so = new SerializedObject(db);

            var entries_prop = m_so.FindProperty("m_entries");
            var nodes_prop = m_so.FindProperty("m_nodes");

            entries_prop.ClearArray();
            nodes_prop.ClearArray();

            int e = 0;
            foreach(var entry in entries)
            {
                entries_prop.InsertArrayElementAtIndex(e);

                var item = entries_prop.GetArrayElementAtIndex(e);
                item.FindPropertyRelative("DialogueID").stringValue = entry.DialogueID ?? string.Empty;
                item.FindPropertyRelative("EntryNodeID").stringValue = entry.EntryNodeID ?? string.Empty;
                e++;
            }

            int n = 0;
            foreach(var node in nodes)
            {
                nodes_prop.InsertArrayElementAtIndex(n);

                var item = nodes_prop.GetArrayElementAtIndex(n);
                item.FindPropertyRelative("ID").stringValue = node.ID ?? string.Empty;
                item.FindPropertyRelative("Type").enumValueIndex = (int)node.Type;
                item.FindPropertyRelative("Speaker").enumValueIndex = (int)node.Speaker;
                item.FindPropertyRelative("CharacterID").stringValue = node.CharacterID ?? string.Empty;
                item.FindPropertyRelative("Text").stringValue = node.Text ?? string.Empty;
                item.FindPropertyRelative("PortraitKey").stringValue = node.PortraitKey ?? string.Empty;
                item.FindPropertyRelative("NextID").stringValue = node.NextID ?? string.Empty;
                item.FindPropertyRelative("Prompt").stringValue = node.Prompt ?? string.Empty;

                var option_prop = item.FindPropertyRelative("Options");
                option_prop.ClearArray();

                if(node.Options != null)
                {
                    for(int i = 0; i < node.Options.Count; i++)
                    {
                        option_prop.InsertArrayElementAtIndex(i);

                        var option_item = option_prop.GetArrayElementAtIndex(i);
                        option_item.FindPropertyRelative("Text").stringValue = node.Options[i].Text ?? string.Empty;
                        option_item.FindPropertyRelative("NextID").stringValue = node.Options[i].NextID ?? string.Empty;
                    }
                }

                item.FindPropertyRelative("TargetID").stringValue = node.TargetID ?? string.Empty;
                n++;
            }

            m_so.ApplyModifiedProperties();
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
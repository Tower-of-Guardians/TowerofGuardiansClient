#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace DialogueBox.Editor
{
    public sealed class CharacterDatabaseWriter : ICharacterDatabaseWriter
    {
        public void Overwrite(CharacterDatabase db, 
                              IEnumerable<CharacterRawEntry> characters)
        {
            if (db == null) return;

            var m_so = new SerializedObject(db);
            m_so.Update();

            var entries_prop = m_so.FindProperty("m_entries");
            entries_prop.arraySize = 0;

            int i = 0;
            foreach (var character in characters)
            {
                entries_prop.InsertArrayElementAtIndex(i);

                var item = entries_prop.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("CharacterID").stringValue = character.CharacterID ?? string.Empty;
                item.FindPropertyRelative("DisplayName").stringValue = character.DisplayName ?? string.Empty;
                i++;
            }

            m_so.ApplyModifiedProperties();
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueBox
{
    [CreateAssetMenu(fileName = "Character Database", menuName = "Dialogue Box/Character Database")]
    public class CharacterDatabase : ScriptableObject
    {
        [Serializable]
        public sealed class CharacterEntry
        {
            public string CharacterID;
            public string DisplayName;
        }

        [SerializeField] private List<CharacterEntry> m_entries = new();
        private Dictionary<string, CharacterEntry> m_map;

        private void OnEnable()
            => BuildCache();

        private void BuildCache()
        {
            m_map = new(StringComparer.Ordinal);
            
            foreach(var entry in m_entries)
            {
                if(entry == null)
                    continue;

                if(string.IsNullOrEmpty(entry.CharacterID))
                    continue;

                m_map[entry.CharacterID] = entry;
            }
        }

        public bool TryGetEntry(string character_id, out CharacterEntry entry)
        {
            if(m_map == null)
                BuildCache();

            return m_map.TryGetValue(character_id, out entry);
        }

        public string ResolveName(string character_id)
        {
            if(string.IsNullOrEmpty(character_id))
                return string.Empty;

            return TryGetEntry(character_id, out var entry) && !string.IsNullOrEmpty(entry.DisplayName) ? entry.DisplayName : string.Empty;
        }
    }
}


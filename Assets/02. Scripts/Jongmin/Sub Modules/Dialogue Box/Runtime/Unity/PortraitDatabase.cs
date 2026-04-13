using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueBox
{
    [CreateAssetMenu(fileName = "Portrait Database", menuName = "Dialogue Box/Portrait Database")]
    public class PortraitDatabase : ScriptableObject
    {
        [Serializable]
        public struct PortraitEntry
        {
            public string Key;
            public Sprite Sprite;
        }

        [Serializable]
        public sealed class CharacterPortraitSet
        {
            public string CharacterID;
            public string DefaultKey = "default";
            public List<PortraitEntry> Portraits = new();
        }

        [SerializeField] private List<CharacterPortraitSet> m_sets = new();

        private Dictionary<string, CharacterPortraitSet> m_set_map;
        private Dictionary<string, Dictionary<string, Sprite>> m_portrait_maps;

        public void BuildCache()
        {
            if(m_set_map != null)
                return;

            m_set_map = new(StringComparer.Ordinal);
            m_portrait_maps = new(StringComparer.Ordinal);

            foreach(var set in m_sets)
            {
                if(set == null)
                    continue;

                if(string.IsNullOrEmpty(set.CharacterID))
                    continue;

                var map = new Dictionary<string, Sprite>(StringComparer.Ordinal);
                foreach(var portrait in set.Portraits)
                {
                    if(string.IsNullOrWhiteSpace(portrait.Key))
                        continue;

                    if(portrait.Sprite == null)
                        continue;

                    map[portrait.Key] = portrait.Sprite;
                }

                m_portrait_maps[set.CharacterID] = map;
            }
        }

        public bool TryGetPortrait(string character_id, string key, out Sprite sprite)
        {
            BuildCache();
            sprite = null;

            if(string.IsNullOrWhiteSpace(character_id))
                return false;

            if(!m_portrait_maps.TryGetValue(character_id, out var map))
                return false;

            if(string.IsNullOrWhiteSpace(key))
                return false;

            return map.TryGetValue(key, out sprite) && sprite != null;
        }

        public string GetDefaultKey(string character_id)
        {
            BuildCache();

            if(string.IsNullOrWhiteSpace(character_id))
                return "default";

            if(m_set_map != null && m_set_map.TryGetValue(character_id, out var set))
                return string.IsNullOrWhiteSpace(set.DefaultKey) ? "default"
                                                                 : set.DefaultKey;

            return "default";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_set_map = null;
            m_portrait_maps = null;
        }
#endif
    }
}
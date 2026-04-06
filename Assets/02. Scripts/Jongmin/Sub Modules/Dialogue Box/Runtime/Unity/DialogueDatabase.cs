using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueBox
{
    [CreateAssetMenu(menuName = "Dialogue Box/Dialogue Database")]
    public sealed class DialogueDatabase : ScriptableObject, IDialogueDataSource
    {
        [Serializable]
        public class Entry
        {
            public string DialogueID;
            public string EntryNodeID;
        }

        public enum SerializableNodeType
        {
            LINE,
            CHOICE,
            JUMP,
            END
        }

        [Serializable]
        public class SerializableChoiceOption
        {
            public string Text;
            public string NextID;
        }

        [Serializable]
        public class SerializableNode
        {
            public string ID;
            public SerializableNodeType Type;

            public Speaker Speaker;
            public string CharacterID;
            [TextArea] public string Text;
            public string PortraitKey;
            public string NextID;

            [TextArea] public string Prompt;
            public List<SerializableChoiceOption> Options = new();

            public string TargetID;
        }

        [SerializeField] private List<Entry> m_entries = new();
        [SerializeField] private List<SerializableNode> m_nodes = new();

        private Dictionary<string, DialogueNode> m_node_map;
        private Dictionary<string, string> m_entry_map;

        private void OnEnable()
            => BuildCache();

        private void BuildCache()
        {
            m_node_map = new(StringComparer.Ordinal);
            m_entry_map = new(StringComparer.Ordinal);

            foreach(var entry in m_entries)
            {
                if(string.IsNullOrEmpty(entry.DialogueID) || string.IsNullOrEmpty(entry.EntryNodeID))
                    continue;

                m_entry_map[entry.DialogueID] = entry.EntryNodeID;
            }

            foreach(var node in m_nodes)
            {
                if(string.IsNullOrEmpty(node.ID))
                    continue;

                DialogueNode built = node.Type switch
                {
                    SerializableNodeType.LINE       => new LineNode(node.ID,
                                                                    new SpeakerRef(node.Speaker, node.CharacterID),
                                                                    node.Text ?? string.Empty,
                                                                    node.PortraitKey ?? string.Empty,
                                                                    node.NextID ?? string.Empty),
                    SerializableNodeType.CHOICE     => new ChoiceNode(node.ID,
                                                                      node.Prompt ?? string.Empty,
                                                                      BuildOptions(node.Options)),
                    SerializableNodeType.JUMP       => new JumpNode(node.ID, 
                                                                    node.TargetID ?? string.Empty),
                    SerializableNodeType.END        => new EndNode(node.ID),
                    _                               => new EndNode(node.ID)
                };

                m_node_map[node.ID] = built;
            }
        }

        private static List<ChoiceOption> BuildOptions(List<SerializableChoiceOption> source)
        {
            if(source == null || source.Count == 0)
                return new();

            var list = new List<ChoiceOption>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                var s = source[i];
                list.Add(new ChoiceOption(s.Text ?? string.Empty,
                                        s.NextID ?? string.Empty));
            }

            return list;
        }

        public bool TryGetNode(string node_id, out DialogueNode node)
        {
            if(m_node_map == null)
                BuildCache();

            return m_node_map.TryGetValue(node_id, out node);
        }

        public string GetEntryNodeID(string dialogue_id)
        {
            if(m_entry_map == null)
                BuildCache();

            return m_entry_map.TryGetValue(dialogue_id, out var id) ? id : string.Empty;
        }
    }
}


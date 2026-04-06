using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace DialogueBox
{
    public class CSVDialogueRawSource : IDialogueRawSource
    {
        private readonly TextAsset m_node_csv;
        private readonly TextAsset m_choice_csv;

        private bool m_built;

        private readonly List<DialogueRawEntry> m_entries = new();
        private readonly List<DialogueRawNode> m_nodes = new();

        public CSVDialogueRawSource(TextAsset node_csv, TextAsset choice_csv)
        {
            m_node_csv = node_csv;
            m_choice_csv = choice_csv;
        }

        public IEnumerable<DialogueRawEntry> ReadEntries()
        {
            EnsureBuilt();

            return m_entries;
        }

        public IEnumerable<DialogueRawNode> ReadNodes()
        {
            EnsureBuilt();

            return m_nodes;
        }

        private void EnsureBuilt()
        {
            if(m_built)
                return;

            m_built = true;

            if(m_node_csv == null)
                throw new ArgumentNullException(nameof(m_node_csv), "CSVDialogueRawSource: DialogueNode's CSV is null.");

            var node_rows = SimpleCSV.Parse(m_node_csv.text);
            if(node_rows.Count <= 1)
                return;

            var node_header = node_rows[0];
            int idx_dialogue_id     = FindCol(node_header, "DialogueID");
            int idx_node_id         = FindCol(node_header, "NodeID");
            int idx_type            = FindCol(node_header, "Type");
            int idx_speaker         = FindCol(node_header, "Speaker");
            int idx_character_id    = FindCol(node_header, "CharacterID");
            int idx_portrait_key    = FindCol(node_header, "PortraitKey");
            int idx_text            = FindCol(node_header, "Text");
            int idx_next_id         = FindCol(node_header, "NextID");

            var node_map = new Dictionary<(string dialogue_id, string node_id), DialogueRawNode>();

            var first_node_by_dialogue = new Dictionary<string, string>();

            for(int r = 1; r < node_rows.Count; r++)
            {
                var row = node_rows[r];
                if(IsBlankRow(row))
                    continue;

                string dialogue_id = Get(row, idx_dialogue_id);
                string node_id = Get(row, idx_node_id);

                if(string.IsNullOrWhiteSpace(dialogue_id) || string.IsNullOrWhiteSpace(node_id))
                    continue;

                if(!first_node_by_dialogue.ContainsKey(dialogue_id))
                    first_node_by_dialogue[dialogue_id] = node_id;

                var type_str = Get(row, idx_type);
                if(!EnumTryParse(type_str, out DialogueRawNodeType node_type))
                    throw new Exception($"CSVDialogueRawSource: [DialogueNode.csv] NodeType parsing failed. DialogueID={dialogue_id}, NodeID={node_id}, Type={type_str}");

                var raw_node = new DialogueRawNode
                {
                    ID = node_id,
                    Type = node_type
                };

                if(node_type == DialogueRawNodeType.LINE)
                {
                    raw_node.Speaker        = ParseSpeaker(Get(row, idx_speaker));
                    raw_node.CharacterID    = NullIfEmpty(Get(row, idx_character_id));
                    raw_node.PortraitKey    = NullIfEmpty(Get(row, idx_portrait_key));
                    raw_node.Text           = UnescapeNewlines(NullIfEmpty(Get(row, idx_text)));
                    raw_node.NextID         = NullIfEmpty(Get(row, idx_next_id));
                }
                else if(node_type == DialogueRawNodeType.CHOICE)
                {
                    raw_node.Prompt         = UnescapeNewlines(NullIfEmpty(Get(row, idx_text)));
                    raw_node.Options        = new List<DialogueRawChoiceOption>();
                }
                else if(node_type == DialogueRawNodeType.END)
                {}

                var key = (dialogue_id, node_id);
                if(node_map.ContainsKey(key))
                    throw new Exception($"CSVDialogueRawSource: [DialogueNode.csv] has duplicate data. DialogueID={dialogue_id}, NodeID={node_id}");

                node_map[key] = raw_node;
            }

            if(m_choice_csv != null && !string.IsNullOrWhiteSpace(m_choice_csv.text))
            {
                var choice_rows = SimpleCSV.Parse(m_choice_csv.text);
                
                if(choice_rows.Count > 1)
                {
                    var choice_header = choice_rows[0];
                    int c_idx_dialogue_id   = FindCol(choice_header, "DialogueID");
                    int c_idx_node_id       = FindCol(choice_header, "NodeID");
                    int c_idx_option_idx    = FindCol(choice_header, "OptionIndex");
                    int c_idx_option_text   = FindCol(choice_header, "OptionText");
                    int c_idx_next_id       = FindCol(choice_header, "NextID");

                    var option_buckets = new Dictionary<(string dialogue_id, string node_id), List<(int idx, DialogueRawChoiceOption opt)>>();

                    for(int r = 1; r < choice_rows.Count; r++)
                    {
                        var row = choice_rows[r];
                        if(IsBlankRow(row))
                            continue;

                        string dialogue_id = Get(row, c_idx_dialogue_id);
                        string node_id = Get(row, c_idx_node_id);
                        if(string.IsNullOrWhiteSpace(dialogue_id) || string.IsNullOrWhiteSpace(node_id))
                            continue;

                        int option_index = ParseIntSafe(Get(row, c_idx_option_idx), 0);
                        string option_text = UnescapeNewlines(NullIfEmpty(Get(row, c_idx_option_text)));
                        string next_id = NullIfEmpty(Get(row, c_idx_next_id));

                        var opt = new DialogueRawChoiceOption
                        {
                            Text = option_text,
                            NextID = next_id
                        };

                        var key = (dialogue_id, node_id);
                        if(!option_buckets.TryGetValue(key, out var list))
                        {
                            list = new List<(int, DialogueRawChoiceOption)>();
                            option_buckets[key] = list;
                        }
                        list.Add((option_index, opt));
                    }

                    foreach(var kv in option_buckets)
                    {
                        
                        if(!node_map.TryGetValue(kv.Key, out var raw_node))
                            throw new Exception($"CSVDialogueRawSource: [DialogueNode.csv] Node does not exist in DialogueNode. DialogueID={kv.Key.dialogue_id}, NodeID={kv.Key.node_id}");

                        if(raw_node.Type != DialogueRawNodeType.CHOICE)
                            throw new Exception($"CSVDialogueRawSource: [DialogueNode.csv] Node type is not CHOICE. DialogueID={kv.Key.dialogue_id}, NodeID={kv.Key.node_id}, Type={raw_node.Type}");
                    
                        var ordered = kv.Value
                            .OrderBy(x => x.idx <= 0 ? int.MaxValue : x.idx)
                            .ThenBy(x => x.idx)
                            .Select(x => x.opt)
                            .ToList();

                        raw_node.Options = ordered;
                        node_map[kv.Key] = raw_node;
                    }
                }
            }

            m_nodes.Clear();
            m_nodes.AddRange(node_map.Values);

            foreach(var kv in first_node_by_dialogue)
            {
                m_entries.Add(new DialogueRawEntry
                {
                    DialogueID = kv.Key,
                    EntryNodeID = kv.Value
                });
            }

            ValidateLinks(node_map);
        }

        private static void ValidateLinks(Dictionary<(string dialogue_id, string node_id), DialogueRawNode> node_map)
        {
            var set_by_dialogue = new Dictionary<string, HashSet<string>>();
            foreach (var key in node_map.Keys)
            {
                if (!set_by_dialogue.TryGetValue(key.dialogue_id, out var set))
                {
                    set = new HashSet<string>();
                    set_by_dialogue[key.dialogue_id] = set;
                }
                set.Add(key.node_id);
            }

            foreach (var kv in node_map)
            {
                var dialogue_id = kv.Key.dialogue_id;
                var node = kv.Value;

                if (node.Type == DialogueRawNodeType.LINE && !string.IsNullOrWhiteSpace(node.NextID))
                {
                    if (!set_by_dialogue[dialogue_id].Contains(node.NextID))
                        Debug.LogWarning($"CSVDialogueRawSource: [Dialogue] NextID does not exist. DialogueID={dialogue_id}, NodeID={node.ID}, NextID={node.NextID}");
                }

                if (node.Type == DialogueRawNodeType.CHOICE && node.Options != null)
                {
                    foreach (var opt in node.Options)
                    {
                        if (string.IsNullOrWhiteSpace(opt.NextID)) 
                            continue;

                        if (!set_by_dialogue[dialogue_id].Contains(opt.NextID))
                            Debug.LogWarning($"CSVDialogueRawSource: [Dialogue] Choice NextID does not exist. DialogueID={dialogue_id}, NodeID={node.ID}, OptionText={opt.Text}, NextID={opt.NextID}");
                    }
                }
            }
        }

        private static Speaker ParseSpeaker(string s)
        {
            s = (s ?? "").Trim();
            if (string.IsNullOrEmpty(s)) return default;

            if (EnumTryParse(s, out Speaker sp)) return sp;

            var upper = s.ToUpperInvariant();
            if (upper == "PLAYER") return Speaker.PLAYER;
            if (upper == "NPC") return Speaker.NPC;

            throw new Exception($"CSVDialogueRawSource: Speaker parsing failed. {s}");
        }

        private static bool EnumTryParse<T>(string value, out T result) where T : struct
            => Enum.TryParse(value?.Trim(), ignoreCase: true, out result);

        private static int ParseIntSafe(string s, int fallback)
        {
            if (int.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                return v;

            return fallback;
        }

        private static int FindCol(List<string> header, string name)
        {
            for (int i = 0; i < header.Count; i++)
            {
                var h = NormalizeHeader(header[i]);
                if (string.Equals(h, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        private static string NormalizeHeader(string s)
        {
            if (s == null) return "";
            
            return s.Trim()
                    .TrimStart('\uFEFF')
                    .TrimStart('\u200B');
        }

        private static string Get(List<string> row, int idx)
        {
            if (idx < 0) 
                return null;

            if (row == null || idx >= row.Count) 
                return null;

            return row[idx]?.Trim();
        }

        private static string NullIfEmpty(string s)
        {
            s = s?.Trim();
            return string.IsNullOrEmpty(s) ? null : s;
        }

        private static string UnescapeNewlines(string s)
        {
            if (s == null) 
                return null;

            return s.Replace("\\n", "\n");
        }

        private static bool IsBlankRow(List<string> row)
        {
            if (row == null) 
                return true;

            for (int i = 0; i < row.Count; i++)
                if (!string.IsNullOrWhiteSpace(row[i]))
                    return false;

            return true;
        }
    }
}
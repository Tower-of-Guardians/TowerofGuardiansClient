using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueBox
{
    public sealed class CSVCharacterRawSource : ICharacterRawSource
    {
        private readonly TextAsset m_character_csv;
        private bool m_built;

        private readonly List<CharacterRawEntry> m_entries = new();

        public CSVCharacterRawSource(TextAsset character_csv)
            => m_character_csv = character_csv;

        public IEnumerable<CharacterRawEntry> ReadCharacters()
        {
            EnsureBuilt();
            return m_entries;
        }

        private void EnsureBuilt()
        {
            if (m_built)
                return;

            m_built = true;

            if (m_character_csv == null)
                throw new ArgumentNullException(nameof(m_character_csv), "CSVCharacterRawSource: Character CSV가 null입니다.");

            var rows = SimpleCSV.Parse(m_character_csv.text);
            if (rows.Count <= 1)
                return;

            var header = rows[0];
            int idx_character_id = FindCol(header, "CharacterID");
            int idx_display_name = FindCol(header, "DisplayName");

            if (idx_character_id < 0)
                throw new Exception("CSVCharacterRawSource: [Character.csv] 'CharacterID' 컬럼을 찾을 수 없습니다.");
            if (idx_display_name < 0)
                throw new Exception("CSVCharacterRawSource: [Character.csv] 'DisplayName' 컬럼을 찾을 수 없습니다.");

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int r = 1; r < rows.Count; r++)
            {
                var row = rows[r];
                if (IsBlankRow(row))
                    continue;

                string character_id = NullIfEmpty(Get(row, idx_character_id));
                string display_name = NullIfEmpty(Get(row, idx_display_name));

                if (string.IsNullOrWhiteSpace(character_id))
                    continue;

                if (!seen.Add(character_id))
                    throw new Exception($"CSVCharacterRawSource: [Character.csv] CharacterID 중복: {character_id}");

                m_entries.Add(new CharacterRawEntry
                {
                    CharacterID = character_id,
                    DisplayName = display_name ?? string.Empty
                });
            }
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

            return row[idx]?.Trim()
                           ?.TrimStart('\uFEFF')
                           ?.TrimStart('\u200B');
        }

        private static string NullIfEmpty(string s)
        {
            s = s?.Trim();
            return string.IsNullOrEmpty(s) ? null : s;
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
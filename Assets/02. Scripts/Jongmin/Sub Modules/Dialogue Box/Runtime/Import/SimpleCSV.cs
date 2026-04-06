using System.Collections.Generic;
using System.Linq;

namespace DialogueBox
{
    internal static class SimpleCSV
    {
        public static List<List<string>> Parse(string csvText)
        {
            var rows = new List<List<string>>();
            if (string.IsNullOrEmpty(csvText)) return rows;

            int i = 0;
            int len = csvText.Length;

            List<string> row = new List<string>();
            var field = new System.Text.StringBuilder();
            bool inQuotes = false;

            while (i < len)
            {
                char c = csvText[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < len && csvText[i + 1] == '"')
                        {
                            field.Append('"');
                            i += 2;
                            continue;
                        }

                        inQuotes = false;
                        i++;
                        continue;
                    }

                    field.Append(c);
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = true;
                    i++;
                    continue;
                }

                if (c == ',')
                {
                    row.Add(field.ToString());
                    field.Length = 0;
                    i++;
                    continue;
                }

                if (c == '\r' || c == '\n')
                {
                    row.Add(field.ToString());
                    field.Length = 0;

                    rows.Add(row);
                    row = new List<string>();

                    if (c == '\r' && i + 1 < len && csvText[i + 1] == '\n')
                        i += 2;
                    else
                        i++;

                    continue;
                }

                field.Append(c);
                i++;
            }

            row.Add(field.ToString());
            rows.Add(row);

            while (rows.Count > 0 && rows[^1].All(string.IsNullOrWhiteSpace))
                rows.RemoveAt(rows.Count - 1);

            return rows;
        }
    }
}
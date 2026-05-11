using System.Collections.Generic;
using JxModule;
using JxModule.DataTable;
using UnityEngine;

namespace JxDialogueBox
{
    public sealed class DialoguePortraitTable
    {
        private readonly DataTable _table;
        private readonly Dictionary<string, Sprite> _spriteDict = new();

        public DialoguePortraitTable()
        {
            _table = DataTableManager.FindTable<DialoguePortraitDataTableRow>("DT_DialoguePortrait");
        }

        public Sprite GetPortraitSprite(string characterID, string portraitKey)
        {
            if (string.IsNullOrEmpty(characterID))
            {
                return null;
            }

            if (string.IsNullOrEmpty(portraitKey))
            {
                portraitKey = "default";
            }

            var rowID = MakeRowID(characterID, portraitKey);

            if (_spriteDict.TryGetValue(rowID, out var cached))
            {
                return cached;
            }

            var row = _table.Find<DialoguePortraitDataTableRow>(rowID);

            if (row?.portraitTexture == null)
            {
                return null;
            }

            var sprite = row.portraitTexture.ToSprite();
            _spriteDict[rowID] = sprite;

            return sprite;
        }

        private static string MakeRowID(string characterID, string portraitKey)
        {
            return $"{characterID}_{portraitKey}";
        }
    }
}
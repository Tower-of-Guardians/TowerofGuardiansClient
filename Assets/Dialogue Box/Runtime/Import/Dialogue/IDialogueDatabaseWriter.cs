using System.Collections.Generic;

namespace DialogueBox
{
    public interface IDialogueDatabaseWriter
    {
        void Overwrite(DialogueDatabase db,
                       IEnumerable<DialogueRawEntry> entries,
                       IEnumerable<DialogueRawNode> nodes);
    }
}
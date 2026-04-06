using System.Collections.Generic;

namespace DialogueBox
{
    public interface IDialogueRawSource
    {
        IEnumerable<DialogueRawEntry> ReadEntries();
        IEnumerable<DialogueRawNode> ReadNodes();
    }
}
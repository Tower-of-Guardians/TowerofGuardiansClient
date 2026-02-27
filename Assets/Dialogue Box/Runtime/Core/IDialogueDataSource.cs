namespace DialogueBox
{
    public interface IDialogueDataSource
    {
        bool TryGetNode(string node_id, out DialogueNode node);
        string GetEntryNodeID(string dialogue_id);
    }
}
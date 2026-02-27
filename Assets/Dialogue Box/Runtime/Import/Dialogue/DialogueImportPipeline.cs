using System.Linq;

namespace DialogueBox
{
    public static class DialogueImportPipeline
    {
        public static void Import(IDialogueRawSource source, 
                                  IDialogueDatabaseWriter writer, 
                                  DialogueDatabase target)
        {
            var entries = source.ReadEntries();
            var nodes = source.ReadNodes();
            writer.Overwrite(target, entries, nodes);
        }
    }
}
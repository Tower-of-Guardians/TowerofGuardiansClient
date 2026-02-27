namespace DialogueBox
{
    public static class CharacterImportPipeline
    {
        public static void Import(ICharacterRawSource source, 
                                  ICharacterDatabaseWriter writer,
                                  CharacterDatabase target)
        {
            if (source == null || writer == null || target == null)
                return;

            writer.Overwrite(target, source.ReadCharacters());
        }
    }
}
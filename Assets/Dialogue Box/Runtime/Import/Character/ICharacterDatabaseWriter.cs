using System.Collections.Generic;

namespace DialogueBox
{
    public interface ICharacterDatabaseWriter
    {
        void Overwrite(CharacterDatabase db,
                       IEnumerable<CharacterRawEntry> characters);
    }
}
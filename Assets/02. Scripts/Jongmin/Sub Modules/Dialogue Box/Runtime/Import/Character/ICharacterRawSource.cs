using System.Collections.Generic;

namespace DialogueBox
{
    public interface ICharacterRawSource
    {
        IEnumerable<CharacterRawEntry> ReadCharacters();
    }
}
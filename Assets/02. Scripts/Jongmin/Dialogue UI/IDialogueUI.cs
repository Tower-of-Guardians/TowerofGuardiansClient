public interface IDialogueUI
{
    void InitPortrait(CharacterCode player_code, CharacterCode npc_code);
    void StartDialogue(string dialogue_code);
    void StopDialogue();
}
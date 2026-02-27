namespace DialogueBox
{
    public enum Speaker
    {
        PLAYER,
        NPC,
    }

    public readonly struct SpeakerRef
    {
        public readonly Speaker m_speaker;
        public readonly string m_character_id;

        public SpeakerRef(Speaker speaker, string character_id)
        {
            m_speaker = speaker;
            m_character_id = character_id;
        }

        public static SpeakerRef Player(string player_id = "Player")
            => new(Speaker.PLAYER, player_id);

        public static SpeakerRef NPC(string npc_id)
            => new(Speaker.NPC, npc_id);
    }
}
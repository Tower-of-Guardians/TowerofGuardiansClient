using UnityEngine;

namespace DialogueBox
{
    public class PortraitPanelView : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PortraitSlotView m_player_slot;

        [Header("NPC")]
        [SerializeField] private PortraitSlotView m_npc_slot;

        [Header("Alpha")]
        [Range(0f, 1f)][SerializeField] private float m_active_alpha = 1f;
        [Range(0f, 1f)][SerializeField] private float m_deactive_alpha = 0.35f;

        private void Awake()
        {
            m_player_slot.SetCharacter("player");
            m_player_slot.SetPortraitByKey("default");
        }

        public void ApplySpeaker(SpeakerRef speaker, string portrait_key)
        {
            if(speaker.m_speaker == Speaker.PLAYER)
            {
                if(m_player_slot)
                {
                    m_player_slot.SetAlpha(m_active_alpha);

                    if (!string.IsNullOrEmpty(portrait_key))
                        m_player_slot.SetPortraitByKey(portrait_key);
                }

                if(m_npc_slot)
                    m_npc_slot.SetAlpha(m_deactive_alpha);

                return;
            }

            if(m_player_slot)
                m_player_slot.SetAlpha(m_deactive_alpha);

            if (m_npc_slot)
            {
                m_npc_slot.SetAlpha(m_active_alpha);
                m_npc_slot.SetCharacter(speaker.m_character_id);

                if (!string.IsNullOrEmpty(portrait_key))
                    m_npc_slot.SetPortraitByKey(portrait_key);
            }
        }
    }
}


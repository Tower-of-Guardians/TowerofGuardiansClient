using UnityEngine;
using UnityEngine.UI;

namespace DialogueBox
{
    public class PortraitSlotView : MonoBehaviour
    {
        
        [Header("Portrait Database")]
        [SerializeField] private PortraitDatabase m_db;

        [Header("Portrait Image")]
        [SerializeField] private Image m_portrait_image;

        private string m_character_id;
        private string m_default_key = "default";

        public string CharacterID => m_character_id;

        private void Awake()
        {
            if(m_db)
                m_db.BuildCache();
        }

        public void SetCharacter(string character_id, bool force_refresh = false)
        {
            if (string.IsNullOrWhiteSpace(character_id))
                return;

            if (!force_refresh && m_character_id == character_id)
                return;

            m_character_id = character_id;
            m_default_key = m_db != null ? m_db.GetDefaultKey(character_id) : "default";

            SetPortraitByKey(m_default_key);
        }

        public void SetPortraitByKey(string key)
        {
            if(m_portrait_image == null)
                return;

            if(m_db == null)
                return;

            if(string.IsNullOrWhiteSpace(m_character_id))
                return;

            if(string.IsNullOrEmpty(key))
                return;

            if(m_db.TryGetPortrait(m_character_id, key, out var spr))
            {
                m_portrait_image.sprite = spr;
                return;
            }

            if(!string.IsNullOrWhiteSpace(m_default_key) &&
                m_db.TryGetPortrait(m_character_id, m_default_key, out var def))
            {
                m_portrait_image.sprite = def;
            }
        }

        public bool IsPortraitEmpty()
            => m_portrait_image == null || m_portrait_image.sprite == null;

        public void SetAlpha(float alpha)
        {
            if(m_portrait_image == null)
                return;

            Color color = m_portrait_image.color;
            color.a = alpha;
            m_portrait_image.color = color;
        }
    }
}
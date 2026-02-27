using System.Collections;
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

        private Coroutine m_alpha_coroutine;

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

            if(m_alpha_coroutine != null)
                StopCoroutine(m_alpha_coroutine);

            m_alpha_coroutine = StartCoroutine(FadeAlphaRoutine(alpha, 0.3f));
        }

        private IEnumerator FadeAlphaRoutine(float target_alpha, float duration)
        {
            Color start_color = m_portrait_image.color;
            float start_alpha = start_color.a;

            float elapsed = 0f;

            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / duration;
                float new_alpha = Mathf.Lerp(start_alpha, target_alpha, t);

                start_color.a = new_alpha;
                m_portrait_image.color = start_color;

                yield return null;
            }

            // 마지막 보정
            start_color.a = target_alpha;
            m_portrait_image.color = start_color;

            m_alpha_coroutine = null;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace DialogueBox
{
    [RequireComponent(typeof(DialogueView))]
    public class DialogueAutoAdvanceDriver : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueSettings m_settings;

        [Header("Dialogue View")]
        [SerializeField] private DialogueView m_view;

        [Header("Typewriter")]
        [SerializeField] private TypeWriter m_typewriter;

        private Coroutine m_auto_coroutine;

        private void OnEnable()
        {
            if(m_typewriter)
                m_typewriter.OnCompleted += HandleTypingCompleted;
        }

        private void OnDisable()
        {
            if(m_typewriter)
                m_typewriter.OnCompleted -= HandleTypingCompleted;

            StopAutoAdvance();
        }

        private void HandleTypingCompleted()
        {
            if(m_settings == null || !m_settings.AutoAdvanceAllowed)
                return;


            StopAutoAdvance();
            m_auto_coroutine = StartCoroutine(AutoRoutine());
        }

        private IEnumerator AutoRoutine()
        {
            float delay = Mathf.Max(0f, m_settings.AutoAdvanceDelay);
            if(delay > 0f)
                yield return new WaitForSeconds(delay);

            m_auto_coroutine = null;
            m_view.RequestAdvance();
        }

        public void StopAutoAdvance()
        {
            if(m_auto_coroutine != null)
            {
                StopCoroutine(m_auto_coroutine);
                m_auto_coroutine = null;
            }
        }
    }
}


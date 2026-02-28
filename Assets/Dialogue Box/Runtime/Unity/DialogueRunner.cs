using System;
using UnityEngine;

namespace DialogueBox
{
    public sealed class DialogueRunner : MonoBehaviour
    {
        [Header("Dialogue Data")]
        [SerializeField] private DialogueDatabase m_db;

        [Header("Dialogue View")]
        [SerializeField] private DialogueView m_view;

        private DialogueEngine m_engine;

        private string m_current_dialogue_id;
        private bool m_is_running = false;

        public event Action<string> OnDialogueStarted;
        public event Action<string> OnDialogueEnded;

        private void Awake()
        {
            if(m_db == null || m_view == null)
            {
                enabled = false;
                return;
            }

            m_engine = new DialogueEngine(m_db);
            m_engine.OnLine += HandleLine;
            m_engine.OnChoice += HandleChoice;
            m_engine.OnEnded += HandleEnded;

            m_view.Bind(OnNextAction:   () => m_engine.Advance(),
                        OnChooseAction: (idx) => m_engine.Choose(idx));
        }

        private void OnDestroy()
        {
            if(m_engine == null)
                return;

            m_engine.OnLine -= HandleLine;
            m_engine.OnChoice -= HandleChoice;
            m_engine.OnEnded -= HandleEnded;
        }

        public void StartDialogue(string dialogue_id)
        {
            if(string.IsNullOrEmpty(dialogue_id))
                return;

            if(m_is_running)
                return;

            m_is_running = true;
            m_current_dialogue_id = dialogue_id;

            m_view.OpenView();
            OnDialogueStarted?.Invoke(m_current_dialogue_id);
            m_engine.Start(dialogue_id);
        }

        public void HandleLine(DialogueEngine.LineEvent e)
            => m_view.ShowLine(e.Speaker, e.Text, e.PortraitKey);

        public void HandleChoice(DialogueEngine.ChoiceEvent e)
            => m_view.ShowChoice(e.Prompt, e.Options);

        private void HandleEnded()
        {
            m_view.CloseView();
            OnDialogueEnded?.Invoke(m_current_dialogue_id);

            m_is_running = false;
            m_current_dialogue_id = null;
        }
    }
}
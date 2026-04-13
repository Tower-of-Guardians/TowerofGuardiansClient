using System;
using TMPro;
using UnityEngine;

namespace DialogueBox
{
    public class DialogueView : MonoBehaviour
    {
        [Header("Canvas Group")]
        [SerializeField] private CanvasGroup m_canvas_group;

        [Header("Name Text")]
        [SerializeField] private TMP_Text m_name_text;

        [Header("Typewriter")]
        [SerializeField] private TypeWriter m_type_writer;

        [Header("Choice Prompt Text")]
        [SerializeField] private TMP_Text m_prompt_text;

        [Header("Advance Button")]
        [SerializeField] private AdvanceButton m_advance_button;

        [Space(30f), Header("Dependancy")]
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueSettings m_settings;

        [Header("Character Database")]
        [SerializeField] private CharacterDatabase m_character_db;

        [Header("Choice List View")]
        [SerializeField] private ChoiceListView m_choice_list;

        [Header("Portrait View")]
        [SerializeField] private PortraitPanelView m_portraits;

        public event Action OnAdvanceRequested;

        private Action OnNext;
        private Action<int> OnChoose;

        public bool ChoiceMode { get; private set; }

        private void Awake()
        {
            m_type_writer.SetInterval(m_settings.TypingEnabled ? m_settings.TypingSecondsPerCharacter : 0f);
            m_type_writer.OnCompleted += m_advance_button.Hightlight;
        }

        private void OnDestroy()
            => m_type_writer.OnCompleted -= m_advance_button.Hightlight;

        public void Bind(Action OnNextAction, Action<int> OnChooseAction)
        {
            OnNext = OnNextAction;
            OnChoose = OnChooseAction;

            if(m_advance_button)
                m_advance_button.Bind(RequestAdvance);

            if(m_choice_list)
                m_choice_list.Bind(index => OnChoose?.Invoke(index));
        }

        public void OpenView()
            => ToggleView(true);

        public void CloseView()
            => ToggleView(false);

        private void ToggleView(bool active)
        {
            m_canvas_group.alpha = active ? 1f : 0f;
            m_canvas_group.interactable = active;
            m_canvas_group.blocksRaycasts = active;
        }

        public void ShowLine(SpeakerRef speaker, string text, string portrait_key)
        {
            ClearChoice();

            if(m_name_text)
                m_name_text.text = m_character_db ? m_character_db.ResolveName(speaker.m_character_id)
                                                  : speaker.m_character_id ?? string.Empty; 

            if(m_prompt_text)
                m_prompt_text.text = string.Empty;
            
            if(m_type_writer)
                m_type_writer.Play(text ?? string.Empty);

            if(m_portraits)
                m_portraits.ApplySpeaker(speaker, portrait_key);

            if(m_advance_button)
                m_advance_button.Show();
        }

        public void ShowChoice(string prompt, ChoiceOption[] options)
        {
            ChoiceMode = true;

            if(m_name_text)
                m_name_text.text = string.Empty;

            if(m_prompt_text)
                m_prompt_text.text = prompt ?? string.Empty;

            if(m_type_writer)
                m_type_writer.Play(string.Empty);

            if(m_advance_button)
                m_advance_button.Hide();

            if(m_choice_list)
                m_choice_list.Show(options);
        }

        private void ClearChoice()
        {
            if(m_choice_list) 
                m_choice_list.Hide();

            ChoiceMode = false;
        }

        public void MoveChoice(int delta)
        {
            if(!ChoiceMode)
                return;

            if(m_choice_list == null || m_choice_list.Count == 0)
                return;

            m_choice_list.MoveSelection(delta, true);
        }

        public void ConfirmChoice()
        {
            if(!ChoiceMode)
                return;

            if(m_choice_list == null || m_choice_list.Count == 0)
                return;

            m_choice_list.ConfirmSelection();
        }

        public void RequestAdvance()
        {
            if(ChoiceMode)
            {
                ConfirmChoice();
                return;
            }

            OnAdvanceRequested?.Invoke();

            if(m_advance_button && m_advance_button.Hiding)
                return;

            if(m_type_writer && m_type_writer.IsTyping)
            {
                if(m_settings.TypingSkipAllowed)
                {
                    m_advance_button.Hightlight();
                    m_type_writer.Skip();
                }
                return;
            }

            m_advance_button.Normal();
            OnNext?.Invoke();
        }
    }
}
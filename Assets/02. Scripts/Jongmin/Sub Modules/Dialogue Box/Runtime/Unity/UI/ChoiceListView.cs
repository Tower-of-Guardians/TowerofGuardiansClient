using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DialogueBox
{
    public class ChoiceListView : MonoBehaviour
    {
        [Header("Options Canvas Group")]
        [SerializeField] private CanvasGroup m_canvas_group;

        [Header("Option Container")]
        [SerializeField] private Transform m_container;

        [Header("Option Prefab")]
        [SerializeField] private Button m_option_prefab;

        private Action<int> OnChoose;

        private readonly List<Button> m_buttons = new();
        private int m_selected_index = -1;

        public int Count => m_buttons.Count;
        public int SelectedIndex => m_selected_index;

        public void Bind(Action<int> OnChooseAction)
            => OnChoose = OnChooseAction;

        public void Show(ChoiceOption[] options)
        {
            if(m_canvas_group)
                ToggleCanvasGroup(true);

            Clear();

            if(options == null)
                return;

            for(int i = 0; i < options.Length; i++)
            {
                var option_obj = Instantiate(m_option_prefab, m_container);
                m_buttons.Add(option_obj);
                var index = i;

                var tmp = option_obj.GetComponentInChildren<TMP_Text>();
                if(tmp)
                    tmp.text = "· " + options[index].Text;

                option_obj.onClick.AddListener(() => OnChoose?.Invoke(index));
            }

            SetSelected(0);
        }

        public void Hide()
        {
            if(m_canvas_group)
                ToggleCanvasGroup(false);

            Clear();
        }

        public void SetSelected(int index)
        {
            if(m_buttons.Count == 0)
            {
                m_selected_index = -1;
                return;
            }

            m_selected_index = Mathf.Clamp(index, 0, m_buttons.Count - 1);
            
            if(EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(m_buttons[m_selected_index].gameObject);
        }

        public void MoveSelection(int delta, bool wrap = true)
        {
            if(m_buttons.Count == 0)
                return;

            int next_index = m_selected_index + delta;

            if(wrap)
            {
                if(next_index < 0)
                    next_index = m_buttons.Count - 1;
                if(next_index >= m_buttons.Count)
                    next_index = 0;
            }
            else
                next_index = Mathf.Clamp(next_index, 0, m_buttons.Count - 1);

            SetSelected(next_index);
        }

        public void ConfirmSelection()
        {
            if(m_selected_index < 0 || m_selected_index >= m_buttons.Count)
                return;

            Choose(m_selected_index);
        }

        private void Choose(int index)
        {
            if(index < 0 || index >= m_buttons.Count)
                return;

            OnChoose?.Invoke(index);            
        }

        private void Clear()
        {
            if(m_container == null)
                return;

            for(int i = m_container.childCount - 1; i >= 0; i--)
                Destroy(m_container.GetChild(i).gameObject);

            m_buttons.Clear();
            m_selected_index = -1;
        }

        private void ToggleCanvasGroup(bool active)
        {
            m_canvas_group.alpha = active ? 1f : 0f;
            m_canvas_group.blocksRaycasts = active;
            m_canvas_group.interactable = active;            
        }
    }
}

using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DialogueBox
{
    [RequireComponent(typeof(DialogueView))]
    public sealed class DialogueInputDriver : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueSettings m_settings;

        [Header("Dialogue View")]
        [SerializeField] private DialogueView m_view;

#if ENABLE_INPUT_SYSTEM
        private InputAction m_advance_action;
        private InputAction m_select_action;
#endif

        private void OnEnable()
        {
            if(!m_view)
                return;

#if ENABLE_INPUT_SYSTEM
            if(m_settings && m_settings.KeyboardInputAllowed)
            {
                if(m_settings.AdvanceAction)
                {
                    m_advance_action = m_settings.AdvanceAction.action;
                    m_advance_action.performed += OnAdvanced;
                    m_advance_action.Enable();
                }

                if(m_settings.SelectionAction)
                {
                    m_select_action = m_settings.SelectionAction.action;
                    m_select_action.performed += OnSelected;
                    m_select_action.Enable();
                }
            }
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            if (m_advance_action != null)
            {
                m_advance_action.performed -= OnAdvanced;
                m_advance_action.Disable();
                m_advance_action = null;
            }

            if(m_select_action != null)
            {
                m_select_action.performed -= OnSelected;
                m_select_action.Disable();
                m_select_action = null;
            }
#endif            
        }

#if ENABLE_INPUT_SYSTEM
        private void OnAdvanced(InputAction.CallbackContext ctx)
        {
            if (m_settings == null || !m_settings.KeyboardInputAllowed) 
                return;

            var device = ctx.control?.device;
            if (device is Pointer)
                return;

            m_view.RequestAdvance();
        }

        private void OnSelected(InputAction.CallbackContext ctx)
        {
            if(m_settings == null || !m_settings.KeyboardInputAllowed)
                return;

            if(!m_view.ChoiceMode)
                return;

            Vector2 v = ctx.ReadValue<Vector2>();
            if(v.y > 0.5f)
                m_view.MoveChoice(-1);
            else if(v.y < -0.5f)
                m_view.MoveChoice(1);
        }
#endif
    }
}
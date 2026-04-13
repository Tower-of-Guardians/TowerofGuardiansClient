using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogueBox
{
    [CreateAssetMenu(fileName = "Dialogue Settings", menuName = "Dialogue Box/Dialogue Settings")]
    public sealed class DialogueSettings : ScriptableObject
    {
        [Header("Typewriter")]
        [Header("Enable typing effect")]
        public bool TypingEnabled = true;

        [Min(0f)][Header("Typing seconds per character")]
        public float TypingSecondsPerCharacter = 0.03f;

        [Header("Allow typing skip")]
        public bool TypingSkipAllowed = true;

        [Space(30f), Header("Input")]
        [Header("Allow keyboard input")]
        public bool KeyboardInputAllowed = true;

        #if ENABLE_INPUT_SYSTEM
        [Header("Advance Action Reference")]
        public InputActionReference AdvanceAction;
        #endif

        #if ENABLE_INPUT_SYSTEM
        [Header("Selection Action Reference")]
        public InputActionReference SelectionAction;
        #endif

        [Space(30f), Header("Auto Advance")]
        [Header("Enable Auto Advance")]
        public bool AutoAdvanceAllowed = false;

        [Min(0f)][Header("Auto Advance Delay")]
        public float AutoAdvanceDelay = 3f;
    }
}


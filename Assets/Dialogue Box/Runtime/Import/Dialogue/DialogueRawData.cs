using System;
using System.Collections.Generic;

namespace DialogueBox
{
    [Serializable]
    public struct DialogueRawEntry
    {
        public string DialogueID;
        public string EntryNodeID;
    }

    public enum DialogueRawNodeType
    {
        LINE,
        CHOICE,
        JUMP,
        END,
    }

    [Serializable]
    public struct DialogueRawChoiceOption
    {
        public string Text;
        public string NextID;
    }

    [Serializable]
    public class DialogueRawNode
    {
        public string ID;
        public DialogueRawNodeType Type;

        public Speaker Speaker;
        public string CharacterID;
        public string Text;
        public string PortraitKey;
        public string NextID;

        public string Prompt;
        public List<DialogueRawChoiceOption> Options;

        public string TargetID;        
    }
}

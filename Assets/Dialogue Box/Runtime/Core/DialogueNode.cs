using System;
using System.Collections.Generic;

namespace DialogueBox
{
    public enum NodeType
    {
        LINE,
        CHOICE,
        JUMP,
        END
    }

    public abstract class DialogueNode
    {
        public string ID { get; }
        public NodeType Type { get; }

        protected DialogueNode(string id, NodeType type)
        {
            ID = id ?? throw new ArgumentException(nameof(id));
            Type = type;
        }
    }

    public sealed class LineNode : DialogueNode
    {
        public SpeakerRef Speaker { get; }
        public string Text { get; }
        public string PortraitKey { get; }
        public string NextID { get; }

        public LineNode(string id,
                        SpeakerRef speaker,
                        string text,
                        string portrait_key,
                        string next_id)
            : base(id, NodeType.LINE)
        {
            Speaker = speaker;
            Text = text;
            PortraitKey = portrait_key;
            NextID = next_id;
        }
    }

    public readonly struct ChoiceOption
    {
        public readonly string Text;
        public readonly string NextID;

        public ChoiceOption(string text, string next_id)
        {
            Text = text ?? string.Empty;
            NextID = next_id ?? string.Empty;
        }
    }

    public sealed class ChoiceNode : DialogueNode
    {
        public string Prompt { get; }
        public IReadOnlyList<ChoiceOption> Options { get; }

        public ChoiceNode(string id,
                          string prompt,
                          IReadOnlyList<ChoiceOption> options)
            : base(id, NodeType.CHOICE)
        {
            Prompt = prompt ?? string.Empty;
            Options = options ?? Array.Empty<ChoiceOption>();
        }
    }

    public sealed class JumpNode : DialogueNode
    {
        public string TargetID { get; }

        public JumpNode(string id, string target_id)
            : base(id, NodeType.JUMP)
        {
            TargetID = target_id ?? string.Empty;
        }
    }

    public sealed class EndNode : DialogueNode
    {
        public EndNode(string id)
            : base(id, NodeType.END)
        {}
    }
}
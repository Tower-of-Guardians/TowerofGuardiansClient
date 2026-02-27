using System;

namespace DialogueBox
{
    public sealed class DialogueEngine
    {
        public enum EngineState
        {
            IDLE,
            SHOWING_LINE,
            AWAITING_CHOICE,
            ENDED,
        }

        public readonly struct LineEvent
        {
            public readonly SpeakerRef Speaker;
            public readonly string Text;
            public readonly string PortraitKey;
            public readonly string NodeID;

            public LineEvent(SpeakerRef speaker, 
                             string text, 
                             string portrait_key, 
                             string node_id)
            {
                Speaker = speaker;
                Text = text ?? string.Empty;
                PortraitKey = portrait_key ?? string.Empty;
                NodeID = node_id;
            }
        }

        public readonly struct ChoiceEvent
        {
            public readonly string Prompt;
            public readonly ChoiceOption[] Options;
            public readonly string NodeID;

            public ChoiceEvent(string prompt, 
                               ChoiceOption[] options, 
                               string node_id)
            {
                Prompt = prompt ?? string.Empty;
                Options = options ?? Array.Empty<ChoiceOption>();
                NodeID = node_id;
            }
        }

        public event Action<LineEvent> OnLine;
        public event Action<ChoiceEvent> OnChoice;
        public event Action OnEnded;

        public EngineState State { get; private set; } = EngineState.IDLE;
        public string CurrentNodeID { get; private set; } = string.Empty;

        private readonly IDialogueDataSource m_source;

        public DialogueEngine(IDialogueDataSource source)
            => m_source = source;

        public void Start(string dialogue_id)
        {
            var entry_id = m_source.GetEntryNodeID(dialogue_id);
            if(string.IsNullOrEmpty(entry_id))
            {
                EndInternal();
                return;
            } 

            State = EngineState.IDLE;
            Goto(entry_id);
        }

        public void Advance()
        {
            if(State == EngineState.SHOWING_LINE)
            {
                if(m_source.TryGetNode(CurrentNodeID, out var node) && node is LineNode line)
                {
                    if(string.IsNullOrEmpty(line.NextID))
                    {
                        EndInternal();
                        return;
                    }

                    Goto(line.NextID);
                }

                return;
            }

            if(State == EngineState.AWAITING_CHOICE)
                return;
        }

        public void Choose(int option_index)
        {
            if(State != EngineState.AWAITING_CHOICE)
                return;

            if(!m_source.TryGetNode(CurrentNodeID, out var node) || node is not ChoiceNode choice)
                return;

            if(choice.Options == null || option_index < 0 || option_index >= choice.Options.Count)
                return;

            var next_id = choice.Options[option_index].NextID;
            if(string.IsNullOrEmpty(next_id))
            {
                EndInternal();
                return;
            }

            Goto(next_id);
        }

        private void Goto(string node_id)
        {
            while(true)
            {
                CurrentNodeID = node_id;

                if(!m_source.TryGetNode(CurrentNodeID, out var node))
                {
                    EndInternal();
                    return;
                }

                switch(node.Type)
                {
                    case NodeType.LINE:
                    {
                        var line_node = node as LineNode;
                        State = EngineState.SHOWING_LINE;
                        OnLine?.Invoke(new LineEvent(line_node.Speaker, line_node.Text, line_node.PortraitKey, line_node.ID));
                        return;
                    }

                    case NodeType.CHOICE:
                    {
                        var choice_node = node as ChoiceNode;
                        State = EngineState.AWAITING_CHOICE;

                        var options = new ChoiceOption[choice_node.Options.Count];
                        for(int i = 0; i < options.Length; i++)
                            options[i] = choice_node.Options[i];

                        OnChoice?.Invoke(new ChoiceEvent(choice_node.Prompt, options, choice_node.ID));
                        return;
                    }

                    case NodeType.JUMP:
                    {
                        var jump_node = node as JumpNode;
                        if(string.IsNullOrEmpty(jump_node.TargetID))
                        {
                            EndInternal();
                            return;
                        }

                        node_id = jump_node.TargetID;
                        break;
                    }

                    case NodeType.END:
                    default:
                        EndInternal();
                        return;
                }
            }
        }

        private void EndInternal()
        {
            State = EngineState.ENDED;
            CurrentNodeID = string.Empty;
            OnEnded?.Invoke();
        }
    }
}
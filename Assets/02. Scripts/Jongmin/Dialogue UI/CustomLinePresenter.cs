using UnityEngine;
using Yarn.Unity;
using Yarn.Markup;
using Yarn.Unity.Attributes;
using TMPro;
using System.Collections.Generic;

#nullable enable

public class CustomLinePresenter : CustomDialoguePresenter
{
    internal enum TypewriterType
    {
        Instant, ByLetter, ByWord
    }

    [Header("필요 컴포넌트")]
    [MustNotBeNull]
    public CanvasGroup? m_canvas_group;
    
    [MustNotBeNull]
    public TMP_Text? m_line_text;

    [Space(30f), Header("이름 설정")]
    [Group("Character"), SerializeField] private bool m_show_name = true;
    [Group("Character"), ShowIf(nameof(m_show_name)), SerializeField] private TMP_Text? m_character_name_text;
    [Group("Character"), ShowIf(nameof(m_show_name)), SerializeField] private GameObject? m_character_name_container;

    [Space(30f), Header("페이드 효과 설정")]
    [Group("Fade")] public bool m_use_fade_effect = true;
    [Group("Fade"), ShowIf(nameof(m_use_fade_effect))] public float m_fade_up_duration = 0.25f;
    [Group("Fade"), ShowIf(nameof(m_use_fade_effect))] public float m_fade_down_duration = 0.1f;

    [Space(30f), Header("대사 출력 설정")]
    [Group("Typewritter"), SerializeField] internal TypewriterType m_type_writer_style = TypewriterType.ByLetter;

    [Group("Typewritter"), ShowIf(nameof(m_type_writer_style), TypewriterType.ByLetter)]
    [Label("Letters per Second")]
    [Min(0)]
    public int m_letters_per_second = 60;

    [Group("Typewritter"), ShowIf(nameof(m_type_writer_style), TypewriterType.ByWord)]
    [Label("Words per Second")]
    [Min(0)]
    public int m_words_per_second = 10;

    [Label("Event Handlers")]
    [UnityEngine.Serialization.FormerlySerializedAs("actionMarkupHandlers")]
    [SerializeField] List<ActionMarkupHandler> eventHandlers = new List<ActionMarkupHandler>();

    private List<IActionMarkupHandler> ActionMarkupHandlers
    {
        get
        {
            var pauser = new PauseEventProcessor();
            List<IActionMarkupHandler> ActionMarkupHandlers = new()
            {
                pauser,
            };
            ActionMarkupHandlers.AddRange(eventHandlers);
            return ActionMarkupHandlers;
        }
    }

    private void Awake()
    {
        if (m_character_name_container == null && m_character_name_text != null)
            m_character_name_container = m_character_name_text.gameObject;

        switch (m_type_writer_style)
        {
            case TypewriterType.Instant:
                Typewriter = new InstantTypewriter()
                {
                    ActionMarkupHandlers = ActionMarkupHandlers,
                    Text = m_line_text,
                };
                break;

            case TypewriterType.ByLetter:
                Typewriter = new LetterTypewriter()
                {
                    ActionMarkupHandlers = ActionMarkupHandlers,
                    Text = m_line_text,
                    CharactersPerSecond = m_letters_per_second,
                };
                break;

            case TypewriterType.ByWord:
                Typewriter = new WordTypewriter()
                {
                    ActionMarkupHandlers = ActionMarkupHandlers,
                    Text = m_line_text,
                    WordsPerSecond = m_words_per_second,
                };
                break;
        }
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        if (m_canvas_group != null)
            m_canvas_group.alpha = 0;

        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        if (m_canvas_group != null)
            m_canvas_group.alpha = 0;

        return YarnTask.CompletedTask;
    }

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        if(m_show_portrait)
            UpdatePortrait(line.CharacterName);

        if (m_line_text == null)
        {
            Debug.LogError($"{nameof(LinePresenter)}는 텍스트를 출력할 컴포넌트가 없습니다. {line.TextID} (\"{line.RawText}\") 대사를 건너뜁니다.");
            return;
        }

        MarkupParseResult text;
        if (m_character_name_text == null)
        {
            text = m_show_name ? line.Text
                               : line.TextWithoutCharacterName;
        }
        else
        {
            text = line.TextWithoutCharacterName;

            if (m_character_name_container != null)
            {
                if (string.IsNullOrWhiteSpace(line.CharacterName))
                {
                    m_character_name_container.SetActive(false);
                }
                else
                {
                    m_character_name_container.SetActive(true);
                    m_character_name_text.text = line.CharacterName;
                }
            }
        }

        Typewriter ??= new InstantTypewriter()
        {
            ActionMarkupHandlers = ActionMarkupHandlers,
            Text = m_line_text,
        };

        Typewriter.PrepareForContent(text);

        if (m_canvas_group != null)
        {
            if (m_use_fade_effect)
            {
                await Effects.FadeAlphaAsync(m_canvas_group, 0, 1, m_fade_up_duration, token.HurryUpToken);

                if(m_portrait_group != null)
                    await Effects.FadeAlphaAsync(m_portrait_group, 0, 1, 0, token.HurryUpToken);
            }
            else
                m_canvas_group.alpha = 1;
        }

        await Typewriter.RunTypewriter(text, token.HurryUpToken).SuppressCancellationThrow();
        await YarnTask.WaitUntilCanceled(token.NextContentToken).SuppressCancellationThrow();

        Typewriter.ContentWillDismiss();

        if (m_canvas_group != null)
        {
            if (m_use_fade_effect)
            {
                await Effects.FadeAlphaAsync(m_canvas_group, 1, 0, m_fade_down_duration, token.HurryUpToken).SuppressCancellationThrow();

                if(m_portrait_group != null)
                    await Effects.FadeAlphaAsync(m_portrait_group, 1, 0, 0, token.HurryUpToken).SuppressCancellationThrow();
            }
            else
                m_canvas_group.alpha = 0;
        }
    }
}

using UnityEngine;
using Yarn.Unity.Attributes;
using Yarn.Unity;
using System.Collections.Generic;
using TMPro;
using System.Threading;
using UnityEngine.UI;
using System.Collections;

#nullable enable

public class CustomOptionPresenter : DialoguePresenterBase
{
    [Header("필요 컴포넌트")]
    [SerializeField] private CanvasGroup? m_canvas_group;
    [MustNotBeNull] [SerializeField] private OptionItem? m_option_view_prefab;
    private List<OptionItem> m_option_views = new List<OptionItem>();

    [Space(30f), Header("최근 대사 출력 설정")]
    [SerializeField] bool m_show_last_line;

    [ShowIf(nameof(m_show_last_line))]
    [Indent]
    [MustNotBeNullWhen(nameof(m_show_last_line))]
    [SerializeField] TMP_Text? m_last_line_text;

    [ShowIf(nameof(m_show_last_line))]
    [Indent]
    [SerializeField] GameObject? m_last_line_container;

    [ShowIf(nameof(m_show_last_line))]
    [Indent]
    [SerializeField] TMP_Text? m_last_line_character_name_text;

    [ShowIf(nameof(m_show_last_line))]
    [Indent]
    [SerializeField] GameObject? m_last_line_character_name_container;

    [Space(30f), Header("초상화 설정")]
    [Group("Portrait"), SerializeField] private bool m_show_portrait = true;
    [Group("portrait"), SerializeField] private CanvasGroup m_portrait_group;
    [Group("Portrait"), ShowIf(nameof(m_show_portrait)), SerializeField] private Image m_player_portrait;
    [Group("Portrait"), ShowIf(nameof(m_show_portrait)), SerializeField] private Image m_npc_portrait;

    [Space(30f), Header("초상화 강조 설정")]
    [Group("Portrait Hover"), SerializeField] private float m_active_color = 1f;
    [Group("Portrait Hover"), SerializeField] private float m_inactive_color = 0.4f;
    [Group("Portrait Hover"), SerializeField] private float m_fade_durtion = 0.3f;

    LocalizedLine? m_last_seen_line;

    [Space(30f), Header("선택지 출력 설정")]
    public bool m_show_unavailable_options = false;

    [Space(30f), Group("Fade"), Header("페이드 설정")]
    public bool m_use_fade_effect = true;

    [Group("Fade")]
    [ShowIf(nameof(m_use_fade_effect))]
    public float m_fade_up_duration = 0.25f;

    [Group("Fade")]
    [ShowIf(nameof(m_use_fade_effect))]
    public float m_fade_down_duration = 0.1f;

    private const string m_truncate_last_line_mark_up_name = "lastline";

    private void Start()
    {
        if (m_canvas_group != null)
        {
            m_canvas_group.alpha = 0;
            m_canvas_group.interactable = false;
            m_canvas_group.blocksRaycasts = false;
        }

        if (m_last_line_container == null && m_last_line_text != null)
            m_last_line_container = m_last_line_text.gameObject;

        if (m_last_line_character_name_container == null && m_last_line_character_name_text != null)
            m_last_line_character_name_container = m_last_line_character_name_text.gameObject;
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        if (m_canvas_group != null)
        {
            m_canvas_group.alpha = 0;
            m_canvas_group.interactable = false;
            m_canvas_group.blocksRaycasts = false;
        }

        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        if (m_canvas_group != null)
        {
            m_canvas_group.alpha = 0;
            m_canvas_group.interactable = false;
            m_canvas_group.blocksRaycasts = false;
        }

        return YarnTask.CompletedTask;
    }

    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        if (m_show_last_line)
            m_last_seen_line = line;

        return YarnTask.CompletedTask;
    }

    public override async YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogue_options, LineCancellationToken cancellation_token)
    {
        if(m_show_portrait)
            UpdatePortrait("플레이어");
            
        var any_available = false;
        foreach (var option in dialogue_options)
        {
            if (option.IsAvailable)
            {
                any_available = true;
                break;
            }
        }

        if (!any_available)
            return null;

        while (dialogue_options.Length > m_option_views.Count)
        {
            var option_view = CreateNewOptionView();
            m_option_views.Add(option_view);
        }

        YarnTaskCompletionSource<DialogueOption?> selectedOptionCompletionSource = new YarnTaskCompletionSource<DialogueOption?>();

        var completionCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation_token.NextContentToken);
        async YarnTask CancelSourceWhenDialogueCancelled()
        {
            await YarnTask.WaitUntilCanceled(completionCancellationSource.Token);

            if (cancellation_token.IsNextContentRequested == true)
                selectedOptionCompletionSource.TrySetResult(null);
        }

        CancelSourceWhenDialogueCancelled().Forget();

        for (int i = 0; i < dialogue_options.Length; i++)
        {
            var option_view = m_option_views[i];
            var option = dialogue_options[i];

            if (option.IsAvailable == false && m_show_unavailable_options == false)
                continue;

            option_view.gameObject.SetActive(true);
            option_view.Option = option;

            option_view.OnOptionSelected = selectedOptionCompletionSource;
            option_view.completionToken = completionCancellationSource.Token;
        }

        var option_index_to_select = -1;
        for (int i = 0; i < m_option_views.Count; i++)
        {
            var view = m_option_views[i];
            if (!view.isActiveAndEnabled)
                continue;

            if (view.IsHighlighted)
            {
                option_index_to_select = i;
                break;
            }

            if (option_index_to_select == -1)
                option_index_to_select = i;
        }

        if (option_index_to_select > -1)
            m_option_views[option_index_to_select].Select();

        if (m_last_line_container != null)
        {
            if (m_last_seen_line != null && m_show_last_line)
            {
                var line = m_last_seen_line.Text;
                if (m_last_line_character_name_container != null)
                {
                    if (string.IsNullOrWhiteSpace(m_last_seen_line.CharacterName))
                        m_last_line_character_name_container.SetActive(false);
                    else
                    {
                        line = m_last_seen_line.TextWithoutCharacterName;
                        m_last_line_character_name_container.SetActive(true);

                        if (m_last_line_character_name_text != null)
                            m_last_line_character_name_text.text = m_last_seen_line.CharacterName;
                    }
                }
                else
                {
                    line = m_last_seen_line.TextWithoutCharacterName;
                }

                var line_text = line.Text;
                if (line.TryGetAttributeWithName(m_truncate_last_line_mark_up_name, out var markup))
                {
                    var end = line_text.Substring(markup.Position);
                    line_text = "..." + end;
                }

                if (m_last_line_text != null)
                    m_last_line_text.text = line_text;

                m_last_line_container.SetActive(true);
            }
            else
            {
                m_last_line_container.SetActive(false);
            }
        }

        if (m_use_fade_effect && m_canvas_group != null)
        {
            await Effects.FadeAlphaAsync(m_canvas_group, 0, 1, m_fade_up_duration, cancellation_token.HurryUpToken);
            await Effects.FadeAlphaAsync(m_portrait_group, 0, 1, 0, cancellation_token.HurryUpToken);
        }

        if (m_canvas_group != null)
        {
            m_canvas_group.interactable = true;
            m_canvas_group.blocksRaycasts = true;
        }

        var completed_task = await selectedOptionCompletionSource.Task;
        completionCancellationSource.Cancel();

        if (m_canvas_group != null)
        {
            m_canvas_group.interactable = false;
            m_canvas_group.blocksRaycasts = false;
        }

        if (m_use_fade_effect && m_canvas_group != null)
        {
            await Effects.FadeAlphaAsync(m_canvas_group, 1, 0, m_fade_down_duration, cancellation_token.HurryUpToken);
            await Effects.FadeAlphaAsync(m_portrait_group, 1, 0, 0, cancellation_token.HurryUpToken);
        }

        foreach (var optionView in m_option_views)
            optionView.gameObject.SetActive(false);

        await YarnTask.Yield();

        if (cancellation_token.NextContentToken.IsCancellationRequested)
            return await DialogueRunner.NoOptionSelected;

        return completed_task;
    }

    private OptionItem CreateNewOptionView()
    {
        var option_view = Instantiate(m_option_view_prefab);

        var targetTransform = m_canvas_group != null ? m_canvas_group.transform 
                                                     : transform;

        if (option_view == null)
            throw new System.InvalidOperationException($"{nameof(option_view)}가 NULL이기 때문에 선택지를 생성할 수 없습니다.");

        option_view.transform.SetParent(targetTransform.transform, false);
        option_view.transform.SetAsLastSibling();
        option_view.gameObject.SetActive(false);

        return option_view;
    }

    private void UpdatePortrait(string? character_name)
    {
        var player_target = (character_name == "플레이어") ? m_active_color : m_inactive_color;
        var npc_target = (character_name != "플레이어") ? m_active_color : m_inactive_color;

        StartCoroutine(FadePortraitColor(m_player_portrait, player_target));
        StartCoroutine(FadePortraitColor(m_npc_portrait, npc_target));
    }

    private IEnumerator FadePortraitColor(Image image, float color)
    {
        if (image == null) yield break;

        var elapsed_time = 0f;
        var start_color = image.color;
        var target_color = new Color(color, color, color, start_color.a);

        while (elapsed_time < m_fade_durtion)
        {
            elapsed_time += Time.deltaTime;

            image.color = Color.Lerp(start_color, target_color, elapsed_time / m_fade_durtion);

            yield return null;
        }

        image.color = target_color;
    }
}

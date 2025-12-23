using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.Attributes;

#nullable enable

public abstract class CustomDialoguePresenter : DialoguePresenterBase
{
    [Space(30f), Header("초상화 설정")]
    [Group("Portrait"), SerializeField] protected bool m_show_portrait = true;
    [Group("portrait"), SerializeField] protected CanvasGroup? m_portrait_group;
    [Group("Portrait"), ShowIf(nameof(m_show_portrait)), SerializeField] protected Image? m_player_portrait;
    [Group("Portrait"), ShowIf(nameof(m_show_portrait)), SerializeField] protected Image? m_npc_portrait;

    [Space(30f), Header("초상화 강조 설정")]
    [Group("Portrait Hover"), SerializeField] protected float m_active_color = 1f;
    [Group("Portrait Hover"), SerializeField] protected float m_inactive_color = 0.4f;
    [Group("Portrait Hover"), SerializeField] protected float m_fade_durtion = 0.3f;

    private Coroutine? m_player_portrait_coroutine;
    private Coroutine? m_npc_portrait_coroutine;

    protected const string PLAYER_NAME = "이클리스";

    public override abstract YarnTask OnDialogueCompleteAsync();
    public override abstract YarnTask OnDialogueStartedAsync();
    public override abstract YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token);

    protected void UpdatePortrait(string? character_name)
    {
        var player_target = (character_name == PLAYER_NAME) ? m_active_color : m_inactive_color;
        var npc_target = (character_name != PLAYER_NAME) ? m_active_color : m_inactive_color;

        if(m_player_portrait != null)
        {
            if(m_player_portrait_coroutine != null)
                StopCoroutine(m_player_portrait_coroutine);

            m_player_portrait_coroutine = StartCoroutine(FadePortraitColor(m_player_portrait, player_target));
        }
        
        if(m_npc_portrait != null)
        {
            if(m_npc_portrait_coroutine != null)
                StopCoroutine(m_npc_portrait_coroutine);

            m_npc_portrait_coroutine = StartCoroutine(FadePortraitColor(m_npc_portrait, npc_target));
        }
    }

    protected IEnumerator FadePortraitColor(Image image, float color)
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
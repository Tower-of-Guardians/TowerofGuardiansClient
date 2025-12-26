using System.Collections;
using TMPro;
using UnityEngine;

public class CraftmanDialogueBubbleView : MonoBehaviour, IDialogueBubbleView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("말풍선 텍스트")]
    [SerializeField] private TMP_Text m_dialogue_label;

    [Header("초당 출력될 문자")]
    [SerializeField] private float m_char_per_second = 10f;

    private Coroutine m_fade_coroutine;
    private Coroutine m_typing_coroutine;

    public void OpenUI()
        => ToggleUI(true);

    public void CloseUI()
        => ToggleUI(false);

    public void SetBubble(string dialogue_string)
    {
        if(m_typing_coroutine != null)
            StopCoroutine(m_typing_coroutine);

        m_typing_coroutine = StartCoroutine(TypeDialogue(dialogue_string));
    }

    private void ToggleUI(bool active)
    {
        if(m_fade_coroutine != null)
            StopCoroutine(m_fade_coroutine);

        m_fade_coroutine = StartCoroutine(ToggleFade(active));
    }

    private IEnumerator ToggleFade(bool is_in)
    {
        var elapsed_time = 0f;
        var target_time = 0.5f;

        var start_alpha = m_canvas_group.alpha;
        var target_alpha = is_in ? 1f : 0f;

        while(elapsed_time < target_time)
        {
            elapsed_time += Time.deltaTime;

            var delta = elapsed_time / target_time;
            m_canvas_group.alpha = Mathf.Lerp(start_alpha, target_alpha, delta);

            yield return null;
        }

        m_canvas_group.alpha = target_alpha;
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        m_dialogue_label.text = string.Empty;

        var interval = 1f / m_char_per_second;

        var index = 0;
        while (index < dialogue.Length)
        {
            if (dialogue[index] == '<')
            {
                var tag_end_index = dialogue.IndexOf('>', index);
                if (tag_end_index == -1)
                    break;

                string tag = dialogue.Substring(index, tag_end_index - index + 1);
                m_dialogue_label.text += tag;

                index = tag_end_index + 1;
                continue;
            }

            m_dialogue_label.text += dialogue[index];
            index++;

            yield return new WaitForSeconds(interval);
        }
    }
}

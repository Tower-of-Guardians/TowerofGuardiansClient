using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour, IInventoryView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("열기 버튼")]
    [SerializeField] private Button m_open_button;

    [Header("나가기 버튼")]
    [SerializeField] private Button m_close_button;

    private Coroutine m_fade_coroutine;
    private InventoryPresenter m_presenter;

    public void Inject(InventoryPresenter presenter)
    {
        m_presenter = presenter;

        m_open_button.onClick.AddListener(m_presenter.OpenUI);
        m_close_button.onClick.AddListener(m_presenter.CloseUI);
    }

    public void OpenUI()
        => ToggleUI(true);

    public void CloseUI()
        => ToggleUI(false);

    private void ToggleUI(bool active)
    {
        if(m_fade_coroutine != null)
            StopCoroutine(m_fade_coroutine);

        m_fade_coroutine = StartCoroutine(FadeCanvas(active));
    }

    private IEnumerator FadeCanvas(bool is_in)
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

        m_canvas_group.alpha = is_in ? 1f : 0f;
        m_canvas_group.interactable = is_in;
        m_canvas_group.blocksRaycasts = is_in;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantShopView : MonoBehaviour, IMerchantShopView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("상인 초상화")]
    [SerializeField] private Image m_portrait_image;

    [Header("카드 판매 버튼")]
    [SerializeField] private Button m_sale_button;

    [Header("카드 판매 텍스트")]
    [SerializeField] private TMP_Text m_sale_label;

    private MerchantShopPresenter m_presenter;
    private Coroutine m_fade_coroutine;

    public void Inject(MerchantShopPresenter presenter)
    {
        m_presenter = presenter;

        m_sale_button.onClick.AddListener(m_presenter.OnClickedSale);
    }

    public void OpenUI()
        => ToggleUI(true);

    public void CloseUI()
        => ToggleUI(false);

    public void ToggleSaleButton(bool active)
    {
        m_sale_button.interactable = active;

        m_sale_label.text = active ? "카드 판매"
                                   : "<color=red>카드 판매</color>";
    }

    private void ToggleUI(bool active)
    {
        if(m_fade_coroutine != null)
            StopCoroutine(m_fade_coroutine);

        //SetPortraitAlpha(active ? 1f : 0f);
        m_fade_coroutine = StartCoroutine(ToggleActive(active));
    }

    private IEnumerator ToggleActive(bool is_in)
    {
        var elapsed_time = 0f;
        var target_time = 0.5f;

        var start_alpha = m_canvas_group.alpha;
        var end_alpha = is_in ? 1f : 0f;

        while(elapsed_time < target_time)
        {
            elapsed_time += Time.deltaTime;

            var delta = elapsed_time / target_time;
            m_canvas_group.alpha = Mathf.Lerp(start_alpha, end_alpha, delta);

            yield return null;
        }

        m_canvas_group.alpha = end_alpha;
        m_canvas_group.interactable = is_in;
        m_canvas_group.blocksRaycasts = is_in;
    }

    private void SetPortraitAlpha(float alpha)
    {
        var color = m_portrait_image.color;
        color.a = alpha;
        m_portrait_image.color = color;
    }
}

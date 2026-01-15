using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusView : MonoBehaviour, IStatusView
{
    [Header("UI 관련 컴포넌트")]
    [Header("레벨 텍스트")]
    [SerializeField] private TMP_Text m_level_label;

    [Header("경험치 슬라이더")]
    [SerializeField] private Slider m_exp_slider;

    [Header("골드 텍스트")]
    [SerializeField] private TMP_Text m_gold_label;

    private StatusPresenter m_presenter;

    private void OnDestroy()
        => m_presenter?.Dispose();

    public void Inject(StatusPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void UpdateGold(int gold)
    {
        m_gold_label.text = $"{gold}G";
    }

    public void UpdateLevel(int level, float exp)
    {
        m_level_label.text = $"Lv.{level}";
        m_exp_slider.value = exp;
    }
}

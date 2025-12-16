using UnityEngine;
using UnityEngine.UI;

public class InventoryTabView : MonoBehaviour, IInventoryTabView
{
    [Header("카드 토글")]
    [SerializeField] private Toggle m_card_toggle;

    [Header("마법 토글")]
    [SerializeField] private Toggle m_magic_toggle;

    private InventoryTabPresenter m_presenter;

    public void Inject(InventoryTabPresenter presenter)
    {
        m_presenter = presenter;

        m_card_toggle.onValueChanged.AddListener((isOn) => m_presenter.OnValueChangedCard(isOn));
        m_magic_toggle.onValueChanged.AddListener((isOn) => m_presenter.OnValueChangedMagic(isOn));
    }

    public void Initialize()
    {
        m_card_toggle.isOn = true;
        m_presenter.OnValueChangedCard(true);
    }

    public void UpdateCardToggle(bool isOn)
    {
        var rect = m_card_toggle.transform as RectTransform;
        rect.localScale = isOn ? Vector3.one
                               : Vector3.one * 0.75f;

        if(isOn)
            rect.SetAsLastSibling();
    }

    public void UpdateMagicToggle(bool isOn)
    {
        var rect = m_magic_toggle.transform as RectTransform;
        rect.localScale = isOn ? Vector3.one
                               : Vector3.one * 0.75f;

        if(isOn)
            rect.SetAsLastSibling();
    }
}

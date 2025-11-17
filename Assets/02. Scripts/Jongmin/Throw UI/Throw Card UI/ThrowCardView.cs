using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowCardView : MonoBehaviour, IThrowCardView
{
    private ThrowCardPresenter m_presenter;

    public void Inject(ThrowCardPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void UpdateUI(CardData card_data)
    {
        // TODO: 카드 UI 설정
    }

#region Events
    public void OnPointerClick(PointerEventData eventData)
        => m_presenter.OnPointerClick();
#endregion Events
}

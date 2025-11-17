using UnityEngine.EventSystems;

public interface IThrowCardView : IPointerClickHandler
{
    void Inject(ThrowCardPresenter presenter);
    
    void UpdateUI(CardData card_data);
}
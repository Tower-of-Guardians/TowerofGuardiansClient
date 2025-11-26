using UnityEngine;

public class ThrowCardPresenter
{
    private readonly IThrowCardView m_view;
    
    public CardData Data { get; private set; }

    public ThrowCardPresenter(IThrowCardView view, CardData card_data)
    {
        m_view = view;
        // TODO: 카드 데이터 주입

        m_view.Inject(this);
    }
}

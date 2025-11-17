using UnityEngine;

public class ThrowCardPresenter
{
    private readonly IThrowCardView m_view;
    private readonly ThrowPresenter m_throw_presenter;
    
    public CardData Data { get; private set; }

    public ThrowCardPresenter(IThrowCardView view,
                              ThrowPresenter throw_presenter)
    {
        m_view = view;
        m_throw_presenter = throw_presenter;
        // TODO: 카드 데이터 주입

        m_view.Inject(this);
    }

    public void OnPointerClick()
        => m_throw_presenter.OnClickedCard(m_view);
}

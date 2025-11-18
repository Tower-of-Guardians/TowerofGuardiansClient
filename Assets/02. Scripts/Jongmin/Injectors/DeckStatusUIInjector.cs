using UnityEngine;

public class DeckStatusUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("덱 상태 관리 뷰")]
    [SerializeField] private DeckStatusView m_deck_status_view;
    
    public void Inject()
    {
        InjectStatus();
    }

    private void InjectStatus()
    {
        DIContainer.Register<IDeckStatusView>(m_deck_status_view);

        var deck_status_presenter = new DeckStatusPresenter(m_deck_status_view);
        DIContainer.Register<DeckStatusPresenter>(deck_status_presenter);
    }
}

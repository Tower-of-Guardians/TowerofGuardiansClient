using UnityEngine;

public class HandUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("핸드 뷰")]
    [SerializeField] private HandView m_hand_view;

    public void Inject()
    {
        InjectHand();
    }

    private void InjectHand()
    {
        DIContainer.Register<IHandView>(m_hand_view);

        var hand_presenter = new HandPresenter(m_hand_view,
                                               DIContainer.Resolve<AttackFieldPresenter>(),
                                               DIContainer.Resolve<DefendFieldPresenter>(),
                                               DIContainer.Resolve<ThrowPresenter>());
        DIContainer.Register<HandPresenter>(hand_presenter);
    }
}

using UnityEngine;

public class CardEffectorInjector : MonoBehaviour, IInjector
{
    [Header("핸드 드로어")]
    [SerializeField] private DrawCardEffector m_draw_card_effector;

    [Header("교체 → 핸드")]
    [SerializeField] private ThrowCardToHandEffector m_throw_card_to_hand_effector;

    [Header("교체 → 교체")]
    [SerializeField] private ThrowCardToThrowEffector m_throw_card_to_throw_effector;

    public void Inject()
    {
        m_draw_card_effector.Inject(DIContainer.Resolve<HandPresenter>(),
                                    DIContainer.Resolve<TurnManager>());

        m_throw_card_to_hand_effector.Inject(DIContainer.Resolve<ThrowPresenter>(),
                                             DIContainer.Resolve<HandPresenter>());

        m_throw_card_to_throw_effector.Inject(DIContainer.Resolve<ThrowPresenter>());
    }
}

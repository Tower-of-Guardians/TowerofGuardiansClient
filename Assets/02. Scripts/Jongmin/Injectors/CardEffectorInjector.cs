using UnityEngine;

public class CardEffectorInjector : MonoBehaviour, IInjector
{
    [Header("핸드 드로어")]
    [SerializeField] DrawCardEffector m_draw_card_effector;

    public void Inject()
    {
        m_draw_card_effector.Inject(DIContainer.Resolve<HandPresenter>());
    }
}

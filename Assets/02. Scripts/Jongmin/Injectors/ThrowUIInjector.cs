using UnityEngine;

public class ThrowUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("교체 뷰")]
    [SerializeField] private ThrowView m_throw_view;

    [Header("레이아웃 교체 뷰")]
    [SerializeField] private LayoutThrowView m_layout_throw_view;

    public void Inject()
    {
        InjectThrow();
    }

    private void InjectThrow()
    {
        DIContainer.Register<IThrowView>(m_throw_view);

        var throw_presenter = new ThrowPresenter(m_layout_throw_view,
                                                 DIContainer.Resolve<TurnManager>());
        DIContainer.Register<ThrowPresenter>(throw_presenter);
    }
}

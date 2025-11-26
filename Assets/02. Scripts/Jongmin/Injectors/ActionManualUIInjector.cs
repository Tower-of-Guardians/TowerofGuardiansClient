using UnityEngine;

public class ActionManualUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("행동 메뉴얼 뷰")]
    [SerializeField] private ActionManualView m_action_manual_view;

    [Header("교체 메뉴얼 뷰")]
    [SerializeField] private ThrowManualView m_throw_manual_view;
    
    public void Inject()
    {
        InjectActionManual();
        InjectThrowManual();
    }

    private void InjectActionManual()
    {
        var action_manual_presenter = new ActionManualPresenter(m_action_manual_view,
                                                                DIContainer.Resolve<TurnManager>());
    }

    private void InjectThrowManual()
    {
        var throw_manual_presenter = new ThrowManualPresenter(m_throw_manual_view,
                                                        DIContainer.Resolve<TurnManager>());
    }
}

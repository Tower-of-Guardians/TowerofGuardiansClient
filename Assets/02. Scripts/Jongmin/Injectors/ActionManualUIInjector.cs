using UnityEngine;

public class ActionManualUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("행동 메뉴얼 뷰 목록")]
    [SerializeField] private ActionManualView[] m_action_manual_views;
    
    public void Inject()
    {
        InjectActionManual();
    }

    private void InjectActionManual()
    {
        foreach(var view in m_action_manual_views)
        {
            var action_manual_presenter = new ActionManualPresenter(view,
                                                                    DIContainer.Resolve<TurnManager>());
        }
    }
}

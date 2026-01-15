using UnityEngine;

public class StatusUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("상태 뷰")]
    [SerializeField] private StatusView m_status_view;

    public void Inject()
    {
        InjectStatus();
    }

    private void InjectStatus()
    {
        DIContainer.Register<IStatusView>(m_status_view);
        
        var status_presenter = new StatusPresenter(m_status_view);
        DIContainer.Register<StatusPresenter>(status_presenter);
    }
}

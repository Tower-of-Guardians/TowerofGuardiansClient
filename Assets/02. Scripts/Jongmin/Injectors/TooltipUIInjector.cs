using UnityEngine;

public class TooltipUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("툴팁 뷰")]
    [SerializeField] private TooltipView m_tooltip_view;

    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("플레이어")]
    [SerializeField] private PlayerDescriptor m_player_descriptor;

    [Header("몬스터")]
    [SerializeField] private MonsterDescriptor[] m_monster_descriptors;

    private IDescriptableUI[] m_descriptables;

    private void Awake()
    {
        m_descriptables = m_canvas.GetComponentsInChildren<IDescriptableUI>();
    }

    public void Inject()
    {
        InstallTooltip();
        InjectTooltip();
    }

    private void InstallTooltip()
    {
        DIContainer.Register<ITooltipView>(m_tooltip_view);

        var tooltip_presenter = new TooltipPresenter(m_tooltip_view);
        DIContainer.Register<TooltipPresenter>(tooltip_presenter);
    }

    private void InjectTooltip()
    {
        var tooltip_presenter = DIContainer.Resolve<TooltipPresenter>();

        foreach(var descriptable in m_descriptables)
            descriptable.Inject(tooltip_presenter);

        TempInject(tooltip_presenter);
    }

    private void TempInject(TooltipPresenter tooltip_presenter)
    {
        m_player_descriptor.Inject(tooltip_presenter);
        foreach(var descriptor in m_monster_descriptors)
        {
            descriptor.Inject(tooltip_presenter);
        }
    }
}
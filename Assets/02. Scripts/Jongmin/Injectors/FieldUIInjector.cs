using UnityEngine;

public class FieldUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("필드 디자이너")]
    [SerializeField] private FieldUIDesigner m_designer;

    [Header("공격 필드 뷰")]
    [SerializeField] private FieldView m_attack_field_view;

    [Header("방어 필드 뷰")]
    [SerializeField] private FieldView m_defend_field_view;

    public void Inject()
    {
        InjectField();
    }

    private void InjectField()
    {
        var turn_manager = DIContainer.Resolve<TurnManager>();
        var throw_presenter = DIContainer.Resolve<ThrowPresenter>();

        var attack_field_presenter = new AttackFieldPresenter(m_attack_field_view,
                                                              m_designer,
                                                              turn_manager,
                                                              throw_presenter);
        DIContainer.Register<AttackFieldPresenter>(attack_field_presenter);

        var defend_field_presenter = new DefendFieldPresenter(m_defend_field_view,
                                                              m_designer,
                                                              turn_manager,
                                                              throw_presenter);
        DIContainer.Register<DefendFieldPresenter>(defend_field_presenter);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HandView : MonoBehaviour, IHandView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("테스트 제거 버튼")]
    [SerializeField] private Button m_remove_button;

    private HandPresenter m_presenter;

    public void Inject(HandUIDesigner designer,
                       HandCardContainer container,
                       HandCardFactory factory,
                       HandCardLayoutController layout_controller,
                       HandCardEventController event_controller,
                       CardInfoUI card_info_ui,
                       TurnManager turn_manager)
    {
        factory.Inject(event_controller);
        layout_controller.Inject(container, m_presenter);
        event_controller.Inject(designer, m_presenter, container, layout_controller, card_info_ui, turn_manager);
    }

    public void Inject(HandPresenter presenter)
        => m_presenter = presenter;

    public void OpenUI() 
        => ToggleUI(true);

    public void CloseUI() 
        => ToggleUI(false);

    private void ToggleUI(bool active)
    {
        m_canvas_group.alpha = active ? 1f : 0f;
        m_canvas_group.interactable = active;
        m_canvas_group.blocksRaycasts = active;
    }
}
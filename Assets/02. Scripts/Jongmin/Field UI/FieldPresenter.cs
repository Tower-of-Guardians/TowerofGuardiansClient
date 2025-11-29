using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public abstract class FieldPresenter : IDisposable
{
    protected readonly IFieldView m_view;
    protected readonly FieldUIDesigner m_designer;
    protected readonly TurnManager m_turn_manager;
    private HandPresenter m_hand_presenter;
    private readonly ThrowPresenter m_throw_presenter;

    protected List<IFieldCardView> m_card_list;
    protected Dictionary<IFieldCardView, FieldCardPresenter> m_card_dict;

    protected bool m_is_active = true;

    public FieldPresenter(IFieldView view,
                          FieldUIDesigner designer,
                          TurnManager turn_manager,
                          ThrowPresenter throw_presenter)
    {
        m_card_list = new();
        m_card_dict = new();

        m_view = view;
        m_designer = designer;
        m_turn_manager = turn_manager;
        m_throw_presenter = throw_presenter;

        m_throw_presenter.OnUpdatedToggleUI += ToggleActive;

        m_view.Inject(this);
    }

    public void Inject(HandPresenter hand_presenter)
        => m_hand_presenter = hand_presenter;

    public abstract bool InstantiateCard(CardData card_data);

    public abstract void ToggleManual(bool active);

    public void Return()
    {
        m_card_list.Clear();
        m_card_dict.Clear();

        m_view.ReturnCards();
    }

    protected void ToggleActive(bool active)
        => m_is_active = !active;

    public void Dispose()
        => m_throw_presenter.OnUpdatedToggleUI -= ToggleActive;

#region Events
    public void OnDroped(IHandCardView card_view)
    {
        if(!m_is_active)
            return;

        if(!m_turn_manager.CanAction())
        {
            m_view.PrintNotice("<color=red>더 이상 행동할 수 없습니다.</color>");
            return;
        }

        var card_data = m_hand_presenter.GetCardData(card_view);
        if(InstantiateCard(card_data))
        {
            m_hand_presenter.RemoveCard(card_view);
            m_turn_manager.UpdateActionCount(1);
        }
    }
    #endregion Events
}

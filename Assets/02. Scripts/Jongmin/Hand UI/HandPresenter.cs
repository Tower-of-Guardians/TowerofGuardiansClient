using System.Collections.Generic;

public class HandPresenter
{
    private readonly IHandView m_view;
    private readonly FieldPresenter m_attack_field_presenter;
    private readonly FieldPresenter m_defend_field_presenter;
    private readonly ThrowPresenter m_throw_presenter;

    private List<IHandCardView> m_card_list;
    private Dictionary<IHandCardView, HandCardPresenter> m_card_dict;
    private IHandCardView m_hover_card;

    public List<IHandCardView> Cards
    {
        get => m_card_list;
        set => m_card_list = value;
    }

    public IHandCardView HoverCard
    {
        get => m_hover_card;
        set => m_hover_card = value;
    }

    public HandPresenter(IHandView view,
                         FieldPresenter attack_field_presenter,
                         FieldPresenter defend_field_presenter,
                         ThrowPresenter throw_presenter)
    {
        m_card_list = new();
        m_card_dict = new();

        m_view = view;
        m_attack_field_presenter = attack_field_presenter;
        m_defend_field_presenter = defend_field_presenter;
        m_throw_presenter = throw_presenter;

        m_view.Inject(this);
        m_attack_field_presenter.Inject(this);
        m_defend_field_presenter.Inject(this);
        throw_presenter.Inject(this);
    }

    public void InstantiateCard()
    {
        var card_view = m_view.InstantiateCardView();
        m_card_list.Add(card_view);

        var card_presenter = new HandCardPresenter(card_view);
        m_card_dict.TryAdd(card_view, card_presenter);

        m_view.UpdateUI();
    }

    public void RemoveCard(IHandCardView card_view)
    {
        m_hover_card = null;

        m_card_list.Remove(card_view);

        if(m_card_dict.TryGetValue(card_view, out var card_presenter))
            card_presenter.Return();

        m_card_dict.Remove(card_view);
    }

    public void OpenUI() => m_view.OpenUI();
    public void CloseUI() => m_view.CloseUI();


    public void ToggleFieldPreview(bool active)
    {
        m_attack_field_presenter.ToggleManual(active);
        m_defend_field_presenter.ToggleManual(active);
        m_throw_presenter.ToggleManual(active);
    }

#region Events
    public void OnPointerEnter(IHandCardView card_view) => m_hover_card = card_view;
    public void OnPointerExit() => m_hover_card = null;
#endregion Events

#region Test
    public IHandCardView Test_RemoveCard()
    {
        if(m_card_list.Count == 0)
            return null;

        var target = m_card_list[^1];
        m_card_list.Remove(target);
        m_card_dict.Remove(target);

        return target;
    }
#endregion Test
}
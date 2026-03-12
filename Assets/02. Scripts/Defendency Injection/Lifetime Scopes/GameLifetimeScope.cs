using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Designer")]
    [SerializeField] private HandUIDesigner _handUIDesigner;
    [SerializeField] private FieldUIDesigner _fieldUIDesigner;
    [SerializeField] private DiscardUIDesigner _discardUIDesigner;
    [SerializeField] private TurnRuleDesigner _turnRuleDesigner;

    [Space(20), Header("Field Context")]
    [SerializeField] private FieldContext _atkFieldContext;
    [SerializeField] private FieldContext _defFieldContext;

    protected override void Configure(IContainerBuilder builder)
    {
        ConfigureCore(builder);
        ConfigureDiscardUI(builder);
        ConfigureHandUI(builder);
        ConfigureFieldUI(builder);
    }

    private void ConfigureCore(IContainerBuilder builder)
    {
        builder.RegisterInstance<ITurnRuleService>(_turnRuleDesigner);
        builder.RegisterComponentInHierarchy<TurnManager>()
               .AsSelf()
               .As<ITurnHandLimitPort>();
        builder.RegisterComponentInHierarchy<Notice>().As<INotice>();
        builder.RegisterComponentInHierarchy<CardInfoUI>();
        builder.RegisterComponentInHierarchy<DrawCardEffector>();
        builder.RegisterComponentInHierarchy<AttackCardToThrowEffector>();
        builder.RegisterComponentInHierarchy<DefendCardToThrowEffector>();
        builder.Register<CardDropSystem>(Lifetime.Singleton);
    }

    private void ConfigureDiscardUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(_discardUIDesigner);
        builder.RegisterInstance(new CardContainer<IDiscardCardUI, DiscardCardPresenter>());
        builder.RegisterComponentInHierarchy<DiscardUI>().AsSelf().As<IDiscardUI>();
        builder.RegisterComponentInHierarchy<DiscardCardEventController>();
        builder.RegisterComponentInHierarchy<DiscardCardLayoutController>();
        builder.RegisterComponentInHierarchy<DiscardCardFactory>().AsSelf().As<ICardFactory<IDiscardCardUI>>();
        builder.RegisterComponentInHierarchy<ThrowCardToHandEffector>();
        builder.RegisterComponentInHierarchy<ThrowCardToThrowEffector>();
        builder.RegisterEntryPoint<DiscardPresenter>()
               .AsSelf()
               .As<IDiscardCardRemovePort>()
               .As<ICardDropTarget<IDiscardCardUI>>();

        builder.RegisterBuildCallback(resolver =>
        {
            var discardPresenter = resolver.Resolve<DiscardPresenter>();
            var discardUIDesigner = resolver.Resolve<DiscardUIDesigner>();
            var discardUI = resolver.Resolve<DiscardUI>();
            var discardContainer = resolver.Resolve<CardContainer<IDiscardCardUI, DiscardCardPresenter>>();
            var discardLayout = resolver.Resolve<DiscardCardLayoutController>();
            var discardEvent = resolver.Resolve<DiscardCardEventController>();
            var discardFactory = resolver.Resolve<DiscardCardFactory>();
            var cardDropSystem = resolver.Resolve<CardDropSystem>();
            var discardToHandEffector = resolver.Resolve<ThrowCardToHandEffector>();
            var discardToDiscardEffector = resolver.Resolve<ThrowCardToThrowEffector>();
            var handPresenter = resolver.Resolve<HandPresenter>();

            discardEvent.Construct(discardUIDesigner,
                                   discardPresenter,
                                   discardContainer,
                                   discardLayout,
                                   cardDropSystem);
            discardFactory.Construct(discardEvent);
            discardUI.BindPresenter(discardPresenter);
            discardPresenter.BindEffectors(discardToHandEffector, discardToDiscardEffector);

            DIContainer.Register<CardContainer<IDiscardCardUI, DiscardCardPresenter>>(discardContainer);

            handPresenter.OnTogglePreviews += discardPresenter.TogglePreview;
        });
    }

    private void ConfigureHandUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(_handUIDesigner);
        builder.RegisterInstance(new CardContainer<IHandCardUI, HandCardPresenter>());
        builder.RegisterComponentInHierarchy<HandUI>().As<IHandUI>();
        builder.RegisterComponentInHierarchy<HandCardEventController>();
        builder.RegisterComponentInHierarchy<HandCardLayoutController>();
        builder.RegisterComponentInHierarchy<HandCardFactory>().AsSelf().As<ICardFactory<IHandCardUI>>();
        builder.RegisterComponentInHierarchy<HandCardToThrowEffector>();
        builder.RegisterEntryPoint<HandPresenter>()
               .AsSelf()
               .As<IHandCardCreatePort>()
               .As<IHandCardRemovePort>()
               .As<ICardDropTarget<IHandCardUI>>();

        builder.RegisterBuildCallback(resolver =>
        {
            var handEvent = resolver.Resolve<HandCardEventController>();
            var handFactory = resolver.Resolve<HandCardFactory>();
            handFactory.Construct(handEvent);

            var handPresenter = resolver.Resolve<HandPresenter>();
            var discardPresenter = resolver.Resolve<DiscardPresenter>();
            var turnManager = resolver.Resolve<TurnManager>();
            var drawCardEffector = resolver.Resolve<DrawCardEffector>();
            turnManager.Inject(handPresenter);
            drawCardEffector.Inject(handPresenter, turnManager);

            DIContainer.Register<HandPresenter>(handPresenter);
            DIContainer.Register<DiscardPresenter>(discardPresenter);
            DIContainer.Register<TurnManager>(turnManager);
        });
    }

    private void ConfigureFieldUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(_fieldUIDesigner);
        builder.RegisterInstance(_atkFieldContext).Keyed(FieldType.Attack);
        builder.RegisterInstance(_defFieldContext).Keyed(FieldType.Defense);

        builder.RegisterInstance<IFieldUI>(_atkFieldContext.FieldUI).Keyed(FieldType.Attack);
        builder.RegisterInstance<IFieldUI>(_defFieldContext.FieldUI).Keyed(FieldType.Defense);

        builder.RegisterInstance(new CardContainer<IFieldCardUI, FieldCardPresenter>()).Keyed(FieldType.Attack);
        builder.RegisterInstance(new CardContainer<IFieldCardUI, FieldCardPresenter>()).Keyed(FieldType.Defense);

        builder.RegisterInstance(_atkFieldContext.FieldCardLayout).Keyed(FieldType.Attack);
        builder.RegisterInstance(_defFieldContext.FieldCardLayout).Keyed(FieldType.Defense);

        builder.RegisterInstance(_atkFieldContext.FieldCardEvent).Keyed(FieldType.Attack);
        builder.RegisterInstance(_defFieldContext.FieldCardEvent).Keyed(FieldType.Defense);

        builder.RegisterInstance(_atkFieldContext.FieldCardFactory).Keyed(FieldType.Attack);
        builder.RegisterInstance(_defFieldContext.FieldCardFactory).Keyed(FieldType.Defense);
        builder.RegisterInstance<ICardFactory<IFieldCardUI>>(_atkFieldContext.FieldCardFactory).Keyed(FieldType.Attack);
        builder.RegisterInstance<ICardFactory<IFieldCardUI>>(_defFieldContext.FieldCardFactory).Keyed(FieldType.Defense);

        builder.RegisterEntryPoint<AttackFieldPresenter>()
               .AsSelf()
               .As<IAttackFieldCardRemovePort>()
               .As<IATKCardDropTarget>();
        builder.RegisterEntryPoint<DefendFieldPresenter>()
               .AsSelf()
               .As<IDefendFieldCardRemovePort>()
               .As<IDEFCardDropTarget>();

        builder.RegisterBuildCallback(resolver =>
        {
            var atkPresenter = resolver.Resolve<AttackFieldPresenter>();
            var defPresenter = resolver.Resolve<DefendFieldPresenter>();
            var handPresenter = resolver.Resolve<HandPresenter>();
            var discardPresenter = resolver.Resolve<DiscardPresenter>();
            var cardDropSystem = resolver.Resolve<CardDropSystem>();
            var fieldUIDesigner = resolver.Resolve<FieldUIDesigner>();

            var atkContainer = resolver.Resolve<CardContainer<IFieldCardUI, FieldCardPresenter>>(FieldType.Attack);
            var defContainer = resolver.Resolve<CardContainer<IFieldCardUI, FieldCardPresenter>>(FieldType.Defense);

            var atkLayout = resolver.Resolve<FieldCardLayoutController>(FieldType.Attack);
            var defLayout = resolver.Resolve<FieldCardLayoutController>(FieldType.Defense);

            var atkEvent = resolver.Resolve<FieldCardEventController>(FieldType.Attack);
            var defEvent = resolver.Resolve<FieldCardEventController>(FieldType.Defense);
            var atkCardToThrowEffector = resolver.Resolve<AttackCardToThrowEffector>();
            var defCardToThrowEffector = resolver.Resolve<DefendCardToThrowEffector>();

            atkEvent.Construct(atkPresenter,
                               defPresenter,
                               atkContainer,
                               defContainer,
                               atkLayout,
                               defLayout,
                               defEvent,
                               cardDropSystem,
                               fieldUIDesigner,
                               GameData.Instance.attackField);

            defEvent.Construct(defPresenter,
                               atkPresenter,
                               defContainer,
                               atkContainer,
                               defLayout,
                               atkLayout,
                               atkEvent,
                               cardDropSystem,
                               fieldUIDesigner,
                               GameData.Instance.defenseField);

            _atkFieldContext.FieldCardFactory.Construct(atkEvent);
            _defFieldContext.FieldCardFactory.Construct(defEvent);
            atkCardToThrowEffector.Construct(atkPresenter, atkContainer);
            defCardToThrowEffector.Construct(defPresenter, defContainer);

            handPresenter.OnTogglePreviews += atkPresenter.TogglePreview;
            handPresenter.OnTogglePreviews += defPresenter.TogglePreview;

            discardPresenter.OnDiscardUIVisibilityChanged += atkPresenter.UpdateInteraction;
            discardPresenter.OnDiscardUIVisibilityChanged += defPresenter.UpdateInteraction;
        });
    }
}

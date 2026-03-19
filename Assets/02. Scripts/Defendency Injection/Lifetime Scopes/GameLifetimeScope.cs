using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Designer")]
    [SerializeField] private HandUIDesigner handUIDesigner;
    [SerializeField] private FieldUIDesigner fieldUIDesigner;
    [SerializeField] private DiscardUIDesigner discardUIDesigner;
    [SerializeField] private TurnRuleDesigner turnRuleDesigner;

    [Space(20), Header("Field Context")]
    [SerializeField] private FieldContext atkFieldContext;
    [SerializeField] private FieldContext defFieldContext;

    protected override void Configure(IContainerBuilder builder)
    {
        ConfigureCore(builder);
        ConfigureManualUI(builder);
        ConfigureDeckUI(builder);
        ConfigureDiscardUI(builder);
        ConfigureHandUI(builder);
        ConfigureFieldUI(builder);
        ConfigureResultUI(builder);
    }

    private void ConfigureCore(IContainerBuilder builder)
    {
        builder.RegisterInstance<ITurnRuleService>(turnRuleDesigner);
        builder.RegisterComponentInHierarchy<TurnManager>()
               .AsSelf()
               .As<ITurnHandLimitPort>();

        builder.RegisterComponentInHierarchy<StatusUI>().As<IStatusUI>();
        builder.RegisterEntryPoint<StatusPresenter>(Lifetime.Scoped).AsSelf();
        builder.Register<CardDropSystem>(Lifetime.Singleton);
        
        builder.RegisterComponentInHierarchy<NotifierUI>().As<INotifierUI>();
        
        builder.RegisterComponentInHierarchy<CardInfoUI>();
        
        builder.RegisterComponentInHierarchy<DrawCardEffector>();
        builder.RegisterComponentInHierarchy<AttackCardToThrowEffector>();
        builder.RegisterComponentInHierarchy<DefendCardToThrowEffector>();
    }

    private void ConfigureManualUI(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ActionManualUI>().As<IActionManualUI>();
        builder.RegisterEntryPoint<ActionManualPresenter>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<DiscardManualUI>().As<IDiscardManualUI>();
        builder.RegisterEntryPoint<DiscardManualPresenter>(Lifetime.Scoped);
    }

    private void ConfigureDeckUI(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<DeckUI>().As<IDeckUI>();
        builder.RegisterComponentInHierarchy<DeckCardFactory>().As<ICardFactory<IDeckCardUI>>();
        builder.RegisterInstance(new CardContainer<IDeckCardUI, DeckCardPresenter>());
        builder.RegisterEntryPoint<DeckPresenter>(Lifetime.Scoped);
    }

    private void ConfigureDiscardUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(discardUIDesigner);
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
        builder.RegisterInstance(handUIDesigner);
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
            drawCardEffector.Inject(handPresenter, turnManager);

            DIContainer.Register<HandPresenter>(handPresenter);
            DIContainer.Register<DiscardPresenter>(discardPresenter);
            DIContainer.Register<TurnManager>(turnManager);
        });
    }

    private void ConfigureFieldUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(fieldUIDesigner);
        builder.RegisterInstance(atkFieldContext).Keyed(FieldType.Attack);
        builder.RegisterInstance(defFieldContext).Keyed(FieldType.Defense);

        builder.RegisterInstance<IFieldUI>(atkFieldContext.FieldUI).Keyed(FieldType.Attack);
        builder.RegisterInstance<IFieldUI>(defFieldContext.FieldUI).Keyed(FieldType.Defense);

        builder.RegisterInstance(new CardContainer<IFieldCardUI, FieldCardPresenter>()).Keyed(FieldType.Attack);
        builder.RegisterInstance(new CardContainer<IFieldCardUI, FieldCardPresenter>()).Keyed(FieldType.Defense);

        builder.RegisterInstance(atkFieldContext.FieldCardLayout).Keyed(FieldType.Attack);
        builder.RegisterInstance(defFieldContext.FieldCardLayout).Keyed(FieldType.Defense);

        builder.RegisterInstance(atkFieldContext.FieldCardEvent).Keyed(FieldType.Attack);
        builder.RegisterInstance(defFieldContext.FieldCardEvent).Keyed(FieldType.Defense);

        builder.RegisterInstance(atkFieldContext.FieldCardFactory).Keyed(FieldType.Attack);
        builder.RegisterInstance(defFieldContext.FieldCardFactory).Keyed(FieldType.Defense);
        builder.RegisterInstance<ICardFactory<IFieldCardUI>>(atkFieldContext.FieldCardFactory).Keyed(FieldType.Attack);
        builder.RegisterInstance<ICardFactory<IFieldCardUI>>(defFieldContext.FieldCardFactory).Keyed(FieldType.Defense);

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

            atkFieldContext.FieldCardFactory.Construct(atkEvent);
            defFieldContext.FieldCardFactory.Construct(defEvent);
            atkCardToThrowEffector.Construct(atkPresenter, atkContainer);
            defCardToThrowEffector.Construct(defPresenter, defContainer);

            handPresenter.OnTogglePreviews += atkPresenter.TogglePreview;
            handPresenter.OnTogglePreviews += defPresenter.TogglePreview;

            discardPresenter.OnDiscardUIVisibilityChanged += atkPresenter.UpdateInteraction;
            discardPresenter.OnDiscardUIVisibilityChanged += defPresenter.UpdateInteraction;
        });
    }

    private void ConfigureResultUI(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<ResultPresenter>(Lifetime.Scoped).AsSelf();
        builder.RegisterComponentInHierarchy<ResultUI>().As<IResultUI>();
        
        builder.RegisterEntryPoint<ResultRewardPresenter>(Lifetime.Scoped).AsSelf();
        builder.RegisterComponentInHierarchy<ResultRewardUI>().As<IResultRewardUI>();
        
        builder.RegisterEntryPoint<ResultShopPresenter>(Lifetime.Scoped).AsSelf();
        builder.RegisterInstance(new CardContainer<IResultCardUI, ResultCardPresenter>());
        builder.RegisterComponentInHierarchy<ResultCardFactory>();
        builder.RegisterComponentInHierarchy<ResultShopUI>().As<IResultShopUI>();

        builder.RegisterEntryPoint<ResultDeckInvenPresenter>(Lifetime.Scoped).AsSelf();
        builder.RegisterInstance<ICardBehavior>(new ReadonlyCardBehavior());

        builder.RegisterComponentInHierarchy<ResultDeckInvenUI>().AsSelf().As<IDeckInvenUI>();
        builder.Register<IDeckInvenUI>(resolver => resolver.Resolve<ResultDeckInvenUI>(), Lifetime.Scoped)
               .Keyed(DeckInvenType.Result);

        var resultDeckInvenCardContainer = new CardContainer<IDeckInvenCardUI, DeckInvenCardPresenter>();
        builder.RegisterInstance(resultDeckInvenCardContainer);
        builder.RegisterInstance(resultDeckInvenCardContainer).Keyed(DeckInvenType.Result);

        builder.RegisterComponentInHierarchy<ResultDeckInvenCardFactory>().AsSelf().As<ICardFactory<IDeckInvenCardUI>>();
        builder.Register<ICardFactory<IDeckInvenCardUI>>(resolver => resolver.Resolve<ResultDeckInvenCardFactory>(), Lifetime.Scoped)
               .Keyed(DeckInvenType.Result);
        
        builder.RegisterComponentInHierarchy<ResultUISequencer>();

        builder.RegisterBuildCallback(resolver =>
        {
            var resultPresenter = resolver.Resolve<ResultPresenter>();
            var resultRewardPresenter = resolver.Resolve<ResultRewardPresenter>();
            var resultUISequencer = resolver.Resolve<ResultUISequencer>();

            DIContainer.Register<ResultPresenter>(resultPresenter);
            DIContainer.Register<ResultRewardPresenter>(resultRewardPresenter);
            DIContainer.Register<ResultUISequencer>(resultUISequencer);
        });
    }
}

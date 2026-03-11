using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Designer")]
    [SerializeField] private HandUIDesigner _handUIDesigner;
    [SerializeField] private FieldUIDesigner _fieldUIDesigner;
    [SerializeField] private ThrowUIDesigner _throwUIDesigner;
    [SerializeField] private TurnRuleDesigner _turnRuleDesigner;

    [Space(20), Header("Field Context")]
    [SerializeField] private FieldContext _atkFieldContext;
    [SerializeField] private FieldContext _defFieldContext;

    protected override void Configure(IContainerBuilder builder)
    {
        ConfigureCore(builder);
        ConfigureThrowUI(builder);
        ConfigureHandUI(builder);
        ConfigureFieldUI(builder);
    }

    private void ConfigureCore(IContainerBuilder builder)
    {
        builder.RegisterInstance<ITurnRuleService>(_turnRuleDesigner);
        builder.RegisterComponentInHierarchy<TurnManager>();
        builder.RegisterComponentInHierarchy<Notice>().As<INotice>();
        builder.RegisterComponentInHierarchy<CardInfoUI>();
        builder.Register<CardDropSystem>(Lifetime.Singleton);
    }

    private void ConfigureThrowUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(_throwUIDesigner);
        builder.RegisterInstance(new ThrowCardContainer());
        builder.RegisterComponentInHierarchy<LayoutThrowView>().AsSelf().As<IThrowView>();
        builder.RegisterComponentInHierarchy<ThrowCardEventController>();
        builder.RegisterComponentInHierarchy<ThrowCardLayoutController>();
        builder.RegisterComponentInHierarchy<ThrowCardFactory>().AsSelf().As<IThrowCardFactory>();
        builder.Register<ThrowPresenter>(Lifetime.Singleton)
               .AsSelf()
               .As<ICardDropTarget<IThrowCardView>>();

        builder.RegisterBuildCallback(resolver =>
        {
            _ = resolver.Resolve<ThrowPresenter>();
            var throwView = resolver.Resolve<LayoutThrowView>();
            var throwContainer = resolver.Resolve<ThrowCardContainer>();
            var throwDesigner = resolver.Resolve<ThrowUIDesigner>();
            var throwLayout = resolver.Resolve<ThrowCardLayoutController>();
            var throwEvent = resolver.Resolve<ThrowCardEventController>();
            var throwFactory = resolver.Resolve<ThrowCardFactory>();

            throwView.Inject(throwContainer,
                             throwDesigner,
                             throwLayout,
                             throwEvent,
                             throwFactory);
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
               .As<ICardDropTarget<IHandCardUI>>();

        builder.RegisterBuildCallback(resolver =>
        {
            var handEvent = resolver.Resolve<HandCardEventController>();
            var handFactory = resolver.Resolve<HandCardFactory>();
            handFactory.Construct(handEvent);

            var handPresenter = resolver.Resolve<HandPresenter>();
            var throwPresenter = resolver.Resolve<ThrowPresenter>();
            var turnManager = resolver.Resolve<TurnManager>();
            turnManager.Inject(handPresenter);

            DIContainer.Register<HandPresenter>(handPresenter);
            DIContainer.Register<ThrowPresenter>(throwPresenter);
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
               .As<IATKCardDropTarget>();
        builder.RegisterEntryPoint<DefendFieldPresenter>()
               .AsSelf()
               .As<IDEFCardDropTarget>();

        builder.RegisterBuildCallback(resolver =>
        {
            var atkPresenter = resolver.Resolve<AttackFieldPresenter>();
            var defPresenter = resolver.Resolve<DefendFieldPresenter>();
            var handPresenter = resolver.Resolve<HandPresenter>();
            var cardDropSystem = resolver.Resolve<CardDropSystem>();
            var fieldUIDesigner = resolver.Resolve<FieldUIDesigner>();

            var atkContainer = resolver.Resolve<CardContainer<IFieldCardUI, FieldCardPresenter>>(FieldType.Attack);
            var defContainer = resolver.Resolve<CardContainer<IFieldCardUI, FieldCardPresenter>>(FieldType.Defense);

            var atkLayout = resolver.Resolve<FieldCardLayoutController>(FieldType.Attack);
            var defLayout = resolver.Resolve<FieldCardLayoutController>(FieldType.Defense);

            var atkEvent = resolver.Resolve<FieldCardEventController>(FieldType.Attack);
            var defEvent = resolver.Resolve<FieldCardEventController>(FieldType.Defense);

            atkEvent.Inject(atkPresenter,
                            defPresenter,
                            atkContainer,
                            defContainer,
                            atkLayout,
                            defLayout,
                            defEvent,
                            cardDropSystem,
                            fieldUIDesigner,
                            GameData.Instance.attackField);

            defEvent.Inject(defPresenter,
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

            handPresenter.OnTogglePreviews += atkPresenter.TogglePreview;
            handPresenter.OnTogglePreviews += defPresenter.TogglePreview;
        });
    }
}

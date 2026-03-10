using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Designer")]
    [SerializeField] private HandUIDesigner _handUIDesigner;

    protected override void Configure(IContainerBuilder builder)
    {
        ConfigureHandUI(builder);
    }

    private void ConfigureHandUI(IContainerBuilder builder)
    {
        builder.RegisterInstance(_handUIDesigner);
        builder.RegisterComponentInHierarchy<HandUI>().As<IHandUI>();
        builder.RegisterComponentInHierarchy<HandCardEventController>();
        builder.RegisterComponentInHierarchy<HandCardLayoutController>();
        builder.RegisterComponentInHierarchy<HandCardFactory>().As<ICardFactory<IHandCardUI>>();
        builder.RegisterComponentInHierarchy<HandCardToThrowEffector>();
        builder.RegisterEntryPoint<HandPresenter>();
    }
}

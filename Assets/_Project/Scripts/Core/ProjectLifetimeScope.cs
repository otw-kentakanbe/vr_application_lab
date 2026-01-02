using UnityEngine;
using VContainer;
using VContainer.Unity;

[DefaultExecutionOrder(-10000)]
public sealed class ProjectLifetimeScope : LifetimeScope
{
    [Header("Scene References")]
    [SerializeField] private AppStateHolder appStateHolder;
    [SerializeField] private PostProcessStateBinder postProcessStateBinder;
    [SerializeField] private TouchToggleButton touchToggleButton;
    [SerializeField] private AppStatePresenter appStatePresenter;
    [SerializeField] private PowerChangedListener powerChangedListener;

    protected override void Configure(IContainerBuilder builder)
    {
        // << VContainer Processs >>
        if (appStateHolder != null) builder.RegisterComponent(appStateHolder);
        if (postProcessStateBinder != null) builder.RegisterComponent(postProcessStateBinder);
        if (touchToggleButton != null) builder.RegisterComponent(touchToggleButton);
        if (appStatePresenter != null) builder.RegisterComponent(appStatePresenter);
        if (powerChangedListener != null) builder.RegisterComponent(powerChangedListener);
    }
}

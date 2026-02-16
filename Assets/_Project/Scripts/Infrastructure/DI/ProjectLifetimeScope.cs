using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Secene 全体の依存関係を管理する LifetimeScope クラス
/// </summary>
[DefaultExecutionOrder(-10000)]
public sealed class ProjectLifetimeScope : LifetimeScope
{
    [Header("Scene References")]
    [SerializeField] private AppStateHolder appStateHolder;
    [SerializeField] private PowerStateEffectsController powerStateEffectsController;
    [SerializeField] private PowerToggleInteractor powerToggleInteractor;
    [SerializeField] private PowerStatePresenter powerStatePresenter;
    [SerializeField] private WeatherForecastUI weatherForecastUI;
    [SerializeField] private WeatherForecastConfig weatherForecastConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        RegisterSharedState(builder);
        RegisterPower(builder);
        RegisterWeather(builder);
    }

    private void RegisterSharedState(IContainerBuilder builder)
    {
        if (appStateHolder != null) builder.RegisterComponent(appStateHolder);
    }

    private void RegisterPower(IContainerBuilder builder)
    {
        if (powerStateEffectsController != null)
        {
            // IPowerStateOutput インターフェースとして登録することで、PowerStateEffectsController を直接参照せずに、IPowerStateOutput として依存注入できるようになる
            // 直接参照しないことで、PowerStateEffectsController を別の実装に差し替えやすくなり、柔軟性が向上する
            // 例えば、テスト用のモック実装を作成して、IPowerStateOutput として登録すれば、PowerStatePresenter のテストが容易になる
            builder.RegisterComponent(powerStateEffectsController).As<IPowerStateOutput>();
        }

        if (powerToggleInteractor != null)
        {
            builder.RegisterComponent(powerToggleInteractor).As<IPowerToggleInput>();
            builder.Register(_ =>
                // PowerToggleInteractor の transform を受け取って、クリックエフェクトを再生する
                new PowerToggleClickedView(
                    powerToggleInteractor.InteractorTransform,
                    new Vector3(4f, 0f, 0f),
                    0.5f,
                    1,
                    3.0f),
                Lifetime.Singleton);
        }

        if (powerStatePresenter != null) builder.RegisterComponent(powerStatePresenter);
    }

    private void RegisterWeather(IContainerBuilder builder)
    {
        if (weatherForecastUI != null) builder.RegisterComponent(weatherForecastUI);
        if (weatherForecastConfig != null) builder.RegisterInstance(weatherForecastConfig);

        // model: config 注入
        builder.Register(resolver =>
        {
            var config = resolver.Resolve<WeatherForecastConfig>();
            return new WeatherForecastModel(config.BaseUrl, config.CacheTtlSeconds);
        }, Lifetime.Singleton);

        // viewmodel: model + config 注入
        builder.Register<WeatherForecastViewModel>(Lifetime.Singleton);
    }
}

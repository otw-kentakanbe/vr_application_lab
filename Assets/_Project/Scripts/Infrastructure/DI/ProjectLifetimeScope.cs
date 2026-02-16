using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Scene 全体の依存関係を管理する LifetimeScope クラス
/// </summary>
[DefaultExecutionOrder(-10000)]
public sealed class ProjectLifetimeScope : LifetimeScope
{
    private static readonly Vector3 ToggleClickJumpEndPosition = new(4f, 0f, 0f);
    private const float ToggleClickJumpPower = 0.5f;
    private const int ToggleClickJumpCount = 1;
    private const float ToggleClickJumpDurationSeconds = 3.0f;

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
        if (appStateHolder == null) return;

        builder.RegisterComponent(appStateHolder);
    }

    private void RegisterPower(IContainerBuilder builder)
    {
        RegisterPowerOutput(builder);
        RegisterPowerInput(builder);
        RegisterPowerPresenter(builder);
    }

    private void RegisterWeather(IContainerBuilder builder)
    {
        RegisterWeatherView(builder);
        RegisterWeatherConfig(builder);

        builder.Register(resolver =>
        {
            var config = resolver.Resolve<WeatherForecastConfig>();
            return new WeatherForecastModel(config.BaseUrl, config.CacheTtlSeconds);
        }, Lifetime.Singleton);

        builder.Register<WeatherForecastViewModel>(Lifetime.Singleton);
    }

    private void RegisterPowerOutput(IContainerBuilder builder)
    {
        if (powerStateEffectsController == null) return;
        // IPowerStateOutput インターフェースとして登録することで、PowerStateEffectsController を直接参照せずに、IPowerStateOutput として依存注入できるようになる
        // 直接参照しないことで、PowerStateEffectsController を別の実装に差し替えやすくなり、柔軟性が向上する
        // 例えば、テスト用のモック実装を作成して、IPowerStateOutput として登録すれば、PowerStatePresenter のテストが容易になる
        builder.RegisterComponent(powerStateEffectsController).As<IPowerStateOutput>();
    }

    private void RegisterPowerInput(IContainerBuilder builder)
    {
        if (powerToggleInteractor == null) return;

        builder.RegisterComponent(powerToggleInteractor).As<IPowerToggleInput>();
        builder.Register(_ =>
            // PowerToggleInteractor の transform を受け取って、クリックエフェクトを再生する
            new PowerToggleClickedView(
                powerToggleInteractor.InteractorTransform,
                ToggleClickJumpEndPosition,
                ToggleClickJumpPower,
                ToggleClickJumpCount,
                ToggleClickJumpDurationSeconds),
            Lifetime.Singleton);
    }

    private void RegisterPowerPresenter(IContainerBuilder builder)
    {
        if (powerStatePresenter == null) return;
        builder.RegisterComponent(powerStatePresenter);
    }

    private void RegisterWeatherView(IContainerBuilder builder)
    {
        if (weatherForecastUI == null) return;
        builder.RegisterComponent(weatherForecastUI);
    }

    private void RegisterWeatherConfig(IContainerBuilder builder)
    {
        if (weatherForecastConfig == null) return;
        builder.RegisterInstance(weatherForecastConfig);
    }
}

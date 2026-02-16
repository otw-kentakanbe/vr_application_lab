using UnityEngine;
using VContainer;
using VContainer.Unity;

/**
* Secene 全体の依存関係を管理する LifetimeScope クラス
*/
// set the execution order to the front to avoid injection timing inconsistencies.
[DefaultExecutionOrder(-10000)]
public sealed class ProjectLifetimeScope : LifetimeScope
{
    [Header("Scene References")]
    [SerializeField] private AppStateHolder appStateHolder;
    [SerializeField] private PostProcessStateBinder postProcessStateBinder;
    [SerializeField] private TouchToggleButton touchToggleButton;
    [SerializeField] private AppStatePresenter appStatePresenter;
    [SerializeField] private WeatherForecastUI weatherForecastUI;
    [SerializeField] private WeatherForecastConfig weatherForecastConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        RegisterSceneComponents(builder);
        RegisterWeatherForecast(builder);
    }

    // Scene 上のコンポーネントや ScriptableObject をコンテナに登録
    private void RegisterSceneComponents(IContainerBuilder builder)
    {
        if (appStateHolder != null) builder.RegisterComponent(appStateHolder);
        if (postProcessStateBinder != null) builder.RegisterComponent(postProcessStateBinder);
        if (touchToggleButton != null) builder.RegisterComponent(touchToggleButton);
        if (appStatePresenter != null) builder.RegisterComponent(appStatePresenter);
        if (weatherForecastUI != null) builder.RegisterComponent(weatherForecastUI);
        if (weatherForecastConfig != null) builder.RegisterInstance(weatherForecastConfig);
    }

    private void RegisterWeatherForecast(IContainerBuilder builder)
    {
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

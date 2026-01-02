using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

public sealed class WeatherForecastViewModel : IDisposable
{
    private readonly WeatherForecastModel _model;

    public WeatherForecastViewModel(WeatherForecastConfig config)
    {
        _model = new WeatherForecastModel(config.BaseUrl, config.CacheTtlSeconds);
        Cities = config.Cities;
        DisplayText = new ReactiveProperty<string>("Ready");
    }

    public IReadOnlyList<WeatherForecastConfig.CityConfig> Cities { get; }
    public ReactiveProperty<string> DisplayText { get; }

    public async UniTask SelectCity(WeatherForecastConfig.CityConfig city, CancellationToken token)
    {
        DisplayText.Value = $"Loading {city.DisplayName}...";
        DisplayText.Value = await _model.FetchCityAsync(city, token);
    }

    public void Dispose()
    {
        DisplayText.Dispose();
    }
}

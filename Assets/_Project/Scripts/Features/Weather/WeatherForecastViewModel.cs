using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

/**
* WeatherForecastViewModel クラス
* - Model からデータを取得し、UI に表示するためのロジックを担当する
*   - Model 経由で都市の天気予報を取得し、その結果を ReactiveProperty<string> として UI に提供
*/
public sealed class WeatherForecastViewModel : IDisposable
{
    private readonly WeatherForecastModel _model;

    public WeatherForecastViewModel(WeatherForecastModel model, WeatherForecastConfig config)
    {
        _model = model;
        Cities = config.Cities;
        ReactiveDisplayText = new ReactiveProperty<string>("Ready");
    }

    public IReadOnlyList<WeatherForecastConfig.CityConfig> Cities { get; }
    public ReactiveProperty<string> ReactiveDisplayText { get; }

    public async UniTask SelectCity(WeatherForecastConfig.CityConfig city, CancellationToken token)
    {
        ReactiveDisplayText.Value = $"Loading {city.DisplayName}...";
        ReactiveDisplayText.Value = await _model.FetchCityAsync(city, token);
    }

    // ViewModel は IDisposable を実装して、ReactiveProperty の Dispose を呼び出すことで、購読解除やリソース解放を行う
    public void Dispose()
    {
        ReactiveDisplayText.Dispose();
    }
}

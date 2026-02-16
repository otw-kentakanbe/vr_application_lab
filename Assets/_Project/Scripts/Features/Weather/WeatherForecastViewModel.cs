using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

/// <summary>
/// WeatherForecastViewModel クラス
/// - WeatherForecastModel からデータを取得して、UI に表示するためのロジックを担当する
/// - 都市が選択されたときに、WeatherForecastModel の FetchCityAsync を呼び出して、結果を ReactiveProperty<string> に反映する
/// </summary>
public sealed class WeatherForecastViewModel : IDisposable
{
    private const string LogPrefix = "[WeatherForecastViewModel]";

    private readonly WeatherForecastModel _model;
    private CancellationTokenSource _requestCts;

    public WeatherForecastViewModel(WeatherForecastModel model, WeatherForecastConfig config)
    {
        _model = model;
        Cities = config.Cities;
        ReactiveDisplayText = new ReactiveProperty<string>("Ready");
        ReactiveIsLoading = new ReactiveProperty<bool>(false);
    }

    public IReadOnlyList<WeatherForecastConfig.CityConfig> Cities { get; }
    public ReactiveProperty<string> ReactiveDisplayText { get; }
    public ReactiveProperty<bool> ReactiveIsLoading { get; }

    public async UniTask SelectCityAsync(WeatherForecastConfig.CityConfig city, CancellationToken token)
    {
        if (ReactiveIsLoading.Value) return;

        _requestCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        ReactiveIsLoading.Value = true;
        ReactiveDisplayText.Value = $"Loading {city.DisplayName}...";

        try
        {
            ReactiveDisplayText.Value = await _model.FetchCityAsync(city, _requestCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation caused by view lifecycle stop.
        }
        catch (Exception ex)
        {
            Debug.LogError($"{LogPrefix} Unexpected exception while fetching weather.");
            Debug.LogException(ex);
            ReactiveDisplayText.Value = "Unexpected Error.";
        }
        finally
        {
            ReactiveIsLoading.Value = false;
            CancelAndDisposeRequestCts();
        }
    }

    // ViewModel は IDisposable を実装して、ReactiveProperty の Dispose を呼び出すことで、購読解除やリソース解放を行う
    public void Dispose()
    {
        CancelAndDisposeRequestCts();
        ReactiveDisplayText.Dispose();
        ReactiveIsLoading.Dispose();
    }

    private void CancelAndDisposeRequestCts()
    {
        _requestCts?.Cancel();
        _requestCts?.Dispose();
        _requestCts = null;
    }
}

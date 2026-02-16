using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// WeatherForecastModel クラス
/// - 天気予報の API からデータを取得する
/// </summary>
public sealed class WeatherForecastModel
{
    private const int DefaultCacheTtlSeconds = 60 * 60;
    private const string NoDataText = "No data.";

    private readonly string _baseUrl;
    private readonly int _cacheTtlSeconds;
    private readonly Dictionary<string, CacheEntry> _cache = new();

    public WeatherForecastModel(string baseUrl, int cacheTtlSeconds)
    {
        _baseUrl = baseUrl;
        _cacheTtlSeconds = cacheTtlSeconds > 0 ? cacheTtlSeconds : DefaultCacheTtlSeconds;
    }

    public async UniTask<string> FetchCityAsync(WeatherForecastConfig.CityConfig city, CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // return cache if exists and not expired.
        if (_cache.TryGetValue(city.Key, out var cached) && now - cached.FetchedAt <= _cacheTtlSeconds)
        {
            return cached.DisplayText;
        }

        var url = $"{_baseUrl}&latitude={city.Latitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&longitude={city.Longitude.ToString(CultureInfo.InvariantCulture)}";

        // using var: IDisposable を実装しているオブジェクトをスコープの終わりで自動的に破棄する
        using var req = UnityWebRequest.Get(url);
        await req.SendWebRequest().ToUniTask(cancellationToken: token);

        if (req.result != UnityWebRequest.Result.Success)
        {
            return $"API Error: {req.error}";
        }

        var json = req.downloadHandler.text;
        var data = JsonUtility.FromJson<OpenMeteoResponseDto>(json);
        var displayText = BuildDisplayText(city.DisplayName, data);

        // set the cache save time to “when the successful response is received”.
        _cache[city.Key] = new CacheEntry(displayText, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return displayText;
    }

    private static string BuildDisplayText(string city, OpenMeteoResponseDto data)
    {
        if (data?.hourly?.time == null || data.hourly.temperature_2m == null ||
            data.hourly.time.Length == 0 || data.hourly.temperature_2m.Length == 0)
        {
            return $"{city}\n{NoDataText}";
        }

        var time = data.hourly.time[0];
        var temp = data.hourly.temperature_2m[0];

        return $"{city}\nTimezone: {data.timezone}\nTime: {time}\nTemp: {temp} °C";
    }

    private readonly struct CacheEntry
    {
        public CacheEntry(string displayText, long fetchedAt)
        {
            DisplayText = displayText;
            FetchedAt = fetchedAt;
        }

        public string DisplayText { get; }
        public long FetchedAt { get; }
    }
}

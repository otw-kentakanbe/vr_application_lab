using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public sealed class OpenMeteoResponse
{
    public string timezone;
    public Hourly hourly;
}

[Serializable]
public sealed class Hourly
{
    public string[] time;
    public float[] temperature_2m;
}

public sealed class WeatherForecastUI : MonoBehaviour
{
    private const int CacheTtlSeconds = 60 * 60;

    [Header("API")]
    [SerializeField] private string baseUrl =
        "https://api.open-meteo.com/v1/forecast?hourly=temperature_2m";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private Button tokyoButton;
    [SerializeField] private Button newYorkButton;

    private readonly Dictionary<string, CacheEntry> _cache = new();
    private CancellationTokenSource _cts;

    private void Start()
    {
        if (outputText == null || tokyoButton == null || newYorkButton == null)
        {
            Debug.LogError("[WeatherForecastUI] UI references are not assigned.", this);
            enabled = false;
            return;
        }

        _cts = new CancellationTokenSource();
        tokyoButton.onClick.AddListener(() => FetchCity("Tokyo", 35.68f, 139.76f).Forget());
        tokyoButton.onClick.AddListener(() => Debug.Log("Tokyo clicked"));
        newYorkButton.onClick.AddListener(() => FetchCity("NewYork", 40.71f, -74.01f).Forget());
        outputText.text = "Ready";
    }

    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        if (tokyoButton != null) tokyoButton.onClick.RemoveAllListeners();
        if (newYorkButton != null) newYorkButton.onClick.RemoveAllListeners();
    }

    private async UniTask FetchCity(string city, float latitude, float longitude)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // using cache if it exists.
        if (_cache.TryGetValue(city, out var cached) && now - cached.FetchedAt <= CacheTtlSeconds)
        {
            outputText.text = cached.DisplayText;
            return;
        }

        outputText.text = $"Loading {city}...";

        var url = $"{baseUrl}&latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";

        // << API Connection Process >>
        using var req = UnityWebRequest.Get(url);
        await req.SendWebRequest().ToUniTask(cancellationToken: _cts.Token);

        if (req.result != UnityWebRequest.Result.Success)
        {
            outputText.text = $"API Error: {req.error}";
            return;
        }

        var json = req.downloadHandler.text;
        var data = JsonUtility.FromJson<OpenMeteoResponse>(json);
        var displayText = BuildDisplayText(city, data);

        _cache[city] = new CacheEntry(displayText, now);
        outputText.text = displayText;
    }

    private static string BuildDisplayText(string city, OpenMeteoResponse data)
    {
        if (data?.hourly?.time == null || data.hourly.temperature_2m == null || data.hourly.time.Length == 0)
        {
            return $"{city}\nNo data.";
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

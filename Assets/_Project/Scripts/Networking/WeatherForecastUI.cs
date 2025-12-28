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
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    [Header("Config")]
    [SerializeField] private WeatherForecastConfig config;

    private readonly Dictionary<string, CacheEntry> _cache = new();
    private readonly List<Button> _generatedButtons = new();
    private CancellationTokenSource _cts;

    private void Start()
    {
        if (outputText == null || config == null || buttonContainer == null || buttonPrefab == null)
        {
            Debug.LogError("[WeatherForecastUI] UI or config references are not assigned.", this);
            enabled = false;
            return;
        }

        _cts = new CancellationTokenSource();
        BuildCityButtons();
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

        CleanupButtons();
    }

    private async UniTask FetchCity(WeatherForecastConfig.CityConfig city)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // using cache if it exists.
        if (_cache.TryGetValue(city.Key, out var cached) && now - cached.FetchedAt <= config.CacheTtlSeconds)
        {
            outputText.text = cached.DisplayText;
            return;
        }

        outputText.text = $"Loading {city.DisplayName}...";

        var url = $"{config.BaseUrl}&latitude={city.Latitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&longitude={city.Longitude.ToString(CultureInfo.InvariantCulture)}";

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
        var displayText = BuildDisplayText(city.DisplayName, data);

        _cache[city.Key] = new CacheEntry(displayText, now);
        outputText.text = displayText;
    }

    private void BuildCityButtons()
    {
        CleanupButtons();

        foreach (var city in config.Cities)
        {
            var button = Instantiate(buttonPrefab, buttonContainer);
            _generatedButtons.Add(button);

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = city.DisplayName;

            button.onClick.AddListener(() => FetchCity(city).Forget());
        }
    }

    private void CleanupButtons()
    {
        foreach (var button in _generatedButtons)
        {
            if (button != null) button.onClick.RemoveAllListeners();
        }
        _generatedButtons.Clear();

        if (buttonContainer == null) return;
        for (var i = buttonContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonContainer.GetChild(i).gameObject);
        }
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

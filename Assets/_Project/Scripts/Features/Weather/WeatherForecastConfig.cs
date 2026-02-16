using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WeatherForecastConfig クラス
/// - 天気予報の API の endpoint や、都市の情報を保持する ScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Config/Weather Forecast Config")]
public sealed class WeatherForecastConfig : ScriptableObject
{
    [Header("API")]
    [SerializeField] private string _baseUrl =
        "https://api.open-meteo.com/v1/forecast?hourly=temperature_2m";
    [SerializeField] private int _cacheTtlSeconds = 60 * 60;

    [Header("Cities")]
    [SerializeField] private List<CityConfig> _cities = new();

    public string BaseUrl => _baseUrl;
    public int CacheTtlSeconds => _cacheTtlSeconds;
    public IReadOnlyList<CityConfig> Cities => _cities;

    [Serializable]
    public sealed class CityConfig
    {
        [SerializeField] private string _key = "tokyo";
        [SerializeField] private string _displayName = "Tokyo";
        [SerializeField] private float _latitude = 35.68f;
        [SerializeField] private float _longitude = 139.76f;

        public string Key => _key;
        public string DisplayName => _displayName;
        public float Latitude => _latitude;
        public float Longitude => _longitude;
    }
}

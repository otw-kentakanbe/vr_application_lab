using System;
using System.Collections.Generic;
using UnityEngine;

// << ScriptableObject Process >>
[CreateAssetMenu(menuName = "Config/Weather Forecast Config")]
public sealed class WeatherForecastConfig : ScriptableObject
{
    [Header("API")]
    [SerializeField] private string baseUrl =
        "https://api.open-meteo.com/v1/forecast?hourly=temperature_2m";
    [SerializeField] private int cacheTtlSeconds = 60 * 60;

    [Header("Cities")]
    [SerializeField] private List<CityConfig> cities = new();

    public string BaseUrl => baseUrl;
    public int CacheTtlSeconds => cacheTtlSeconds;
    public IReadOnlyList<CityConfig> Cities => cities;

    [Serializable]
    public sealed class CityConfig
    {
        [SerializeField] private string key = "tokyo";
        [SerializeField] private string displayName = "Tokyo";
        [SerializeField] private float latitude = 35.68f;
        [SerializeField] private float longitude = 139.76f;

        public string Key => key;
        public string DisplayName => displayName;
        public float Latitude => latitude;
        public float Longitude => longitude;
    }
}

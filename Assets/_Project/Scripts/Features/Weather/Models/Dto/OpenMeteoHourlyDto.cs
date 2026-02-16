using System;

/// <summary>
/// Open-Meteo API の hourly フィールド DTO。
/// </summary>
[Serializable]
// using internal, because this DTO is only used within the Weather feature and should not be exposed to other features.
internal sealed class OpenMeteoHourlyDto
{
    public string[] time;
    public float[] temperature_2m;
}

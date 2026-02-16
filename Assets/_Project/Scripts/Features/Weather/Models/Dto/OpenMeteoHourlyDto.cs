using System;

/// <summary>
/// Open-Meteo API の hourly フィールド DTO。
/// </summary>
[Serializable]
public sealed class OpenMeteoHourlyDto
{
    public string[] time;
    public float[] temperature_2m;
}

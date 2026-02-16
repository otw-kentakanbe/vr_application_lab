using System;

/// <summary>
/// Open-Meteo API のレスポンスルート DTO。
/// </summary>
[Serializable]
public sealed class OpenMeteoResponseDto
{
    public string timezone;
    public OpenMeteoHourlyDto hourly;
}

using System;

/// <summary>
/// Open-Meteo API のレスポンスルート DTO。
/// </summary>
[Serializable]
// using internal, because this DTO is only used within the Weather feature and should not be exposed to other features.
internal sealed class OpenMeteoResponseDto
{
    public string timezone;
    public OpenMeteoHourlyDto hourly;
}

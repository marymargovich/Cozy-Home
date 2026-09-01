namespace CozyHome.Weather
{
    /// <summary>
    /// Canonical weather state used by the visual weather system.
    /// StrongWind is included as a dedicated weather state for the more aggressive wind group.
    /// </summary>
    public enum WeatherType
    {
        None,
        Clear,
        LightRain,
        HeavyRain,
        Thunderstorm,
        LightSnow,
        Snowstorm,
        Hail,
        Fog,
        Wind,
        StrongWind
    }
}

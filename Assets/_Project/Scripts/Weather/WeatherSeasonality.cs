using System;
using System.Collections.Generic;
using UnityEngine;

namespace CozyHome.Weather
{
    /// <summary>
    /// Defines which weather states are valid for each calendar month.
    /// </summary>
    public static class WeatherSeasonality
    {
        public static bool IsWeatherAllowedForMonth(WeatherType type, int month)
        {
            if (month < 1 || month > 12)
            {
                return false;
            }

            switch (type)
            {
                case WeatherType.None:
                    return false;
                case WeatherType.Clear:
                case WeatherType.Fog:
                case WeatherType.Hail:
                    return true;
                case WeatherType.LightSnow:
                case WeatherType.Snowstorm:
                    return month == 12 || month == 1 || month == 2 || month == 3 || month == 4 || month == 11;
                case WeatherType.LightRain:
                case WeatherType.HeavyRain:
                case WeatherType.Thunderstorm:
                case WeatherType.Wind:
                case WeatherType.StrongWind:
                    return month >= 3 && month <= 11;
                default:
                    return false;
            }
        }

        public static bool IsWeatherAllowedForCurrentMonth(WeatherType type)
        {
            return IsWeatherAllowedForMonth(type, DateTime.Now.Month);
        }

        public static WeatherType[] GetAllowedWeatherTypesForMonth(int month)
        {
            List<WeatherType> allowedWeather = new List<WeatherType>();

            foreach (WeatherType weatherType in Enum.GetValues(typeof(WeatherType)))
            {
                if (weatherType != WeatherType.None && IsWeatherAllowedForMonth(weatherType, month))
                {
                    allowedWeather.Add(weatherType);
                }
            }

            return allowedWeather.ToArray();
        }

        public static WeatherType GetRandomWeatherForMonth(int month)
        {
            WeatherType[] allowedWeather = GetAllowedWeatherTypesForMonth(month);
            if (allowedWeather.Length == 0)
            {
                return WeatherType.Clear;
            }

            return allowedWeather[UnityEngine.Random.Range(0, allowedWeather.Length)];
        }

        public static WeatherType GetRandomWeatherForCurrentMonth()
        {
            return GetRandomWeatherForMonth(DateTime.Now.Month);
        }
    }
}

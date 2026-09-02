using System;
using UnityEngine;

namespace CozyHome.Weather
{
    /// <summary>
    /// Controls the active visual weather state.
    /// The existing random-weather logic can later call SetWeather(...) instead of using this test toggle.
    /// </summary>
    public class WeatherController : MonoBehaviour
    {
        public event Action<WeatherType> OnWeatherChanged;

        [Header("Weather groups")]
        [SerializeField] private WeatherVisualGroup lightRain;
        [SerializeField] private WeatherVisualGroup heavyRain;
        [SerializeField] private WeatherVisualGroup thunderstorm;
        [SerializeField] private WeatherVisualGroup lightSnow;
        [SerializeField] private WeatherVisualGroup snowstorm;
        [SerializeField] private WeatherVisualGroup hail;
        [SerializeField] private WeatherVisualGroup fog;
        [SerializeField] private WeatherVisualGroup wind;
        [SerializeField] private WeatherVisualGroup strongWind;

        [Header("Audio")]
        [SerializeField] private WeatherAudioManager weatherAudioManager;

        [Header("Test mode")]
        [SerializeField] private bool useTestWeatherOnStart = true;
        [SerializeField] private WeatherType testWeather = WeatherType.LightRain;

        public WeatherType CurrentWeather { get; private set; } = WeatherType.None;

        private WeatherVisualGroup[] allWeatherGroups;

        private void Awake()
        {
            CacheGroups();
            EnsureAudioManager();
            SetWeather(WeatherType.None);
        }

        private void Start()
        {
            CacheGroups();
            EnsureAudioManager();

            if (useTestWeatherOnStart)
            {
                SetWeather(testWeather);
                return;
            }

            SetRandomWeather();
        }

        private void OnValidate()
        {
            CacheGroups();
            ValidateWeatherSetup();
        }

        public void SetWeather(WeatherType type)
        {
            CacheGroups();

            for (int i = 0; i < allWeatherGroups.Length; i++)
            {
                WeatherVisualGroup group = allWeatherGroups[i];
                if (group != null)
                {
                    group.Stop();
                }
            }

            CurrentWeather = type;
            OnWeatherChanged?.Invoke(type);

            if (type == WeatherType.None || type == WeatherType.Clear)
            {
                if (type == WeatherType.Clear)
                {
                    weatherAudioManager?.SetWeather(type);
                    return;
                }

                weatherAudioManager?.StopWeather();
                return;
            }

            WeatherVisualGroup targetGroup = GetGroupForType(type);
            if (targetGroup != null)
            {
                targetGroup.Play();
            }

            weatherAudioManager?.SetWeather(type);
        }

        public void StopAllWeather()
        {
            SetWeather(WeatherType.None);
        }

        public void ValidateWeatherSetup()
        {
            CacheGroups();

            for (int i = 0; i < allWeatherGroups.Length; i++)
            {
                WeatherVisualGroup group = allWeatherGroups[i];
                if (group != null)
                {
                    continue;
                }
            }

            if (strongWind == null)
            {
                Debug.LogWarning("WeatherController: StrongWind visual group is not assigned. The StrongWind state will not activate in the inspector.");
            }

            if (weatherAudioManager == null)
            {
                EnsureAudioManager();
            }
        }

        private void EnsureAudioManager()
        {
            if (weatherAudioManager == null)
            {
                weatherAudioManager = GetComponent<WeatherAudioManager>();
            }

            if (weatherAudioManager == null)
            {
                weatherAudioManager = gameObject.AddComponent<WeatherAudioManager>();
            }
        }

        private void CacheGroups()
        {
            allWeatherGroups = new[]
            {
                lightRain,
                heavyRain,
                thunderstorm,
                lightSnow,
                snowstorm,
                hail,
                fog,
                wind,
                strongWind,
            };
        }

        public WeatherType GenerateRandomWeatherForMonth(int month)
        {
            return WeatherSeasonality.GetRandomWeatherForMonth(month);
        }

        public WeatherType GenerateRandomWeatherForCurrentMonth()
        {
            return WeatherSeasonality.GetRandomWeatherForCurrentMonth();
        }

        public bool IsWeatherAllowedForCurrentMonth(WeatherType type)
        {
            return WeatherSeasonality.IsWeatherAllowedForCurrentMonth(type);
        }

        public void SetRandomWeather()
        {
            SetWeather(GenerateRandomWeatherForCurrentMonth());
        }

        private WeatherVisualGroup GetGroupForType(WeatherType type)
        {
            switch (type)
            {
                case WeatherType.LightRain:
                    return lightRain;
                case WeatherType.HeavyRain:
                    return heavyRain;
                case WeatherType.Thunderstorm:
                    return thunderstorm;
                case WeatherType.LightSnow:
                    return lightSnow;
                case WeatherType.Snowstorm:
                    return snowstorm;
                case WeatherType.Hail:
                    return hail;
                case WeatherType.Fog:
                    return fog;
                case WeatherType.Wind:
                    return wind;
                case WeatherType.StrongWind:
                    return strongWind;
                case WeatherType.None:
                case WeatherType.Clear:
                default:
                    return null;
            }
        }
    }
}

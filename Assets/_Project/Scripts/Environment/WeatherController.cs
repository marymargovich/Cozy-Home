using UnityEngine;

namespace CozyHome.Weather
{
    /// <summary>
    /// Controls the active visual weather state.
    /// The existing random-weather logic can later call SetWeather(...) instead of using this test toggle.
    /// </summary>
    public class WeatherController : MonoBehaviour
    {
        [Header("Weather groups")]
        [SerializeField] private WeatherVisualGroup lightRain;
        [SerializeField] private WeatherVisualGroup heavyRain;
        [SerializeField] private WeatherVisualGroup thunderstorm;
        [SerializeField] private WeatherVisualGroup lightSnow;
        [SerializeField] private WeatherVisualGroup snowstorm;
        [SerializeField] private WeatherVisualGroup hail;
        [SerializeField] private WeatherVisualGroup fog;
        [SerializeField] private WeatherVisualGroup wind;

        [Header("Test mode")]
        [SerializeField] private bool useTestWeatherOnStart = true;
        [SerializeField] private WeatherType testWeather = WeatherType.LightRain;

        public WeatherType CurrentWeather { get; private set; } = WeatherType.Clear;

        private WeatherVisualGroup[] allWeatherGroups;

        private void Awake()
        {
            CacheGroups();
            SetWeather(WeatherType.Clear);
        }

        private void Start()
        {
            CacheGroups();

            if (useTestWeatherOnStart)
            {
                SetWeather(testWeather);
            }
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

            if (type == WeatherType.Clear)
            {
                return;
            }

            WeatherVisualGroup targetGroup = GetGroupForType(type);
            if (targetGroup != null)
            {
                targetGroup.Play();
            }
        }

        public void StopAllWeather()
        {
            SetWeather(WeatherType.Clear);
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
            };
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
                case WeatherType.Clear:
                default:
                    return null;
            }
        }
    }
}

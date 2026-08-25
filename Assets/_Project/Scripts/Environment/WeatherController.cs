using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.Environment
{
    /// <summary>
    /// Planned weather system for the window scene.
    /// </summary>
    [ExecuteAlways]
    public class WeatherController : MonoBehaviour
    {
        public enum WeatherState
        {
            Clear,
            LightRain,
            HeavyDownpour,
            Thunderstorm,
            Snowfall,
            Blizzard,
            Hail
        }

        [Header("Planned weather effects")]
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem heavyRainParticles;
        [SerializeField] private ParticleSystem snowfallParticles;
        [SerializeField] private ParticleSystem blizzardParticles;
        [SerializeField] private ParticleSystem hailParticles;
        [SerializeField] private Image lightningFlashOverlay;
        [SerializeField] private Image windowDropletsOverlay;

        private void Awake()
        {
            ApplyWeather(WeatherState.Clear);
        }

        public void ApplyWeather(WeatherState state)
        {
            // Planned future weather system.
        }
    }
}

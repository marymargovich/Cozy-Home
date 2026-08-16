using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.Environment
{
    /// <summary>
    /// Manages the active weather state for the window scene, including particles, overlay effects, and thunder flashes.
    /// </summary>
    [ExecuteAlways]
    public class WeatherController : MonoBehaviour
    {
        [Header("Particle Systems")]
        // The particle system used for light rain.
        [SerializeField] private ParticleSystem rainParticles;

        // The particle system used for heavy rain and downpours.
        [SerializeField] private ParticleSystem heavyRainParticles;

        // The particle system used for snow drift effects.
        [SerializeField] private ParticleSystem snowfallParticles;

        // The particle system used for blizzard conditions.
        [SerializeField] private ParticleSystem blizzardParticles;

        // The particle system used for hail conditions.
        [SerializeField] private ParticleSystem hailParticles;

        [Header("Overlays & Effects")]
        // The UI overlay used to add a quick lightning flash during thunderstorms.
        [SerializeField] private Image lightningFlashOverlay;

        // The UI overlay used to represent rain or droplet buildup on the window glass.
        [SerializeField] private Image windowDropletsOverlay;

        [Header("Season Context Reference")]
        // The seasonal controller used to filter incompatible weather states based on the active season.
        [SerializeField] private SeasonalLandscapeController seasonController;

        [Header("Debug / Testing")]
        // Enables the debug override so the designer can preview a specific weather state without random generation.
        [SerializeField] private bool overrideWeatherRNG = false;

        // The test weather state to apply while the override is active.
        [SerializeField] private WeatherState debugWeather;

        /// <summary>
        /// Represents the weather states available for the diorama scene.
        /// </summary>
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

        private Coroutine lightningRoutine;

        private void Awake()
        {
            CacheReferences();

            if (overrideWeatherRNG)
            {
                ApplyWeather(debugWeather);
                return;
            }

            ApplyWeather(GenerateRandomWeather());
        }

        private void OnValidate()
        {
            CacheReferences();

            if (overrideWeatherRNG)
            {
                ApplyWeather(debugWeather);
                return;
            }

            ApplyWeather(GenerateRandomWeather());
        }

        /// <summary>
        /// Finds and caches any required image or particle references if they were not assigned in the Inspector.
        /// </summary>
        private void CacheReferences()
        {
            if (rainParticles == null)
            {
                rainParticles = GetComponentInChildren<ParticleSystem>();
            }

            if (heavyRainParticles == null)
            {
                heavyRainParticles = GetComponentInChildren<ParticleSystem>();
            }

            if (snowfallParticles == null)
            {
                snowfallParticles = GetComponentInChildren<ParticleSystem>();
            }

            if (blizzardParticles == null)
            {
                blizzardParticles = GetComponentInChildren<ParticleSystem>();
            }

            if (hailParticles == null)
            {
                hailParticles = GetComponentInChildren<ParticleSystem>();
            }
        }

        /// <summary>
        /// Generates a weather state using the random number generator while respecting the current season.
        /// </summary>
        /// <returns>A valid weather state for the current season.</returns>
        public WeatherState GenerateRandomWeather()
        {
            WeatherState[] possibleStates =
            {
                WeatherState.Clear,
                WeatherState.LightRain,
                WeatherState.HeavyDownpour,
                WeatherState.Thunderstorm,
                WeatherState.Snowfall,
                WeatherState.Blizzard,
                WeatherState.Hail
            };

            if (seasonController != null)
            {
                SeasonalLandscapeController.Season season = seasonController.GetCurrentSeason();

                if (season == SeasonalLandscapeController.Season.Summer)
                {
                    possibleStates = new[]
                    {
                        WeatherState.Clear,
                        WeatherState.LightRain,
                        WeatherState.HeavyDownpour,
                        WeatherState.Thunderstorm
                    };
                }
                else if (season == SeasonalLandscapeController.Season.Spring || season == SeasonalLandscapeController.Season.Autumn)
                {
                    possibleStates = new[]
                    {
                        WeatherState.Clear,
                        WeatherState.LightRain,
                        WeatherState.HeavyDownpour,
                        WeatherState.Thunderstorm,
                        WeatherState.Snowfall,
                        WeatherState.Hail
                    };
                }
                else if (season == SeasonalLandscapeController.Season.Winter)
                {
                    possibleStates = new[]
                    {
                        WeatherState.Clear,
                        WeatherState.LightRain,
                        WeatherState.HeavyDownpour,
                        WeatherState.Thunderstorm,
                        WeatherState.Snowfall,
                        WeatherState.Blizzard,
                        WeatherState.Hail
                    };
                }
            }

            return possibleStates[UnityEngine.Random.Range(0, possibleStates.Length)];
        }

        /// <summary>
        /// Applies the requested weather state by toggling active particle effects and overlays.
        /// </summary>
        /// <param name="state">The weather state to apply.</param>
        public void ApplyWeather(WeatherState state)
        {
            StopAllParticleSystems();
            StopLightningRoutine();

            SetDropletsOverlayVisible(false);

            if (lightningFlashOverlay != null)
            {
                lightningFlashOverlay.enabled = false;
                var flashColor = lightningFlashOverlay.color;
                flashColor.a = 0f;
                lightningFlashOverlay.color = flashColor;
            }

            switch (state)
            {
                case WeatherState.Clear:
                    break;

                case WeatherState.LightRain:
                    PlayParticleSystem(rainParticles);
                    SetDropletsOverlayVisible(true, 0.25f);
                    break;

                case WeatherState.HeavyDownpour:
                    PlayParticleSystem(heavyRainParticles);
                    SetDropletsOverlayVisible(true, 0.65f);
                    break;

                case WeatherState.Thunderstorm:
                    PlayParticleSystem(heavyRainParticles);
                    SetDropletsOverlayVisible(true, 0.75f);
                    StartLightningRoutine();
                    break;

                case WeatherState.Snowfall:
                    PlayParticleSystem(snowfallParticles);
                    break;

                case WeatherState.Blizzard:
                    PlayParticleSystem(blizzardParticles);
                    SetDropletsOverlayVisible(true, 0.2f);
                    break;

                case WeatherState.Hail:
                    PlayParticleSystem(hailParticles);
                    SetDropletsOverlayVisible(true, 0.35f);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Stops all weather particle systems to avoid overlapping effects.
        /// </summary>
        private void StopAllParticleSystems()
        {
            StopParticleSystem(rainParticles);
            StopParticleSystem(heavyRainParticles);
            StopParticleSystem(snowfallParticles);
            StopParticleSystem(blizzardParticles);
            StopParticleSystem(hailParticles);
        }

        /// <summary>
        /// Enables and plays the provided particle system if it has been assigned.
        /// </summary>
        /// <param name="particles">The particle system to play.</param>
        private void PlayParticleSystem(ParticleSystem particles)
        {
            if (particles == null)
            {
                return;
            }

            particles.gameObject.SetActive(true);
            particles.Play();
        }

        /// <summary>
        /// Stops and disables the provided particle system if it has been assigned.
        /// </summary>
        /// <param name="particles">The particle system to stop.</param>
        private void StopParticleSystem(ParticleSystem particles)
        {
            if (particles == null)
            {
                return;
            }

            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            particles.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows or hides the window droplet overlay and optionally fades it to a target alpha value.
        /// </summary>
        /// <param name="isVisible">Whether the overlay should be visible.</param>
        /// <param name="alpha">The target alpha value when the overlay is visible.</param>
        private void SetDropletsOverlayVisible(bool isVisible, float alpha = 0f)
        {
            if (windowDropletsOverlay == null)
            {
                return;
            }

            Color overlayColor = windowDropletsOverlay.color;
            overlayColor.a = isVisible ? alpha : 0f;
            windowDropletsOverlay.color = overlayColor;
            windowDropletsOverlay.enabled = isVisible;
        }

        /// <summary>
        /// Starts the lightning flash coroutine for thunderstorm conditions.
        /// </summary>
        private void StartLightningRoutine()
        {
            if (lightningFlashOverlay == null)
            {
                return;
            }

            if (lightningRoutine != null)
            {
                StopCoroutine(lightningRoutine);
            }

            lightningRoutine = StartCoroutine(LightningFlashRoutine());
        }

        /// <summary>
        /// Stops the active lightning coroutine if it is currently running.
        /// </summary>
        private void StopLightningRoutine()
        {
            if (lightningRoutine != null)
            {
                StopCoroutine(lightningRoutine);
                lightningRoutine = null;
            }
        }

        /// <summary>
        /// Triggers random lightning flash pulses at irregular intervals between 3 and 8 seconds.
        /// </summary>
        /// <returns>A coroutine that continually schedules flashes.</returns>
        private IEnumerator LightningFlashRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 8f));

                if (lightningFlashOverlay == null)
                {
                    continue;
                }

                float flashAlpha = UnityEngine.Random.Range(0.15f, 0.8f);
                Color flashColor = lightningFlashOverlay.color;
                flashColor.a = flashAlpha;
                lightningFlashOverlay.color = flashColor;
                lightningFlashOverlay.enabled = true;

                float duration = UnityEngine.Random.Range(0.08f, 0.2f);
                yield return new WaitForSeconds(duration);

                flashColor.a = 0f;
                lightningFlashOverlay.color = flashColor;
                lightningFlashOverlay.enabled = false;
            }
        }
    }
}

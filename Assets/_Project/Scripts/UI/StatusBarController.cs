using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CozyHome.Environment;
using CozyHome.Weather;

namespace CozyHome.UI
{
    /// <summary>
    /// Updates the top-left status icons to reflect the active time of day, season, and weather state.
    /// </summary>
    [ExecuteAlways]
    public class StatusBarController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum TimeOfDay
        {
            Morning,
            Day,
            Evening,
            Night
        }

        public enum SeasonType
        {
            Spring,
            Summer,
            Autumn,
            Winter
        }

        [Header("Target Icons")]
        [SerializeField] private Image iconTimeOfDay;
        [SerializeField] private Image iconSeason;
        [SerializeField] private Image iconWeather;

        [Header("Time-of-Day Sprites")]
        [SerializeField] private Sprite morningSprite;
        [SerializeField] private Sprite daySprite;
        [SerializeField] private Sprite eveningSprite;
        [SerializeField] private Sprite nightSprite;

        [Header("Season Sprites")]
        [SerializeField] private Sprite springSprite;
        [SerializeField] private Sprite summerSprite;
        [SerializeField] private Sprite autumnSprite;
        [SerializeField] private Sprite winterSprite;

        [Header("Weather Sprites")]
        [SerializeField] private Sprite clearSprite;
        [SerializeField] private Sprite lightRainSprite;
        [SerializeField] private Sprite heavyRainSprite;
        [SerializeField] private Sprite thunderstormSprite;
        [SerializeField] private Sprite lightSnowSprite;
        [SerializeField] private Sprite snowstormSprite;
        [SerializeField] private Sprite hailSprite;
        [SerializeField] private Sprite fogSprite;
        [SerializeField] private Sprite windSprite;
        [SerializeField] private Sprite strongWindSprite;

        [Header("Legacy Inspector Compatibility")]
        [SerializeField] private Sprite[] legacyTimeOfDaySprites = new Sprite[4];
        [SerializeField] private Sprite[] legacySeasonSprites = new Sprite[4];
        [SerializeField] private Sprite[] legacyWeatherSprites = new Sprite[10];

        [Header("Tooltip")]
        [SerializeField] private CanvasGroup tooltipCanvasGroup;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float displayDelay = 0.05f;

        [Header("Debug / Testing")]
        [SerializeField] private bool useDebugOverride;
        [SerializeField] private TimeOfDay debugTimeOfDay = TimeOfDay.Day;
        [SerializeField] private SeasonType debugSeason = SeasonType.Spring;
        [SerializeField] private WeatherType debugWeather = WeatherType.Clear;

        private TimeOfDay activeTimeOfDay;
        private SeasonType activeSeason;
        private WeatherType activeWeather;
        private Coroutine tooltipDelayRoutine;
        private Coroutine tooltipFadeRoutine;

        private void Awake()
        {
            CacheIcons();
            ResolveTooltipReferences();
            InitializeTooltip();
            RefreshAll();
            RefreshLanguageFromManager();
            UpdateTooltipText();
        }

        private void OnEnable()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            }

            RefreshLanguageFromManager();
        }

        private void OnDisable()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
            }
        }

        private void OnValidate()
        {
            CacheIcons();
            ResolveTooltipReferences();

            if (tooltipCanvasGroup != null)
            {
                InitializeTooltip();
            }

            if (useDebugOverride)
            {
                SetTimeOfDay(debugTimeOfDay);
                SetSeason(debugSeason);
                SetWeather(debugWeather);
                UpdateTooltipText();
                return;
            }

            if (!Application.isPlaying)
            {
                activeWeather = debugWeather;
                ApplyWeatherSprite();
                UpdateTooltipText();
                return;
            }

            RefreshAll();
            UpdateTooltipText();
        }

        private void CacheIcons()
        {
            if (iconTimeOfDay == null)
            {
                iconTimeOfDay = GetComponentInChildren<Image>();
            }

            if (iconSeason == null)
            {
                iconSeason = GetComponentInChildren<Image>();
            }

            if (iconWeather == null)
            {
                iconWeather = GetComponentInChildren<Image>();
            }
        }

        private void ResolveTooltipReferences()
        {
            if (tooltipCanvasGroup == null)
            {
                tooltipCanvasGroup = GetComponentInChildren<CanvasGroup>(true);
            }

            if (tooltipText == null)
            {
                tooltipText = GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }

        private void InitializeTooltip()
        {
            if (tooltipCanvasGroup == null)
            {
                return;
            }

            tooltipCanvasGroup.alpha = 0f;
            tooltipCanvasGroup.interactable = false;
            tooltipCanvasGroup.blocksRaycasts = false;

            if (tooltipText != null)
            {
                tooltipText.text = string.Empty;
                tooltipText.raycastTarget = false;
                ApplyTextDirection(GetCurrentLanguage());
            }
        }

        private IEnumerator FadeTooltip(float targetAlpha)
        {
            if (tooltipCanvasGroup == null)
            {
                yield break;
            }

            float startingAlpha = tooltipCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                tooltipCanvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, t);
                yield return null;
            }

            tooltipCanvasGroup.alpha = targetAlpha;
            tooltipCanvasGroup.interactable = false;
            tooltipCanvasGroup.blocksRaycasts = false;
        }

        private IEnumerator ShowTooltipAfterDelay()
        {
            yield return new WaitForSeconds(displayDelay);
            UpdateTooltipText();
            StartTooltipFade(1f);
        }

        private void StartTooltipFade(float targetAlpha)
        {
            if (tooltipFadeRoutine != null)
            {
                StopCoroutine(tooltipFadeRoutine);
            }

            tooltipFadeRoutine = StartCoroutine(FadeTooltip(targetAlpha));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipCanvasGroup == null)
            {
                return;
            }

            if (tooltipDelayRoutine != null)
            {
                StopCoroutine(tooltipDelayRoutine);
            }

            tooltipDelayRoutine = StartCoroutine(ShowTooltipAfterDelay());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipDelayRoutine != null)
            {
                StopCoroutine(tooltipDelayRoutine);
                tooltipDelayRoutine = null;
            }

            StartTooltipFade(0f);
        }

        private void HandleLanguageChanged(AppLanguage newLanguage)
        {
            ApplyTextDirection(newLanguage);

            if (tooltipCanvasGroup != null && tooltipCanvasGroup.alpha > 0.01f)
            {
                UpdateTooltipText();
            }
        }

        private AppLanguage GetCurrentLanguage()
        {
            return LanguageManager.Instance != null ? LanguageManager.Instance.CurrentLanguage : AppLanguage.RU;
        }

        private void RefreshLanguageFromManager()
        {
            ApplyTextDirection(GetCurrentLanguage());
        }

        private void SyncLegacyInspectorSprites()
        {
            if (legacyTimeOfDaySprites != null && legacyTimeOfDaySprites.Length >= 4)
            {
                morningSprite ??= legacyTimeOfDaySprites[0];
                daySprite ??= legacyTimeOfDaySprites[1];
                eveningSprite ??= legacyTimeOfDaySprites[2];
                nightSprite ??= legacyTimeOfDaySprites[3];
            }

            if (legacySeasonSprites != null && legacySeasonSprites.Length >= 4)
            {
                springSprite ??= legacySeasonSprites[0];
                summerSprite ??= legacySeasonSprites[1];
                autumnSprite ??= legacySeasonSprites[2];
                winterSprite ??= legacySeasonSprites[3];
            }

            if (legacyWeatherSprites != null && legacyWeatherSprites.Length >= 10)
            {
                clearSprite ??= legacyWeatherSprites[(int)WeatherType.Clear];
                lightRainSprite ??= legacyWeatherSprites[(int)WeatherType.LightRain];
                heavyRainSprite ??= legacyWeatherSprites[(int)WeatherType.HeavyRain];
                thunderstormSprite ??= legacyWeatherSprites[(int)WeatherType.Thunderstorm];
                lightSnowSprite ??= legacyWeatherSprites[(int)WeatherType.LightSnow];
                snowstormSprite ??= legacyWeatherSprites[(int)WeatherType.Snowstorm];
                hailSprite ??= legacyWeatherSprites[(int)WeatherType.Hail];
                fogSprite ??= legacyWeatherSprites[(int)WeatherType.Fog];
                windSprite ??= legacyWeatherSprites[(int)WeatherType.Wind];
                strongWindSprite ??= legacyWeatherSprites[(int)WeatherType.StrongWind];
            }
        }

        private Sprite GetTimeOfDaySprite(TimeOfDay phase)
        {
            return phase switch
            {
                TimeOfDay.Morning => morningSprite,
                TimeOfDay.Day => daySprite,
                TimeOfDay.Evening => eveningSprite,
                TimeOfDay.Night => nightSprite,
                _ => daySprite
            };
        }

        private Sprite GetSeasonSprite(SeasonType season)
        {
            return season switch
            {
                SeasonType.Spring => springSprite,
                SeasonType.Summer => summerSprite,
                SeasonType.Autumn => autumnSprite,
                SeasonType.Winter => winterSprite,
                _ => springSprite
            };
        }

        private Sprite GetWeatherSprite(WeatherType weather)
        {
            return weather switch
            {
                WeatherType.Clear => clearSprite,
                WeatherType.LightRain => lightRainSprite,
                WeatherType.HeavyRain => heavyRainSprite,
                WeatherType.Thunderstorm => thunderstormSprite,
                WeatherType.LightSnow => lightSnowSprite,
                WeatherType.Snowstorm => snowstormSprite,
                WeatherType.Hail => hailSprite,
                WeatherType.Fog => fogSprite,
                WeatherType.Wind => windSprite,
                WeatherType.StrongWind => strongWindSprite,
                _ => clearSprite
            };
        }

        private void ApplyTextDirection(AppLanguage language)
        {
            if (tooltipText == null)
            {
                return;
            }

            tooltipText.alignment = TextAlignmentOptions.Center;
            tooltipText.isRightToLeftText = language == AppLanguage.HE;
        }

        private void UpdateTooltipText()
        {
            if (tooltipText == null)
            {
                return;
            }

            AppLanguage language = GetCurrentLanguage();
            ApplyTextDirection(language);

            string timeText = GetLocalizedTimeOfDay();
            string seasonText = GetLocalizedSeason();
            string weatherText = GetLocalizedWeather();

            string separator = language == AppLanguage.HE ? " • " : " · ";
            string tooltipTextValue = string.Join(separator, new[] { timeText, seasonText, weatherText });
            tooltipText.text = tooltipTextValue;
        }

        private string GetLocalizedTimeOfDay()
        {
            AppLanguage language = GetCurrentLanguage();

            if (activeTimeOfDay == TimeOfDay.Morning)
            {
                return language switch
                {
                    AppLanguage.RU => "Утро",
                    AppLanguage.EN => "Morning",
                    AppLanguage.HE => "בוקר",
                    _ => "Утро"
                };
            }

            if (activeTimeOfDay == TimeOfDay.Day)
            {
                return language switch
                {
                    AppLanguage.RU => "День",
                    AppLanguage.EN => "Day",
                    AppLanguage.HE => "יום",
                    _ => "День"
                };
            }

            if (activeTimeOfDay == TimeOfDay.Evening)
            {
                return language switch
                {
                    AppLanguage.RU => "Вечер",
                    AppLanguage.EN => "Evening",
                    AppLanguage.HE => "ערב",
                    _ => "Вечер"
                };
            }

            return language switch
            {
                AppLanguage.RU => "Ночь",
                AppLanguage.EN => "Night",
                AppLanguage.HE => "לילה",
                _ => "Ночь"
            };
        }

        private string GetLocalizedSeason()
        {
            AppLanguage language = GetCurrentLanguage();

            if (activeSeason == SeasonType.Spring)
            {
                return language switch
                {
                    AppLanguage.RU => "Весна",
                    AppLanguage.EN => "Spring",
                    AppLanguage.HE => "אביב",
                    _ => "Весна"
                };
            }

            if (activeSeason == SeasonType.Summer)
            {
                return language switch
                {
                    AppLanguage.RU => "Лето",
                    AppLanguage.EN => "Summer",
                    AppLanguage.HE => "קיץ",
                    _ => "Лето"
                };
            }

            if (activeSeason == SeasonType.Autumn)
            {
                return language switch
                {
                    AppLanguage.RU => "Осень",
                    AppLanguage.EN => "Autumn",
                    AppLanguage.HE => "סתיו",
                    _ => "Осень"
                };
            }

            return language switch
            {
                AppLanguage.RU => "Зима",
                AppLanguage.EN => "Winter",
                AppLanguage.HE => "חורף",
                _ => "Зима"
            };
        }

        private string GetLocalizedWeather()
        {
            AppLanguage language = GetCurrentLanguage();

            if (activeWeather == WeatherType.Clear)
            {
                return language switch
                {
                    AppLanguage.RU => "Ясно",
                    AppLanguage.EN => "Clear",
                    AppLanguage.HE => "בהיר",
                    _ => "Ясно"
                };
            }

            if (activeWeather == WeatherType.Fog)
            {
                return language switch
                {
                    AppLanguage.RU => "Туман",
                    AppLanguage.EN => "Fog",
                    AppLanguage.HE => "ערפל",
                    _ => "Туман"
                };
            }

            if (activeWeather == WeatherType.Wind)
            {
                return language switch
                {
                    AppLanguage.RU => "Ветер",
                    AppLanguage.EN => "Wind",
                    AppLanguage.HE => "רוח",
                    _ => "Ветер"
                };
            }

            if (activeWeather == WeatherType.StrongWind)
            {
                return language switch
                {
                    AppLanguage.RU => "Сильный ветер",
                    AppLanguage.EN => "Strong Wind",
                    AppLanguage.HE => "רוח חזקה",
                    _ => "Сильный ветер"
                };
            }

            if (activeWeather == WeatherType.LightRain)
            {
                return language switch
                {
                    AppLanguage.RU => "Лёгкий дождь",
                    AppLanguage.EN => "Light Rain",
                    AppLanguage.HE => "גשם קל",
                    _ => "Лёгкий дождь"
                };
            }

            if (activeWeather == WeatherType.HeavyRain)
            {
                return language switch
                {
                    AppLanguage.RU => "Сильный дождь",
                    AppLanguage.EN => "Heavy Rain",
                    AppLanguage.HE => "גשם חזק",
                    _ => "Сильный дождь"
                };
            }

            if (activeWeather == WeatherType.LightSnow)
            {
                return language switch
                {
                    AppLanguage.RU => "Небольшой снег",
                    AppLanguage.EN => "Light Snow",
                    AppLanguage.HE => "שלג קל",
                    _ => "Небольшой снег"
                };
            }

            if (activeWeather == WeatherType.Snowstorm)
            {
                return language switch
                {
                    AppLanguage.RU => "Метель",
                    AppLanguage.EN => "Snowstorm",
                    AppLanguage.HE => "סופת שלגים",
                    _ => "Метель"
                };
            }

            if (activeWeather == WeatherType.Hail)
            {
                return language switch
                {
                    AppLanguage.RU => "Град",
                    AppLanguage.EN => "Hail",
                    AppLanguage.HE => "ברד",
                    _ => "Град"
                };
            }

            return language switch
            {
                AppLanguage.RU => "Гроза",
                AppLanguage.EN => "Thunderstorm",
                AppLanguage.HE => "סופת רעמים",
                _ => "Гроза"
            };
        }

        /// <summary>
        /// Returns the current phase from the main environment controller when it is available.
        /// </summary>
        public TimeOfDay GetCurrentTimeOfDay()
        {
            TimeOfDayController environmentController = FindAnyObjectByType<TimeOfDayController>();
            if (environmentController != null)
            {
                return ConvertControllerTimeOfDay(environmentController.GetCurrentTimeOfDay());
            }

            int currentHour = DateTime.Now.Hour;
            if (currentHour >= 6 && currentHour <= 10)
            {
                return TimeOfDay.Morning;
            }

            if (currentHour >= 11 && currentHour <= 16)
            {
                return TimeOfDay.Day;
            }

            if (currentHour >= 17 && currentHour <= 21)
            {
                return TimeOfDay.Evening;
            }

            return TimeOfDay.Night;
        }

        /// <summary>
        /// Returns the current season from the landscape controller when it is available.
        /// </summary>
        public SeasonType GetCurrentSeason()
        {
            SeasonalLandscapeController environmentController = FindAnyObjectByType<SeasonalLandscapeController>();
            if (environmentController != null)
            {
                return ConvertControllerSeason(environmentController.GetCurrentSeason());
            }

            int currentMonth = DateTime.Now.Month;
            if (currentMonth >= 3 && currentMonth <= 5)
            {
                return SeasonType.Spring;
            }

            if (currentMonth >= 6 && currentMonth <= 8)
            {
                return SeasonType.Summer;
            }

            if (currentMonth >= 9 && currentMonth <= 11)
            {
                return SeasonType.Autumn;
            }

            return SeasonType.Winter;
        }

        /// <summary>
        /// Returns a generated weather state when no dedicated weather manager is available yet.
        /// </summary>
        private WeatherType GetSeasonallyValidWeatherForHour(int currentMonth, int currentHour)
        {
            WeatherType[] allowedWeather = WeatherSeasonality.GetAllowedWeatherTypesForMonth(currentMonth);
            if (allowedWeather.Length == 0)
            {
                return WeatherType.Clear;
            }

            if (currentHour >= 22 || currentHour <= 4)
            {
                return WeatherType.Clear;
            }

            bool isWinterSnowMonth = currentMonth == 12 || currentMonth == 1 || currentMonth == 2 || currentMonth == 3 || currentMonth == 4 || currentMonth == 11;
            if (isWinterSnowMonth)
            {
                if (currentHour >= 9 && currentHour <= 16)
                {
                    return Array.Exists(allowedWeather, type => type == WeatherType.LightSnow) ? WeatherType.LightSnow : WeatherType.Clear;
                }

                return Array.Exists(allowedWeather, type => type == WeatherType.Snowstorm) ? WeatherType.Snowstorm : WeatherType.Clear;
            }

            if (currentHour >= 9 && currentHour <= 16)
            {
                if (Array.Exists(allowedWeather, type => type == WeatherType.LightRain))
                {
                    return WeatherType.LightRain;
                }

                if (Array.Exists(allowedWeather, type => type == WeatherType.Wind))
                {
                    return WeatherType.Wind;
                }

                if (Array.Exists(allowedWeather, type => type == WeatherType.Fog))
                {
                    return WeatherType.Fog;
                }

                if (Array.Exists(allowedWeather, type => type == WeatherType.Hail))
                {
                    return WeatherType.Hail;
                }
            }

            if (Array.Exists(allowedWeather, type => type == WeatherType.Fog))
            {
                return WeatherType.Fog;
            }

            if (Array.Exists(allowedWeather, type => type == WeatherType.Clear))
            {
                return WeatherType.Clear;
            }

            return allowedWeather[0];
        }

        public WeatherType GetCurrentWeather()
        {
            WeatherController weatherController = FindAnyObjectByType<WeatherController>();
            if (weatherController != null)
            {
                return weatherController.CurrentWeather != WeatherType.None
                    ? weatherController.CurrentWeather
                    : WeatherSeasonality.GetRandomWeatherForCurrentMonth();
            }

            return WeatherSeasonality.GetRandomWeatherForCurrentMonth();
        }

        public WeatherType GetRandomWeather()
        {
            return WeatherSeasonality.GetRandomWeatherForCurrentMonth();
        }

        public void SetRandomWeather()
        {
            SetWeather(GetRandomWeather());
        }

        public void SetTimeOfDay(TimeOfDay phase)
        {
            activeTimeOfDay = phase;
            ApplyTimeOfDaySprite();
            UpdateTooltipText();
        }

        public void SetSeason(SeasonType season)
        {
            activeSeason = season;
            ApplySeasonSprite();
            UpdateTooltipText();
        }

        public void SetWeather(WeatherType weather)
        {
            activeWeather = weather;
            ApplyWeatherSprite();
            UpdateTooltipText();
        }

        public void RefreshAll()
        {
            if (useDebugOverride)
            {
                SetTimeOfDay(debugTimeOfDay);
                SetSeason(debugSeason);
                SetWeather(debugWeather);
                return;
            }

            SetTimeOfDay(GetCurrentTimeOfDay());
            SetSeason(GetCurrentSeason());
            SetWeather(GetCurrentWeather());
        }

        private void ApplyTimeOfDaySprite()
        {
            if (iconTimeOfDay == null)
            {
                return;
            }

            SyncLegacyInspectorSprites();
            iconTimeOfDay.sprite = GetTimeOfDaySprite(activeTimeOfDay);
        }

        private void ApplySeasonSprite()
        {
            if (iconSeason == null)
            {
                return;
            }

            SyncLegacyInspectorSprites();
            iconSeason.sprite = GetSeasonSprite(activeSeason);
        }

        private void ApplyWeatherSprite()
        {
            if (iconWeather == null)
            {
                return;
            }

            SyncLegacyInspectorSprites();
            iconWeather.sprite = GetWeatherSprite(activeWeather);
        }

        private static TimeOfDay ConvertControllerTimeOfDay(CozyHome.Environment.TimeOfDayController.TimeOfDay controllerPhase)
        {
            return controllerPhase switch
            {
                CozyHome.Environment.TimeOfDayController.TimeOfDay.Morning => TimeOfDay.Morning,
                CozyHome.Environment.TimeOfDayController.TimeOfDay.Day => TimeOfDay.Day,
                CozyHome.Environment.TimeOfDayController.TimeOfDay.Evening => TimeOfDay.Evening,
                CozyHome.Environment.TimeOfDayController.TimeOfDay.Night => TimeOfDay.Night,
                _ => TimeOfDay.Day
            };
        }

        private static SeasonType ConvertControllerSeason(CozyHome.Environment.SeasonalLandscapeController.Season controllerSeason)
        {
            return controllerSeason switch
            {
                CozyHome.Environment.SeasonalLandscapeController.Season.Spring => SeasonType.Spring,
                CozyHome.Environment.SeasonalLandscapeController.Season.Summer => SeasonType.Summer,
                CozyHome.Environment.SeasonalLandscapeController.Season.Autumn => SeasonType.Autumn,
                CozyHome.Environment.SeasonalLandscapeController.Season.Winter => SeasonType.Winter,
                _ => SeasonType.Spring
            };
        }
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CozyHome.Environment;

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

        public enum WeatherType
        {
            Clear = 0,
            Fog = 1,
            Wind = 2,
            LightRain = 3,
            HeavyRain = 4,
            LightSnow = 5,
            Snowstorm = 6,
            Hail = 7,
            Thunderstorm = 8
        }

        public enum AppLanguage
        {
            RU,
            EN,
            HE
        }

        [Header("Target Icons")]
        [SerializeField] private Image iconTimeOfDay;
        [SerializeField] private Image iconSeason;
        [SerializeField] private Image iconWeather;

        [Header("Time-of-Day Sprites")]
        [SerializeField] private Sprite[] timeOfDaySprites = new Sprite[4];

        [Header("Season Sprites")]
        [SerializeField] private Sprite[] seasonSprites = new Sprite[4];

        [Header("Weather Sprites")]
        [SerializeField] private Sprite[] weatherSprites = new Sprite[9];

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
        [SerializeField] private AppLanguage currentLanguage = AppLanguage.RU;

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
            UpdateTooltipText();
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
                ApplyTextDirection();
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
            tooltipCanvasGroup.interactable = targetAlpha > 0.01f;
            tooltipCanvasGroup.blocksRaycasts = targetAlpha > 0.01f;
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

        public void SetLanguage(AppLanguage language)
        {
            currentLanguage = language;
            ApplyTextDirection();
            UpdateTooltipText();
        }

        private void ApplyTextDirection()
        {
            if (tooltipText == null)
            {
                return;
            }

            if (currentLanguage == AppLanguage.HE)
            {
                tooltipText.isRightToLeftText = true;
                tooltipText.alignment = TextAlignmentOptions.Right;
            }
            else
            {
                tooltipText.isRightToLeftText = false;
                tooltipText.alignment = TextAlignmentOptions.Left;
            }
        }

        private void UpdateTooltipText()
        {
            if (tooltipText == null)
            {
                return;
            }

            ApplyTextDirection();
            string tooltipTextValue = $"{GetLocalizedTimeOfDay()} · {GetLocalizedSeason()} · {GetLocalizedWeather()}";
            tooltipText.text = tooltipTextValue;
        }

        private string GetLocalizedTimeOfDay()
        {
            if (activeTimeOfDay == TimeOfDay.Morning)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Утро",
                    AppLanguage.EN => "Morning",
                    AppLanguage.HE => "בוקר",
                    _ => "Утро"
                };
            }

            if (activeTimeOfDay == TimeOfDay.Day)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "День",
                    AppLanguage.EN => "Day",
                    AppLanguage.HE => "יום",
                    _ => "День"
                };
            }

            if (activeTimeOfDay == TimeOfDay.Evening)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Вечер",
                    AppLanguage.EN => "Evening",
                    AppLanguage.HE => "ערב",
                    _ => "Вечер"
                };
            }

            return currentLanguage switch
            {
                AppLanguage.RU => "Ночь",
                AppLanguage.EN => "Night",
                AppLanguage.HE => "לילה",
                _ => "Ночь"
            };
        }

        private string GetLocalizedSeason()
        {
            if (activeSeason == SeasonType.Spring)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Весна",
                    AppLanguage.EN => "Spring",
                    AppLanguage.HE => "אביב",
                    _ => "Весна"
                };
            }

            if (activeSeason == SeasonType.Summer)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Лето",
                    AppLanguage.EN => "Summer",
                    AppLanguage.HE => "קיץ",
                    _ => "Лето"
                };
            }

            if (activeSeason == SeasonType.Autumn)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Осень",
                    AppLanguage.EN => "Autumn",
                    AppLanguage.HE => "סתיו",
                    _ => "Осень"
                };
            }

            return currentLanguage switch
            {
                AppLanguage.RU => "Зима",
                AppLanguage.EN => "Winter",
                AppLanguage.HE => "חורף",
                _ => "Зима"
            };
        }

        private string GetLocalizedWeather()
        {
            if (activeWeather == WeatherType.Clear)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Ясно",
                    AppLanguage.EN => "Clear",
                    AppLanguage.HE => "בהיר",
                    _ => "Ясно"
                };
            }

            if (activeWeather == WeatherType.Fog)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Туман",
                    AppLanguage.EN => "Fog",
                    AppLanguage.HE => "ערפל",
                    _ => "Туман"
                };
            }

            if (activeWeather == WeatherType.Wind)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Ветер",
                    AppLanguage.EN => "Wind",
                    AppLanguage.HE => "רוח",
                    _ => "Ветер"
                };
            }

            if (activeWeather == WeatherType.LightRain)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Лёгкий дождь",
                    AppLanguage.EN => "Light Rain",
                    AppLanguage.HE => "גשם קל",
                    _ => "Лёгкий дождь"
                };
            }

            if (activeWeather == WeatherType.HeavyRain)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Сильный дождь",
                    AppLanguage.EN => "Heavy Rain",
                    AppLanguage.HE => "גשם חזק",
                    _ => "Сильный дождь"
                };
            }

            if (activeWeather == WeatherType.LightSnow)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Небольшой снег",
                    AppLanguage.EN => "Light Snow",
                    AppLanguage.HE => "שלג קל",
                    _ => "Небольшой снег"
                };
            }

            if (activeWeather == WeatherType.Snowstorm)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Метель",
                    AppLanguage.EN => "Snowstorm",
                    AppLanguage.HE => "סופת שלגים",
                    _ => "Метель"
                };
            }

            if (activeWeather == WeatherType.Hail)
            {
                return currentLanguage switch
                {
                    AppLanguage.RU => "Град",
                    AppLanguage.EN => "Hail",
                    AppLanguage.HE => "ברד",
                    _ => "Град"
                };
            }

            return currentLanguage switch
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
        public WeatherType GetCurrentWeather()
        {
            int currentHour = DateTime.Now.Hour;
            int currentMonth = DateTime.Now.Month;

            if (currentHour >= 22 || currentHour <= 4)
            {
                return WeatherType.Clear;
            }

            if (currentMonth == 12 || currentMonth == 1 || currentMonth == 2)
            {
                return currentHour >= 9 && currentHour <= 16 ? WeatherType.LightSnow : WeatherType.Snowstorm;
            }

            if (currentMonth == 6 || currentMonth == 7 || currentMonth == 8)
            {
                return currentHour >= 9 && currentHour <= 16 ? WeatherType.LightRain : WeatherType.Clear;
            }

            if (currentMonth == 9 || currentMonth == 10 || currentMonth == 11)
            {
                return WeatherType.Fog;
            }

            if (currentHour >= 9 && currentHour <= 16)
            {
                return WeatherType.Wind;
            }

            return WeatherType.Clear;
        }

        public WeatherType GetRandomWeather()
        {
            return (WeatherType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(WeatherType)).Length);
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

            if (timeOfDaySprites == null || timeOfDaySprites.Length <= (int)activeTimeOfDay)
            {
                iconTimeOfDay.sprite = null;
                return;
            }

            iconTimeOfDay.sprite = timeOfDaySprites[(int)activeTimeOfDay];
        }

        private void ApplySeasonSprite()
        {
            if (iconSeason == null)
            {
                return;
            }

            if (seasonSprites == null || seasonSprites.Length <= (int)activeSeason)
            {
                iconSeason.sprite = null;
                return;
            }

            iconSeason.sprite = seasonSprites[(int)activeSeason];
        }

        private void ApplyWeatherSprite()
        {
            if (iconWeather == null)
            {
                return;
            }

            if (weatherSprites == null || weatherSprites.Length <= (int)activeWeather)
            {
                iconWeather.sprite = null;
                return;
            }

            iconWeather.sprite = weatherSprites[(int)activeWeather];
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

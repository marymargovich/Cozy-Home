using System;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.Environment
{
    /// <summary>
    /// Applies a time-of-day tint to both the exterior landscape and interior room overlays.
    /// </summary>
    [ExecuteAlways]
    public class TimeOfDayController : MonoBehaviour
    {
        [Header("Target UI Overlays")]
        // The landscape overlay behind the window cutout that receives the exterior tint color.
        [SerializeField] private Image landscapeTintOverlay;

        // The interior room overlay that receives the room tint color for the same time period.
        [SerializeField] private Image roomTintOverlay;

        [Header("Time Configurations")]
        // The visual tint configuration used during the morning period.
        [SerializeField] private TimeOfDayConfig morningConfig;

        // The visual tint configuration used during the daytime period.
        [SerializeField] private TimeOfDayConfig dayConfig;

        // The visual tint configuration used during the evening period.
        [SerializeField] private TimeOfDayConfig eveningConfig;

        // The visual tint configuration used during the nighttime period.
        [SerializeField] private TimeOfDayConfig nightConfig;

        [Header("Lamp Tint Reductions")]
        // The tint reduction applied when the floor lamp is active.
        [Range(0f, 1f)] [SerializeField] private float floorLampTintReduction = 0.10f;

        // The tint reduction applied when the ceiling lamp is active.
        [Range(0f, 1f)] [SerializeField] private float ceilingLampTintReduction = 0.25f;

        // The tint reduction applied when the garland is active.
        [Range(0f, 1f)] [SerializeField] private float garlandTintReduction = 0.08f;

        [Header("Debug / Testing")]
        // Enables the debug override so the designer can preview a specific time without waiting for the system clock.
        [SerializeField] private bool overrideSystemTime = false;

        // The time-of-day state to preview when the override is enabled.
        [SerializeField] private TimeOfDay debugTimeOfDay;

        /// <summary>
        /// Represents the four main periods used for the environment tinting cycle.
        /// </summary>
        public enum TimeOfDay
        {
            Morning,
            Day,
            Evening,
            Night
        }

        /// <summary>
        /// Stores the tint colors for both the landscape and room overlays for a specific time period.
        /// </summary>
        [System.Serializable]
        public struct TimeOfDayConfig
        {
            // The color applied to the landscape artwork overlay.
            public Color landscapeColor;

            // The color applied to the indoor room overlay.
            public Color roomColor;
        }

        private bool isFloorLampOn;
        private bool isCeilingLampOn;
        private bool isGarlandOn;

        private void Awake()
        {
            CacheTargetImages();
            ApplyTimeOfDay(GetCurrentTimeOfDay());
        }

        private void OnValidate()
        {
            CacheTargetImages();

            if (overrideSystemTime)
            {
                ApplyTimeOfDay(debugTimeOfDay);
                return;
            }

            ApplyTimeOfDay(GetCurrentTimeOfDay());
        }

        /// <summary>
        /// Caches the UI image references if they were not assigned in the Inspector.
        /// </summary>
        private void CacheTargetImages()
        {
            if (landscapeTintOverlay == null)
            {
                landscapeTintOverlay = GetComponentInChildren<Image>();
            }

            if (roomTintOverlay == null)
            {
                roomTintOverlay = GetComponentInChildren<Image>();
            }

            if (landscapeTintOverlay == null)
            {
                Debug.LogWarning($"{nameof(TimeOfDayController)} requires a landscape Image reference or a child Image component.", this);
            }

            if (roomTintOverlay == null)
            {
                Debug.LogWarning($"{nameof(TimeOfDayController)} requires a room Image reference or a child Image component.", this);
            }
        }

        /// <summary>
        /// Returns the current time-of-day period based on the local system clock.
        /// </summary>
        /// <returns>The calculated time-of-day state.</returns>
        public TimeOfDay GetCurrentTimeOfDay()
        {
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

        public void SetFloorLampState(bool isOn)
        {
            isFloorLampOn = isOn;
            RefreshLampTint();
        }

        public void SetCeilingLampState(bool isOn)
        {
            isCeilingLampOn = isOn;
            RefreshLampTint();
        }

        public void SetGarlandState(bool isOn)
        {
            isGarlandOn = isOn;
            RefreshLampTint();
        }

        private void RefreshLampTint()
        {
            if (roomTintOverlay == null)
            {
                return;
            }

            TimeOfDay activeTimeOfDay = overrideSystemTime ? debugTimeOfDay : GetCurrentTimeOfDay();

            TimeOfDayConfig activeConfig = activeTimeOfDay switch
            {
                TimeOfDay.Morning => morningConfig,
                TimeOfDay.Day => dayConfig,
                TimeOfDay.Evening => eveningConfig,
                TimeOfDay.Night => nightConfig,
                _ => default
            };

            Color baseRoomColor = activeConfig.roomColor;
            float totalReduction = 0f;

            if (isFloorLampOn)
            {
                totalReduction += floorLampTintReduction;
            }

            if (isCeilingLampOn)
            {
                totalReduction += ceilingLampTintReduction;
            }

            if (isGarlandOn)
            {
                totalReduction += garlandTintReduction;
            }

            totalReduction = Mathf.Clamp01(totalReduction);
            float adjustedAlpha = Mathf.Clamp01(baseRoomColor.a * (1f - totalReduction));
            roomTintOverlay.color = new Color(baseRoomColor.r, baseRoomColor.g, baseRoomColor.b, adjustedAlpha);
        }

        /// <summary>
        /// Applies the color configuration for the specified time-of-day state to both tinted overlays.
        /// </summary>
        /// <param name="timeOfDay">The time period whose colors should be applied.</param>
        public void ApplyTimeOfDay(TimeOfDay timeOfDay)
        {
            TimeOfDayConfig config = timeOfDay switch
            {
                TimeOfDay.Morning => morningConfig,
                TimeOfDay.Day => dayConfig,
                TimeOfDay.Evening => eveningConfig,
                TimeOfDay.Night => nightConfig,
                _ => default
            };

            if (landscapeTintOverlay != null)
            {
                landscapeTintOverlay.color = config.landscapeColor;
            }

            if (roomTintOverlay != null)
            {
                roomTintOverlay.color = config.roomColor;
                RefreshLampTint();
            }
        }
    }
}

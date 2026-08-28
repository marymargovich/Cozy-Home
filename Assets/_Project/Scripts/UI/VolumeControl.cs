using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CozyHome.UI
{
    /// <summary>
    /// Manages the master sound volume, mute state, icon visuals, and hover-driven visibility of the volume slider.
    /// </summary>
    public class VolumeControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const string VolumePrefsKey = "MasterVolume";
        private const float DefaultVolume = 1f;
        private static readonly Color MutedIconColor = new Color(0.5f, 0.5f, 0.5f, 0.45f);
        private static readonly Color NormalIconColor = new Color(1f, 1f, 1f, 1f);

        [SerializeField] private Button volumeButton;
        [SerializeField] private Image volumeIcon;
        [SerializeField] private CanvasGroup sliderContainerCanvasGroup;
        [SerializeField] private Slider volumeSlider;

        private bool isMuted;
        private float lastVolumeBeforeMute = DefaultVolume;
        private Coroutine hideSliderCoroutine;

        private void Awake()
        {
            BindButtonAndSliderEvents();
            LoadAndApplySavedVolume();
            ConfigureSliderVisibility(false, true);
            UpdateVolumeIconState();
        }

        private void Start()
        {
            LoadAndApplySavedVolume();
            ConfigureSliderVisibility(false, true);
            UpdateVolumeIconState();
        }

        private void OnDestroy()
        {
            if (volumeButton != null)
            {
                volumeButton.onClick.RemoveListener(OnVolumeButtonClicked);
            }

            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderValueChanged);
            }

            if (hideSliderCoroutine != null)
            {
                StopCoroutine(hideSliderCoroutine);
            }
        }

        /// <summary>
        /// Shows the slider container when the pointer enters the volume UI area.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowSliderContainer();
        }

        /// <summary>
        /// Starts a short delay before hiding the slider container after pointer exit.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            ScheduleSliderHide();
        }

        /// <summary>
        /// Toggles the master volume mute state and restores the last non-zero volume when unmuting.
        /// </summary>
        public void OnVolumeButtonClicked()
        {
            if (isMuted)
            {
                RestoreVolumeFromMute();
                return;
            }

            MuteVolume();
        }

        private void BindButtonAndSliderEvents()
        {
            if (volumeButton != null)
            {
                volumeButton.onClick.RemoveListener(OnVolumeButtonClicked);
                volumeButton.onClick.AddListener(OnVolumeButtonClicked);
            }

            if (volumeSlider != null)
            {
                volumeSlider.minValue = 0f;
                volumeSlider.maxValue = 1f;
                volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderValueChanged);
                volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
            }
        }

        private void LoadAndApplySavedVolume()
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, DefaultVolume);
            savedVolume = Mathf.Clamp01(savedVolume);

            if (volumeSlider != null)
            {
                volumeSlider.value = savedVolume;
            }

            lastVolumeBeforeMute = savedVolume;
            isMuted = Mathf.Approximately(savedVolume, 0f);
            AudioListener.volume = savedVolume;
        }

        private void OnVolumeSliderValueChanged(float newValue)
        {
            float clampedValue = Mathf.Clamp01(newValue);

            if (Mathf.Approximately(clampedValue, 0f))
            {
                MuteVolume();
                return;
            }

            if (isMuted)
            {
                isMuted = false;
            }

            lastVolumeBeforeMute = clampedValue;
            AudioListener.volume = clampedValue;
            SaveVolume(clampedValue);
            UpdateVolumeIconState();
        }

        private void MuteVolume()
        {
            if (volumeSlider != null)
            {
                lastVolumeBeforeMute = Mathf.Max(0f, volumeSlider.value);
            }
            else
            {
                lastVolumeBeforeMute = Mathf.Max(0f, AudioListener.volume);
            }

            isMuted = true;
            AudioListener.volume = 0f;
            SaveVolume(0f);
            UpdateVolumeIconState();
        }

        private void RestoreVolumeFromMute()
        {
            float restoredVolume = lastVolumeBeforeMute > 0f ? lastVolumeBeforeMute : (volumeSlider != null ? volumeSlider.value : DefaultVolume);
            if (Mathf.Approximately(restoredVolume, 0f))
            {
                restoredVolume = DefaultVolume;
            }

            isMuted = false;
            lastVolumeBeforeMute = restoredVolume;
            AudioListener.volume = restoredVolume;

            if (volumeSlider != null)
            {
                volumeSlider.value = restoredVolume;
            }

            SaveVolume(restoredVolume);
            UpdateVolumeIconState();
        }

        private void SaveVolume(float value)
        {
            float safeVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(VolumePrefsKey, safeVolume);
            PlayerPrefs.Save();
        }

        private void UpdateVolumeIconState()
        {
            if (volumeIcon == null)
            {
                return;
            }

            volumeIcon.color = isMuted ? MutedIconColor : NormalIconColor;
        }

        private void ShowSliderContainer()
        {
            if (sliderContainerCanvasGroup == null)
            {
                return;
            }

            if (hideSliderCoroutine != null)
            {
                StopCoroutine(hideSliderCoroutine);
            }

            StartCoroutine(FadeVolumeSlider(1f, 0.15f, true));
        }

        private void ScheduleSliderHide()
        {
            if (sliderContainerCanvasGroup == null)
            {
                return;
            }

            if (hideSliderCoroutine != null)
            {
                StopCoroutine(hideSliderCoroutine);
            }

            hideSliderCoroutine = StartCoroutine(HideSliderAfterDelay());
        }

        private IEnumerator HideSliderAfterDelay()
        {
            yield return new WaitForSeconds(0.35f);
            StartCoroutine(FadeVolumeSlider(0f, 0.2f, false));
        }

        private IEnumerator FadeVolumeSlider(float targetAlpha, float duration, bool isInteractive)
        {
            if (sliderContainerCanvasGroup == null)
            {
                yield break;
            }

            float startAlpha = sliderContainerCanvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);
                sliderContainerCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                yield return null;
            }

            sliderContainerCanvasGroup.alpha = targetAlpha;
            sliderContainerCanvasGroup.interactable = isInteractive;
            sliderContainerCanvasGroup.blocksRaycasts = isInteractive;

            if (!isInteractive)
            {
                hideSliderCoroutine = null;
            }
        }

        private void ConfigureSliderVisibility(bool isVisible, bool immediate)
        {
            if (sliderContainerCanvasGroup == null)
            {
                return;
            }

            if (immediate)
            {
                sliderContainerCanvasGroup.alpha = isVisible ? 1f : 0f;
                sliderContainerCanvasGroup.interactable = isVisible;
                sliderContainerCanvasGroup.blocksRaycasts = isVisible;
                return;
            }

            StartCoroutine(FadeVolumeSlider(isVisible ? 1f : 0f, 0.15f, isVisible));
        }
    }
}

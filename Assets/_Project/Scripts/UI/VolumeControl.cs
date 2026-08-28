using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.UI
{
    /// <summary>
    /// Manages the master sound volume, mute state, button image styling, and slider visibility.
    /// </summary>
    public class VolumeControl : MonoBehaviour
    {
        private const string VolumePrefsKey = "MasterVolume";
        private const float DefaultVolume = 1f;
        private static readonly Color MutedButtonColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        private static readonly Color UnmutedButtonColor = new Color(0f, 0f, 0f, 0f);

        [SerializeField] private Button volumeButton;
        [SerializeField] private CanvasGroup sliderContainerCanvasGroup;
        [SerializeField] private Slider volumeSlider;

        private bool isMuted;
        private float lastVolumeBeforeMute = DefaultVolume;
        private Coroutine sliderFadeCoroutine;

        private void Start()
        {
            BindButtonAndSliderEvents();
            LoadAndApplySavedVolume();
            ConfigureSliderVisibility(false, true);
            UpdateVolumeButtonState();
        }

        private void OnDestroy()
        {
            if (volumeButton != null)
            {
                volumeButton.onClick.RemoveListener(OnVolumeButtonClicked);
            }

            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }

            if (sliderFadeCoroutine != null)
            {
                StopCoroutine(sliderFadeCoroutine);
            }
        }

        /// <summary>
        /// Handles the volume button click flow: unmute, show the slider, or mute and hide it.
        /// </summary>
        public void OnVolumeButtonClicked()
        {
            if (isMuted)
            {
                float restoredVolume = lastVolumeBeforeMute > 0.001f ? lastVolumeBeforeMute : DefaultVolume;

                isMuted = false;
                lastVolumeBeforeMute = restoredVolume;
                AudioListener.volume = restoredVolume;

                if (volumeSlider != null)
                {
                    volumeSlider.SetValueWithoutNotify(restoredVolume);
                }

                SaveVolume(restoredVolume);
                UpdateVolumeButtonState();
                SetSliderVisible(true, false);
                return;
            }

            if (IsSliderVisible())
            {
                MuteVolume();
                SetSliderVisible(false, false);
                return;
            }

            SetSliderVisible(true, false);
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
                volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
                volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        private void LoadAndApplySavedVolume()
        {
            float savedVolume = PlayerPrefs.HasKey(VolumePrefsKey)
                ? Mathf.Clamp01(PlayerPrefs.GetFloat(VolumePrefsKey))
                : DefaultVolume;

            if (volumeSlider != null)
            {
                volumeSlider.SetValueWithoutNotify(savedVolume);
            }

            lastVolumeBeforeMute = savedVolume > 0.001f ? savedVolume : DefaultVolume;
            isMuted = savedVolume <= 0.001f;
            AudioListener.volume = isMuted ? 0f : savedVolume;
        }

        private void OnSliderValueChanged(float newValue)
        {
            float clampedValue = Mathf.Clamp01(newValue);

            if (clampedValue > 0.001f)
            {
                isMuted = false;
                lastVolumeBeforeMute = clampedValue;
                AudioListener.volume = clampedValue;
                SaveVolume(clampedValue);
                UpdateVolumeButtonState();
                return;
            }

            isMuted = true;
            lastVolumeBeforeMute = lastVolumeBeforeMute > 0.001f ? lastVolumeBeforeMute : DefaultVolume;
            AudioListener.volume = 0f;
            SaveVolume(0f);
            UpdateVolumeButtonState();
        }

        private void MuteVolume()
        {
            float currentVolume = volumeSlider != null ? volumeSlider.value : AudioListener.volume;

            lastVolumeBeforeMute = currentVolume > 0.001f ? currentVolume : DefaultVolume;
            isMuted = true;
            AudioListener.volume = 0f;

            if (volumeSlider != null)
            {
                volumeSlider.SetValueWithoutNotify(0f);
            }

            SaveVolume(0f);
            UpdateVolumeButtonState();
        }

        private void SaveVolume(float value)
        {
            float safeVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(VolumePrefsKey, safeVolume);
            PlayerPrefs.Save();
        }

        private void UpdateVolumeButtonState()
        {
            if (volumeButton == null || volumeButton.image == null)
            {
                return;
            }

            volumeButton.image.color = isMuted ? MutedButtonColor : UnmutedButtonColor;
        }

        private bool IsSliderVisible()
        {
            if (sliderContainerCanvasGroup == null)
            {
                return false;
            }

            return sliderContainerCanvasGroup.alpha > 0.01f || sliderContainerCanvasGroup.interactable;
        }

        private void SetSliderVisible(bool isVisible, bool immediate)
        {
            if (sliderContainerCanvasGroup == null)
            {
                return;
            }

            if (sliderFadeCoroutine != null)
            {
                StopCoroutine(sliderFadeCoroutine);
                sliderFadeCoroutine = null;
            }

            if (immediate)
            {
                sliderContainerCanvasGroup.alpha = isVisible ? 1f : 0f;
                sliderContainerCanvasGroup.interactable = isVisible;
                sliderContainerCanvasGroup.blocksRaycasts = isVisible;
                return;
            }

            sliderFadeCoroutine = StartCoroutine(FadeVolumeSlider(isVisible ? 1f : 0f, 0.15f, isVisible));
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
            sliderFadeCoroutine = null;
        }

        private void ConfigureSliderVisibility(bool isVisible, bool immediate)
        {
            SetSliderVisible(isVisible, immediate);
        }
    }
}

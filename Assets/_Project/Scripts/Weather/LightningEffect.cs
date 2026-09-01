using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.Weather
{
    /// <summary>
    /// Handles the visual lightning flash for a thunderstorm weather state.
    /// This component does not manage audio playback.
    /// </summary>
    public class LightningEffect : MonoBehaviour
    {
        [Header("Visual flash")]
        [SerializeField] private Image flashImage;
        [SerializeField] private CanvasGroup flashCanvasGroup;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] [Range(0.05f, 1f)] private float flashDuration = 0.18f;
        [SerializeField] [Range(0.05f, 1f)] private float flashDecayDuration = 0.35f;
        [SerializeField] [Range(0f, 1f)] private float maxFlashAlpha = 0.9f;
        [SerializeField] [Range(0.5f, 30f)] private float minFlashInterval = 5f;
        [SerializeField] [Range(0.5f, 30f)] private float maxFlashInterval = 15f;

        private Coroutine lightningLoopRoutine;
        private Coroutine flashRoutine;
        private bool isActive;

        private void Awake()
        {
            ResetVisualImmediately();
        }

        private void OnDisable()
        {
            Deactivate();
        }

        public void Activate()
        {
            if (isActive)
            {
                return;
            }

            isActive = true;
            ResetVisualImmediately();
            lightningLoopRoutine = StartCoroutine(LightningLoop());
        }

        public void Deactivate()
        {
            isActive = false;

            if (lightningLoopRoutine != null)
            {
                StopCoroutine(lightningLoopRoutine);
                lightningLoopRoutine = null;
            }

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                flashRoutine = null;
            }

            ResetVisualImmediately();
        }

        private IEnumerator LightningLoop()
        {
            while (isActive)
            {
                float waitTime = Random.Range(minFlashInterval, maxFlashInterval);
                yield return new WaitForSeconds(waitTime);

                if (!isActive)
                {
                    yield break;
                }

                TriggerFlash();
            }
        }

        private void TriggerFlash()
        {
            if (!isActive)
            {
                return;
            }

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashFadeRoutine());
        }

        private IEnumerator FlashFadeRoutine()
        {
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / Mathf.Max(flashDuration, 0.01f));
                SetFlashAlpha(Mathf.Lerp(0f, maxFlashAlpha, t));
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < flashDecayDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / Mathf.Max(flashDecayDuration, 0.01f));
                SetFlashAlpha(Mathf.Lerp(maxFlashAlpha, 0f, t));
                yield return null;
            }

            SetFlashAlpha(0f);
            flashRoutine = null;
        }

        private void SetFlashAlpha(float alpha)
        {
            float normalizedAlpha = Mathf.Clamp01(alpha);

            if (flashImage != null)
            {
                Color tint = flashColor;
                tint.a = normalizedAlpha;
                flashImage.color = tint;
            }

            if (flashCanvasGroup != null)
            {
                flashCanvasGroup.alpha = normalizedAlpha;
            }
        }

        private void ResetVisualImmediately()
        {
            if (flashImage != null)
            {
                Color tint = flashImage.color;
                tint.a = 0f;
                flashImage.color = tint;
            }

            if (flashCanvasGroup != null)
            {
                flashCanvasGroup.alpha = 0f;
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.UI
{
    /// <summary>
    /// Shows a one-time contextual hint after several valid room interactions.
    /// </summary>
    public class SecretHintController : MonoBehaviour
    {
        public static SecretHintController Instance { get; private set; }

        [Header("Hint root")]
        [SerializeField] private GameObject hintRoot;
        [SerializeField] private Image hintImage;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Localized hint sprites")]
        [SerializeField] private Sprite englishSprite;
        [SerializeField] private Sprite russianSprite;
        [SerializeField] private Sprite hebrewSprite;

        [Header("Hint timing")]
        [SerializeField] private int interactionsBeforeHint = 5;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float visibleDuration = 3.0f;
        [SerializeField] private float fadeOutDuration = 0.6f;

        private int interactionCount;
        private bool hintShown;
        private Coroutine hintRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            HideHintImmediate();
            ApplySpriteForCurrentLanguage();
            SubscribeToLanguageChanges();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            UnsubscribeFromLanguageChanges();
        }

        private void OnEnable()
        {
            ApplySpriteForCurrentLanguage();
            SubscribeToLanguageChanges();
        }

        private void OnDisable()
        {
            UnsubscribeFromLanguageChanges();
        }

        public static void TryRegisterInteraction()
        {
            if (Instance != null)
            {
                Instance.RegisterInteraction();
            }
        }

        public static void MarkHintAsSeen()
        {
            if (Instance != null)
            {
                Instance.MarkHintAsSeenInternal();
            }
        }

        public void RegisterInteraction()
        {
            if (hintShown)
            {
                return;
            }

            interactionCount++;
            if (interactionCount < interactionsBeforeHint)
            {
                return;
            }

            ShowHint();
        }

        private void ShowHint()
        {
            if (hintShown)
            {
                return;
            }

            hintShown = true;
            ApplySpriteForCurrentLanguage();
            if (hintRoot != null)
            {
                hintRoot.SetActive(true);
            }

            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (hintRoutine != null)
            {
                StopCoroutine(hintRoutine);
            }

            hintRoutine = StartCoroutine(AnimateHintRoutine());
        }

        public void MarkHintAsSeenInternal()
        {
            hintShown = true;
            if (hintRoutine != null)
            {
                StopCoroutine(hintRoutine);
                hintRoutine = null;
            }

            HideHintImmediate();
        }

        private IEnumerator AnimateHintRoutine()
        {
            if (canvasGroup == null)
            {
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / Mathf.Max(fadeInDuration, 0.0001f));
                yield return null;
            }

            canvasGroup.alpha = 1f;
            yield return new WaitForSecondsRealtime(visibleDuration);

            elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / Mathf.Max(fadeOutDuration, 0.0001f));
                yield return null;
            }

            canvasGroup.alpha = 0f;
            HideHintImmediate();
            hintRoutine = null;
        }

        private void HideHintImmediate()
        {
            if (hintRoot != null)
            {
                hintRoot.SetActive(false);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        private void ApplySpriteForCurrentLanguage()
        {
            if (hintImage == null)
            {
                return;
            }

            Sprite spriteToUse = englishSprite;
            if (LanguageManager.Instance != null)
            {
                switch (LanguageManager.Instance.CurrentLanguage)
                {
                    case AppLanguage.RU:
                        spriteToUse = russianSprite;
                        break;
                    case AppLanguage.HE:
                        spriteToUse = hebrewSprite;
                        break;
                    default:
                        spriteToUse = englishSprite;
                        break;
                }
            }

            hintImage.sprite = spriteToUse;
        }

        private void SubscribeToLanguageChanges()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            }
        }

        private void UnsubscribeFromLanguageChanges()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
            }
        }

        private void HandleLanguageChanged(AppLanguage language)
        {
            ApplySpriteForCurrentLanguage();
            if (hintRoot != null && hintRoot.activeSelf)
            {
                if (hintRoutine != null)
                {
                    StopCoroutine(hintRoutine);
                }

                hintRoutine = StartCoroutine(AnimateHintRoutine());
            }
        }
    }
}

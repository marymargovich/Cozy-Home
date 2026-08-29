using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.UI
{
    public class InfoModalController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup modalCanvasGroup;
        [SerializeField] private Image infoBoardImage;
        [SerializeField] private Sprite infoSpriteRu;
        [SerializeField] private Sprite infoSpriteEn;
        [SerializeField] private Sprite infoSpriteHe;
        [SerializeField] private Button btnCloseDefault;
        [SerializeField] private Button btnCloseHe;
        [SerializeField] private float fadeDuration = 0.2f;

        private Coroutine fadeRoutine;

        private void Awake()
        {
            if (modalCanvasGroup == null)
            {
                modalCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (btnCloseDefault != null)
            {
                btnCloseDefault.onClick.RemoveListener(CloseModal);
                btnCloseDefault.onClick.AddListener(CloseModal);
            }

            if (btnCloseHe != null)
            {
                btnCloseHe.onClick.RemoveListener(CloseModal);
                btnCloseHe.onClick.AddListener(CloseModal);
            }

            SetPassiveHiddenState();
            RefreshLocalizedSprite();
        }

        private void Start()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            }

            RefreshLocalizedSprite();
        }

        private void OnEnable()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            }

            RefreshLocalizedSprite();
        }

        private void OnDisable()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
            }

            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }
        }

        private void OnDestroy()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
            }

            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }
        }

        public void OpenModal()
        {
            if (modalCanvasGroup == null)
            {
                return;
            }

            modalCanvasGroup.interactable = true;
            modalCanvasGroup.blocksRaycasts = true;
            RefreshLocalizedSprite();
            StartFade(1f);
        }

        public void CloseModal()
        {
            if (modalCanvasGroup == null)
            {
                return;
            }

            modalCanvasGroup.interactable = false;
            modalCanvasGroup.blocksRaycasts = false;
            StartFade(0f);
        }

        private void SetPassiveHiddenState()
        {
            if (modalCanvasGroup == null)
            {
                return;
            }

            modalCanvasGroup.alpha = 0f;
            modalCanvasGroup.interactable = false;
            modalCanvasGroup.blocksRaycasts = false;
        }

        private void StartFade(float targetAlpha)
        {
            if (modalCanvasGroup == null)
            {
                return;
            }

            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
        }

        private IEnumerator FadeRoutine(float targetAlpha)
        {
            if (modalCanvasGroup == null)
            {
                yield break;
            }

            float startingAlpha = modalCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                modalCanvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, t);
                yield return null;
            }

            modalCanvasGroup.alpha = targetAlpha;
            modalCanvasGroup.interactable = targetAlpha > 0.01f;
            modalCanvasGroup.blocksRaycasts = targetAlpha > 0.01f;
            fadeRoutine = null;
        }

        private void HandleLanguageChanged(AppLanguage newLanguage)
        {
            RefreshLocalizedSprite();
        }

        private void RefreshLocalizedSprite()
        {
            AppLanguage language = LanguageManager.Instance != null ? LanguageManager.Instance.CurrentLanguage : AppLanguage.RU;

            if (language == AppLanguage.HE)
            {
                if (btnCloseHe != null)
                {
                    btnCloseHe.gameObject.SetActive(true);
                }

                if (btnCloseDefault != null)
                {
                    btnCloseDefault.gameObject.SetActive(false);
                }

                if (infoBoardImage != null && infoSpriteHe != null)
                {
                    infoBoardImage.sprite = infoSpriteHe;
                }

                return;
            }

            if (btnCloseDefault != null)
            {
                btnCloseDefault.gameObject.SetActive(true);
            }

            if (btnCloseHe != null)
            {
                btnCloseHe.gameObject.SetActive(false);
            }

            Sprite targetSprite = language switch
            {
                AppLanguage.RU => infoSpriteRu,
                AppLanguage.EN => infoSpriteEn,
                _ => infoSpriteRu
            };

            if (infoBoardImage != null && targetSprite != null)
            {
                infoBoardImage.sprite = targetSprite;
            }
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CozyHome.UI
{
    public class SettingsModalController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup modalCanvasGroup;
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnLangRU;
        [SerializeField] private Button btnLangEN;
        [SerializeField] private Button btnLangHE;
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnQuit;
        [SerializeField] private TMP_Text textRestartLabel;
        [SerializeField] private TMP_Text textQuitLabel;
        [SerializeField] private float fadeDuration = 0.2f;

        private Coroutine fadeRoutine;

        private void Awake()
        {
            if (modalCanvasGroup == null)
            {
                modalCanvasGroup = GetComponent<CanvasGroup>();
            }

            BindButtons();
            SetPassiveHiddenState();
            RefreshLocalizedLabels();
        }

        private void Start()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            }

            RefreshLocalizedLabels();
        }

        private void OnEnable()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            }

            RefreshLocalizedLabels();
        }

        private void OnDisable()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
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

        private void BindButtons()
        {
            if (btnClose != null)
            {
                btnClose.onClick.RemoveListener(CloseModal);
                btnClose.onClick.AddListener(CloseModal);
            }

            if (btnLangRU != null)
            {
                btnLangRU.onClick.RemoveListener(SetLanguageRU);
                btnLangRU.onClick.AddListener(SetLanguageRU);
            }

            if (btnLangEN != null)
            {
                btnLangEN.onClick.RemoveListener(SetLanguageEN);
                btnLangEN.onClick.AddListener(SetLanguageEN);
            }

            if (btnLangHE != null)
            {
                btnLangHE.onClick.RemoveListener(SetLanguageHE);
                btnLangHE.onClick.AddListener(SetLanguageHE);
            }

            if (btnRestart != null)
            {
                btnRestart.onClick.RemoveListener(RestartScene);
                btnRestart.onClick.AddListener(RestartScene);
            }

            if (btnQuit != null)
            {
                btnQuit.onClick.RemoveListener(QuitToSplash);
                btnQuit.onClick.AddListener(QuitToSplash);
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
            RefreshLocalizedLabels();
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
            RefreshLocalizedLabels();
        }

        private void RefreshLocalizedLabels()
        {
            AppLanguage language = LanguageManager.Instance != null ? LanguageManager.Instance.CurrentLanguage : AppLanguage.RU;

            if (textRestartLabel != null)
            {
                textRestartLabel.text = GetRestartText(language);
                textRestartLabel.isRightToLeftText = language == AppLanguage.HE;
                textRestartLabel.alignment = TextAlignmentOptions.Center;
            }

            if (textQuitLabel != null)
            {
                textQuitLabel.text = GetQuitText(language);
                textQuitLabel.isRightToLeftText = language == AppLanguage.HE;
                textQuitLabel.alignment = TextAlignmentOptions.Center;
            }
        }

        private static string GetRestartText(AppLanguage language)
        {
            return language switch
            {
                AppLanguage.RU => "Начать заново",
                AppLanguage.EN => "Restart",
                AppLanguage.HE => "התחל מחדש",
                _ => "Начать заново"
            };
        }

        private static string GetQuitText(AppLanguage language)
        {
            return language switch
            {
                AppLanguage.RU => "Выход",
                AppLanguage.EN => "Exit",
                AppLanguage.HE => "יציאה",
                _ => "Выход"
            };
        }

        private void SetLanguageRU()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.SetLanguage(AppLanguage.RU);
            }
        }

        private void SetLanguageEN()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.SetLanguage(AppLanguage.EN);
            }
        }

        private void SetLanguageHE()
        {
            if (LanguageManager.Instance != null)
            {
                LanguageManager.Instance.SetLanguage(AppLanguage.HE);
            }
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void QuitToSplash()
        {
            SceneManager.LoadScene("SplashScene");
        }
    }
}

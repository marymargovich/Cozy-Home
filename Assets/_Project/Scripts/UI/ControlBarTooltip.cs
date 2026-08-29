using System.Collections;
using TMPro;
using UnityEngine;

namespace CozyHome.UI
{
    public enum ControlTooltipType
    {
        Volume,
        Record,
        Screenshot,
        ClearAll,
        Information,
        Settings
    }

    public class ControlBarTooltip : MonoBehaviour
    {
        [SerializeField] private CanvasGroup tooltipCanvasGroup;
        [SerializeField] private RectTransform tooltipRect;
        [SerializeField] private TMP_Text tooltipText;
        [SerializeField] private RectTransform canvasRootRect;
        [SerializeField] private float fadeDuration = 0.12f;
        [SerializeField] private float verticalOffset = 18f;

        private Coroutine fadeRoutine;

        private void Awake()
        {
            if (tooltipCanvasGroup == null)
            {
                tooltipCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (tooltipRect == null)
            {
                tooltipRect = GetComponent<RectTransform>();
            }

            if (canvasRootRect == null)
            {
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    canvasRootRect = parentCanvas.GetComponent<RectTransform>();
                }
            }

            InitializePassiveState();
        }

        private void OnEnable()
        {
            InitializePassiveState();
        }

        public void Show(ControlTooltipType type, RectTransform sourceRect)
        {
            if (tooltipCanvasGroup == null || tooltipRect == null || tooltipText == null)
            {
                return;
            }

            if (LanguageManager.Instance == null)
            {
                Hide();
                return;
            }

            tooltipText.text = GetLocalizedText(type, LanguageManager.Instance.CurrentLanguage);
            ApplyTextDirection(LanguageManager.Instance.CurrentLanguage);
            PositionAbove(sourceRect);
            StartFade(1f);
        }

        public void Hide()
        {
            StartFade(0f);
        }

        private void InitializePassiveState()
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
                tooltipText.raycastTarget = false;
            }
        }

        private void StartFade(float targetAlpha)
        {
            if (tooltipCanvasGroup == null)
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
            if (tooltipCanvasGroup == null)
            {
                yield break;
            }

            float startAlpha = tooltipCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                tooltipCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            tooltipCanvasGroup.alpha = targetAlpha;
            tooltipCanvasGroup.interactable = false;
            tooltipCanvasGroup.blocksRaycasts = false;
            fadeRoutine = null;
        }

        private void PositionAbove(RectTransform sourceRect)
        {
            if (sourceRect == null)
            {
                return;
            }

            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                return;
            }

            RectTransform canvasRect = canvasRootRect != null ? canvasRootRect : parentCanvas.GetComponent<RectTransform>();
            if (canvasRect == null)
            {
                return;
            }

            Vector3[] sourceCorners = new Vector3[4];
            sourceRect.GetWorldCorners(sourceCorners);
            Vector2 sourceTopCenterScreen = (sourceCorners[1] + sourceCorners[2]) * 0.5f;

            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                sourceTopCenterScreen,
                parentCanvas.worldCamera,
                out localPosition);

            float tooltipHalfHeight = tooltipRect.rect.height * 0.5f;
            float tooltipHalfWidth = tooltipRect.rect.width * 0.5f;
            float halfCanvasWidth = canvasRect.rect.width * 0.5f;
            float halfCanvasHeight = canvasRect.rect.height * 0.5f;

            Vector2 targetPosition = localPosition + new Vector2(0f, tooltipHalfHeight + verticalOffset);
            targetPosition.x = Mathf.Clamp(targetPosition.x, -halfCanvasWidth + tooltipHalfWidth, halfCanvasWidth - tooltipHalfWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -halfCanvasHeight + tooltipHalfHeight, halfCanvasHeight - tooltipHalfHeight);

            tooltipRect.anchoredPosition = targetPosition;
        }

        private void ApplyTextDirection(AppLanguage language)
        {
            if (tooltipText == null)
            {
                return;
            }

            tooltipText.alignment = TextAlignmentOptions.Center;

            if (language == AppLanguage.HE)
            {
                tooltipText.isRightToLeftText = true;
            }
            else
            {
                tooltipText.isRightToLeftText = false;
            }
        }

        private static string GetLocalizedText(ControlTooltipType type, AppLanguage language)
        {
            return type switch
            {
                ControlTooltipType.Volume => language switch
                {
                    AppLanguage.RU => "Громкость",
                    AppLanguage.EN => "Volume",
                    AppLanguage.HE => "עוצמת קול",
                    _ => "Громкость"
                },
                ControlTooltipType.Record => language switch
                {
                    AppLanguage.RU => "Запись",
                    AppLanguage.EN => "Record",
                    AppLanguage.HE => "הקלטה",
                    _ => "Запись"
                },
                ControlTooltipType.Screenshot => language switch
                {
                    AppLanguage.RU => "Снимок экрана",
                    AppLanguage.EN => "Screenshot",
                    AppLanguage.HE => "צילום מסך",
                    _ => "Снимок экрана"
                },
                ControlTooltipType.ClearAll => language switch
                {
                    AppLanguage.RU => "Выключить всё",
                    AppLanguage.EN => "Clear All",
                    AppLanguage.HE => "כבה הכול",
                    _ => "Выключить всё"
                },
                ControlTooltipType.Information => language switch
                {
                    AppLanguage.RU => "Информация",
                    AppLanguage.EN => "Information",
                    AppLanguage.HE => "מידע",
                    _ => "Информация"
                },
                ControlTooltipType.Settings => language switch
                {
                    AppLanguage.RU => "Настройки",
                    AppLanguage.EN => "Settings",
                    AppLanguage.HE => "הגדרות",
                    _ => "Настройки"
                },
                _ => string.Empty
            };
        }
    }
}

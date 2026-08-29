using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CozyHome.Audio;
using CozyHome.Interaction;

namespace CozyHome.UI
{
    [DisallowMultipleComponent]
    public class ControlBarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform panelSettingsBar;
        [SerializeField] private Button btnToggleSettings;
        [SerializeField] private Button btnRec;
        [SerializeField] private Button btnScreenshot;
        [SerializeField] private Button btnClearAll;
        [SerializeField] private Button btnInfo;
        [SerializeField] private Button btnGeneralSettings;

        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private Vector2 hiddenOffset = new Vector2(220f, 0f);
        [SerializeField] private Vector2 openOffset = Vector2.zero;

        [Header("Indicators")]
        [SerializeField] private GameObject recActiveIndicator;

        [Header("Info Modal")]
        [SerializeField] private InfoModalController infoModalController;

        [Header("Events")]
        [SerializeField] private UnityEvent onRecClicked;
        [SerializeField] private UnityEvent onInfoRequested;
        [SerializeField] private UnityEvent onGeneralSettingsRequested;

        private bool isPanelOpen;
        private bool isCapturingScreenshot;
        private Coroutine panelAnimationCoroutine;
        private CanvasGroup controlBarCanvasGroup;
        private SceneAudioRecorder sceneAudioRecorder;
        private Coroutine recordingIndicatorCoroutine;
        private CanvasGroup recIndicatorCanvasGroup;
        private GameObject shutterOverlay;
        private Image shutterImage;
        private static readonly Color TransparentButtonColor = new Color(1f, 1f, 1f, 0f);

        private void Awake()
        {
            BindButtonHandlers();
            EnsureRecordingIndicator();
            ResolveAudioRecorder();
            ApplyRecordingVisualState();
            ApplyPanelStateInstantly(isPanelOpen);
        }

        private void OnValidate()
        {
            animationDuration = Mathf.Max(0.01f, animationDuration);
            ApplyPanelStateInstantly(isPanelOpen);
        }

        private void BindButtonHandlers()
        {
            if (btnToggleSettings != null)
            {
                btnToggleSettings.onClick.AddListener(ToggleSettingsPanel);
            }

            if (btnRec != null)
            {
                btnRec.onClick.AddListener(OnRecClicked);
            }

            if (btnScreenshot != null)
            {
                btnScreenshot.onClick.AddListener(OnScreenshotClicked);
            }

            if (btnClearAll != null)
            {
                btnClearAll.onClick.AddListener(OnClearAllClicked);
            }

            if (btnInfo != null)
            {
                btnInfo.onClick.AddListener(OnInfoClicked);
            }

            if (btnGeneralSettings != null)
            {
                btnGeneralSettings.onClick.AddListener(OnGeneralSettingsClicked);
            }
        }

        public void ToggleSettingsPanel()
        {
            isPanelOpen = !isPanelOpen;
            MovePanelTo(isPanelOpen ? openOffset : hiddenOffset);
        }

        public void SetPanelOpen(bool open)
        {
            if (isPanelOpen == open)
            {
                return;
            }

            isPanelOpen = open;
            MovePanelTo(isPanelOpen ? openOffset : hiddenOffset);
        }

        public void OnRecClicked()
        {
            if (sceneAudioRecorder == null)
            {
                ResolveAudioRecorder();
            }

            if (sceneAudioRecorder == null)
            {
                Debug.LogWarning("ControlBarController: SceneAudioRecorder was not found.");
                onRecClicked?.Invoke();
                return;
            }

            if (sceneAudioRecorder.IsRecording)
            {
                string savedPath = sceneAudioRecorder.StopRecordingAndSave();
                if (!string.IsNullOrEmpty(savedPath))
                {
                    Debug.Log("ControlBarController: Recording saved to " + savedPath);
                }
            }
            else
            {
                sceneAudioRecorder.StartRecording();
                Debug.Log("ControlBarController: Recording started.");
            }

            ApplyRecordingVisualState();
            onRecClicked?.Invoke();
        }

        public void OnScreenshotClicked()
        {
            if (isCapturingScreenshot)
            {
                return;
            }

            StartCoroutine(TakeScreenshotRoutine());
        }

        public void OnClearAllClicked()
        {
            RoomItem[] roomItems = FindObjectsByType<RoomItem>(FindObjectsInactive.Exclude);
            if (roomItems == null || roomItems.Length == 0)
            {
                Debug.Log("ControlBarController: No RoomItem objects found.");
                return;
            }

            foreach (RoomItem roomItem in roomItems)
            {
                if (roomItem == null)
                {
                    continue;
                }

                roomItem.ClearCurrentState();
            }

            Debug.Log("ControlBarController: All active room sounds were stopped.");
        }

        public void OnInfoClicked()
        {
            if (infoModalController != null)
            {
                infoModalController.OpenModal();
            }
            else
            {
                Debug.LogWarning("ControlBarController: InfoModalController reference is missing.");
            }

            Debug.Log("ControlBarController: Info window requested.");
            onInfoRequested?.Invoke();
        }

        public void OnGeneralSettingsClicked()
        {
            Debug.Log("ControlBarController: General settings requested.");
            onGeneralSettingsRequested?.Invoke();
        }

        private void MovePanelTo(Vector2 targetPosition)
        {
            if (panelSettingsBar == null)
            {
                return;
            }

            if (panelAnimationCoroutine != null)
            {
                StopCoroutine(panelAnimationCoroutine);
            }

            panelAnimationCoroutine = StartCoroutine(MovePanelRoutine(targetPosition));
        }

        private IEnumerator MovePanelRoutine(Vector2 targetPosition)
        {
            Vector2 startPosition = panelSettingsBar.anchoredPosition;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / animationDuration);
                panelSettingsBar.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            panelSettingsBar.anchoredPosition = targetPosition;
            panelAnimationCoroutine = null;
        }

        private void ApplyPanelStateInstantly(bool open)
        {
            if (panelSettingsBar == null)
            {
                return;
            }

            panelSettingsBar.anchoredPosition = open ? openOffset : hiddenOffset;
        }

        public static bool IsPointerOverAnyUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private IEnumerator TakeScreenshotRoutine()
        {
            if (isCapturingScreenshot)
            {
                yield break;
            }

            isCapturingScreenshot = true;

            if (controlBarCanvasGroup == null)
            {
                controlBarCanvasGroup = GetComponent<CanvasGroup>();
                if (controlBarCanvasGroup == null)
                {
                    controlBarCanvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            float originalControlBarAlpha = controlBarCanvasGroup.alpha;
            bool originalControlBarInteractable = controlBarCanvasGroup.interactable;
            bool originalControlBarBlocksRaycasts = controlBarCanvasGroup.blocksRaycasts;

            List<(CanvasGroup group, float alpha, bool interactable, bool blocksRaycasts)> hiddenGroups = new List<(CanvasGroup group, float alpha, bool interactable, bool blocksRaycasts)>();

            HideCanvasGroup(controlBarCanvasGroup, hiddenGroups);

            if (infoModalController != null)
            {
                infoModalController.CloseModal();
            }

            ControlBarTooltip[] tooltips = FindObjectsByType<ControlBarTooltip>(FindObjectsInactive.Include);
            foreach (ControlBarTooltip tooltip in tooltips)
            {
                if (tooltip == null)
                {
                    continue;
                }

                HidePrivateCanvasGroup(tooltip, "tooltipCanvasGroup", hiddenGroups);
            }

            SettingsModalController[] modals = FindObjectsByType<SettingsModalController>(FindObjectsInactive.Include);
            foreach (SettingsModalController modal in modals)
            {
                if (modal == null)
                {
                    continue;
                }

                HidePrivateCanvasGroup(modal, "modalCanvasGroup", hiddenGroups);
            }

            EnsureShutterOverlay();
            GameObject shutterObject = shutterOverlay;
            if (shutterObject == null)
            {
                isCapturingScreenshot = false;
                yield break;
            }

            shutterObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.45f);

            shutterObject.SetActive(true);
            if (shutterImage != null)
            {
                shutterImage.color = new Color(0f, 0f, 0f, 0.7f);
            }

            yield return new WaitForSecondsRealtime(0.08f);

            shutterObject.SetActive(false);
            yield return new WaitForEndOfFrame();

            string targetPath = MediaExportUtility.BuildExportPath("screenshot", ".png");
            ScreenCapture.CaptureScreenshot(targetPath);

            shutterObject.SetActive(true);
            if (shutterImage != null)
            {
                shutterImage.color = new Color(0f, 0f, 0f, 0.7f);
            }

            float elapsed = 0f;
            float fadeDuration = 0.15f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                if (shutterImage != null)
                {
                    shutterImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0.7f, 0f, t));
                }
                yield return null;
            }

            if (shutterImage != null)
            {
                shutterImage.color = new Color(0f, 0f, 0f, 0f);
            }

            shutterObject.SetActive(false);

            RestoreHiddenGroups(hiddenGroups);

            controlBarCanvasGroup.alpha = originalControlBarAlpha;
            controlBarCanvasGroup.interactable = originalControlBarInteractable;
            controlBarCanvasGroup.blocksRaycasts = originalControlBarBlocksRaycasts;

            isCapturingScreenshot = false;
            Debug.Log("ControlBarController: Screenshot saved to " + targetPath);
        }

        private void EnsureShutterOverlay()
        {
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas == null)
            {
                Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include);
                if (canvases != null && canvases.Length > 0)
                {
                    rootCanvas = canvases[0];
                }
            }

            if (rootCanvas == null)
            {
                Debug.LogWarning("ControlBarController: No Canvas found for screenshot shutter overlay.");
                return;
            }

            if (shutterOverlay == null)
            {
                shutterOverlay = new GameObject("ScreenshotShutterOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            }

            if (shutterOverlay.GetComponent<CanvasRenderer>() == null)
            {
                shutterOverlay.AddComponent<CanvasRenderer>();
            }

            shutterOverlay.transform.SetParent(rootCanvas.transform, false);
            shutterOverlay.transform.SetAsLastSibling();
            shutterOverlay.layer = LayerMask.NameToLayer("UI");

            RectTransform overlayRect = shutterOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayRect.localScale = Vector3.one;

            shutterImage = shutterOverlay.GetComponent<Image>();
            if (shutterImage == null)
            {
                shutterImage = shutterOverlay.AddComponent<Image>();
            }

            shutterImage.raycastTarget = false;
            shutterImage.color = new Color(0f, 0f, 0f, 0f);
            shutterOverlay.SetActive(false);
        }

        private void ResolveAudioRecorder()
        {
            if (sceneAudioRecorder != null)
            {
                return;
            }

            sceneAudioRecorder = FindAnyObjectByType<SceneAudioRecorder>();
        }

        private void EnsureRecordingIndicator()
        {
            if (btnRec == null)
            {
                return;
            }

            if (recActiveIndicator == null)
            {
                recActiveIndicator = new GameObject("RecActiveIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
                recActiveIndicator.transform.SetParent(btnRec.transform, false);

                RectTransform indicatorRect = recActiveIndicator.GetComponent<RectTransform>();
                indicatorRect.anchorMin = new Vector2(0.5f, 0.5f);
                indicatorRect.anchorMax = new Vector2(0.5f, 0.5f);
                indicatorRect.pivot = new Vector2(0.5f, 0.5f);
                indicatorRect.sizeDelta = new Vector2(12f, 12f);
                indicatorRect.anchoredPosition = new Vector2(0f, 0f);

                Image indicatorImage = recActiveIndicator.GetComponent<Image>();
                indicatorImage.color = new Color(1f, 0f, 0f, 0f);
                indicatorImage.raycastTarget = false;

                recIndicatorCanvasGroup = recActiveIndicator.GetComponent<CanvasGroup>();
                recIndicatorCanvasGroup.alpha = 0f;
                recIndicatorCanvasGroup.blocksRaycasts = false;
                recIndicatorCanvasGroup.interactable = false;
            }
            else
            {
                recIndicatorCanvasGroup = recActiveIndicator.GetComponent<CanvasGroup>();
                if (recIndicatorCanvasGroup == null)
                {
                    recIndicatorCanvasGroup = recActiveIndicator.AddComponent<CanvasGroup>();
                }

                CanvasRenderer renderer = recActiveIndicator.GetComponent<CanvasRenderer>();
                if (renderer != null)
                {
                    renderer.SetMaterial(null, null);
                }
            }

            recActiveIndicator.SetActive(false);
        }

        private void ApplyRecordingVisualState()
        {
            if (btnRec == null || btnRec.image == null)
            {
                return;
            }

            btnRec.image.color = TransparentButtonColor;

            if (recActiveIndicator == null)
            {
                EnsureRecordingIndicator();
            }

            if (sceneAudioRecorder != null && sceneAudioRecorder.IsRecording)
            {
                if (recActiveIndicator != null)
                {
                    recActiveIndicator.SetActive(true);
                    if (recordingIndicatorCoroutine != null)
                    {
                        StopCoroutine(recordingIndicatorCoroutine);
                    }

                    recordingIndicatorCoroutine = StartCoroutine(PulseRecordingIndicator());
                }
            }
            else
            {
                if (recActiveIndicator != null)
                {
                    recActiveIndicator.SetActive(false);
                    if (recIndicatorCanvasGroup != null)
                    {
                        recIndicatorCanvasGroup.alpha = 0f;
                    }
                }
            }
        }

        private IEnumerator PulseRecordingIndicator()
        {
            if (recActiveIndicator == null || recIndicatorCanvasGroup == null)
            {
                yield break;
            }

            recActiveIndicator.SetActive(true);
            recIndicatorCanvasGroup.alpha = 1f;

            while (sceneAudioRecorder != null && sceneAudioRecorder.IsRecording)
            {
                float pulse = 0.5f + Mathf.PingPong(Time.unscaledTime * 2.5f, 1f) * 0.5f;
                recIndicatorCanvasGroup.alpha = Mathf.Lerp(0.35f, 1f, pulse);
                Vector3 scale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 1f), new Vector3(1.25f, 1.25f, 1f), pulse);
                recActiveIndicator.transform.localScale = scale;
                yield return null;
            }

            recIndicatorCanvasGroup.alpha = 0f;
            recActiveIndicator.SetActive(false);
        }


        private static void HideCanvasGroup(CanvasGroup canvasGroup, List<(CanvasGroup group, float alpha, bool interactable, bool blocksRaycasts)> hiddenGroups)
        {
            if (canvasGroup == null)
            {
                return;
            }

            hiddenGroups.Add((canvasGroup, canvasGroup.alpha, canvasGroup.interactable, canvasGroup.blocksRaycasts));
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private static void HidePrivateCanvasGroup(object target, string fieldName, List<(CanvasGroup group, float alpha, bool interactable, bool blocksRaycasts)> hiddenGroups)
        {
            if (target == null)
            {
                return;
            }

            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                return;
            }

            CanvasGroup canvasGroup = field.GetValue(target) as CanvasGroup;
            HideCanvasGroup(canvasGroup, hiddenGroups);
        }

        private static void RestoreHiddenGroups(List<(CanvasGroup group, float alpha, bool interactable, bool blocksRaycasts)> hiddenGroups)
        {
            if (hiddenGroups == null)
            {
                return;
            }

            for (int i = hiddenGroups.Count - 1; i >= 0; i--)
            {
                CanvasGroup group = hiddenGroups[i].group;
                if (group == null)
                {
                    continue;
                }

                group.alpha = hiddenGroups[i].alpha;
                group.interactable = hiddenGroups[i].interactable;
                group.blocksRaycasts = hiddenGroups[i].blocksRaycasts;
            }
        }
    }
}

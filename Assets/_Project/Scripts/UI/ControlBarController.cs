using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CozyHome.Interaction;

namespace CozyHome.UI
{
    [DisallowMultipleComponent]
    public class ControlBarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform panelSettingsBar;
        [SerializeField] private Button btnToggleSettings;
        [SerializeField] private Button btnVolume;
        [SerializeField] private Button btnRec;
        [SerializeField] private Button btnScreenshot;
        [SerializeField] private Button btnClearAll;
        [SerializeField] private Button btnInfo;
        [SerializeField] private Button btnGeneralSettings;

        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private Vector2 hiddenOffset = new Vector2(220f, 0f);
        [SerializeField] private Vector2 openOffset = Vector2.zero;

        [Header("Events")]
        [SerializeField] private UnityEvent onRecClicked;
        [SerializeField] private UnityEvent onInfoRequested;
        [SerializeField] private UnityEvent onGeneralSettingsRequested;

        private bool isPanelOpen;
        private Coroutine panelAnimationCoroutine;

        private void Awake()
        {
            BindButtonHandlers();
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

            if (btnVolume != null)
            {
                btnVolume.onClick.AddListener(OnVolumeClicked);
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

        public void OnVolumeClicked()
        {
            bool isMuted = AudioListener.pause || Mathf.Approximately(AudioListener.volume, 0f);
            AudioListener.pause = !isMuted;
            AudioListener.volume = isMuted ? 1f : 0f;

            Debug.Log(isMuted ? "ControlBarController: Master audio enabled." : "ControlBarController: Master audio muted.");
        }

        public void OnRecClicked()
        {
            Debug.Log("ControlBarController: Rec button clicked. Recording feature is not implemented yet.");
            onRecClicked?.Invoke();
        }

        public void OnScreenshotClicked()
        {
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

                roomItem.SetActiveForAudio(false);

                AudioSource audioSource = roomItem.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }

            Debug.Log("ControlBarController: All active room sounds were stopped.");
        }

        public void OnInfoClicked()
        {
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
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            CanvasGroup rootCanvasGroup = GetComponentInParent<CanvasGroup>();

            if (rootCanvas != null)
            {
                rootCanvas.enabled = false;
            }

            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.interactable = false;
                rootCanvasGroup.blocksRaycasts = false;
            }

            yield return null;

            string fileName = "CozyHome_Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png";
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            ScreenCapture.CaptureScreenshot(savePath);

            yield return new WaitForSeconds(0.1f);

            if (rootCanvas != null)
            {
                rootCanvas.enabled = true;
            }

            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.interactable = true;
                rootCanvasGroup.blocksRaycasts = true;
            }

            Debug.Log("ControlBarController: Screenshot saved to " + savePath);
        }
    }
}

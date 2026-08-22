using UnityEngine;
using UnityEngine.InputSystem;

namespace CozyHome.UI
{
    /// <summary>
    /// Spawns floating UI notes at a given screen or world position and lets each note handle
    /// its own lifetime cleanup after it is created.
    /// </summary>
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        /// <summary>
        /// Reads the current pointer position from the Input System and safely falls back if the
        /// pointer device is not available in the current platform or input mode.
        /// </summary>
        public static Vector3 GetPointerScreenPosition()
        {
            Vector2 screenPosition = Vector2.zero;
            if (UnityEngine.InputSystem.Pointer.current != null)
            {
                screenPosition = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            }

            // Fallback for platforms without a pointer device such as touch or pen-only setups.
            if (screenPosition == Vector2.zero)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    return mainCamera.pixelRect.center;
                }
            }

            return screenPosition;
        }

        [Header("Floating Note Effect")]
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private Transform notesContainer;

        private void Awake()
        {
            // Keep a single manager instance so props can access it without a manual reference.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            // Make sure the container exists before a note is spawned.
            if (notesContainer == null)
            {
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                notesContainer = parentCanvas != null ? parentCanvas.transform : transform;
            }
        }

        /// <summary>
        /// Creates a floating note at a screen-space or world-space point and keeps it under the
        /// active canvas so it behaves like a UI effect.
        /// </summary>
        public void SpawnNoteAt(Vector2 screenPosition)
        {
            SpawnNoteAt((Vector3)screenPosition);
        }

        public void SpawnNoteAt(Vector3 screenOrWorldPosition)
        {
            // Make sure the prefab is assigned before trying to instantiate it.
            if (notePrefab == null)
            {
                Debug.LogWarning("EffectManager: notePrefab is not assigned.");
                return;
            }

            // Use the configured container if one is set. Otherwise, fall back to the current
            // canvas transform so the note still renders in the UI.
            if (notesContainer == null)
            {
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                notesContainer = parentCanvas != null ? parentCanvas.transform : transform;
            }

            // Determine the correct canvas point from the supplied value.
            Vector3 canvasPosition = screenOrWorldPosition;
            Canvas canvas = notesContainer.GetComponentInParent<Canvas>();
            Camera activeCamera = canvas != null ? canvas.worldCamera : Camera.main;

            if (canvas != null)
            {
                // Screen-space overlays use screen coordinates directly.
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    if (Camera.main != null && screenOrWorldPosition.z != 0f)
                    {
                        canvasPosition = Camera.main.WorldToScreenPoint(screenOrWorldPosition);
                    }
                }
                else
                {
                    // World-space and camera-space canvases expect a screen point before local conversion.
                    if (activeCamera != null)
                    {
                        canvasPosition = activeCamera.WorldToScreenPoint(screenOrWorldPosition);
                    }
                }
            }

            // Create the note as a child of the notes container.
            GameObject noteObject = Instantiate(notePrefab, notesContainer, false);

            // Position the note in the parent canvas space to match the caller's point.
            RectTransform noteRect = noteObject.GetComponent<RectTransform>();
            RectTransform containerRect = notesContainer as RectTransform;

            if (noteRect != null && containerRect != null)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    containerRect,
                    canvasPosition,
                    activeCamera,
                    out localPoint);

                noteRect.localPosition = localPoint;
                return;
            }

            // Fallback if the prefab does not have a RectTransform.
            noteObject.transform.position = canvasPosition;
        }
    }
}

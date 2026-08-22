using UnityEngine;

namespace CozyHome.UI
{
    /// <summary>
    /// Handles the lifetime of a floating note prefab. It removes itself automatically after
    /// a short delay so the UI effect can fade out cleanly without additional cleanup code.
    /// </summary>
    public class FloatingNoteEffect : MonoBehaviour
    {
        [Header("Floating Note Lifetime")]
        [Tooltip("How long this note remains visible before it is destroyed.")]
        [SerializeField] private float lifeTime = 0.8f;

        private void OnEnable()
        {
            // Cancel any previous destroy call before scheduling a new one. This keeps the
            // effect safe if the object is reused or re-enabled.
            CancelInvoke(nameof(DestroySelf));

            // Start the countdown as soon as the note becomes active.
            Invoke(nameof(DestroySelf), lifeTime);
        }

        private void OnDisable()
        {
            // If the note is disabled before its timer finishes, stop the pending destroy call.
            CancelInvoke(nameof(DestroySelf));
        }

        private void DestroySelf()
        {
            // Remove the note from the scene when its lifetime is over.
            Destroy(gameObject);
        }
    }
}

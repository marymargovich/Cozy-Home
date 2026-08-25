using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using CozyHome.Audio;
using CozyHome.UI;

namespace CozyHome.Interaction
{
    [RequireComponent(typeof(Button), typeof(Image), typeof(Animator))]
    public class InteractiveAnimatedToggle : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite offSprite;
        [SerializeField] private string defaultAnimationState = "Fireplace_Burn";
        [SerializeField] private string secretAnimationState = "Secret";
        [SerializeField] [Min(0f)] private float doubleClickThreshold = 0.3f;
        [SerializeField] private bool startActive = false;

        [Header("Events")]
        [SerializeField] private UnityEvent<bool> onStateChanged;

        private Button button;
        private Image image;
        private Animator animator;

        private bool isActive;
        private Coroutine singleTapCoroutine;
        private Coroutine secretAnimationCoroutine;

        private void Awake()
        {
            CacheComponents();

            isActive = startActive;
            ApplyState();
        }

        private void OnDisable()
        {
            CancelSingleTapCoroutine();
            CancelSecretAnimationCoroutine();
        }

        private void OnValidate()
        {
            if (doubleClickThreshold < 0f)
            {
                doubleClickThreshold = 0f;
            }
        }

        private void CacheComponents()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            animator = GetComponent<Animator>();

            if (button == null)
            {
                Debug.LogWarning("InteractiveAnimatedToggle requires a Button component.", this);
            }

            if (image == null)
            {
                Debug.LogWarning("InteractiveAnimatedToggle requires an Image component.", this);
            }

            if (animator == null)
            {
                Debug.LogWarning("InteractiveAnimatedToggle requires an Animator component.", this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleButtonClick(eventData);
        }

        private void HandleButtonClick(PointerEventData eventData = null)
        {
            if (button == null || animator == null || image == null)
            {
                return;
            }

            // Capture the pointer position from the event data when possible, or use the generic
            // Input System pointer as a safe fallback for mouse and touch input.
            Vector3 notePosition = transform.position;
            if (eventData != null)
            {
                notePosition = eventData.position;
            }
            else if (UnityEngine.InputSystem.Pointer.current != null)
            {
                notePosition = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            }
            else
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    notePosition = mainCamera.WorldToScreenPoint(transform.position);
                }
            }

            if (MusicPuzzleManager.Instance == null || !MusicPuzzleManager.Instance.IsObjectActiveInCurrentRound(this))
            {
                return;
            }

            EffectManager.Instance?.SpawnNoteAt(notePosition);
            MusicPuzzleManager.Instance.PlayInteractionSound(this);

            if (singleTapCoroutine != null)
            {
                CancelSingleTapCoroutine();
                TriggerDoubleTap();
                return;
            }

            singleTapCoroutine = StartCoroutine(HandleSingleTapDelay());
        }

        private IEnumerator HandleSingleTapDelay()
        {
            yield return new WaitForSeconds(doubleClickThreshold);
            singleTapCoroutine = null;
            TriggerSingleTap();
        }

        private void TriggerSingleTap()
        {
            if (isActive && secretAnimationCoroutine != null)
            {
                isActive = false;
                CancelSecretAnimationCoroutine();
                SetInactive();
                onStateChanged?.Invoke(isActive);
                return;
            }

            isActive = !isActive;

            if (isActive)
            {
                PlayDefaultAnimation();
                onStateChanged?.Invoke(isActive);
                return;
            }

            CancelSecretAnimationCoroutine();
            SetInactive();
            onStateChanged?.Invoke(isActive);
        }

        private void TriggerDoubleTap()
        {
            isActive = true;
            CancelSecretAnimationCoroutine();
            PlaySecretAnimation();
            onStateChanged?.Invoke(isActive);
        }

        private void ApplyState()
        {
            if (isActive)
            {
                PlayDefaultAnimation();
                onStateChanged?.Invoke(isActive);
                return;
            }

            SetInactive();
            onStateChanged?.Invoke(isActive);
        }

        private void SetInactive()
        {
            if (animator != null)
            {
                animator.enabled = false;
            }

            if (image != null)
            {
                if (offSprite != null)
                {
                    image.sprite = offSprite;
                }

                image.enabled = true;
            }
        }

        private void PlayDefaultAnimation()
        {
            if (animator != null)
            {
                animator.enabled = true;

                if (!string.IsNullOrWhiteSpace(defaultAnimationState))
                {
                    animator.Play(defaultAnimationState, 0, 0f);
                }
            }

            if (image != null)
            {
                image.enabled = true;
            }
        }

        private void PlaySecretAnimation()
        {
            if (animator != null)
            {
                animator.enabled = true;

                if (!string.IsNullOrWhiteSpace(secretAnimationState))
                {
                    animator.Play(secretAnimationState, 0, 0f);
                }
            }

            if (image != null)
            {
                image.enabled = true;
            }

            secretAnimationCoroutine = StartCoroutine(ReturnToDefaultAnimationAfterSecret());
        }

        private IEnumerator ReturnToDefaultAnimationAfterSecret()
        {
            yield return null;

            if (animator == null)
            {
                secretAnimationCoroutine = null;
                yield break;
            }

            float secretAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;

            if (secretAnimationLength > 0f)
            {
                yield return new WaitForSeconds(secretAnimationLength);
            }

            if (!isActive)
            {
                secretAnimationCoroutine = null;
                yield break;
            }

            PlayDefaultAnimation();
            secretAnimationCoroutine = null;
        }

        private void CancelSingleTapCoroutine()
        {
            if (singleTapCoroutine != null)
            {
                StopCoroutine(singleTapCoroutine);
                singleTapCoroutine = null;
            }
        }

        private void CancelSecretAnimationCoroutine()
        {
            if (secretAnimationCoroutine != null)
            {
                StopCoroutine(secretAnimationCoroutine);
                secretAnimationCoroutine = null;
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using CozyHome.Audio;
using CozyHome.Environment;
using CozyHome.UI;

namespace CozyHome.Interaction
{
    [RequireComponent(typeof(Image), typeof(Button))]
    [RequireComponent(typeof(AudioSource), typeof(Animator))]
    public class RoomItem : MonoBehaviour, IPointerClickHandler
    {
        public enum LampType
        {
            None,
            FloorLamp,
            CeilingLamp,
            Garland1,
            Garland2
        }

        [Header("Environment / Lamp State")]
        [SerializeField] private LampType lampType = LampType.None;
        [SerializeField] private TimeOfDayController timeOfDayController;

        [Header("Music / Sound")]
        [SerializeField] private bool canPlaySound = true;
        [SerializeField] private bool isGuaranteedSound = false;
        [SerializeField] private AudioClip[] soundVariations;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool loopSound = false;

        [Header("Animation")]
        [SerializeField] private bool hasAnimation = false;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private string defaultAnimationState = "";
        [SerializeField] private string secretAnimationState = "";
        [SerializeField] [Min(0f)] private float doubleClickThreshold = 0.3f;
        [SerializeField] private bool startActive = false;

        public bool CanPlaySound => canPlaySound;
        public bool IsGuaranteedSound => isGuaranteedSound;
        public bool HasAnimation => hasAnimation;
        public bool IsActiveForAudio { get; private set; }
        public AudioClip AssignedAudioClip => assignedAudioClip;

        private Animator animator;
        private Image image;
        private bool isAnimationOn;
        private Coroutine singleTapCoroutine;
        private Coroutine secretAnimationCoroutine;
        private bool isRegistered;
        private AudioClip assignedAudioClip;

        private void Awake()
        {
            ResolveTimeOfDayController();
            CacheAudioSource();

            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
                audioSource.loop = loopSound;
            }

            if (soundVariations != null && soundVariations.Length > 0)
            {
                AssignRandomSoundVariation();
            }
            else if (audioSource != null && audioSource.clip != null)
            {
                assignedAudioClip = audioSource.clip;
            }

            if (hasAnimation)
            {
                CacheAnimationComponents();
                isAnimationOn = startActive;
                ApplyAnimationState();
            }

            if (isGuaranteedSound)
            {
                IsActiveForAudio = true;
            }

            RegisterWithManager();
        }

        private void Start()
        {
            ResolveTimeOfDayController();

            if (isGuaranteedSound)
            {
                IsActiveForAudio = true;
            }

            RegisterWithManager();
        }

        private void OnEnable()
        {
            RegisterWithManager();
        }

        private void OnDisable()
        {
            CancelSingleTapCoroutine();
            CancelSecretAnimationCoroutine();

            if (MusicPuzzleManager.Instance != null)
            {
                MusicPuzzleManager.Instance.UnregisterRoomItem(this);
                isRegistered = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleClick(eventData);
        }

        public void SetActiveForAudio(bool active)
        {
            IsActiveForAudio = active;
        }

        public void AssignRandomSoundVariation()
        {
            if (soundVariations == null || soundVariations.Length == 0)
            {
                if (audioSource != null)
                {
                    assignedAudioClip = audioSource.clip;
                }
                return;
            }

            assignedAudioClip = soundVariations[Random.Range(0, soundVariations.Length)];
            if (audioSource != null)
            {
                audioSource.clip = assignedAudioClip;
            }
        }

        public void RerollAssignedClip()
        {
            AssignRandomSoundVariation();
        }

        public void ClearCurrentState()
        {
            CancelSingleTapCoroutine();
            CancelSecretAnimationCoroutine();

            if (audioSource != null)
            {
                audioSource.Stop();
            }

            isAnimationOn = false;
            if (hasAnimation)
            {
                SetInactiveAnimation();
            }

            SyncLampStateToController(false);
        }

        public bool TryPlayAssignedSound()
        {
            if (audioSource == null)
            {
                return false;
            }

            AudioClip clipToPlay = assignedAudioClip;
            if (clipToPlay == null && soundVariations != null && soundVariations.Length > 0)
            {
                AssignRandomSoundVariation();
                clipToPlay = assignedAudioClip;
            }
            else if (clipToPlay == null && audioSource.clip != null)
            {
                clipToPlay = audioSource.clip;
            }

            if (clipToPlay == null)
            {
                return false;
            }

            if (audioSource.clip != clipToPlay)
            {
                audioSource.clip = clipToPlay;
            }

            audioSource.Stop();
            audioSource.Play();
            return true;
        }

        public void HandleClick(PointerEventData eventData = null)
        {
            if (hasAnimation)
            {
                HandleAnimationClick();
            }

            SecretHintController.TryRegisterInteraction();

            bool isActiveForAudio = MusicPuzzleManager.Instance != null && MusicPuzzleManager.Instance.IsItemActive(this);
            if (!isActiveForAudio)
            {
                return;
            }

            Vector3 notePosition = transform.position;
            if (eventData != null)
            {
                notePosition = eventData.position;
            }
            else if (Pointer.current != null)
            {
                notePosition = Pointer.current.position.ReadValue();
            }
            else
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    notePosition = mainCamera.WorldToScreenPoint(transform.position);
                }
            }

            EffectManager.Instance?.SpawnNoteAt(notePosition);
            TryPlayAssignedSound();
        }

        private void RegisterWithManager()
        {
            if (isRegistered || MusicPuzzleManager.Instance == null)
            {
                return;
            }

            MusicPuzzleManager.Instance.RegisterRoomItem(this);
            isRegistered = true;
        }

        private void CacheAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void CacheAnimationComponents()
        {
            animator = GetComponent<Animator>();
            image = GetComponent<Image>();

            if (animator == null)
            {
                Debug.LogWarning("RoomItem hasAnimation is enabled but an Animator is missing.", this);
            }

            if (image == null)
            {
                Debug.LogWarning("RoomItem hasAnimation is enabled but an Image is missing.", this);
            }
        }

        private void HandleAnimationClick()
        {
            if (animator == null || image == null)
            {
                return;
            }

            if (singleTapCoroutine != null)
            {
                CancelSingleTapCoroutine();
                TriggerDoubleTapAnimation();
                return;
            }

            singleTapCoroutine = StartCoroutine(HandleSingleTapDelay());
        }

        private IEnumerator HandleSingleTapDelay()
        {
            yield return new WaitForSeconds(doubleClickThreshold);
            singleTapCoroutine = null;
            TriggerSingleTapAnimation();
        }

        private void TriggerSingleTapAnimation()
        {
            if (isAnimationOn && secretAnimationCoroutine != null)
            {
                isAnimationOn = false;
                CancelSecretAnimationCoroutine();
                SetInactiveAnimation();
                SyncLampStateToController(isAnimationOn);
                return;
            }

            isAnimationOn = !isAnimationOn;

            if (isAnimationOn)
            {
                PlayDefaultAnimation();
                SyncLampStateToController(isAnimationOn);
                return;
            }

            CancelSecretAnimationCoroutine();
            SetInactiveAnimation();
            SyncLampStateToController(isAnimationOn);
        }

        private void TriggerDoubleTapAnimation()
        {
            isAnimationOn = true;
            CancelSecretAnimationCoroutine();
            PlaySecretAnimation();
            SyncLampStateToController(isAnimationOn);
        }

        private void ResolveTimeOfDayController()
        {
            if (timeOfDayController != null)
            {
                return;
            }

            timeOfDayController = GetComponentInParent<TimeOfDayController>();
            if (timeOfDayController == null)
            {
                timeOfDayController = FindAnyObjectByType<TimeOfDayController>();
            }
        }

        private void SyncLampStateToController(bool isOn)
        {
            if (audioSource != null && !isOn)
            {
                audioSource.Stop();
            }

            if (lampType == LampType.None)
            {
                return;
            }

            ResolveTimeOfDayController();
            if (timeOfDayController == null)
            {
                return;
            }

            switch (lampType)
            {
                case LampType.FloorLamp:
                    timeOfDayController.SetFloorLampState(isOn);
                    break;
                case LampType.CeilingLamp:
                    timeOfDayController.SetCeilingLampState(isOn);
                    break;
                case LampType.Garland1:
                    timeOfDayController.SetGarland1State(isOn);
                    break;
                case LampType.Garland2:
                    timeOfDayController.SetGarland2State(isOn);
                    break;
            }
        }

        private void ApplyAnimationState()
        {
            if (isAnimationOn)
            {
                PlayDefaultAnimation();
                return;
            }

            SetInactiveAnimation();
        }

        private void SetInactiveAnimation()
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

            if (!isAnimationOn)
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

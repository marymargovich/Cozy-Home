using UnityEngine;
using CozyHome.UI;

namespace CozyHome.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicInteractableSlot : MonoBehaviour
    {
        [SerializeField] private bool isStaticSlot = false;
        [SerializeField] private bool isAlwaysActive = false;
        [SerializeField] private AudioClip[] soundVariations;
        [SerializeField] private bool loopSound = false;

        public bool IsActive { get; set; } = false;
        public bool IsPlaying { get; private set; } = false;
        public bool IsAlwaysActive => isAlwaysActive;
        public bool IsStaticSlot => isStaticSlot;

        private AudioSource audioSource;
        private bool isRegistered = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = loopSound;

            if (soundVariations != null && soundVariations.Length > 0)
            {
                audioSource.clip = soundVariations[Random.Range(0, soundVariations.Length)];
            }

            if (isAlwaysActive)
            {
                IsActive = true;
            }

            RegisterWithManager();
        }

        private void Start()
        {
            if (isAlwaysActive)
            {
                IsActive = true;
            }

            RegisterWithManager();
        }

        private void OnEnable()
        {
            RegisterWithManager();
        }

        private void OnDisable()
        {
            if (MusicPuzzleManager.Instance != null)
            {
                MusicPuzzleManager.Instance.UnregisterSlot(this, isStaticSlot);
                isRegistered = false;
            }
        }

        private void RegisterWithManager()
        {
            if (isRegistered || MusicPuzzleManager.Instance == null)
            {
                return;
            }

            MusicPuzzleManager.Instance.RegisterSlot(this, isStaticSlot);
            isRegistered = true;
        }

        public bool TryPlayAssignedSound()
        {
            if (audioSource == null)
            {
                return false;
            }

            AudioClip clipToPlay = null;
            if (soundVariations != null && soundVariations.Length > 0)
            {
                clipToPlay = soundVariations[Random.Range(0, soundVariations.Length)];
                audioSource.clip = clipToPlay;
            }
            else if (audioSource.clip != null)
            {
                clipToPlay = audioSource.clip;
            }

            if (clipToPlay == null)
            {
                return false;
            }

            audioSource.Stop();
            audioSource.Play();
            return true;
        }

        public void HandleToggleState(bool turnedOn)
        {
            if (!IsActive)
            {
                return;
            }

            if (turnedOn)
            {
                IsPlaying = true;

                if (EffectManager.Instance != null && IsActive)
                {
                    EffectManager.Instance.SpawnNoteAt(transform.position);
                }

                TryPlayAssignedSound();
            }
            else
            {
                IsPlaying = false;

                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}

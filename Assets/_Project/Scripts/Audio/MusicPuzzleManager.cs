using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CozyHome.Audio
{
    public class MusicPuzzleManager : MonoBehaviour
    {
        public static MusicPuzzleManager Instance { get; private set; }

        [SerializeField] private AudioClip[] recordPlayerTracks;
        [SerializeField] private AudioClip[] propSoundClips;

        private readonly List<MusicInteractableSlot> interactiveSlots = new();
        private readonly List<MusicInteractableSlot> staticSlots = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            InitializeRound();
        }

        public void RegisterSlot(MusicInteractableSlot slot, bool isStatic)
        {
            if (slot == null)
            {
                return;
            }

            List<MusicInteractableSlot> targetList = isStatic ? staticSlots : interactiveSlots;

            if (!targetList.Contains(slot))
            {
                targetList.Add(slot);
            }
        }

        public void UnregisterSlot(MusicInteractableSlot slot, bool isStatic)
        {
            if (slot == null)
            {
                return;
            }

            List<MusicInteractableSlot> targetList = isStatic ? staticSlots : interactiveSlots;
            targetList.Remove(slot);
        }

        public void InitializeRound(int interactiveActiveTarget = 3, int staticActiveTarget = 4)
        {
            foreach (MusicInteractableSlot slot in interactiveSlots)
            {
                if (slot != null)
                {
                    slot.IsActive = true;
                }
            }

            foreach (MusicInteractableSlot slot in staticSlots)
            {
                if (slot != null && !slot.IsAlwaysActive)
                {
                    slot.IsActive = false;
                }
            }

            ActivateRandomSlots(staticSlots, staticActiveTarget);

            int activeCount = interactiveSlots.Count(slot => slot != null && slot.IsActive)
                + staticSlots.Count(slot => slot != null && slot.IsActive);

            Debug.Log($"MusicPuzzleManager: Active slots in this round: {activeCount}");
        }

        public AudioClip GetRandomRecordPlayerTrack()
        {
            if (recordPlayerTracks == null || recordPlayerTracks.Length == 0)
            {
                return null;
            }

            return recordPlayerTracks[Random.Range(0, recordPlayerTracks.Length)];
        }

        public AudioClip GetRandomPropSoundClip()
        {
            if (propSoundClips == null || propSoundClips.Length == 0)
            {
                return null;
            }

            return propSoundClips[Random.Range(0, propSoundClips.Length)];
        }

        public bool IsObjectActiveInCurrentRound(Component target)
        {
            if (target == null)
            {
                return false;
            }

            if (target.TryGetComponent(out MusicInteractableSlot slot))
            {
                return slot.IsActive || slot.IsAlwaysActive || !slot.IsStaticSlot;
            }

            return false;
        }

        public bool PlayInteractionSound(Component target)
        {
            if (target == null)
            {
                return false;
            }

            if (target.TryGetComponent(out MusicInteractableSlot slot))
            {
                return slot.TryPlayAssignedSound();
            }

            AudioSource audioSource = target.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return false;
            }

            if (audioSource.clip == null)
            {
                return false;
            }

            audioSource.Stop();
            audioSource.Play();
            return true;
        }

        private void ActivateRandomSlots(List<MusicInteractableSlot> slots, int targetCount)
        {
            if (slots == null || slots.Count == 0)
            {
                return;
            }

            List<MusicInteractableSlot> availableSlots = slots
                .Where(slot => slot != null && !slot.IsAlwaysActive)
                .ToList();

            if (availableSlots.Count == 0)
            {
                return;
            }

            int validTarget = Mathf.Clamp(targetCount, 0, availableSlots.Count);
            var shuffledSlots = availableSlots.OrderBy(_ => Random.value).ToList();

            for (int i = 0; i < validTarget; i++)
            {
                if (i < shuffledSlots.Count)
                {
                    shuffledSlots[i].IsActive = true;
                }
            }
        }
    }
}

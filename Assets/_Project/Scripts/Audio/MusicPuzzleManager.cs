using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CozyHome.Interaction;

namespace CozyHome.Audio
{
    public class MusicPuzzleManager : MonoBehaviour
    {
        public static MusicPuzzleManager Instance { get; private set; }

        [SerializeField] private AudioClip[] recordPlayerTracks;
        [SerializeField] private AudioClip[] propSoundClips;

        private readonly List<RoomItem> roomItems = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DiscoverRoomItems();
        }

        private void Start()
        {
            DiscoverRoomItems();
            InitializeRound();
        }

        public void RegisterRoomItem(RoomItem item)
        {
            if (item == null || roomItems.Contains(item))
            {
                return;
            }

            roomItems.Add(item);
        }

        private void DiscoverRoomItems()
        {
            RoomItem[] discoveredItems = FindObjectsByType<RoomItem>(FindObjectsSortMode.None);
            if (discoveredItems == null || discoveredItems.Length == 0)
            {
                return;
            }

            foreach (RoomItem item in discoveredItems)
            {
                RegisterRoomItem(item);
            }
        }

        public void UnregisterRoomItem(RoomItem item)
        {
            if (item == null)
            {
                return;
            }

            roomItems.Remove(item);
        }

        public void InitializeRound(int staticActiveTarget = 4)
        {
            if (roomItems.Count == 0)
            {
                return;
            }

            List<RoomItem> candidatesForRound = roomItems
                .Where(item => item != null && item.CanPlaySound && !item.IsGuaranteedSound)
                .ToList();

            foreach (RoomItem item in roomItems)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.IsGuaranteedSound)
                {
                    item.SetActiveForAudio(true);
                }
                else
                {
                    item.SetActiveForAudio(false);
                }
            }

            int validTarget = Mathf.Clamp(staticActiveTarget, 0, candidatesForRound.Count);
            var shuffledItems = candidatesForRound.OrderBy(_ => Random.value).ToList();

            for (int i = 0; i < validTarget; i++)
            {
                if (i < shuffledItems.Count)
                {
                    shuffledItems[i].SetActiveForAudio(true);
                }
            }

            int activeCount = roomItems.Count(item => item != null && item.CanPlaySound && item.IsActiveForAudio);
            Debug.Log($"MusicPuzzleManager: Active sound items in this round: {activeCount}");
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

        public bool IsItemActive(RoomItem item)
        {
            if (item == null)
            {
                return false;
            }

            return item.CanPlaySound && item.IsActiveForAudio;
        }

        public bool PlayInteractionSound(RoomItem item)
        {
            if (item == null)
            {
                return false;
            }

            return item.TryPlayAssignedSound();
        }
    }
}

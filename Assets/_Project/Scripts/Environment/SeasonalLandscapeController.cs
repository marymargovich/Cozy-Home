using System;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.Environment
{
    /// <summary>
    /// Updates a landscape background image to match the current season.
    /// </summary>
    [ExecuteAlways]
    public class SeasonalLandscapeController : MonoBehaviour
    {
        // The UI image that displays the season-specific landscape background.
        [SerializeField] private Image targetImage;

        // Background sprite used when the system date indicates spring.
        [SerializeField] private Sprite springSprite;

        // Background sprite used when the system date indicates summer.
        [SerializeField] private Sprite summerSprite;

        // Background sprite used when the system date indicates autumn.
        [SerializeField] private Sprite autumnSprite;

        // Background sprite used when the system date indicates winter.
        [SerializeField] private Sprite winterSprite;

        [Header("Room Decor Slots")]
        [SerializeField] private SeasonalSpriteSlot rugSlot;
        [SerializeField] private SeasonalSpriteSlot vaseSlot;
        [SerializeField] private SeasonalSpriteSlot plaidSlot;

        [Header("Debug / Testing")]
        // Enables the debug override so the designer can preview a season without waiting for the system date.
        [SerializeField] private bool overrideSystemSeason = false;

        // The season to preview when the override is enabled.
        [SerializeField] private Season debugSeason;

        [System.Serializable]
        public class SeasonalSpriteSlot
        {
            public Image targetImage;
            public Sprite springSprite;
            public Sprite summerSprite;
            public Sprite autumnSprite;
            public Sprite winterSprite;

            public void Apply(Season season)
            {
                if (targetImage == null)
                {
                    return;
                }

                Sprite spriteToApply = season switch
                {
                    Season.Spring => springSprite,
                    Season.Summer => summerSprite,
                    Season.Autumn => autumnSprite,
                    Season.Winter => winterSprite,
                    _ => null
                };

                if (spriteToApply != null)
                {
                    targetImage.gameObject.SetActive(true);
                    targetImage.sprite = spriteToApply;
                }
                else
                {
                    targetImage.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Represents the four standard seasonal periods used for the landscape artwork.
        /// </summary>
        public enum Season
        {
            Spring,
            Summer,
            Autumn,
            Winter
        }

        private void Awake()
        {
            CacheTargetImage();
            Season initialSeason = overrideSystemSeason ? debugSeason : GetCurrentSeason();
            ApplySeason(initialSeason);
        }

        private void OnValidate()
        {
            CacheTargetImage();

            // Use the debug season in the editor for instant visual testing when the override is active.
            if (overrideSystemSeason)
            {
                ApplySeason(debugSeason);
                return;
            }

            ApplySeason(GetCurrentSeason());
        }

        /// <summary>
        /// Finds the Image component on this GameObject if one has not been assigned in the Inspector.
        /// </summary>
        private void CacheTargetImage()
        {
            if (targetImage != null)
            {
                return;
            }

            targetImage = GetComponent<Image>();

            if (targetImage == null)
            {
                Debug.LogWarning($"{nameof(SeasonalLandscapeController)} requires an Image component on the same GameObject or a reference in the Inspector.", this);
            }
        }

        /// <summary>
        /// Returns the season based on the current local system month.
        /// </summary>
        /// <returns>The calculated season for the current date.</returns>
        public Season GetCurrentSeason()
        {
            int currentMonth = DateTime.Now.Month;

            if (currentMonth >= 3 && currentMonth <= 5)
            {
                return Season.Spring;
            }

            if (currentMonth >= 6 && currentMonth <= 8)
            {
                return Season.Summer;
            }

            if (currentMonth >= 9 && currentMonth <= 11)
            {
                return Season.Autumn;
            }

            return Season.Winter;
        }

        /// <summary>
        /// Applies the sprite associated with the provided season to the target UI image.
        /// </summary>
        /// <param name="season">The season whose sprite should be displayed.</param>
        public void ApplySeason(Season season)
        {
            if (targetImage == null)
            {
                CacheTargetImage();

                if (targetImage == null)
                {
                    return;
                }
            }

            Sprite spriteToApply = season switch
            {
                Season.Spring => springSprite,
                Season.Summer => summerSprite,
                Season.Autumn => autumnSprite,
                Season.Winter => winterSprite,
                _ => null
            };

            if (spriteToApply == null)
            {
                Debug.LogWarning($"No sprite is assigned for season '{season}' on {nameof(SeasonalLandscapeController)}.", this);
                return;
            }

            if (targetImage.sprite != spriteToApply)
            {
                targetImage.sprite = spriteToApply;
            }

            rugSlot?.Apply(season);
            vaseSlot?.Apply(season);
            plaidSlot?.Apply(season);
        }
    }
}

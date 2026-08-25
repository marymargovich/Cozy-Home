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
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite springSprite;
        [SerializeField] private Sprite summerSprite;
        [SerializeField] private Sprite autumnSprite;
        [SerializeField] private Sprite winterSprite;

        [System.Serializable]
        public class SeasonalSlotData
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
                    return;
                }

                targetImage.sprite = null;
                targetImage.gameObject.SetActive(false);
            }
        }

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
            ApplySeason(GetCurrentSeason());
        }

        private void OnValidate()
        {
            CacheTargetImage();
            ApplySeason(GetCurrentSeason());
        }

        private void CacheTargetImage()
        {
            if (targetImage != null)
            {
                return;
            }

            targetImage = GetComponent<Image>();
        }

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

            if (spriteToApply != null)
            {
                targetImage.sprite = spriteToApply;
            }
        }
    }
}

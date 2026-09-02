using UnityEngine;

namespace CozyHome.Weather
{
    /// <summary>
    /// Scrolls a repeating UI weather layer using two copies of the same image.
    /// </summary>
    [DisallowMultipleComponent]
    public class WeatherLayerScroller : MonoBehaviour
    {
        [Header("Layer pair")]
        [SerializeField] private RectTransform imageA;
        [SerializeField] private RectTransform imageB;

        [Header("Movement")]
        [SerializeField] private Vector2 direction = new Vector2(0.25f, -1f);
        [SerializeField] private float speed = 8f;

        private RectTransform weatherBounds;
        private Vector2 normalizedDirection;
        private float elapsedDistance;
        private Vector2 initialAAnchor;
        private Vector2 initialBAnchor;

        private void Reset()
        {
            ResolveReferences();
            if (imageA == null)
            {
                imageA = GetComponent<RectTransform>();
            }

            if (imageB == null && imageA != null && imageA.parent != null)
            {
                int siblingIndex = imageA.GetSiblingIndex() + 1;
                if (siblingIndex < imageA.parent.childCount)
                {
                    Transform nextSibling = imageA.parent.GetChild(siblingIndex);
                    if (nextSibling != null)
                    {
                        imageB = nextSibling as RectTransform;
                    }
                }
            }

            direction = new Vector2(0.25f, -1f);
            speed = 8f;
        }

        private void Awake()
        {
            ResolveReferences();
            ConfigureMovement();
            ResetPosition();
        }

        private void OnEnable()
        {
            ResolveReferences();
            ConfigureMovement();
            ResetPosition();
        }

        private void Update()
        {
            if (imageA == null || imageB == null)
            {
                return;
            }

            elapsedDistance += speed * Time.deltaTime;
            UpdateLayerPositions();
        }

        public void SetLayerPair(RectTransform a, RectTransform b)
        {
            imageA = a;
            imageB = b;
            ResolveReferences();
            ResetPosition();
        }

        public void SetMovement(Vector2 newDirection, float newSpeed)
        {
            direction = newDirection;
            speed = newSpeed;
            ConfigureMovement();
            ResetPosition();
        }

        public void Play()
        {
            if (imageA != null)
            {
                imageA.gameObject.SetActive(true);
            }

            if (imageB != null)
            {
                imageB.gameObject.SetActive(true);
            }

            enabled = true;
            ResetPosition();
        }

        public void Stop()
        {
            enabled = false;

            if (imageA != null)
            {
                imageA.anchoredPosition = Vector2.zero;
                imageA.gameObject.SetActive(false);
            }

            if (imageB != null)
            {
                imageB.anchoredPosition = Vector2.zero;
                imageB.gameObject.SetActive(false);
            }
        }

        public void ResetPosition()
        {
            if (imageA == null || imageB == null || weatherBounds == null)
            {
                return;
            }

            elapsedDistance = 0f;
            ApplyFixedLayerAnchors();
            UpdateLayerPositions();
        }

        private void ResolveReferences()
        {
            if (imageA == null)
            {
                imageA = GetComponent<RectTransform>();
            }

            if (imageB == null && imageA != null && imageA.parent != null)
            {
                for (int i = 0; i < imageA.parent.childCount; i++)
                {
                    RectTransform candidate = imageA.parent.GetChild(i) as RectTransform;
                    if (candidate != null && candidate != imageA)
                    {
                        imageB = candidate;
                        break;
                    }
                }
            }

            if (imageA != null)
            {
                weatherBounds = imageA.parent as RectTransform;
            }
            else if (imageB != null)
            {
                weatherBounds = imageB.parent as RectTransform;
            }
        }

        private void ConfigureMovement()
        {
            if (direction.sqrMagnitude < 0.0001f)
            {
                normalizedDirection = Vector2.down;
            }
            else
            {
                normalizedDirection = direction.normalized;
            }
        }

        private void ApplyFixedLayerAnchors()
        {
            initialAAnchor = Vector2.zero;
            initialBAnchor = -GetTileOffset();

            imageA.anchoredPosition = initialAAnchor;
            imageB.anchoredPosition = initialBAnchor;
        }

        private void UpdateLayerPositions()
        {
            if (imageA == null || imageB == null)
            {
                return;
            }

            float cycleLength = GetRepeatCycleLength();
            if (cycleLength <= Mathf.Epsilon)
            {
                imageA.anchoredPosition = initialAAnchor;
                imageB.anchoredPosition = initialBAnchor;
                return;
            }

            float wrappedDistance = Mathf.Repeat(elapsedDistance, cycleLength);
            Vector2 offset = normalizedDirection * wrappedDistance;

            imageA.anchoredPosition = initialAAnchor + offset;
            imageB.anchoredPosition = initialBAnchor + offset;
        }

        private float GetRepeatCycleLength()
        {
            if (weatherBounds == null)
            {
                return 0f;
            }

            float absX = Mathf.Abs(normalizedDirection.x);
            float absY = Mathf.Abs(normalizedDirection.y);

            float tileWidth = Mathf.Max(weatherBounds.rect.width, 1f);
            float tileHeight = Mathf.Max(weatherBounds.rect.height, 1f);

            if (absY >= absX)
            {
                return tileHeight / Mathf.Max(absY, 0.0001f);
            }

            return tileWidth / Mathf.Max(absX, 0.0001f);
        }

        private Vector2 GetTileOffset()
        {
            float cycleLength = GetRepeatCycleLength();
            if (cycleLength <= Mathf.Epsilon)
            {
                return Vector2.zero;
            }

            return normalizedDirection * cycleLength;
        }
    }
}

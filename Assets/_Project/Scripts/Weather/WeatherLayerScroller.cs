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
            InitializeLayerPair();
        }

        private void OnEnable()
        {
            ResolveReferences();
            ConfigureMovement();
            InitializeLayerPair();
        }

        private void Update()
        {
            if (imageA == null || imageB == null)
            {
                return;
            }

            Vector2 delta = normalizedDirection * speed * Time.deltaTime;
            imageA.anchoredPosition += delta;
            imageB.anchoredPosition += delta;

            WrapImage(imageA);
            WrapImage(imageB);
        }

        public void SetLayerPair(RectTransform a, RectTransform b)
        {
            imageA = a;
            imageB = b;
            ResolveReferences();
            InitializeLayerPair();
        }

        public void SetMovement(Vector2 newDirection, float newSpeed)
        {
            direction = newDirection;
            speed = newSpeed;
            ConfigureMovement();
            InitializeLayerPair();
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
            InitializeLayerPair();
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

        private void InitializeLayerPair()
        {
            if (imageA == null || imageB == null || weatherBounds == null)
            {
                return;
            }

            imageA.anchoredPosition = Vector2.zero;
            imageB.anchoredPosition = -GetTileOffset();
        }

        private Vector2 GetTileOffset()
        {
            Vector2 dir = normalizedDirection;
            float absX = Mathf.Abs(dir.x);
            float absY = Mathf.Abs(dir.y);

            float tileWidth = Mathf.Max(weatherBounds.rect.width, 1f);
            float tileHeight = Mathf.Max(weatherBounds.rect.height, 1f);

            float travelDistance;
            if (absY >= absX)
            {
                travelDistance = tileHeight / Mathf.Max(absY, 0.0001f);
            }
            else
            {
                travelDistance = tileWidth / Mathf.Max(absX, 0.0001f);
            }

            Vector2 offset = dir * travelDistance;
            return offset;
        }

        private void WrapImage(RectTransform rect)
        {
            if (rect == null || weatherBounds == null)
            {
                return;
            }

            Vector2 size = weatherBounds.rect.size;
            Vector2 position = rect.anchoredPosition;

            if (normalizedDirection.x > 0f && position.x > size.x)
            {
                position.x -= size.x * 2f;
            }
            else if (normalizedDirection.x < 0f && position.x < -size.x)
            {
                position.x += size.x * 2f;
            }

            if (normalizedDirection.y > 0f && position.y > size.y)
            {
                position.y -= size.y * 2f;
            }
            else if (normalizedDirection.y < 0f && position.y < -size.y)
            {
                position.y += size.y * 2f;
            }

            rect.anchoredPosition = position;
        }
    }
}

using UnityEngine;

namespace CozyHome.Weather
{
    /// <summary>
    /// Controls the three layered rain scrollers for a light-rain visual state.
    /// </summary>
    [DisallowMultipleComponent]
    public class WeatherRainController : MonoBehaviour
    {
        [Header("Rain layers")]
        [SerializeField] private WeatherLayerScroller farLayer;
        [SerializeField] private WeatherLayerScroller midLayer;
        [SerializeField] private WeatherLayerScroller nearLayer;

        [Header("Light rain defaults")]
        [SerializeField] private Vector2 farDirection = new Vector2(0.25f, -1f);
        [SerializeField] private float farSpeed = 12f;

        [SerializeField] private Vector2 midDirection = new Vector2(0.25f, -1f);
        [SerializeField] private float midSpeed = 20f;

        [SerializeField] private Vector2 nearDirection = new Vector2(0.25f, -1f);
        [SerializeField] private float nearSpeed = 24f;

        private WeatherLayerScroller[] rainLayers;

        private void Awake()
        {
            CacheLayers();
        }

        private void Start()
        {
            CacheLayers();
            PlayLightRain();
        }

        private void OnEnable()
        {
            CacheLayers();
        }

        public void PlayLightRain()
        {
            CacheLayers();

            if (farLayer != null)
            {
                farLayer.SetMovement(farDirection, farSpeed);
                farLayer.Play();
            }

            if (midLayer != null)
            {
                midLayer.SetMovement(midDirection, midSpeed);
                midLayer.Play();
            }

            if (nearLayer != null)
            {
                nearLayer.SetMovement(nearDirection, nearSpeed);
                nearLayer.Play();
            }
        }

        public void StopRain()
        {
            CacheLayers();

            if (farLayer != null)
            {
                farLayer.Stop();
            }

            if (midLayer != null)
            {
                midLayer.Stop();
            }

            if (nearLayer != null)
            {
                nearLayer.Stop();
            }
        }

        private void CacheLayers()
        {
            rainLayers = new[]
            {
                farLayer,
                midLayer,
                nearLayer,
            };
        }
    }
}

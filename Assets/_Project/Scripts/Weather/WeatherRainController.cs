using UnityEngine;

namespace CozyHome.Weather
{
    // Legacy compatibility wrapper: older code can still call the public methods, but the active weather flow is WeatherController -> WeatherVisualGroup -> WeatherLayerScroller.
    [System.Obsolete("Use WeatherController + WeatherVisualGroup instead.")]
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

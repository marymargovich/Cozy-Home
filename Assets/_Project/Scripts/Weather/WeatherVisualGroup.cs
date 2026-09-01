using System.Collections.Generic;
using UnityEngine;

namespace CozyHome.Weather
{
    /// <summary>
    /// Represents a single visual weather state and the objects it owns.
    /// </summary>
    public class WeatherVisualGroup : MonoBehaviour
    {
        [SerializeField] private WeatherType weatherType;
        [SerializeField] private GameObject rootObject;
        [SerializeField] private List<WeatherLayerScroller> scrollingLayers = new List<WeatherLayerScroller>();
        [SerializeField] private List<GameObject> staticVisuals = new List<GameObject>();

        public WeatherType WeatherType => weatherType;

        public void Play()
        {
            if (rootObject != null)
            {
                rootObject.SetActive(true);
            }

            if (staticVisuals != null)
            {
                for (int i = 0; i < staticVisuals.Count; i++)
                {
                    GameObject visual = staticVisuals[i];
                    if (visual != null)
                    {
                        visual.SetActive(true);
                    }
                }
            }

            if (scrollingLayers != null)
            {
                for (int i = 0; i < scrollingLayers.Count; i++)
                {
                    WeatherLayerScroller layer = scrollingLayers[i];
                    if (layer != null)
                    {
                        layer.Play();
                    }
                }
            }
        }

        public void Stop()
        {
            if (scrollingLayers != null)
            {
                for (int i = 0; i < scrollingLayers.Count; i++)
                {
                    WeatherLayerScroller layer = scrollingLayers[i];
                    if (layer != null)
                    {
                        layer.Stop();
                    }
                }
            }

            if (staticVisuals != null)
            {
                for (int i = 0; i < staticVisuals.Count; i++)
                {
                    GameObject visual = staticVisuals[i];
                    if (visual != null)
                    {
                        visual.SetActive(false);
                    }
                }
            }

            if (rootObject != null)
            {
                rootObject.SetActive(false);
            }
        }
    }
}

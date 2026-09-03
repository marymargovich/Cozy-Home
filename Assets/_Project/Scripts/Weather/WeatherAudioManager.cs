using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CozyHome.Weather
{
    /// <summary>
    /// Manages the active weather ambient loop for the current weather state.
    /// Playback is intentionally direct and immediate: no coroutines, no crossfades, and no delayed transitions.
    /// </summary>
    public class WeatherAudioManager : MonoBehaviour
    {
        [Serializable]
        private sealed class WeatherSoundPool
        {
            [SerializeField] private WeatherType weatherType;
            [SerializeField] private AudioClip[] clips = Array.Empty<AudioClip>();
            [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
            [SerializeField] [Range(0.1f, 2f)] private float pitch = 1f;
            [SerializeField] private bool loop = true;

            public WeatherType WeatherType => weatherType;
            public AudioClip[] Clips => clips;
            public float Volume => volume;
            public float Pitch => pitch;
            public bool Loop => loop;
        }

        [Header("Weather audio")]
        [SerializeField] private AudioSource weatherSource;
        [SerializeField] private AudioMixerGroup outputGroup;

        [Header("Weather audio pools")]
        [SerializeField] private List<WeatherSoundPool> weatherPools = new List<WeatherSoundPool>();

        private WeatherType currentWeather = WeatherType.None;

        public WeatherType CurrentWeather => currentWeather;

        private void Awake()
        {
            EnsureAudioSource();
        }

        private void OnValidate()
        {
            EnsureAudioSource();
        }

        public void SetWeather(WeatherType weatherType)
        {
            if (weatherType == WeatherType.None || weatherType == WeatherType.Clear)
            {
                StopWeather();
                return;
            }

            EnsureAudioSource();

            WeatherSoundPool pool = GetSoundPool(weatherType);
            if (pool == null || pool.Clips == null || pool.Clips.Length == 0)
            {
                StopWeather();
                return;
            }

            AudioClip selectedClip = GetRandomClip(pool.Clips);
            if (selectedClip == null)
            {
                StopWeather();
                return;
            }

            currentWeather = weatherType;
            weatherSource.Stop();
            weatherSource.clip = selectedClip;
            weatherSource.loop = pool.Loop;
            weatherSource.pitch = pool.Pitch;
            weatherSource.volume = pool.Volume;
            weatherSource.outputAudioMixerGroup = outputGroup;
            weatherSource.spatialBlend = 0f;
            weatherSource.Play();
        }

        public void StopWeather()
        {
            currentWeather = WeatherType.None;

            if (weatherSource == null)
            {
                return;
            }

            weatherSource.Stop();
            weatherSource.clip = null;
        }

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            return clips[UnityEngine.Random.Range(0, clips.Length)];
        }

        private WeatherSoundPool GetSoundPool(WeatherType weatherType)
        {
            if (weatherPools == null)
            {
                return null;
            }

            for (int i = 0; i < weatherPools.Count; i++)
            {
                WeatherSoundPool pool = weatherPools[i];
                if (pool != null && pool.WeatherType == weatherType)
                {
                    return pool;
                }
            }

            return null;
        }

        private void EnsureAudioSource()
        {
            if (weatherSource == null)
            {
                GameObject sourceObject = new GameObject("WeatherAudio");
                sourceObject.transform.SetParent(transform, false);
                weatherSource = sourceObject.AddComponent<AudioSource>();
            }

            if (weatherSource == null)
            {
                return;
            }

            weatherSource.playOnAwake = false;
            weatherSource.loop = true;
            weatherSource.spatialBlend = 0f;
            weatherSource.outputAudioMixerGroup = outputGroup;
        }
    }
}

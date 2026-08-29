using System;
using UnityEngine;

namespace CozyHome.UI
{
    public enum AppLanguage
    {
        RU,
        EN,
        HE
    }

    public class LanguageManager : MonoBehaviour
    {
        private const string LanguagePrefsKey = "AppLanguage";

        public static LanguageManager Instance { get; private set; }

        public event Action<AppLanguage> OnLanguageChanged;

        public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.RU;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject languageManagerObject = new GameObject("LanguageManager");
            languageManagerObject.AddComponent<LanguageManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            CurrentLanguage = LoadSavedLanguage();
        }

        public void SetLanguage(AppLanguage language)
        {
            CurrentLanguage = language;
            PlayerPrefs.SetInt(LanguagePrefsKey, (int)language);
            PlayerPrefs.Save();
            OnLanguageChanged?.Invoke(language);
        }

        private static AppLanguage LoadSavedLanguage()
        {
            if (!PlayerPrefs.HasKey(LanguagePrefsKey))
            {
                return AppLanguage.RU;
            }

            int savedValue = PlayerPrefs.GetInt(LanguagePrefsKey, (int)AppLanguage.RU);
            return Enum.IsDefined(typeof(AppLanguage), savedValue)
                ? (AppLanguage)savedValue
                : AppLanguage.RU;
        }
    }
}

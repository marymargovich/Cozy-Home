using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CozyHome.Core
{
    /// <summary>
    /// Handles the splash screen entry interaction and loads the next scene.
    /// </summary>
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "MainRoomScene";
        [SerializeField] private Button enterButton;

        private bool isTransitioning;

        private void Awake()
        {
            RegisterButtonListener();
        }

        private void OnDestroy()
        {
            if (enterButton != null)
            {
                enterButton.onClick.RemoveListener(OnScreenClicked);
            }
        }

        private void RegisterButtonListener()
        {
            if (enterButton != null)
            {
                enterButton.onClick.AddListener(OnScreenClicked);
            }
        }

        public void OnScreenClicked()
        {
            // Ignore repeated clicks while the scene transition is already in progress.
            if (isTransitioning)
            {
                return;
            }

            // Prevent extra triggers from fast repeated presses.
            isTransitioning = true;

            // Load the configured scene.
            SceneManager.LoadScene(nextSceneName);
        }
    }
}

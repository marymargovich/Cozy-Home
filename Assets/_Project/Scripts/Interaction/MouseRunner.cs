using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CozyHome.Interaction
{
    public class MouseRunner : MonoBehaviour
    {
        [SerializeField] private GameObject mouseObject;
        [SerializeField] private RectTransform mouseRect;
        [SerializeField] private RectTransform startPoint;
        [SerializeField] private RectTransform endPoint;
        [SerializeField] private float moveSpeed = 600f;

        private Coroutine runRoutine;
        private bool isMoving;

        private void Awake()
        {
            InitializeMouse();
        }

        private void OnDisable()
        {
            if (runRoutine != null)
            {
                StopCoroutine(runRoutine);
                runRoutine = null;
            }

            isMoving = false;

            if (mouseObject != null)
            {
                mouseObject.SetActive(false);
            }
        }

        private void InitializeMouse()
        {
            if (mouseObject != null)
            {
                mouseObject.SetActive(false);
            }

            if (mouseRect != null && startPoint != null)
            {
                mouseRect.anchoredPosition = startPoint.anchoredPosition;
            }
        }

        public void TriggerRun()
        {
            if (isMoving)
            {
                return;
            }

            if (mouseObject == null || mouseRect == null || startPoint == null || endPoint == null)
            {
                Debug.LogWarning($"{nameof(MouseRunner)} has missing references.", this);
                return;
            }

            isMoving = true;
            mouseObject.SetActive(true);
            mouseRect.anchoredPosition = startPoint.anchoredPosition;

            if (runRoutine != null)
            {
                StopCoroutine(runRoutine);
            }

            runRoutine = StartCoroutine(RunToDestination());
        }

        private IEnumerator RunToDestination()
        {
            while (Vector2.Distance(mouseRect.anchoredPosition, endPoint.anchoredPosition) > 0.01f)
            {
                float step = moveSpeed * Time.deltaTime;
                mouseRect.anchoredPosition = Vector2.MoveTowards(mouseRect.anchoredPosition, endPoint.anchoredPosition, step);
                yield return null;
            }

            mouseRect.anchoredPosition = endPoint.anchoredPosition;

            if (mouseObject != null)
            {
                mouseObject.SetActive(false);
            }

            isMoving = false;
            runRoutine = null;
        }
    }
}

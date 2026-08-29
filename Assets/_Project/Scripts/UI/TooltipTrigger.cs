using UnityEngine;
using UnityEngine.EventSystems;

namespace CozyHome.UI
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ControlBarTooltip tooltip;
        [SerializeField] private ControlTooltipType tooltipType;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip == null)
            {
                return;
            }

            RectTransform sourceRect = GetComponent<RectTransform>();
            tooltip.Show(tooltipType, sourceRect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip == null)
            {
                return;
            }

            tooltip.Hide();
        }
    }
}

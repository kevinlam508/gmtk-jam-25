using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public string header;
    
    [TextArea(1, 10)]
    [SerializeField] public string content;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.instance.ShowTooltip(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.instance.HideTooltip();
    }
}

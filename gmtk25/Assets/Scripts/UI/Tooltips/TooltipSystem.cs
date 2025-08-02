using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{

    public static TooltipSystem instance;
    [SerializeField] private TooltipSetter tooltip;

    public void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        
    }

    public void ShowTooltip(string content, string header = "")
    {
        tooltip.SetText(content, header);
        tooltip.ShowTooltip();
    }

    public void HideTooltip()
    {
        tooltip.HideTooltip();
        //In theory, we should also disable the tooltip if the gameobject the tooltip is assigned to gets disabled
    }



}

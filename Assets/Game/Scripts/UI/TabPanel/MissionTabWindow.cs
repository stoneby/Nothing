using UnityEngine;
using System.Collections;

public class MissionTabWindow : TabPanelBase 
{
    public override void OnToggle()
    {
        if (CurItem.ItemIndex < 4)
        {
            MissionModelLocator.Instance.RaidType = CurrIndex;
        }
    }
}




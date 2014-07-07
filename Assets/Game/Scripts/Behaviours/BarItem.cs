using UnityEngine;
using System.Collections;

public class BarItem : MonoBehaviour
{
    public UILabel NameLabel;

    private string BarItemName
    {
        set
        {
            if(NameLabel != null)
            {
                NameLabel.text = value;
            }
        }
    }

    public virtual void Init(string itemName)
    {
        BarItemName = itemName;
    }

    public virtual void CleanUp()
    {
        BarItemName = "";
    }
}

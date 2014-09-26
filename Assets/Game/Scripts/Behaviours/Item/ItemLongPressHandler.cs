using UnityEngine;

[RequireComponent(typeof(NGUILongPress))]
public class ItemLongPressHandler : MonoBehaviour 
{
    private GameObject cloneDetail;
    public GameObject DetailInfoObj;
    public bool UseTemplateId = false;

    public NGUILongPress InstallLongPress()
    {
        var longPress = gameObject.GetComponent<NGUILongPress>();
        if (longPress)
        {
            longPress.OnLongPress = OnLongPress;
            longPress.OnLongPressFinish = OnLongPressFinish;
        }
        return longPress;
    }

    private void OnLongPress(GameObject go)
    {
        cloneDetail = NGUITools.AddChild(gameObject, DetailInfoObj);
        var windowShower = cloneDetail.GetComponent<FloatWindowShower>();
        windowShower.Show();
        var heroDetailInfo = cloneDetail.GetComponent<ItemDetailControl>();
        var itemBase = go.GetComponent<ItemBase>();
        if(UseTemplateId)
        {
            heroDetailInfo.Refresh(itemBase.TempId);
        }
        else
        {
            heroDetailInfo.Refresh(itemBase.TheItemInfo);
        }     
    }

    private void OnLongPressFinish(GameObject go)
    {
        NGUITools.Destroy(cloneDetail);
        cloneDetail = null;
    }

}

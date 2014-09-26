using UnityEngine;

[RequireComponent(typeof(NGUILongPress))]
public class HeroLongPressHandler : MonoBehaviour 
{
    private GameObject cloneDetail;
    public GameObject HeroDetailInfo;

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
        cloneDetail = NGUITools.AddChild(gameObject, HeroDetailInfo);
        var windowShower = cloneDetail.GetComponent<FloatWindowShower>();
        windowShower.Show();
        var heroDetailInfo = cloneDetail.GetComponent<HeroDetailInfoControl>();
        heroDetailInfo.Refresh(go.GetComponent<HeroItemBase>().HeroInfo);
    }

    private void OnLongPressFinish(GameObject go)
    {
        NGUITools.Destroy(cloneDetail);
        cloneDetail = null;
    }
}

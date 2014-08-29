using UnityEngine;

public class HeroEquipControl : MonoBehaviour
{
    private GameObject spriteBg;
    private GameObject spriteIcon;
    public int PosIndex;

    public bool IsLocked = false;
    public int TemplateId = 0;
    public string Uuid = "";
    private UIEventListener.VoidDelegate normalClicked;
	// Use this for initialization
	void Start ()
	{
        spriteBg = transform.FindChild("Sprite bg").gameObject;
        spriteIcon = transform.FindChild("Sprite icon").gameObject;

        var lis = UIEventListener.Get(gameObject);
        lis.onClick = OnClickHandler;
	    SetContent();
	}
	
    public void SetData(string uuid, int tempid)
    {
        Uuid = uuid;
        TemplateId = tempid;
        SetContent();
    }

    private void SetContent()
    {
        if (spriteBg == null) return;

        var sp = spriteBg.GetComponent<UISprite>();
        sp.spriteName = IsLocked ? "Lock" : "ExtendF";
        if (TemplateId > 0)
        {
            spriteIcon.SetActive(true);
            var icon = spriteIcon.GetComponent<UISprite>();
            icon.spriteName = ItemType.ItemHeadPrefix + ItemModeLocator.Instance.GetIconId(TemplateId);
        }
        else
        {
            spriteIcon.SetActive(false);
        }
    }

    public UIEventListener.VoidDelegate ClickedHandler
    {
        get { return normalClicked; }
        set
        {
            normalClicked = value;
        }
    }

    private void OnClickHandler(GameObject obj)
    {
        if (IsLocked) return;
        if (normalClicked != null)
        {
            normalClicked(gameObject);
        }
    }
}

using UnityEngine;
using System.Collections;

public class HeroEquipControl : MonoBehaviour
{
    private GameObject SpriteBg;
    private GameObject SpriteIcon;
    public int PosIndex;

    public bool IsLocked = false;
    public int TemplateId = 0;
    public string Uuid = "";
    public int SelectTemplateId = 0;
    public string Selectuuid = "";
    private UIEventListener.VoidDelegate normalClicked;
	// Use this for initialization
	void Start ()
	{
        SpriteBg = transform.FindChild("Sprite bg").gameObject;
        SpriteIcon = transform.FindChild("Sprite icon").gameObject;

        var lis = UIEventListener.Get(gameObject);
        lis.onClick = OnClickHandler;
	    SetContent();
	}
	
    public void SetData(string uuid, int tempid)
    {
        Uuid = uuid;
        TemplateId = tempid;
        Selectuuid = "";
        SelectTemplateId = 0;
        SetContent();
    }

    public void SetSelectData(string uuid, int tempid)
    {
        Selectuuid = uuid;
        SelectTemplateId = tempid;
        SetContent();
    }

    private void SetContent()
    {
        if (SpriteBg == null) return;

        var sp = SpriteBg.GetComponent<UISprite>();
        sp.spriteName = IsLocked ? "Lock" : "ExtendF";
        if (TemplateId > 0 || SelectTemplateId > 0)
        {
            SpriteIcon.SetActive(true);
            var icon = SpriteIcon.GetComponent<UISprite>();
            icon.spriteName = (SelectTemplateId > 0) ? "Item_" + ItemModeLocator.Instance.GetIconId(SelectTemplateId) : 
                "Item_" + ItemModeLocator.Instance.GetIconId(TemplateId);
        }
        else
        {
            SpriteIcon.SetActive(false);
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

using KXSGCodec;
using Template.Auto.Raid;
using UnityEngine;
using System.Collections;

public class RaidBigItemControl : KxItemRender
{

    public MapVO map;

    public bool IsChapter = true;

    private GameObject MapImgTex;
    private GameObject MapTitleLb;
    private GameObject MapLockSp;

    private GameObject MapTitleTex;

    private GameObject MapEventContainer;
    private GameObject MapEventSprite;
    private GameObject MapEventLabel;

    private GameObject MapStarContainer;
    private GameObject MapStarLabel;


    private bool HaveInit = false;
    public bool IsLock = true;

    public int RaidIndex;
    public RaidTemplate RaidTemp;
    public RaidInfo RaidData;
    public RaidAddtionInfo RaidAdditionData;
	// Use this for initialization
	void Start () 
    {
        MapImgTex = transform.FindChild("Texture map").gameObject;
        MapTitleLb = transform.FindChild("Label name").gameObject;
        MapLockSp = transform.FindChild("Sprite lock").gameObject;
	    MapTitleTex = transform.FindChild("Texture chapter title").gameObject;
	    MapEventContainer = transform.FindChild("Event Container").gameObject;
        MapEventSprite = transform.FindChild("Event Container/Event Sprite").gameObject;
        MapEventLabel = transform.FindChild("Event Container/Event Label").gameObject;
        MapStarContainer = transform.FindChild("Star Container").gameObject;
        MapStarLabel = transform.FindChild("Star Container/Process Label").gameObject;

        MapEventContainer.SetActive(false);
        MapStarContainer.SetActive(false);
        MapTitleLb.SetActive(false);
        MapTitleTex.SetActive(false);

	    HaveInit = true;
        OnHovered = HoverHandler;
        InitItem(true);
	    SetContent();
    }

    public void SetLock(bool islock)
    {
        IsLock = islock;
        //IsLock = false;
        
        SetContent();
       // var tt = MapImgTex.GetComponent<UITexture>();
        
    }

    public override void SetData<T>(T data)
    {
        map = data as MapVO;
        if (!IsChapter)
        {
            RaidTemp = MissionModelLocator.Instance.GetRaidByTemplateId(int.Parse(map.id) + 100);
        }
        SetContent();
        //throw new System.NotImplementedException();
    }

    public void SetRaidData(RaidInfo theraid)
    {
        RaidData = theraid;
        RaidIndex = ItemIndex;
        RaidTemp = MissionModelLocator.Instance.GetRaidByTemplateId(RaidData.TemplateId);
        RaidAdditionData = MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(RaidData.TemplateId);
    }

    private void SetContent()
    {
        if (HaveInit && map != null)
        {
            Logger.Log("MapVO:" + map.id + ", " + map.x + ", " + map.y);
            var tt = MapImgTex.GetComponent<UITexture>();
            if (IsChapter)
            {
                tt.mainTexture = (Texture2D)Resources.Load("Textures/Mission/chapter" + map.id, typeof(Texture2D));
                MapTitleTex.SetActive(true);
                var tit = MapTitleTex.GetComponent<UITexture>();
                tit.mainTexture = (Texture2D)Resources.Load("Textures/Mission/cname" + map.id, typeof(Texture2D));
            }
            else
            {
                tt.mainTexture = (Texture2D)Resources.Load("Textures/Mission/" + map.id, typeof(Texture2D));
                MapTitleLb.SetActive(true);
                var lb = MapTitleLb.GetComponent<UILabel>();
                lb.text = map.id + "." + RaidTemp.RaidName;
            }
            tt.width = map.w;
            tt.height = map.h;
            transform.localPosition = new Vector3(map.x, map.y, 0);
            //tt.color = new Color(0.4f, 0.4f, 0.4f);
            tt.alpha = 1;

            var cd = gameObject.GetComponent<PolygonCollider2D>();
            cd.points = map.points;
            //IsLock = false;
            if (IsLock)
            {
                MapLockSp.SetActive(true);
                tt.color = new Color(0.2f, 0.2f, 0.2f);
            }
            else
            {
                MapLockSp.SetActive(false);
                tt.color = new Color(1, 1, 1);
                
                if (!IsChapter)
                {
                    MapStarContainer.SetActive(true);
                    var lb = MapStarLabel.GetComponent<UILabel>();
                    //var starcount = MissionModelLocator.Instance.GetRaidStarCount(RaidData.TemplateId);
                    lb.text = MissionModelLocator.Instance.GetStageCountByRaidId(RaidTemp.Id);

                    if (RaidAdditionData != null)
                    {
                        MapEventContainer.SetActive(true);
                        var sp = MapEventSprite.GetComponent<UISprite>();
                        var lbname = MapEventLabel.GetComponent<UILabel>();

                        switch (RaidAdditionData.AddtionType)
                        {
                            case RaidType.RaidAddtionTypeDrop:
                                sp.spriteName = "icon_box";
                                lbname.text = "x" + (RaidAdditionData.AddtionRate / 100);
                                break;
                            case RaidType.RaidAddtionTypeEnergy:
                                sp.spriteName = "icon_energy";
                                lbname.text = "x" + (RaidAdditionData.AddtionRate / 100);
                                break;
                            case RaidType.RaidAddtionTypeGold:
                                sp.spriteName = "icon_gold";
                                lbname.text = "x" + (RaidAdditionData.AddtionRate / 100);
                                break;
                            case RaidType.RaidAddtionTypeSprit:
                                sp.spriteName = "icon_wuhun";
                                lbname.text = "x" + (RaidAdditionData.AddtionRate / 100);
                                break;
                        }
                    }
                    else
                    {
                        MapEventContainer.SetActive(false);
                    }
                }
            }

       
        }
    }

    private void HoverHandler(GameObject game, bool state)
    {
        if (state)
        {
            var tt = MapImgTex.GetComponent<UITexture>();
            //tt.color = new Color(0.0f, 1.0f, 0.0f);
            //tt.color.a = 100;
            //tt.alpha = 0.5f;
            //MapImgTex.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
        else
        {
            var tt = MapImgTex.GetComponent<UITexture>();
            //tt.color = new Color(1, 1, 1, 1);
            //tt.alpha = 1;
            //MapImgTex.transform.localScale = new Vector3(1.0f, 1.0f, 1);
        }
    }
}

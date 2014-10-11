using KXSGCodec;
using Template.Auto.Raid;
using UnityEngine;
using System.Collections;

public class RaidBigItemControl : KxItemRender
{
    private GameObject mapImgTex;
    private GameObject mapTitleLb;
    private GameObject mapLockSp;
    private GameObject mapTitleTex;
    private GameObject mapEventContainer;
    private GameObject mapEventSprite;
    private GameObject mapEventLabel;
    private GameObject mapStarContainer;
    private GameObject mapStarLabel;
    private const string BasePath = "AssetBundles/Textures/Mission";
    private bool initialized;

    public MapVO map;
    public bool IsChapter = true;
    public bool IsLock = true;
    public int RaidIndex;
    public RaidTemplate RaidTemp;
    public RaidInfo RaidData;
    public RaidAddtionInfo RaidAdditionData;

	// Use this for initialization
	void Start () 
    {
        mapImgTex = transform.FindChild("Texture map").gameObject;
        mapTitleLb = transform.FindChild("Label name").gameObject;
        mapLockSp = transform.FindChild("Sprite lock").gameObject;
	    mapTitleTex = transform.FindChild("Texture chapter title").gameObject;
	    mapEventContainer = transform.FindChild("Event Container").gameObject;
        mapEventSprite = transform.FindChild("Event Container/Event Sprite").gameObject;
        mapEventLabel = transform.FindChild("Event Container/Event Label").gameObject;
        mapStarContainer = transform.FindChild("Star Container").gameObject;
        mapStarLabel = transform.FindChild("Star Container/Process Label").gameObject;

        mapEventContainer.SetActive(false);
        mapStarContainer.SetActive(false);
        mapTitleLb.SetActive(false);
        mapTitleTex.SetActive(false);

	    initialized = true;
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
        if (initialized && map != null)
        {
            Logger.Log("MapVO:" + map.id + ", " + map.x + ", " + map.y);
            var tt = mapImgTex.GetComponent<UITexture>();
            if (IsChapter)
            {
                tt.mainTexture = ResoucesManager.Instance.Load<Texture2D>(string.Format("{0}/{1}{2}", BasePath, "chapter", map.id));
                mapTitleTex.SetActive(true);
                var tit = mapTitleTex.GetComponent<UITexture>();
                tit.mainTexture = ResoucesManager.Instance.Load<Texture2D>(string.Format("{0}/{1}{2}", BasePath, "cname", map.id));
            }
            else
            {
                tt.mainTexture = Resources.Load<Texture2D>(string.Format("{0}/{1}", BasePath, map.id));
                mapTitleLb.SetActive(true);
                var lb = mapTitleLb.GetComponent<UILabel>();
                lb.text = RaidTemp.RaidName;
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
                mapLockSp.SetActive(true);
                tt.color = new Color(0.3f, 0.3f, 0.3f);
            }
            else
            {
                mapLockSp.SetActive(false);
                tt.color = new Color(1, 1, 1);
                
                if (!IsChapter)
                {
                    mapStarContainer.SetActive(true);
                    var lb = mapStarLabel.GetComponent<UILabel>();
                    //var starcount = MissionModelLocator.Instance.GetRaidStarCount(RaidData.TemplateId);
                    lb.text = MissionModelLocator.Instance.GetStageCountByRaidId(RaidTemp.Id);

                    if (RaidAdditionData != null)
                    {
                        mapEventContainer.SetActive(true);
                        var sp = mapEventSprite.GetComponent<UISprite>();
                        var lbname = mapEventLabel.GetComponent<UILabel>();

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
                        mapEventContainer.SetActive(false);
                    }
                }
            }

       
        }
    }

    private void HoverHandler(GameObject game, bool state)
    {
        if (state)
        {
            var tt = mapImgTex.GetComponent<UITexture>();
            //tt.color = new Color(0.0f, 1.0f, 0.0f);
            //tt.color.a = 100;
            //tt.alpha = 0.5f;
            //MapImgTex.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
        else
        {
            var tt = mapImgTex.GetComponent<UITexture>();
            //tt.color = new Color(1, 1, 1, 1);
            //tt.alpha = 1;
            //MapImgTex.transform.localScale = new Vector3(1.0f, 1.0f, 1);
        }
    }
}

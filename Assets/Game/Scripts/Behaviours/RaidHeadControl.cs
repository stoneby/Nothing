using System.Collections.Generic;
using KXSGCodec;
using Property;
using Template.Auto.Hero;
using UnityEngine;
using System.Collections;

public class RaidHeadControl : MonoBehaviour
{
    private GameObject ContentContainer;
    private GameObject HeadTex;
    private GameObject SpriteHead;
    private GameObject JobSprite;
    private GameObject FlagSprite;
    private List<GameObject> Stars;

    private HeroTemplate HeroTemp;
    private HeroInfo HeroData;
    private HeroPropInfo FriendHeroData;
    private bool IsFriend = false;
    private int TheType;

    public int Attrack;
    public int Hp;
    public int Recover;
    public int Mp;

	// Use this for initialization
	void Start ()
	{
        ContentContainer = transform.FindChild("Container content").gameObject;
        //HeadTex = transform.FindChild("Container content/Texture head").gameObject;
        SpriteHead = transform.FindChild("Container content/Sprite head").gameObject;
        JobSprite = transform.FindChild("Container content/Sprite job").gameObject;
        FlagSprite = transform.FindChild("Container content/Sprite flag").gameObject;
        Stars = new List<GameObject>();
        Stars.Add(transform.FindChild("Container content/Sprite star1").gameObject);
        Stars.Add(transform.FindChild("Container content/Sprite star2").gameObject);
        Stars.Add(transform.FindChild("Container content/Sprite star3").gameObject);
        Stars.Add(transform.FindChild("Container content/Sprite star4").gameObject);
        Stars.Add(transform.FindChild("Container content/Sprite star5").gameObject);
	    SetContent();
	}

    public void SetData(HeroInfo h0, int type)
    {
        IsFriend = false;
        TheType = type;
        HeroData = h0;
        Attrack = 0;
        Hp = 0;
        Recover = 0;
        Mp = 0;
        if (HeroData != null)
        {
            HeroTemp = HeroModelLocator.Instance.GetHeroByTemplateId(HeroData.TemplateId);
            
            if (HeroData.Prop.ContainsKey(RoleProperties.ROLE_ATK))
            {
                Attrack += HeroData.Prop[RoleProperties.ROLE_ATK];
            }
            if (HeroData.Prop.ContainsKey(RoleProperties.ROLE_HP))
            {
                Hp += HeroData.Prop[RoleProperties.ROLE_HP];
            }
            if (HeroData.Prop.ContainsKey(RoleProperties.ROLE_RECOVER))
            {
                Recover += HeroData.Prop[RoleProperties.ROLE_RECOVER];
            }
            if (HeroData.Prop.ContainsKey(RoleProperties.ROLE_MP))
            {
                Mp += HeroData.Prop[RoleProperties.ROLE_MP];
            }
        }
            
        SetContent();
    }

    public void SetFriendData(HeroPropInfo h0, int type)
    {
        IsFriend = true;
        TheType = type;
        FriendHeroData = h0;
        Attrack = 0;
        Hp = 0;
        Recover = 0;
        Mp = 0;
        if (FriendHeroData != null)
        {
            HeroTemp = HeroModelLocator.Instance.GetHeroByTemplateId(FriendHeroData.HeroTemplateId);
            
            if (FriendHeroData.Prop.ContainsKey(RoleProperties.ROLE_ATK))
            {
                Attrack += FriendHeroData.Prop[RoleProperties.ROLE_ATK];
            }
            if (FriendHeroData.Prop.ContainsKey(RoleProperties.ROLE_HP))
            {
                Hp += FriendHeroData.Prop[RoleProperties.ROLE_HP];
            }
            if (FriendHeroData.Prop.ContainsKey(RoleProperties.ROLE_RECOVER))
            {
                Recover += FriendHeroData.Prop[RoleProperties.ROLE_RECOVER];
            }
            if (FriendHeroData.Prop.ContainsKey(RoleProperties.ROLE_MP))
            {
                Mp += FriendHeroData.Prop[RoleProperties.ROLE_MP];
            }
        }

        SetContent();
    }

    private void SetContent()
    {
        if (FlagSprite == null) return;

        if (!IsFriend && HeroData == null)
        {
            ContentContainer.SetActive(false);
            return;
        }

        if (IsFriend && FriendHeroData == null)
        {
            ContentContainer.SetActive(false);
            return;
        }

        ContentContainer.SetActive(true);
        
        if (TheType == 1)
        {
            FlagSprite.SetActive(true);
            var sp = FlagSprite.GetComponent<UISprite>();
            sp.spriteName = "leader";
        }
        else if (TheType == 2)
        {
            FlagSprite.SetActive(true);
            var sp = FlagSprite.GetComponent<UISprite>();
            sp.spriteName = "leader1";
        }
        else
        {
            FlagSprite.SetActive(false);
        }

        var headsp = SpriteHead.GetComponent<UISprite>();
        int k = 0;
        if (HeroTemp != null)
        {
            k = HeroTemp.Id%14;
            headsp.spriteName = "head_" + k;
            //HeadTex
            var jobsp = JobSprite.GetComponent<UISprite>();
            jobsp.spriteName = "job_" + HeroTemp.Job;

            for (int i = 0; i < Stars.Count; i++)
            {
                Stars[i].SetActive(i < HeroTemp.Star);
            }
        }
        else
        {
            Logger.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@template is null");
        }
    }
}

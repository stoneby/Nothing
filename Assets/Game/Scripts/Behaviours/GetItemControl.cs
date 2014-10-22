using System.Collections;
using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class GetItemControl : MonoBehaviour
{
    private GameObject containerHero;
    private GameObject containerItem;
    private GameObject containerBox;
    private GameObject spriteItem;
    private GameObject spriteHero;
    private GameObject spriteHeroBg;

    private GameObject spriteNew;

    public RewardItem Data;

    private bool haveNotInit = true;

    private bool isOpen;

    private GameObject TopCantainer;
    private GameObject JobSprite;
    private List<GameObject> Stars;
	void Start ()
	{
        containerHero = transform.FindChild("Container hero").gameObject;
        containerItem = transform.FindChild("Container item").gameObject;
        containerBox = transform.FindChild("Sprite box").gameObject;

        TopCantainer = transform.FindChild("Container top").gameObject;
        TopCantainer.SetActive(false);

        Stars = new List<GameObject>();
        Stars.Add(transform.FindChild("Container top/Sprite star1").gameObject);
        Stars.Add(transform.FindChild("Container top/Sprite star2").gameObject);
        Stars.Add(transform.FindChild("Container top/Sprite star3").gameObject);
        Stars.Add(transform.FindChild("Container top/Sprite star4").gameObject);
        Stars.Add(transform.FindChild("Container top/Sprite star5").gameObject);

        JobSprite = transform.FindChild("Container top/Sprite job").gameObject;

        spriteHero = transform.FindChild("Container hero/Sprite head").gameObject;
        spriteHeroBg = transform.FindChild("Container hero/Sprite bg").gameObject;
        spriteItem = transform.FindChild("Container item/Sprite head").gameObject;
        spriteNew = transform.FindChild("Sprite new").gameObject;
        spriteNew.SetActive(false);
	    haveNotInit = false;
	    Reset();
	}
	
    public void SetData(RewardItem thedata)
    {
        Data = thedata;
        isOpen = false;
        Reset();
    }

    private void Reset()
    {
        if (haveNotInit) return;

        spriteNew.SetActive(false);
        SetOpen();
    }

    public void Open()
    {
        isOpen = true;
        if (containerBox == null) return;

        SetOpen();
    }

    private void SetOpen()
    {
        if (isOpen)
        {
            containerBox.SetActive(false);

            StartCoroutine(ShowBattleWin());
        }
        else
        {
            containerHero.SetActive(false);
            containerItem.SetActive(false);

            var sp = containerBox.GetComponent<UISprite>();
            if (Data.RewardType == 1)
            {
                sp.spriteName = "default_head";
            }
            else
            {
                sp.spriteName = "Box_Big";
            }

            containerBox.SetActive(true);
        }
    }

    private IEnumerator ShowBattleWin()
    {
        if (Data.RewardType == 1)
        {
            EffectManager.PlayEffect(EffectType.Jiesuan, 1.6f, 0, 0, gameObject.transform.position, 0, 1.0f);
            yield return new WaitForSeconds(1.6f);
            var sp = spriteHeroBg.GetComponent<UISprite>();
            sp.spriteName = "item_box";
            containerHero.SetActive(true);
            containerItem.SetActive(false);
            sp = spriteHero.GetComponent<UISprite>();
            var herodata = HeroModelLocator.Instance.FindHero(long.Parse(Data.Uuid));
            var tem = HeroModelLocator.Instance.GetHeroByTemplateId(herodata.TemplateId);
            
            spriteNew.SetActive(Data.IsNew);
            TopCantainer.SetActive(true);
            if (tem != null)
            {
                HeroConstant.SetHeadByIndex(sp, tem.Icon - 1);

                var jobsp = JobSprite.GetComponent<UISprite>();
                jobsp.spriteName = "job_" + tem.Job;

                for (int i = 0; i < Stars.Count; i++)
                {
                    Stars[i].SetActive(i < tem.Star);
                }
            }
        }
        else if (Data.RewardType == 2)
        {
            EffectManager.PlayEffect(EffectType.Baoxiang, 1.6f, 0, 0, gameObject.transform.position, 0, 1.0f);
            yield return new WaitForSeconds(1.6f);
            containerHero.SetActive(false);
            containerItem.SetActive(true);
            var sp = spriteItem.GetComponent<UISprite>();
            //Logger.Log("掉落 " + Data.Uuid);
            var item = ItemModeLocator.Instance.FindItem(Data.Uuid);

            var tem = ItemModeLocator.Instance.GetIconId(item.TmplId);
            sp.spriteName = (tem != 0) ? ItemType.ItemHeadPrefix + tem : "item_111002";
            spriteNew.SetActive(Data.IsNew);
            TopCantainer.SetActive(true);
            var q = ItemModeLocator.Instance.GetQuality(item.TmplId);
            int k = ItemHelper.GetStarCount(q);
            for (int i = 0; i < Stars.Count; i++)
            {
                Stars[i].SetActive(i < k);
            }
            var jobsp = JobSprite.GetComponent<UISprite>();
            jobsp.spriteName = "job_" + ItemModeLocator.Instance.GetJob(item.TmplId);
        }
        else if (Data.RewardType == 3)//碎片
        {
            EffectManager.PlayEffect(EffectType.Jiesuan, 1.6f, 0, 0, gameObject.transform.position, 0, 1.0f);
            yield return new WaitForSeconds(1.6f);
            var sp = spriteHeroBg.GetComponent<UISprite>();
            sp.spriteName = "fragment_box";
            containerHero.SetActive(true);
            containerItem.SetActive(false);
            sp = spriteHero.GetComponent<UISprite>();
            //var herodata = HeroModelLocator.Instance.FindHero(long.Parse(Data.Uuid));
            var tem = HeroModelLocator.Instance.GetHeroByTemplateId(int.Parse(Data.Uuid));
            //HeroConstant.SetHeadByIndex(sp, tem.Icon - 1);
            spriteNew.SetActive(Data.IsNew);
            TopCantainer.SetActive(true);
            if (tem != null)
            {
                HeroConstant.SetHeadByIndex(sp, tem.Icon - 1);

                var jobsp = JobSprite.GetComponent<UISprite>();
                jobsp.spriteName = "job_" + tem.Job;

                for (int i = 0; i < Stars.Count; i++)
                {
                    Stars[i].SetActive(i < tem.Star);
                }
            }
        }
    }
}

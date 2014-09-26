using System.Collections;
using KXSGCodec;
using UnityEngine;

public class GetItemControl : MonoBehaviour
{
    private GameObject containerHero;
    private GameObject containerItem;
    private GameObject containerBox;
    private GameObject spriteItem;
    private GameObject spriteHero;

    private GameObject spriteNew;

    public RewardItem Data;

    private bool haveNotInit = true;

    private bool isOpen;

	void Start ()
	{
        containerHero = transform.FindChild("Container hero").gameObject;
        containerItem = transform.FindChild("Container item").gameObject;
        containerBox = transform.FindChild("Sprite box").gameObject;

        spriteHero = transform.FindChild("Container hero/Sprite head").gameObject;
        spriteItem = transform.FindChild("Container item/Sprite head").gameObject;
        spriteNew = transform.FindChild("Sprite new").gameObject;
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

        spriteNew.SetActive(Data.IsNew);
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

                sp.spriteName = "box";
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
            containerHero.SetActive(true);
            containerItem.SetActive(false);
            var sp = spriteHero.GetComponent<UISprite>();
            var herodata = HeroModelLocator.Instance.FindHero(long.Parse(Data.Uuid));
            var tem = HeroModelLocator.Instance.GetHeroByTemplateId(herodata.TemplateId);
            HeroConstant.SetHeadByIndex(sp, tem.Icon - 1);
        }
        else
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
            
        }
    }
}

using KXSGCodec;
using UnityEngine;

public class GetItemControl : MonoBehaviour
{
    private GameObject containerHero;
    private GameObject containerItem;
    private GameObject containerBox;

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
            
            if (Data.RewardType == 1)
            {
                containerHero.SetActive(true);
                containerItem.SetActive(false);
                var sp = spriteHero.GetComponent<UISprite>();
                var herodata = HeroModelLocator.Instance.FindHero(long.Parse(Data.Uuid));
                int k = 0;
                if (herodata != null)
                {
                    k = herodata.TemplateId % 14;
                }
                sp.spriteName = "head_" + k;
            }
            else
            {
                containerHero.SetActive(false);
                containerItem.SetActive(true);
            }
        }
        else
        {
            containerHero.SetActive(false);
            containerItem.SetActive(false);
            containerBox.SetActive(true);
        }
    }
}

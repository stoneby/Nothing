using KXSGCodec;
using UnityEngine;
using System.Collections;

public class GetItemControl : MonoBehaviour
{

    private GameObject ContainerHero;
    private GameObject ContainerItem;
    private GameObject ContainerBox;

    private GameObject HeroHeadTex;
    private GameObject HeroLevelLabel;

    private GameObject ItemTex;

    private GameObject SpriteNew;

    public RewardItem Data;

    private bool HaveNotInit = true;

    private bool IsOpen = false;

	// Use this for initialization
	void Start ()
	{
        ContainerHero = transform.FindChild("Container hero").gameObject;
        ContainerItem = transform.FindChild("Container item").gameObject;
        ContainerBox = transform.FindChild("Sprite box").gameObject;

        HeroHeadTex = transform.FindChild("Container hero/Texture head").gameObject;
        HeroLevelLabel = transform.FindChild("Container hero/Label level").gameObject;

        ItemTex = transform.FindChild("Container item/Texture item").gameObject;

        SpriteNew = transform.FindChild("Sprite new").gameObject;
	    HaveNotInit = false;
	    Reset();
	}
	
	// Update is called once per frame
    public void SetData(RewardItem thedata)
    {
        Data = thedata;
        IsOpen = false;
        Reset();
    }

    private void Reset()
    {
        if (HaveNotInit) return;
       

        if (Data.IsNew)
        {
            SpriteNew.SetActive(true);
        }
        else
        {
            SpriteNew.SetActive(false);
        }
        SetOpen();
    }

    public void Open()
    {
        IsOpen = true;
        if (ContainerBox == null) return;

        SetOpen();
    }

    private void SetOpen()
    {
        if (IsOpen)
        {
            ContainerBox.SetActive(false);
            
            if (Data.RewardType == 1)
            {
                ContainerHero.SetActive(true);
            }
            else
            {
                ContainerItem.SetActive(true);
            }
        }
        else
        {
            ContainerHero.SetActive(false);
            ContainerItem.SetActive(false);
            ContainerBox.SetActive(true);
        }
    }
}

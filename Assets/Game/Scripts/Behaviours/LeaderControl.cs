using UnityEngine;

public class LeaderControl : MonoBehaviour
{
    public GameObject SpriteBg;
    public GameObject SpriteLight;
    public GameObject SpriteHead;

    public int LeaderIndex;

    public int HeadIndex;//0标识无效

    private GameObject shineObj;
    private int baseCd;//需要气力值
    private int currentCd;
    private int cd;

    private UIEventListener HeadUIEventListener;

    void Start()
    {
        
        
    }

    public void Init(int headindex, int basecd, int theindex = 0)
    {
        SpriteBg = transform.FindChild("Sprite - bg").gameObject;
        SpriteHead = transform.FindChild("Sprite - head").gameObject;
        SpriteLight = transform.FindChild("Sprite - light").gameObject;

        HeadUIEventListener = UIEventListener.Get(SpriteHead);
        if (HeadUIEventListener != null) HeadUIEventListener.onClick += OnHeadClick;

        SpriteLight.SetActive(false);
        HeadIndex = headindex;
        baseCd = basecd;
        LeaderIndex = theindex;
        var sp = SpriteHead.GetComponent<UISprite>();
        sp.spriteName = "head_" + HeadIndex.ToString();
    }

    public void Reset(int currentcd)
    {
        currentCd = currentcd;
        if (HeadIndex > 0)
        {
            if (currentcd >= baseCd)
            {
                SpriteLight.SetActive(true);
            }
            else
            {
                SpriteLight.SetActive(false);
            }
        }
        else
        {
            SpriteLight.SetActive(false);
        }
    }

    private void OnHeadClick(GameObject game)
    {
        if (HeadIndex > 0 && currentCd >= baseCd)
        {
            var e = new LeaderUseEvent();
            e.CDCount = baseCd;
            e.SkillIndex = LeaderIndex;
            EventManager.Instance.Post(e);
        }
    }
}

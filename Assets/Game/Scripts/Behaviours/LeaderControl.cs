using UnityEngine;

public class LeaderControl : MonoBehaviour
{
    public GameObject CdLabel;
    public GameObject HeadButton;
    public int LeaderIndex;

    private GameObject shineObj;
    private int baseCd;
    private int cd;

    public void Init(string normalimage, string downimage, string disableimage, int basecd, int theindex)
    {
        var btn = HeadButton.GetComponent<UIImageButton>();
        btn.normalSprite = btn.hoverSprite = normalimage;
        btn.disabledSprite = disableimage;
        btn.pressedSprite = downimage;
        baseCd = basecd;
        LeaderIndex = theindex;
        Reset();
    }

    public void Reset()
    {
        cd = baseCd;
        var btn = HeadButton.GetComponent<UIImageButton>();
        btn.isEnabled = (cd <= 0);
        if (shineObj != null)
        {
            Destroy(shineObj);
            shineObj = null;
        }
        ShowCd();
    }


    public void WorseCd()
    {
        if (cd <= 0) return;

        cd--;
        if (cd <= 0)
        {
            cd = 0;
            ShowCd();
            var btn = HeadButton.GetComponent<UIImageButton>();
            btn.isEnabled = true;
            if (shineObj == null) shineObj = EffectManager.ShowEffect(EffectType.HeadFlash, 0, 0, transform.position);
        }
        else
        {
            ShowCd();
        }
    }

    private void ShowCd()
    {
        var lb = CdLabel.GetComponent<UILabel>();
        lb.text = (cd > 0) ? cd.ToString() : "";
    }
}

using Template.Auto.Reward;
using Template.Auto.Sign;
using UnityEngine;
using System.Collections;

public class SignItemInfo : MonoBehaviour 
{
    private UIEventListener BgUIEventListener;

    private GameObject TitleLabel;
    private GameObject AtkLabel;
    private GameObject HpLabel;
    private GameObject QiliLabel;
    private GameObject RecoverLabel;
    private GameObject SkillTitleLabel;
    private GameObject SkillLabel;


    private GameObject IconItem;

    private void Init()
    {
        if (TitleLabel == null)
        {
            TitleLabel = transform.FindChild("Sprite info/Label name").gameObject;
            AtkLabel = transform.FindChild("Sprite info/Label atk").gameObject;
            HpLabel = transform.FindChild("Sprite info/Label hp").gameObject;
            QiliLabel = transform.FindChild("Sprite info/Label qili").gameObject;
            RecoverLabel = transform.FindChild("Sprite info/Label recover").gameObject;
            SkillTitleLabel = transform.FindChild("Sprite info/Label title").gameObject;
            SkillLabel = transform.FindChild("Sprite info/Label skill").gameObject;

            IconItem = transform.FindChild("SignItem").gameObject;

            BgUIEventListener = UIEventListener.Get(transform.FindChild("Container click").gameObject);
            BgUIEventListener.onClick += ClickBgHandler;
        }
    }

    public void OnEnterEquip(RewardTemplate thetemp)
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        Init();

        var item = IconItem.GetComponent<SignItemControl>();
        item.SetData(thetemp.Id, false, false);

        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = item.Name;

        lb = AtkLabel.GetComponent<UILabel>();
        lb.text = "攻击    [FFC300]" + GetValue(ItemModeLocator.Instance.GetAttack(thetemp.RewardTempId, 1)) + "[-]";

        lb = HpLabel.GetComponent<UILabel>();
        lb.text = "HP  [FFC300]" + GetValue(ItemModeLocator.Instance.GetHp(thetemp.RewardTempId, 1)) + "[-]";
        lb = QiliLabel.GetComponent<UILabel>();
        lb.text = "气力   [FFC300]" + GetValue(ItemModeLocator.Instance.GetMp(thetemp.RewardTempId)) + "[-]";

        lb = RecoverLabel.GetComponent<UILabel>();
        lb.text = "回复   [FFC300]" + GetValue(ItemModeLocator.Instance.GetRecover(thetemp.RewardTempId, 1)) + "[-]";

        lb = SkillTitleLabel.GetComponent<UILabel>();
        lb.text = "装备技能";

        lb = SkillLabel.GetComponent<UILabel>();
        lb.text = ItemModeLocator.Instance.GetDesc(thetemp.RewardTempId);
    }

    private string GetValue(int val)
    {
        if (val >= 0)
        {
            return val.ToString();
        }
        else
        {
            return "-";
        }
    }

    public void OnEnterHero(RewardTemplate thetemp)
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        Init();

        var item = IconItem.GetComponent<SignItemControl>();
        item.SetData(thetemp.Id, false, false);

        var tem = HeroModelLocator.Instance.GetHeroByTemplateId(thetemp.RewardTempId);

        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = item.Name;
        lb = AtkLabel.GetComponent<UILabel>();
        lb.text = "攻击    [FFC300]" + tem.Attack + "[-]";

        lb = HpLabel.GetComponent<UILabel>();
        lb.text = "HP  [FFC300]" + tem.HP + "[-]";
        lb = QiliLabel.GetComponent<UILabel>();
        lb.text = "气力   [FFC300]" + tem.MP + "[-]";

        lb = RecoverLabel.GetComponent<UILabel>();
        lb.text = "回复   [FFC300]" + tem.Recover + "[-]";


        var heroskilltemp = HeroModelLocator.Instance.GetLeaderSkillTemplateById(tem.LeaderSkill);
        if (tem.LeaderSkill <= 0 || heroskilltemp == null)
        {
            SkillTitleLabel.SetActive(false);
            SkillLabel.SetActive(false);
        }
        else
        {
            SkillTitleLabel.SetActive(true);
            SkillLabel.SetActive(true);
            lb = SkillTitleLabel.GetComponent<UILabel>();
            lb.text = "武将技能";

            lb = SkillLabel.GetComponent<UILabel>();
            lb.text = heroskilltemp.Desc;
        }
        
    }

    private void ClickBgHandler(GameObject obj)
    {
        GlobalWindowSoundController.Instance.PlayCloseSound();
        gameObject.SetActive(false);
    }
}

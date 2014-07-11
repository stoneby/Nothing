using System.Globalization;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class ItemSnapShot : MonoBehaviour
{
    private UIEventListener viewDetailLis;
    private UIEventListener equipLis;
    private UIEventListener cancelLis;

    private UILabel atkLabel;
    private UILabel hpLabel;
    private UILabel recoverLabel;
    private UILabel mpLabel;
    private UILabel nameLabel;

    private NewEquipItem itemToEquip;
    private UIHeroDetailWindow detailWindow;

    private void Awake()
    {
        viewDetailLis = UIEventListener.Get(transform.Find("Buttons/ViewDetailBtn").gameObject);
        equipLis = UIEventListener.Get(transform.Find("Buttons/EquipBtn").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Buttons/CancelBtn").gameObject);
        detailWindow = WindowManager.Instance.GetWindow<UIHeroDetailWindow>();
        var property = transform.Find("Property");
        atkLabel = property.Find("Attack/AttackValue").GetComponent<UILabel>();
        hpLabel = property.Find("HP/HPValue").GetComponent<UILabel>();
        recoverLabel = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        mpLabel = property.Find("MP/MPValue").GetComponent<UILabel>();
        nameLabel = transform.Find("Name").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        viewDetailLis.onClick = OnViewDetail;
        equipLis.onClick = OnEquip;
        cancelLis.onClick = OnCancel;
    }

    private void UnInstallHandlers()
    {
        viewDetailLis.onClick = null;
        equipLis.onClick = null;
        cancelLis.onClick = null;
    }

    private void OnViewDetail(GameObject go)
    {
        ItemModeLocator.Instance.GetItemDetailPos = ItemType.GetItemDetailInHeroInfo;
        var csmsg = new CSQueryItemDetail { BagIndex = itemToEquip.BagIndex};
        NetManager.SendMessage(csmsg);
    }

    private void OnEquip(GameObject go)
    {
        var info = ItemModeLocator.Instance.FindItem(itemToEquip.BagIndex);

        if (detailWindow.HeroInfo.EquipUuid[detailWindow.CurEquipIndex] != info.Id)
        {
            var msg = new CSHeroChangeEquip()
            {
                EquipUuid = info.Id,
                HeroUuid = HeroBaseInfoWindow.CurUuid,
                Index = detailWindow.CurEquipIndex
            };
            NetManager.SendMessage(msg);
        }
    }

    private void OnCancel(GameObject go)
    {
        UnInstallHandlers();
        NGUITools.Destroy(gameObject);
    }

    public void Init(NewEquipItem equipItem)
    {
        var info = ItemModeLocator.Instance.FindItem(equipItem.BagIndex);
        atkLabel.text = ItemModeLocator.Instance.GetAttack(info.TmplId, info.Level).ToString(CultureInfo.InvariantCulture);
        hpLabel.text = ItemModeLocator.Instance.GetHp(info.TmplId, info.Level).ToString(CultureInfo.InvariantCulture);
        recoverLabel.text = ItemModeLocator.Instance.GetRecover(info.TmplId, info.Level).ToString(CultureInfo.InvariantCulture);
        mpLabel.text = ItemModeLocator.Instance.GetMp(info.TmplId).ToString(CultureInfo.InvariantCulture);
        nameLabel.text = ItemModeLocator.Instance.GetName(info.TmplId);
        itemToEquip = equipItem;
        InstallHandlers();
    }
}

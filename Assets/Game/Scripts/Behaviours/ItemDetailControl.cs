using KXSGCodec;
using UnityEngine;

public class ItemDetailControl : MonoBehaviour
{
    public ItemBase ItemBase;
    public PropertyUpdater PropertyUpdater;
    public UILabel Desc;

    public void Refresh(ItemInfo info)
    {
        ItemBase.InitItem(info);
        var tempId = info.TmplId;
        var level = info.Level;
        PropertyUpdater.UpdateProperty(level, info.MaxLvl,
                                       ItemModeLocator.Instance.GetAttack(tempId, level),
                                       ItemModeLocator.Instance.GetHp(tempId, level),
                                       ItemModeLocator.Instance.GetRecover(tempId, level),
                                       ItemModeLocator.Instance.GetMp(tempId));
        if(Desc)
        {
            Desc.text = ItemModeLocator.Instance.GetDesc(tempId);
        }
    }

    public void Refresh(int tempId)
    {
        ItemBase.InitItem(tempId);
        PropertyUpdater.UpdateProperty(ItemType.BaseLevel, -1,
                                       ItemModeLocator.Instance.GetAttack(tempId, ItemType.BaseLevel),
                                       ItemModeLocator.Instance.GetHp(tempId, ItemType.BaseLevel),
                                       ItemModeLocator.Instance.GetRecover(tempId, ItemType.BaseLevel),
                                       ItemModeLocator.Instance.GetMp(tempId));
        if (Desc)
        {
            Desc.text = ItemModeLocator.Instance.GetDesc(tempId);
        }
    }
}

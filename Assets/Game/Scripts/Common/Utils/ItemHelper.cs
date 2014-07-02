using KXSGCodec;

public class ItemHelper
{
    /// <summary>
    /// The sort order types.
    /// </summary>
    public enum OrderType
    {
        Time,
        Job,
        Rarity,
        Attack,
        Health,
        Recover,
        Level,
        Team
    }

    /// <summary>
    /// Show the info of the item.
    /// </summary>
    /// <param name="orderType">The order type of item.</param>
    /// <param name="equipItem">The equip item script on the item.</param>
    /// <param name="quality">The quality of the item.</param>
    /// <param name="level">The level of the item.</param>
    /// <param name="job">The job of the item.</param>
    /// <param name="atk">The attack of the item.</param>
    /// <param name="hp">The hp value of the item.</param>
    /// <param name="recover">The recover value of the item.</param>
    public static void ShowItem(OrderType orderType, EquipItem equipItem, int quality, short level, sbyte job, int atk, int hp, int recover)
    {
        switch (orderType)
        {
            case OrderType.Time:
                equipItem.ShowByLvl(level);
                break;

            case OrderType.Job:
                equipItem.ShowByJob(job, atk);
                break;

            case OrderType.Rarity:
                equipItem.ShowByLvl(level);
                break;

            case OrderType.Team:
                equipItem.ShowByLvl(level);
                break;

            case OrderType.Attack:
                equipItem.ShowByJob(job, atk);
                break;

            case OrderType.Health:
                equipItem.ShowByHp(hp);
                break;

            case OrderType.Recover:
                equipItem.ShowByRecover(recover);
                break;

            case OrderType.Level:
                equipItem.ShowByLvl(level);
                break;
        }
    }

    /// <summary>
    /// Show the info of the item.
    /// </summary>
    /// <param name="orderType">The order type of item.</param>
    /// <param name="itemTran">The transform of item.</param>
    /// <param name="tempId">The template id of the item.</param>
    /// <param name="level">The level of the item.</param>
    public static void ShowItem(OrderType orderType, EquipItem itemTran, int tempId, short level)
    {
        int quality = 0;
        sbyte job = -1;
        int atk = -1;
        int hp = -1;
        int recover = -1;
        var itemType = ItemModeLocator.Instance.GetItemType(tempId);
        if (itemType == ItemModeLocator.EquipType.EquipTempl)
        {
            var equipTemp = ItemModeLocator.Instance.ItemTemplates.EquipTmpl[tempId];
            quality = equipTemp.Quality;
            job = equipTemp.JobType;
            atk = equipTemp.Attack;
            hp = equipTemp.Hp;
            recover = equipTemp.Recover;
        }
        if (itemType == ItemModeLocator.EquipType.ArmorTemplate)
        {
            var armorTemp = ItemModeLocator.Instance.ItemTemplates.ArmorTmpl[tempId];
            quality = armorTemp.Quality;
            atk = armorTemp.Attack;
            hp = armorTemp.Hp;
            recover = armorTemp.Recover;
        }
        if (itemType == ItemModeLocator.EquipType.MaterialTempl)
        {
            var materialTemp = ItemModeLocator.Instance.ItemTemplates.MaterialTmpl[tempId];
            quality = materialTemp.Quality;
            job = materialTemp.FitJobType;
        }
        ShowItem(orderType, itemTran, quality, level, job, atk, hp, recover);
    }

    /// <summary>
    /// Show the info of the item.
    /// </summary>
    /// <param name="orderType">The order type of item.</param>
    /// <param name="itemTran">The transform of item.</param>
    /// <param name="itemInfo">The info of item.</param>
    public static void ShowItem(OrderType orderType, EquipItem itemTran, ItemInfo itemInfo)
    {
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
        var level = itemInfo.Level;
        var job = ItemModeLocator.Instance.GetJob(itemInfo.TmplId);
        var atk = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level);
        var hp = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level);
        var recover = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level);
        ShowItem(orderType, itemTran, quality, level, job, atk, hp, recover);
    }

    /// <summary>
    /// To check if the item with special bag index is the same job type with the special main item.
    /// </summary>
    /// <param name="mainType">The type of the main item.</param>
    /// <param name="mainTemId">The template id of the main item.</param>
    /// <param name="bagIndex">The bag index of the item to check.</param>
    /// <returns>True, if it has the same job type with main item.</returns>
    public static bool IsSameJobType(ItemModeLocator.EquipType mainType, int mainTemId, short bagIndex)
    {
        if (mainType == ItemModeLocator.EquipType.InvalidTempl || mainType == ItemModeLocator.EquipType.MaterialTempl)
        {
            return false;
        }
        var equipTemplate = ItemModeLocator.Instance.ItemTemplates.EquipTmpl;
        var materialTempl = ItemModeLocator.Instance.ItemTemplates.MaterialTmpl;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        var type = ItemModeLocator.Instance.GetItemType(info.TmplId);
        var result = false;
        if (mainType == ItemModeLocator.EquipType.EquipTempl)
        {
            var mainJobType = equipTemplate[mainTemId].JobType;
            switch (type)
            {
                case ItemModeLocator.EquipType.EquipTempl:
                    result = equipTemplate[info.TmplId].JobType == mainJobType;
                    break;
                case ItemModeLocator.EquipType.MaterialTempl:
                    result = (materialTempl[info.TmplId].FitType == 1 &&
                             materialTempl[info.TmplId].FitJobType == mainJobType);
                    break;
            }
        }
        if (mainType == ItemModeLocator.EquipType.ArmorTemplate)
        {
            switch (type)
            {
                case ItemModeLocator.EquipType.ArmorTemplate:
                    result = true;
                    break;
                case ItemModeLocator.EquipType.MaterialTempl:
                    result = (materialTempl[info.TmplId].FitType == 2);
                    break;
            }
        }
        return result;
    }
}

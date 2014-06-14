using KXSGCodec;
using UnityEngine;
using System.Collections;

public class ItemHelper
{
    public static void ShowItem(short orderType, EquipItem equipItem, int quality, short level, sbyte job, int atk, int hp, int recover)
    {
        switch (orderType)
        {
            //入手顺序排序
            case 0:
                equipItem.ShowByLvl(level);
                break;

            //武将职业排序
            case 1:
                equipItem.ShowByJob(job, atk);
                break;

            //武将稀有度排序
            case 2:
                equipItem.ShowByLvl(level);
                break;

            //照队伍顺序排序
            case 3:
                equipItem.ShowByLvl(level);
                break;

            //攻击力排序
            case 4:
                equipItem.ShowByJob(job, atk);
                break;

            //HP排序
            case 5:
                equipItem.ShowByHp(hp);
                break;

            //回复力排序
            case 6:
                equipItem.ShowByRecover(recover);
                break;

            //等级排序
            case 7:
                equipItem.ShowByLvl(level);
                break;
        }
    }

    public static void ShowItem(short orderType, EquipItem itemTran, int tempId, short level)
    {
        int quality = 0;
        sbyte job = -1;
        int atk = -1;
        int hp = -1;
        int recover = -1;
        var symbol = tempId / 10000;
        if (symbol == 1)
        {
            var equipTemp = ItemModeLocator.Instance.ItemTemplates.EquipTmpl[tempId];
            quality = equipTemp.Quality;
            job = equipTemp.JobType;
            atk = equipTemp.Attack;
            hp = equipTemp.Hp;
            recover = equipTemp.Recover;
        }
        if (symbol == 2)
        {
            var armorTemp = ItemModeLocator.Instance.ItemTemplates.ArmorTmpl[tempId];
            quality = armorTemp.Quality;
            atk = armorTemp.Attack;
            hp = armorTemp.Hp;
            recover = armorTemp.Recover;
        }
        if (symbol == 3)
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
    /// <param name="orderType">The order type of </param>
    /// <param name="itemTran">The transform of item.</param>
    /// <param name="itemInfo">The info of item.</param>
    public static void ShowItem(short orderType, EquipItem itemTran, ItemInfo itemInfo)
    {
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
        var level = itemInfo.Level;
        var job = ItemModeLocator.Instance.GetJob(itemInfo.TmplId);
        var atk = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level);
        var hp = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level);
        var recover = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level);
        ShowItem(orderType, itemTran, quality, level, job, atk, hp, recover);
    }

    public static bool IsSameJobType(ItemModeLocator.EquipType mainType, int mainTemId, short bagIndex)
    {
        if(mainType == ItemModeLocator.EquipType.InvalidTempl || mainType == ItemModeLocator.EquipType.MaterialTempl)
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
            switch(type)
            {
                case  ItemModeLocator.EquipType.EquipTempl:
                    result = equipTemplate[info.TmplId].JobType == mainJobType;
                    break;
                case ItemModeLocator.EquipType.MaterialTempl:
                    result = (materialTempl[info.TmplId].FitType == 1 &&
                             materialTempl[info.TmplId].FitJobType == mainJobType);
                    break;
            }    
        }
        if(mainType == ItemModeLocator.EquipType.ArmorTemplate)
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

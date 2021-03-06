﻿using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

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
    /// The filter types.
    /// </summary>
    public enum EquipType
    {
        All = 4, 
        Other = 3,
        Equip = 2,
        Armor = 1,
        Material = 0,
    }

    public static List<string> SortSpriteNames = new List<string>
                                                     {
                                                         "TimeSort",
                                                         "JobSort",
                                                         "RaritySort",
                                                         "AttackSort",
                                                         "HealthSort",
                                                         "RecoverSort",
                                                         "LevelSort",
                                                         "TeamSort",
                                                     };

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
                equipItem.ShowByJob(atk);
                break;

            case OrderType.Rarity:
                equipItem.ShowByQuality(quality);
                break;

            case OrderType.Team:
                equipItem.ShowByLvl(level);
                break;

            case OrderType.Attack:
                equipItem.ShowByJob(atk);
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
        if (itemType == EquipType.Equip)
        {
            var equipTemp = ItemModeLocator.Instance.ItemTemplates.EquipTmpls[tempId];
            quality = equipTemp.BaseTmpl.Quality;
            job = equipTemp.JobType;
            atk = equipTemp.Attack;
            hp = equipTemp.Hp;
            recover = equipTemp.Recover;
        }
        if (itemType == EquipType.Armor)
        {
            var armorTemp = ItemModeLocator.Instance.ItemTemplates.ArmorTmpls[tempId];
            quality = armorTemp.BaseTmpl.Quality;
            atk = armorTemp.Attack;
            hp = armorTemp.Hp;
            recover = armorTemp.Recover;
        }
        if (itemType == EquipType.Material)
        {
            var materialTemp = ItemModeLocator.Instance.ItemTemplates.MaterialTmpls[tempId];
            quality = materialTemp.BaseTmpl.Quality;
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
    public static bool IsSameJobType(EquipType mainType, int mainTemId, short bagIndex)
    {
        if (mainType == EquipType.All || mainType == EquipType.Other || mainType == EquipType.Material)
        {
            return false;
        }
        var equipTemplate = ItemModeLocator.Instance.ItemTemplates.EquipTmpls;
        var materialTempl = ItemModeLocator.Instance.ItemTemplates.MaterialTmpls;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        var type = ItemModeLocator.Instance.GetItemType(info.TmplId);
        var result = false;
        if (mainType == EquipType.Equip)
        {
            var mainJobType = equipTemplate[mainTemId].JobType;
            switch (type)
            {
                case EquipType.Equip:
                    result = equipTemplate[info.TmplId].JobType == mainJobType;
                    break;
                case EquipType.Material:
                    result = (materialTempl[info.TmplId].FitType == 1 &&
                             materialTempl[info.TmplId].FitJobType == mainJobType);
                    break;
            }
        }
        if (mainType == EquipType.Armor)
        {
            switch (type)
            {
                case EquipType.Armor:
                    result = true;
                    break;
                case EquipType.Material:
                    result = (materialTempl[info.TmplId].FitType == 2);
                    break;
            }
        }
        return result;
    }

    /// <summary>
    /// Filter all items to exclude material items.
    /// </summary>
    /// <returns>The item info list of all items after filtering.</returns>
    public static List<ItemInfo> FilterItems(List<ItemInfo> infos, EquipType type)
    {
        if (infos == null)
        {
            return null;
        }
        switch(type)
        {
            case EquipType.All:
                return new List<ItemInfo>(infos);
            case EquipType.Equip:
                return infos.FindAll(item => ItemModeLocator.Instance.GetItemType(item.TmplId) == EquipType.Equip);
            case EquipType.Material:
                return infos.FindAll(item => ItemModeLocator.Instance.GetItemType(item.TmplId) == EquipType.Material);
            case EquipType.Armor:
                return infos.FindAll(item => ItemModeLocator.Instance.GetItemType(item.TmplId) == EquipType.Armor);
            case EquipType.Other:
                return null;
        }
        return null;
    }

    public static void GetProproties(ItemInfo itemInfo, out int atk, out int hp, out int recover, out int mp)
    {
        atk = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level);
        hp = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level);
        recover = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level);
        mp = ItemModeLocator.Instance.GetMp(itemInfo.TmplId);
    }

    public static void HideItems(Transform items)
    {
        for (var i = 0; i < items.childCount; i++)
        {
            var item = items.GetChild(i);
            if (item.name != "ExtendButton")
            {
                for (var j = 0; j < item.childCount; j++)
                {
                    NGUITools.SetActive(item.gameObject, false);
                }
            }
        }
    }

    public static int GetStarCount(sbyte quality)
    {
        return Mathf.CeilToInt((float)quality / ItemType.QualitiesPerStar);
    }

    public static void InitWrapContents(CustomGrid grid, List<ItemInfo> itemInfos, int countPerGroup, int curMaxCount)
    {
        if (itemInfos == null) return;
        var list = new List<List<ItemInfo>>();
        var rows = Mathf.CeilToInt((float)itemInfos.Count / countPerGroup);
        var curLimitRows = Mathf.CeilToInt((float)curMaxCount / countPerGroup);
        for (var i = 0; i < rows; i++)
        {
            var infosContainer = new List<ItemInfo>();
            for (var j = 0; j < countPerGroup; j++)
            {
                if (i * countPerGroup + j < itemInfos.Count)
                {
                    infosContainer.Add(itemInfos[i * countPerGroup + j]);
                }
            }
            list.Add(infosContainer);
        }
        grid.Init(list, curLimitRows);
    }

    public static void InstallLongPress(GameObject mat, UIEventListener.VoidDelegate normalClick = null, bool useTempId = false)
    {
        var itemLongPressHandler = mat.GetComponent<ItemLongPressHandler>();
        if (itemLongPressHandler)
        {
            itemLongPressHandler.UseTemplateId = useTempId;
            var longPress = itemLongPressHandler.InstallLongPress();
            longPress.OnNormalPress = normalClick;
        }
    }
}

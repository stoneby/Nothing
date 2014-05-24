using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class ItemModeLocator 
{
    #region Private Field

    private static volatile ItemModeLocator instance;
    private static readonly object SyncRoot = new Object();
    private const string ItemTemlatePath = "Templates/Item";
    
    #endregion

    private ItemModeLocator() { }
    public static ItemModeLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new ItemModeLocator();
                }
            }
            return instance;
        }
    }

    private Item itemTemplates;
    public Item ItemTemplates
    {
        get { return itemTemplates ?? (itemTemplates = Utils.Decode<Item>(ItemTemlatePath)); }
    }

    public SCAllItemInfos ScAllItemInfos { get; set; }

    public enum EquipType
    {
        EquipTempl = 2,
        ArmorTemplate = 1,
        MaterialTempl = 0,
        InvalidTempl = -1,
    }

    private sbyte orderType = 1;
    public sbyte OrderType
    {
        get { return orderType; }
        set { orderType = value; }
    }

    /// <summary>
    /// Sort the list of hero info by specific order type.
    /// </summary>
    /// <param name="sortType">The specific order type.</param>
    /// <param name="items">The list of hero info to be sorted.</param>
    public void SortItemList(short sortType, List<ItemInfo> items)
    {
        switch (sortType)
        {
            //按入手顺序排序
            case 0:
                items.Sort(CompareItemByTime);
                break;

            //按武将职业排序
            case 1:
                items.Sort(CompareItemByJob);
                break;

            //按武将稀有度排序
            case 2:
                items.Sort(CompareItemByQuality);
                break;

            //按照队伍顺序排序
            case 3:
                break;

            //按攻击力排序
            case 4:
                items.Sort(CompareItemByAttack);
                break;

            //按HP排序
            case 5:
                items.Sort(CompareItemByHp);
                break;

            //按回复力排序
            case 6:
                items.Sort(CompareItemByRecover);
                break;

            //按等级排序
            case 7:
                items.Sort(CompareItemByLv);
                break;
        }
    }

    #region Private Methods

    /// <summary>
    /// The comparation of item info by time.
    /// </summary>
    /// <param name="p1">The left item info.</param>
    /// <param name="p2">The right item info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByTime(ItemInfo p1, ItemInfo p2)
    {
        int compareResult = p2.CreatedTime.CompareTo(p1.CreatedTime);
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by job.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByJob(ItemInfo p1, ItemInfo p2)
    {
        var type = GetItemType(p1.TmplId);
        int compareResult = GetItemType(p2.TmplId).CompareTo(type);

        if (compareResult == 0)
        {
            if (type == EquipType.EquipTempl)
            {
                var equipTemp = ItemTemplates.EquipTmpl;
                compareResult = equipTemp[p2.TmplId].JobType.CompareTo(equipTemp[p1.TmplId].JobType);
            }
        }
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by rarity.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByQuality(ItemInfo p1, ItemInfo p2)
    {
        var compareResult = GetQuality(p2).CompareTo(GetQuality(p1));
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by attack.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByAttack(ItemInfo p1, ItemInfo p2)
    {
        var compareResult = GetAttack(p2).CompareTo(GetAttack(p1));
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by hp.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByHp(ItemInfo p1, ItemInfo p2)
    {
        var compareResult = GetHp(p2).CompareTo(GetHp(p1));
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by recover.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByRecover(ItemInfo p1, ItemInfo p2)
    {
        int compareResult = GetRecover(p2).CompareTo(GetRecover(p1));
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by level.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByLv(ItemInfo p1, ItemInfo p2)
    {
        int compareResult = p2.Level.CompareTo(p1.Level);
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
    }

    /// <summary>
    /// Get the type of item.
    /// </summary>
    /// <param name="temId">The templete id of the item.</param>
    /// <returns>The equip type of the item.</returns>
    public EquipType GetItemType(int temId)
    {
        if(ItemTemplates.EquipTmpl.ContainsKey(temId))
        {
            return EquipType.EquipTempl;
        }
        if(ItemTemplates.ArmorTmpl.ContainsKey(temId))
        {
            return EquipType.ArmorTemplate;
        }
        if(ItemTemplates.MaterialTmpl.ContainsKey(temId))
        {
            return EquipType.MaterialTempl;
        }

        return EquipType.InvalidTempl;
    }

    /// <summary>
    /// Get the job type of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The job type of the item.</returns>
    public sbyte GetJob(EquipType type, int tempId)
    {
        if (type == EquipType.EquipTempl)
        {
            var equipTmpl = ItemTemplates.EquipTmpl;
            return equipTmpl[tempId].JobType;
        }
        return -1;
    }

    /// <summary>
    /// Get the job type of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The job type of the item.</returns>
    public sbyte GetJob(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetJob(type, itemInfo.TmplId);

    }

    /// <summary>
    /// Get the quility of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The quility of the item.</returns>
    public sbyte GetQuality(EquipType type, int tempId)
    {
        switch(type)
        {
            case  EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Quality;
            case  EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Quality;
            case  EquipType.MaterialTempl:
                var materialTmpl = ItemTemplates.MaterialTmpl;
                return materialTmpl[tempId].Quality;
            default:
                return -1;
        }   
    }

    /// <summary>
    /// Get the quality of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The quality of the item.</returns>
    public sbyte GetQuality(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetQuality(type, itemInfo.TmplId);

    }

    /// <summary>
    /// Get the attack of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The quility of the item.</returns>
    public int GetAttack(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Attack;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Attack;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Get the attack of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item attack of the item.</returns>
    public int GetAttack(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetAttack(type, itemInfo.TmplId);
    }

    /// <summary>
    /// Get the recover of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The attack of the item.</returns>
    public int GetRecover(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Recover;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Recover;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Get the recover of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item recover of the item.</returns>
    public int GetRecover(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetRecover(type, itemInfo.TmplId);
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public int GetHp(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Hp;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Hp;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item hp of the item.</returns>
    public int GetHp(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetHp(type, itemInfo.TmplId);
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public int GetMp(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Mp;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Mp;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item hp of the item.</returns>
    public int GetMp(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetMp(type, itemInfo.TmplId);
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public string GetName(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Name;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Name;
            case EquipType.MaterialTempl:
                var materialTempl = ItemTemplates.MaterialTmpl;
                return materialTempl[tempId].Name;
            default:
                return "";
        }
    }

    /// <summary>
    /// Get the name of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item name of the item.</returns>
    public string GetName(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetName(type, itemInfo.TmplId);
    }

    public string GetDesc(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].Desc;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].Desc;
            case EquipType.MaterialTempl:
                var materialTempl = ItemTemplates.MaterialTmpl;
                return materialTempl[tempId].Desc;
            default:
                return "";
        }
    }

    /// <summary>
    /// Get the name of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item describe of the item.</returns>
    public string GetDesc(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetDesc(type, itemInfo.TmplId);
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public sbyte GetUpLimit(EquipType type, int tempId)
    {
        switch (type)
        {
            case EquipType.EquipTempl:
                var equipTmpl = ItemTemplates.EquipTmpl;
                return equipTmpl[tempId].UpLimit;
            case EquipType.ArmorTemplate:
                var armorTmpl = ItemTemplates.ArmorTmpl;
                return armorTmpl[tempId].UpLimit;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Get the name of the item.
    /// </summary>
    /// <param name="itemInfo">The item info.</param>
    /// <returns>The item name of the item.</returns>
    public sbyte GetUpLimit(ItemInfo itemInfo)
    {
        var type = GetItemType(itemInfo.TmplId);
        return GetUpLimit(type, itemInfo.TmplId);
    }

    /// <summary>
    /// Find the item info in the item list through the item id.
    /// </summary>
    /// <param name="id">The uid of the item.</param>
    /// <returns>The item info found out.</returns>
    public ItemInfo FindItem(string id)
    {
        return ScAllItemInfos.ItemInfos.Find(info => info.Id == id);
    }

    /// <summary>
    /// Find the item info in the item list through the item id.
    /// </summary>
    /// <param name="bagIndex">The bag index of the item.</param>
    /// <returns>The item info found out.</returns>
    public ItemInfo FindItem(short bagIndex)
    {
        return ScAllItemInfos.ItemInfos.Find(info => info.BagIndex == bagIndex);
    }

    #endregion
}

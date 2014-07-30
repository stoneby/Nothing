using System.Collections.Generic;
using KXSGCodec;
using Template;
using Template.Auto.Bag;
using Template.Auto.Item;
using UnityEngine;
using OrderType = ItemHelper.OrderType;
using EquipType = ItemHelper.EquipType;

public class ItemModeLocator 
{
    #region Private Field

    private static volatile ItemModeLocator instance;
    private static readonly object SyncRoot = new Object();
    private const string ItemTemlatePath = "Templates/Item";
    private const string BagTemlatePath = "Templates/Bag";
    private const string ItemConfigPath = "Templates/ItemConfig";

    private Bag bag;
    private Item itemTemplates;
    private ItemConfig itemConfig;
    private OrderType orderType = OrderType.Job;

    #endregion

    #region Public Fields

    public int GetItemPos;
    public int GetItemDetailPos;

    public SCAllItemInfos ScAllItemInfos { get; set; }
    public SCAllItemInfos BuyBackItems { get; set; }
    public SCItemDetail ItemDetail { get; set; }
    public SCServerConfigMsg ServerConfigMsg { get; set; }

    public Bag Bag
    {
        get { return bag ?? (bag = Utils.Decode<Bag>(BagTemlatePath)); }
    }

    public Item ItemTemplates
    {
        get { return itemTemplates ?? (itemTemplates = Utils.Decode<Item>(ItemTemlatePath)); }
    }

    public ItemConfig ItemConfig
    {
        get { return itemConfig ?? (itemConfig = Utils.Decode<ItemConfig>(ItemConfigPath)); }
    }

    public OrderType OrderType
    {
        get { return orderType; }
        set { orderType = value; }
    }

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

    public static bool AlreadyMainRequest;
    public static bool AlreadyBuyBackRequest;

    #endregion

    #region Public Methods

    /// <summary>
    /// Sort the list of hero info by specific order type.
    /// </summary>
    /// <param name="sortType">The specific order type.</param>
    /// <param name="items">The list of hero info to be sorted.</param>
    public void SortItemList(OrderType sortType, List<ItemInfo> items)
    {
        switch (sortType)
        {
            case OrderType.Time:
                items.Sort(CompareItemByTime);
                break;

            case OrderType.Job:
                items.Sort(CompareItemByJob);
                break;

            case OrderType.Rarity:
                items.Sort(CompareItemByQuality);
                break;

            case OrderType.Attack:
                items.Sort(CompareItemByAttack);
                break;

            case OrderType.Health:
                items.Sort(CompareItemByHp);
                break;

            case OrderType.Recover:
                items.Sort(CompareItemByRecover);
                break;

            case OrderType.Level:
                items.Sort(CompareItemByLv);
                break;
        }
    }

    #endregion

    #region Private Methods

    private ItemModeLocator()
    {
    }

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
        int compareResult = GetJob(p1.TmplId).CompareTo(GetJob(p2.TmplId));
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
        var compareResult = GetQuality(p2.TmplId).CompareTo(GetQuality(p1.TmplId));
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
        var compareResult = GetAttack(p2.TmplId, p2.Level).CompareTo(GetAttack(p1.TmplId, p1.Level));
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
        var compareResult = GetHp(p2.TmplId, p2.Level).CompareTo(GetHp(p1.TmplId, p1.Level));
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
        int compareResult = GetRecover(p2.TmplId, p2.Level).CompareTo(GetRecover(p1.TmplId, p1.Level));
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
        if(ItemTemplates.EquipTmpls.ContainsKey(temId))
        {
            return EquipType.Equip;
        }
        if(ItemTemplates.ArmorTmpls.ContainsKey(temId))
        {
            return EquipType.Armor;
        }
        if(ItemTemplates.MaterialTmpls.ContainsKey(temId))
        {
            return EquipType.Material;
        }

        return EquipType.Other;
    }

    /// <summary>
    /// Get the job type of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The job type of the item.</returns>
    public sbyte GetJob(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var materialTempl = ItemTemplates.MaterialTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].JobType;
        }
        if (materialTempl.ContainsKey(tempId))
        {
            return materialTempl[tempId].FitJobType;
        }
        return -1;
    }

    /// <summary>
    /// Get the quility of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The quility of the item.</returns>
    public sbyte GetQuality(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        var materialTempl = ItemTemplates.MaterialTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].BaseTmpl.Quality;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].BaseTmpl.Quality;
        }
        if (materialTempl.ContainsKey(tempId))
        {
            return materialTempl[tempId].BaseTmpl.Quality;
        }
        return -1;
    }

    /// <summary>
    /// Get the attack of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <param name="level">The level of the item.</param>
    /// <returns>The Attack of the item.</returns>
    public int GetAttack(int tempId, short level)
    {
        var additonLevel = (level - ItemType.BaseLevel);
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].Attack + additonLevel * equipTmpl[tempId].AttackLvlParam;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Attack + additonLevel * armorTmpl[tempId].AttackLvlParam;
        }
        return -1;
    }

    /// <summary>
    /// Get the recover of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <param name="level">The level of the item.</param>
    /// <returns>The recover of the item.</returns>
    public int GetRecover(int tempId, short level)
    {
        var additonLevel = (level - ItemType.BaseLevel);
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].Recover + additonLevel * equipTmpl[tempId].RecoverLvlParam;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Recover + additonLevel * armorTmpl[tempId].RecoverLvlParam;
        }
        return -1;
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <param name="level">The level of the item.</param>
    /// <returns>The hp of the item.</returns>
    public int GetHp(int tempId, short level)
    {
        var additonLevel = (level - ItemType.BaseLevel);
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].Hp + additonLevel * equipTmpl[tempId].HpLvlParam;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Hp + additonLevel * armorTmpl[tempId].HpLvlParam;
        }
        return -1;
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public int GetMp(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].Mp;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Mp;
        }
        return -1;
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    ///// <param name="type">The type of the item.</param>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public string GetName(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        var materialTempl = ItemTemplates.MaterialTmpls;
        if(equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].BaseTmpl.Name;
        }
        if(armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].BaseTmpl.Name;
        }
        if(materialTempl.ContainsKey(tempId))
        {
            return materialTempl[tempId].BaseTmpl.Name;
        }
        return "";
    }

    public string GetDesc(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        var materialTempl = ItemTemplates.MaterialTmpls;
        if(equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].BaseTmpl.Desc;
        }
        if(armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].BaseTmpl.Desc;
        }
        if(materialTempl.ContainsKey(tempId))
        {
            return materialTempl[tempId].BaseTmpl.Desc;
        }
        return "";
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public int GetUpLimit(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].UpLimit;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].UpLimit;
        }  
        return -1;
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public bool GetCanLvlUp(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].CanUpLvl;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].CanUpLvl;
        }
        return false;
    }

    /// <summary>
    /// Get the hp of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The hp of the item.</returns>
    public bool GetCanEvolve(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].CanEvolution;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].CanEvolution;
        }
        return false;
    }

    /// <summary>
    /// Get the sale price of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The sale price of the item.</returns>
    public int GetSalePrice(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        var matTmpl = ItemTemplates.MaterialTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].BaseTmpl.SalePrice;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].BaseTmpl.SalePrice;
        }
        if (matTmpl.ContainsKey(tempId))
        {
            return matTmpl[tempId].BaseTmpl.SalePrice;
        }
        return -1;
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

    /// <summary>
    /// Find the item info in the item list through the item id.
    /// </summary>
    /// <param name="bagIndex">The bag index of the item.</param>
    /// <returns>The item info found out.</returns>
    public ItemInfo FindBuyBackItem(short bagIndex)
    {
        if (BuyBackItems == null || BuyBackItems.ItemInfos == null)
        {
            return null;
        }
        return BuyBackItems.ItemInfos.Find(info => info.BagIndex == bagIndex);
    }

    #endregion
}

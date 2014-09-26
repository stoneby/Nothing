using KXSGCodec;
using System.Collections.Generic;
using Template.Auto.Bag;
using Template.Auto.Item;
using UnityEngine;
using EquipType = ItemHelper.EquipType;
using OrderType = ItemHelper.OrderType;

public class ItemModeLocator 
{
    #region Private Field

    private static volatile ItemModeLocator instance;
    private static readonly object SyncRoot = new Object();

    private Bag bag;
    private Item itemTemplates;
    private ItemConfig itemConfig;
    private OrderType orderType = OrderType.Job;
    private const float ConversionRate = 100;

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
        get { return bag ?? (bag = Utils.Decode<Bag>(ResourcePath.FileBag)); }
    }

    public Item ItemTemplates
    {
        get { return itemTemplates ?? (itemTemplates = Utils.Decode<Item>(ResourcePath.FileItem)); }
    }

    public ItemConfig ItemConfig
    {
        get { return itemConfig ?? (itemConfig = Utils.Decode<ItemConfig>(ResourcePath.FileItemConfig)); }
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
    /// <param name="isDescend">Descend or ascend of the sorting.</param>
    public void SortItemList(OrderType sortType, List<ItemInfo> items, bool isDescend = true)
    {
        if (items == null || items.Count <= 1)
        {
            return;
        }
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
        if (isDescend == false)
        {
            items.Reverse();
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
        var tempId1 = p1.TmplId;
        var tempId2 = p2.TmplId;
        var itemType2 = GetItemType(tempId2);
        //Campare by type first, equip > armor > material.
        int compareResult = itemType2.CompareTo(GetItemType(tempId1));
        if (compareResult == 0 )
        {
            //If the type is the same and they are not materials, then compare by job.
            if(itemType2 != EquipType.Material)
            {
                compareResult = GetJob(tempId1).CompareTo(GetJob(tempId2));
            }
            //Level up material is front of evolve material.
            else
            {
                compareResult =
                    ItemTemplates.MaterialTmpls[tempId1].Type.CompareTo(ItemTemplates.MaterialTmpls[tempId2].Type);
                if(compareResult == 0)
                {
                    compareResult = GetJob(tempId1).CompareTo(GetJob(tempId2));
                }
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
    /// Get the icon id of the item.
    /// </summary>
    /// <param name="tempId">The templete id of the item.</param>
    /// <returns>The icon id of the item.</returns>
    public int GetIconId(int tempId)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        var materialTempl = ItemTemplates.MaterialTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return equipTmpl[tempId].BaseTmpl.IconId;
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].BaseTmpl.IconId;
        }
        if (materialTempl.ContainsKey(tempId))
        {
            return materialTempl[tempId].BaseTmpl.IconId;
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
            return equipTmpl[tempId].Attack + Mathf.RoundToInt(additonLevel * equipTmpl[tempId].AttackLvlParam / ConversionRate);
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Attack + Mathf.RoundToInt(additonLevel * armorTmpl[tempId].AttackLvlParam / ConversionRate);
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
            return equipTmpl[tempId].Recover + Mathf.RoundToInt(additonLevel * equipTmpl[tempId].RecoverLvlParam / ConversionRate);
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Recover + Mathf.RoundToInt(additonLevel * armorTmpl[tempId].RecoverLvlParam / ConversionRate);
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
            return equipTmpl[tempId].Hp + Mathf.RoundToInt(additonLevel * equipTmpl[tempId].HpLvlParam / ConversionRate);
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return armorTmpl[tempId].Hp + Mathf.RoundToInt(additonLevel * armorTmpl[tempId].HpLvlParam / ConversionRate);
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

    public int GetAttackLvlParms(int tempId, short parmLvl)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return Mathf.RoundToInt(parmLvl * equipTmpl[tempId].AttackLvlParam / ConversionRate);
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return Mathf.RoundToInt(parmLvl * armorTmpl[tempId].AttackLvlParam / ConversionRate);
        }
        return 0;
    }

    public int GetRecoverLvlParms(int tempId, short parmLvl)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return Mathf.RoundToInt(parmLvl * equipTmpl[tempId].RecoverLvlParam / ConversionRate);
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return Mathf.RoundToInt(parmLvl * armorTmpl[tempId].RecoverLvlParam / ConversionRate);
        }
        return 0;
    }

    public int GetHpLvlParms(int tempId, short parmLvl)
    {
        var equipTmpl = ItemTemplates.EquipTmpls;
        var armorTmpl = ItemTemplates.ArmorTmpls;
        if (equipTmpl.ContainsKey(tempId))
        {
            return Mathf.RoundToInt(parmLvl * equipTmpl[tempId].HpLvlParam / ConversionRate);
        }
        if (armorTmpl.ContainsKey(tempId))
        {
            return Mathf.RoundToInt(parmLvl * armorTmpl[tempId].HpLvlParam / ConversionRate);
        }
        return 0;
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

    public int GetLevelCost(int curExp, short level, short preshowLvl, int preshowCurExp, sbyte quality)
    {
        var lvlTempls = ItemConfig.ItemLvlTmpls;
        var levelCost = 0;
        if (level < preshowLvl)
        {
            for (var i = level; i < preshowLvl; i++)
            {
                levelCost += lvlTempls[i].UpCostGolds[quality - 1];
            }
        }
        var curLevelTemp = lvlTempls[level];
        var preshowLevelTemp = lvlTempls[preshowLvl];
        var valueToSub = (int)((float)curExp / curLevelTemp.MaxExp * preshowLevelTemp.UpCostGolds[quality - 1]);
        var valueToAdd = (int)((float)preshowCurExp / preshowLevelTemp.MaxExp * preshowLevelTemp.UpCostGolds[quality - 1]);
        levelCost += (valueToAdd - valueToSub);
        return levelCost;
    }

    public int GetSellCost(int tempId, int curExp, short lvl, sbyte quality)
    {
        var lvlTempls = ItemConfig.ItemLvlTmpls;
        var sellCost = GetSalePrice(tempId);
        for(var i = 1; i < lvl; i++)
        {
            sellCost += lvlTempls[i].UpCostGolds[quality - 1];
        }
        var curLevelTemp = lvlTempls[lvl];
        var valueToAdd = (int) ((float) curExp / curLevelTemp.MaxExp * curLevelTemp.UpCostGolds[quality - 1]);
        sellCost += valueToAdd;
        return sellCost;
    }

    public bool IsMaterial(int tempId)
    {
        return GetItemType(tempId) == EquipType.Material;
    }

    #endregion
}

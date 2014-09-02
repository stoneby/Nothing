using UnityEngine;

public class ItemType : MonoBehaviour 
{
    /// <summary>
    /// 从道具入口请求道具列表
    /// </summary>
    public const int GetItemInPanel = 1;

    /// <summary>
    /// 从武将选装备请求道具列表
    /// </summary>
    public const int GetItemInHeroInfo = 2;

    /// <summary>
    /// 从道具入口请求道具详情
    /// </summary>
    public const int GetItemDetailInPanel = 3;

    /// <summary>
    /// 从武将选装备请求道具详情
    /// </summary>
    public const int GetItemDetailInHeroInfo = 4;

    /// <summary>
    /// 每三个稀有度等级代表一颗星星
    /// </summary>
    public const int QualitiesPerStar = 3;

    /// <summary>
    /// 代表主道具背包类型
    /// </summary>
    public const sbyte MainItemBagType = 0;

    /// <summary>
    /// 代表回购背包类型
    /// </summary>
    public const sbyte BuyBackItemBagType = 1;

    /// <summary>
    /// 回购数量限制
    /// </summary>
    public const int BuyBackLimit = 20;

    /// <summary>
    /// 基本加成等级，小于等于这个等级没有加成
    /// </summary>
    public const int BaseLevel = 1;

    /// <summary>
    /// 如果回购背包索引为-1，就不放进回购背包
    /// </summary>
    public const int InvalidBuyBackIndex = -1;

    public const string SellConfirmKey = "UISellItem.SellConfirm";
    public const string ItemHeadPrefix = "Item_";
    public const string ExtendContentKey = "UIItemCommon.ExtendContent";
    public const string ExtendLimitKey = "UIItemCommon.ExtendLimit";

    public const string ArmorJob = "job_6";

    public static void SetHeadByTemplate(UISprite sprite, int templateId)
    {
        var index = ItemModeLocator.Instance.GetIconId(templateId);
        SetHeadByIndex(sprite, index);
    }

    public static void SetHeadByIndex(UISprite sprite, int index)
    {
        sprite.spriteName = ItemHeadPrefix + index;
    }
}

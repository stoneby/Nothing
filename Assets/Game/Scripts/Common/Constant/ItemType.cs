﻿using UnityEngine;

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
}

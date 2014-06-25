using UnityEngine;
using System.Collections;

public class ItemType : MonoBehaviour 
{
    public const int GetItemInPanel = 1;
    public const int GetItemInHeroInfo = 2;

    public const int GetItemDetailInPanel = 3;
    public const int GetItemDetailInHeroInfo = 4;

    /// <summary>
    /// 每三个稀有度等级代表一颗星星
    /// </summary>
    public const int QualitiesPerStar = 3;

    public const sbyte MainItemBagType = 0;
    public const sbyte BuyBackItemBagType = 1;
}

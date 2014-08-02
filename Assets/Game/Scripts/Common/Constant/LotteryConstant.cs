using UnityEngine;
using System.Collections;

public class LotteryConstant 
{
    /** 抽卡类型 武将抽卡 */
    public const int LotteryTypeHero = 1;

    /** 抽卡类型 道具抽卡 */
    public const int LotteryTypeItem = 2;

    /** 抽奖类型-单次免费抽卡 */
    public const int LotteryModeFree = 1;

    /** 抽奖类型-单次收费抽卡 */
    public const int LotteryModeOnceCharge = 2;

    /** 抽奖类型-连续10次抽卡 */
    public const int LotteryModeTenthCharge = 3;

    public const string OneTimeHeroLotteryConfirmKey = "UIHeroLottery.OneLotteryHeroConfirm";
    public const string TenTimeHeroLotteryConfirmKey = "UIHeroLottery.TenLotteryHeroConfirm";  
    public const string OneTimeItemLotteryConfirmKey = "UIItemLottery.OneLotteryItemConfirm";
    public const string TenTimeItemLotteryConfirmKey = "UIItemLottery.TenLotteryItemConfirm";
}

using System.Collections.Generic;

public static class StringTable
{
    public static List<string> SortStrings = new List<string>
                                                     {
                                                         "入手排序",
                                                         "职业排序",
                                                         "稀有度排序",
                                                         "攻击力排序",
                                                         "HP排序",
                                                         "回复力排序",
                                                         "等级排序",
                                                         "队伍排序",
                                                     };

    //This is just for demo.
    public const string TeamPrefix = "队伍";

    public const string SellConfirm = "列表中包含A级或以上的道具，确定要出售吗？";
    public const string OneTimeHeroLotteryConfirm = "消耗{0}元宝进行一次武将抽卡操作，确定要继续吗？";
    public const string TenTimeHeroLotteryConfirm = "消耗{0}元宝进行连续十次武将抽卡操作，确定要继续吗？";  
    public const string OneTimeItemLotteryConfirm = "消耗{0}名气值进行一次武将抽卡操作，确定要继续吗？";
    public const string TenTimeItemLotteryConfirm = "消耗{0}名气值进行连续十次武将抽卡操作，确定要继续吗？";
    public const string EvolveNotFullLvl = "道具需要升级到最高等级";
    public const string EvolveNotEnoughMoney = "金币不足";
    public const string EvolveNotEnoughMat = "进化材料不足";
}
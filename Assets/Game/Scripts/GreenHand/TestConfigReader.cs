using System.Collections.Generic;
using UnityEngine;

public class TestConfigReader : GreeenHandConfigReader
{
    public static int MaxConfigIndex = 32;

    public string GetBattleType(int index)
    {
        return null;
    }

    public bool ReadConfig(GreenHandGuideHandler handler, int index)
    {

        if (index == 0)
        {
            ResetConfig(handler);
            handler.ConfigMode = "BattleBlink";
            handler.TextList = new List<string>() { "前方敌袭！\n各个击破方为上策，请主公先随我击杀[b][ffc900]下方的敌将[/b][-]！" };
            handler.CanSelectIndexList = new List<int>() { 14 };
            handler.IsWait = true;
            handler.WaitSec = 1.0f;
            return true;
        }
        if (index == 1)
        {
            ResetConfig(handler);
            handler.ConfigMode = "BattleMove";
            handler.TextList = new List<string>() { "主公，请连接[b][ffc900]同色武将[/b][-]进行攻击！" };
            handler.CanSelectIndexList = new List<int>() { 0, 1, 2 };
            handler.ValidateIndexList = new List<int>() { 0, 1, 2 };
            handler.MoveTraceIndexList = new List<int>() { 0, 2 };
            return true;
        }
        if (index == 2)
        {
            ResetConfig(handler);
            handler.ConfigMode = "BattleMove";
            handler.TextList = new List<string>() { "带有[b][ffc900]蓝光[/b][-]的武将可以发动[b][ffc900]无双技能[/b][-]，伤害爆表。" };
            handler.CanSelectIndexList = new List<int>() { 3, 5, 7 };
            handler.ValidateIndexList = new List<int>() { 3, 7, 5 };
            handler.MoveTraceIndexList = new List<int>() { 3, 7, 5 };
            return true;
        }
        if (index == 3)
        {
            ResetConfig(handler);
            handler.ConfigMode = "BattleMove";
            handler.TextList = new List<string>() { "连接的[b][ffc900]数量越多[/b][-]，[b][ffc900]伤害越高[/b][-]。\n出手越晚的武将额外的伤害倍数加成越高。" };
            handler.CanSelectIndexList = new List<int>() { 6, 7, 8, 5, 2, 12 };
            handler.ValidateIndexList = new List<int>() { 6, 7, 8, 5, 2 };
            handler.MoveTraceIndexList = new List<int>() { 6, 8, 2 };
            return true;
        }
        if (index == 4)
        {
            ResetConfig(handler);
            handler.ConfigMode = "BattleMove";
            handler.TextList = new List<string>() { "[b][ff00ed]粉色[/b][-]可以[b][ffc900]回复HP[/b][-]，主公请试试~" };
            handler.CanSelectIndexList = new List<int>() { 6, 3 };
            handler.ValidateIndexList = new List<int>() { 6, 3 };
            handler.MoveTraceIndexList = new List<int>() { 6, 3 };
            return true;
        }
        if (index == 5)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "主公，[b][ffc900]主将技能[/b][-]已经准备好了！" };
            handler.NextConfigTriggerObjectTag = "GreenHand0";
            return true;
        }
        if (index == 6)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand1";
            return true;
        }
        if (index == 7)
        {
            ResetConfig(handler);
            handler.ConfigMode = "BattleMove";
            handler.TextList = new List<string>() { "连接九个武将时，[b][ffc900]九连击[/b][-]的武将将施展[b][ffc900]全屏攻击[/b][-]。\n主公连连看！" };
            handler.CanSelectIndexList = new List<int>() { 6, 7, 8, 5, 4, 3, 0, 1, 2 };
            handler.ValidateIndexList = new List<int>() { 6, 7, 8, 5, 4, 3, 0, 1, 2 };
            handler.MoveTraceIndexList = new List<int>() { 6, 8, 5, 3, 0, 2 };
            return true;
        }

        if (index == 10)
        {
            ResetConfig(handler);
            handler.ConfigMode = "CreatePlayer";
            handler.TextList = new List<string>() { "恭喜主公首战告捷！\n请问主公希望[b][ffc900]如何称呼[/b][-]您呢？" };
            return true;
        }

        if (index == 20)
        {
            ResetConfig(handler);
            handler.ConfigMode = "GiveHero";
            handler.TextList = new List<string>() { "恭喜主公首战告捷！\n有[b][ffc900]三名武将[/b][-]听闻主公威名，特来投奔！" };
            return true;
        }

        if (index == 30)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "阵容已定！\n请主公[b][ffc900]乘胜追击[/b][-]！" };
            handler.NextConfigTriggerObjectTag = "GreenHand10";
            return true;
        }
        if (index == 31)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand11";
            return true;
        }
        if (index == 32)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand11";
            return true;
        }
        if (index == 33)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand12";
            handler.IsWait = true;
            handler.WaitSec = 1.0f;
            return true;
        }
        if (index == 34)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "选择强力[b][ffc900]队友助战[/b][-]，通关更简单。\n而且可以获得额外的[b][ffc900]名气值奖励[/b][-]哦~" };

            handler.NextConfigTriggerObjectTag = "GreenHand13";
            handler.TagObjectIndex = 1;
            return true;
        }
        if (index == 35)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand14";
            return true;
        }

        if (index == 40)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "主公，前去[b][ffc900]召唤武将[/b][-]加入，增加队伍实力吧~" };
            handler.NextConfigTriggerObjectTag = "GreenHand15";
            return true;
        }
        if (index == 41)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "在这里可以随机召唤出[b][ffc900]3~5星[/b][-]武将陪主公征战沙场哟~\n每次召唤[b][ffc900]24小时[/b][-]后会有一次[b][ffc900]免费[/b][-]召唤机会，主公记得随时上来看看！" };
            handler.NextConfigTriggerObjectTag = "GreenHand16";
            return true;
        }
        if (index == 42)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand17";
            return true;
        }
        if (index == 43)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.NextConfigTriggerObjectTag = "GreenHand18";
            return true;
        }


        if (index == 50)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "主公，赶快把新召唤的武将[b][ffc900]加入队伍[/b][-]吧！" };
            handler.NextConfigTriggerObjectTag = "GreenHand20";
            return true;
        }
        if (index == 51)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "点击[b][ffc900]卸下[/b][-]当前的[b][ffc900]主将[/b][-]" };
            handler.NextConfigTriggerObjectTag = "GreenHand21";
            return true;
        }
        if (index == 52)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "点击[b][ffc900]新武将[/b][-]，作为[b][ffc900]主将[/b][-]上阵~" };
            handler.NextConfigTriggerObjectTag = "GreenHand22";
            handler.TagObjectIndex = 3;
            return true;
        }
        if (index == 53)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalBlink";
            handler.TextList = new List<string>() { "点击卸下的武将，作为[b][ffc900]士兵[/b][-]上阵~" };
            handler.NextConfigTriggerObjectTag = "GreenHand22";
            handler.TagObjectIndex = 2;
            return true;
        }
        if (index == 54)
        {
            ResetConfig(handler);
            handler.ConfigMode = "NormalMove";
            handler.TextList = new List<string>() { "[b][ffc900]拉动绳索[/b][-]可以返回[b][ffc900]主界面[/b][-]哦~" };
            handler.NextConfigTriggerObjectTag = "GreenHand23";
            handler.NormalMoveVec = new Vector3(0, -0.5f, 0);
            return true;
        }

        Logger.LogWarning("Can't read config in configIndex:" + index + ", Config not changed.");
        return false;
    }

    private void ResetConfig(GreenHandGuideHandler handler)
    {
        handler.ConfigMode = null;
        handler.TextList = null;
        handler.NextConfigTriggerObjectTag = null;
        handler.CanSelectIndexList = null;
        handler.ValidateIndexList = null;
        handler.MoveTraceIndexList = null;
        handler.TagObjectIndex = 0;
        handler.NormalMoveVec = new Vector3(0, 0, 0);
        handler.IsWait = false;
        handler.WaitSec = 0;
    }
}

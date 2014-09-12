using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TestConfigReader : GreeenHandConfigReader
{
    public int ReadConfig(GreenHandGuideHandler handler, int index)
    {
        if (index == 0)
        {
            handler.TextList = new List<string>() { "前方敌袭！\n各个击破方为上策，请主公先随我击杀[b][ffc900]下方的敌将[/b][-]！" };
            handler.CanClickIndexList = new List<int>();
            handler.CanSelectIndexList = new List<int>() { 14 };
            handler.ValidateIndexList = new List<int>();
            handler.MoveTraceIndexList = new List<int>();
            return 0;
        }
        if (index == 1)
        {
            handler.TextList = new List<string>() { "主公，请连接[b][ffc900]同色武将[/b][-]进行攻击！" };
            handler.CanClickIndexList = new List<int>();
            handler.CanSelectIndexList = new List<int>() { 0, 1, 2 };
            handler.ValidateIndexList = new List<int>() { 0, 1, 2 };
            handler.MoveTraceIndexList=new List<int>(){0,2};
            return 0;
        }
        if (index == 2)
        {
            handler.TextList = new List<string>() { "带有[b][ffc900]蓝光[/b][-]的武将可以发动[b][ffc900]无双技能[/b][-]，伤害爆表。" };
            handler.CanClickIndexList = new List<int>();
            handler.CanSelectIndexList = new List<int>() { 3, 5, 7 };
            handler.ValidateIndexList = new List<int>() { 3,7,5 };
            handler.MoveTraceIndexList = new List<int>() { 3, 7, 5 };
            return 0;
        }
        if (index == 3)
        {
            handler.TextList = new List<string>() { "连接的[b][ffc900]数量越多[/b][-]，[b][ffc900]伤害越高[/b][-]。\n出手越晚的武将额外的伤害倍数加成越高。" };
            handler.CanClickIndexList = new List<int>(){};
            handler.CanSelectIndexList = new List<int>() { 6, 7, 8, 5, 2, 12 };
            handler.ValidateIndexList = new List<int>() { 6, 7, 8, 5, 2 };
            handler.MoveTraceIndexList = new List<int>(){6,8,2};
            return 0;
        }
        if (index == 4)
        {
            handler.TextList = new List<string>() { "[b][ff00ed]粉色[/b][-]可以[b][ffc900]回复HP[/b][-]，主公请试试~" };
            handler.CanClickIndexList = new List<int>();
            handler.CanSelectIndexList = new List<int>() { 6, 3 };
            handler.ValidateIndexList = new List<int>() { 6, 3 };
            handler.MoveTraceIndexList = new List<int>() { 6, 3 };
            return 0;
        }
        if (index == 5)
        {
            handler.TextList = new List<string>() { "主公，[b][ffc900]主将技能[/b][-]已经准备好了！" };
            handler.CanClickIndexList = new List<int>() { 1 };
            handler.CanSelectIndexList = new List<int>();
            handler.ValidateIndexList = new List<int>();
            handler.MoveTraceIndexList = new List<int>();
            return 0;
        }
        if (index == 6)
        {
            handler.TextList = new List<string>() { "连接九个武将时，[b][ffc900]九连击[/b][-]的武将将施展[b][ffc900]全屏攻击[/b][-]。\n主公连连看！" };
            handler.CanClickIndexList = new List<int>();
            handler.CanSelectIndexList = new List<int>() { 6, 7, 8, 5, 4, 3, 0, 1, 2 };
            handler.ValidateIndexList = new List<int>() { 6, 7, 8, 5, 4, 3, 0, 1, 2 };
            handler.MoveTraceIndexList = new List<int>(){6,8,5,3,0,2};
            return 0;
        }
        Logger.LogWarning("Can't read config in configIndex:" + index + ", Config not changed.");
        return -1;
    }
}

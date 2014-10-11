using System.Collections.Generic;
using KXSGCodec;
using Property;

public class FriendUtils 
{
    /// <summary>
    /// The sort order types.
    /// </summary>
    public enum OrderType
    {
        Atk,
        Level,
        MaxDamage
    }

    public static List<string> SortNameKeys = new List<string>
                                                     {
                                                         "UISortFriend.Atk",
                                                         "UISortFriend.Level",
                                                         "UISortFriend.MaxDamage",
                                                     };

    public static int GetProp(FriendInfo info, int key)
    {
        var result = 0;
        var prop = info.HeroProp;
        var count = prop.Count;
        for (var i = 0; i < count; i++)
        {
            result += prop[i].Prop[key];
        }
        return result;
    }

    public static bool IsFriendBind(FriendInfo info)
    {
        return ((info.Status & FriendConstant.BindFlag) != 0);
    }

    /// <summary>
    /// The comparation of friend info by attack.
    /// </summary>
    /// <param name="p1">The left friend info.</param>
    /// <param name="p2">The right friend info.</param>
    /// <returns>The result of the comparation</returns>
    public static int CompareFriendByAtk(FriendInfo p1, FriendInfo p2)
    {
        return GetProp(p2, RoleProperties.ROLE_ATK).CompareTo(GetProp(p1, RoleProperties.ROLE_ATK));
    }   
    
    /// <summary>
    /// The comparation of friend info by level.
    /// </summary>
    /// <param name="p1">The left friend info.</param>
    /// <param name="p2">The right friend info.</param>
    /// <returns>The result of the comparation</returns>
    public static int CompareFriendByLevel(FriendInfo p1, FriendInfo p2)
    {
        return p2.FriendLvl.CompareTo(p1.FriendLvl);
    }

    /// <summary>
    /// The comparation of friend info by max hit.
    /// </summary>
    /// <param name="p1">The left friend info.</param>
    /// <param name="p2">The right friend info.</param>
    /// <returns>The result of the comparation</returns>
    public static int CompareFriendByMaxHit(FriendInfo p1, FriendInfo p2)
    {
        return p2.MaxDamage.CompareTo(p1.MaxDamage);
    }

    public static void Sort(List<FriendInfo> infos, OrderType orderType)
    {
        if(infos == null || infos.Count <= 1)
        {
            return;
        }
        switch(orderType)
        {
            case OrderType.Atk:
                {
                    infos.Sort(CompareFriendByAtk);
                    break;  
                }      
            case OrderType.Level:
                {
                    infos.Sort(CompareFriendByLevel);
                    break; 
                } 
            case OrderType.MaxDamage:
                {
                    infos.Sort(CompareFriendByMaxHit);
                    break;
                }
        }
    }
}

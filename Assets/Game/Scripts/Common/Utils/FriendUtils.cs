using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

public class FriendUtils 
{
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
    public static int CompareHeroByAtk(FriendInfo p1, FriendInfo p2)
    {
        return GetProp(p2, RoleProperties.ROLE_ATK).CompareTo(GetProp(p1, RoleProperties.ROLE_ATK));
    }
}

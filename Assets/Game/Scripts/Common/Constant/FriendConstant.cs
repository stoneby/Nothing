using UnityEngine;
using System.Collections;

public class FriendConstant
{
    /** 好友关系-同意 */
    public const sbyte FriendApplyAgree = 2;

    /** 好友关系-拒绝 */
    public const sbyte FriendApplyReject = 3;

    public const int FriendHandlerListIndex = 0;
    public const int AddFriendHandlerIndex = 1;
    public const int FriendEnergyHandlerIndex = 3;

    public const int BindFlag = 0x0001;

    public const string ExtendContentKey = "UIFriendEntry.ExtendContent";
    public const string ExtendLimitKey = "UIFriendEntry.ExtendLimit";
    public const sbyte FriendBind = 1;
    public const sbyte FriendUnBind = 2;
}

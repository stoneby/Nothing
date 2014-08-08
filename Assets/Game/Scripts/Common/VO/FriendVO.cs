using System;
using KXSGCodec;
using UnityEngine;
using System.Collections;

#if !SILVERLIGHT
[Serializable]
#endif
public class FriendVO
{
    public FriendInfo Data;
    [NonSerialized]
    public bool IsFriend;
}

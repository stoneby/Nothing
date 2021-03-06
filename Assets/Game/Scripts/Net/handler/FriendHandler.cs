﻿using System.Collections.Generic;
using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    public class FriendHandler
    {
        public static void OnFriendLoadingAll(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                FriendModelLocator.Instance.ScFriendLoadingAll = msg.GetContent() as SCFriendLoadingAll;
                FriendModelLocator.Instance.ExtendFriendTimes =
                    FriendModelLocator.Instance.ScFriendLoadingAll.FriendLimitExtendTimes;
                WindowManager.Instance.Show<UIFriendEntryWindow>(true);
            }
        }

        public static void OnFriendApplyList(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                FriendModelLocator.Instance.ScFriendApplyList = msg.GetContent() as SCFriendApplyList;
                var friendEntryWindow = WindowManager.Instance.GetWindow<UIFriendEntryWindow>();
                var addFriendHandler = friendEntryWindow.FriendHandlers[FriendConstant.AddFriendHandlerIndex] as AddFriendHandler;
                if(addFriendHandler.gameObject.activeInHierarchy)
                {
                    addFriendHandler.Refresh(FriendModelLocator.Instance.ScFriendApplyList.ApplyList);
                }
            }
        }

        public static void OnFriendQueryByName(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                var friendInfo = msg.GetContent() as SCFriendQueryByName;
                var friendApplyWindow = WindowManager.Instance.Show<UIFriendApplyWindow>(true);
                friendApplyWindow.Refresh(friendInfo.ApplyFriendInfo);
            }
        }

        public static void OnFriendGiveEnergySucc(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                var friendList = msg.GetContent() as SCFriendGiveEnergySucc;
                var friendEntryWindow = WindowManager.Instance.GetWindow<UIFriendEntryWindow>();
                var friendListHandler = friendEntryWindow.FriendHandlers[FriendConstant.FriendHandlerListIndex] as FriendListHandler;
                friendListHandler.RefreshGivenSucc(friendList.FriendUuid);
            }
        }

        public static void OnFriendRecieveEnergyList(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                var energyList = FriendModelLocator.Instance.ScFriendRecieveEnergyList = msg.GetContent() as SCFriendRecieveEnergyList;
                if (energyList != null)
                {
                    var friendEntryWindow = WindowManager.Instance.GetWindow<UIFriendEntryWindow>();
                    var energyHandler = friendEntryWindow.FriendHandlers[FriendConstant.FriendEnergyHandlerIndex] as FriendEnergyHandler;
                    if(energyHandler != null)
                    {
                        energyHandler.Refresh(energyList.RecieveEnergyList, energyList.RecieveEnergyTimes,
                                              energyList.RecieveEnergyTimesLimit);
                    }
                }
            }
        }

        public static void OnFriendReciveEnergySucc(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                var energySucc = msg.GetContent() as SCFriendRecieveEnergySucc;
                if (energySucc != null)
                {
                    var friendEntryWindow = WindowManager.Instance.GetWindow<UIFriendEntryWindow>();
                    var energyHandler = friendEntryWindow.FriendHandlers[FriendConstant.FriendEnergyHandlerIndex] as FriendEnergyHandler;
                    if(energyHandler != null)
                    {
                        energyHandler.RefreshReceiveSucc(energySucc.FriendUuid, energySucc.RefreshRecieveEnergyTimes);
                    }
                }
            }
        }

        public static void OnFriendApplySucc(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                var energySucc = msg.GetContent() as SCFriendApplySucc;
                if (energySucc != null)
                {
 
                }
            }
        }

        public static void OnFriendApplyOperSucc(ThriftSCMessage msg)
        {
            if(msg != null)
            {
                var operSucc = msg.GetContent() as SCFriendApplyOperSucc;
                if (operSucc != null)
                {
                    var applyListCached = FriendModelLocator.Instance.ScFriendApplyList.ApplyList;
                    if(operSucc.OperType == FriendConstant.FriendApplyAgree)
                    {
                        var friendList = FriendModelLocator.Instance.ScFriendLoadingAll.FriendList;
                        if(friendList == null)
                        {
                            friendList = FriendModelLocator.Instance.ScFriendLoadingAll.FriendList = new List<FriendInfo>();
                        }
                        var friend = applyListCached.Find(item => item.FriendUuid == operSucc.Uuid);
                        friendList.Add(friend);
                    }
                    else if(operSucc.OperType == FriendConstant.FriendApplyReject)
                    {
                        applyListCached.RemoveAll(item => item.FriendUuid == operSucc.Uuid);
                    }
                    var friendEntryWindow = WindowManager.Instance.GetWindow<UIFriendEntryWindow>();
                    var addFriendHandler = friendEntryWindow.FriendHandlers[FriendConstant.AddFriendHandlerIndex] as AddFriendHandler;
                    addFriendHandler.RefreshApplyOper(operSucc.Uuid, operSucc.OperType);
                }
            }
        }

        public static void OnFriendExtendSucc(ThriftSCMessage msg)
        {
            if (msg != null)
            {
                var extendSucc = msg.GetContent() as SCFriendExtendSucc;
                if (extendSucc != null)
                {
                    FriendModelLocator.Instance.ScFriendLoadingAll.FriendLimit = extendSucc.FriendCountLimit;
                    FriendModelLocator.Instance.ExtendFriendTimes = extendSucc.FriendLimitExtendTimes;
                    WindowManager.Instance.GetWindow<UIFriendEntryWindow>().RefreshFriendCount();
                }
            }
        }

        public static void OnFriendBindSucc(ThriftSCMessage msg)
        {
            if (msg != null)
            {
                var bindSucc = msg.GetContent() as SCFriendBindSucc;
                if (bindSucc != null)
                {
                    var friend = FriendModelLocator.Instance.FindInfo(bindSucc.FriendUuid);
                    if (bindSucc.BindType == FriendConstant.FriendBind)
                    {
                        friend.Status |= FriendConstant.FriendBind;
                    }
                    else
                    {
                        friend.Status = (sbyte)((friend.Status | FriendConstant.FriendBind) ^ FriendConstant.FriendBind);
                    }
                }
            }
        }

        public static void OnFriendDelSucc(ThriftSCMessage msg)
        {
            if (msg != null)
            {
                var deleteSucc = msg.GetContent() as SCFriendDeleteSucc;
                if (deleteSucc != null)
                {
                    FriendModelLocator.Instance.ScFriendLoadingAll.FriendList.RemoveAll(
                        item => item.FriendUuid == deleteSucc.FriendUuid);
                    var friendEntryWindow = WindowManager.Instance.GetWindow<UIFriendEntryWindow>();
                    var friendList = friendEntryWindow.FriendHandlers[FriendConstant.FriendHandlerListIndex] as FriendListHandler;
                    friendList.Refresh(FriendModelLocator.Instance.ScFriendLoadingAll.FriendList);
                }
            }
        }
    }
}

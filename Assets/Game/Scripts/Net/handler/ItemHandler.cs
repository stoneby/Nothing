using System;
using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class ItemHandler
    {
        public delegate void MessageReceived();
        public static MessageReceived ItemListInItemPanel;

        public static bool IsShowMainScreen = false;

        public static void OnAllItemInfos(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCAllItemInfos;
            if (themsg != null)
            {
                if (ItemModeLocator.Instance.GetItemPos == ItemType.GetItemInPanel)
                {
                    if (themsg.BagType == ItemType.MainItemBagType)
                    {
                        ItemModeLocator.Instance.ScAllItemInfos = themsg;
                        ItemModeLocator.AlreadyMainRequest = true;
                        WindowManager.Instance.Show<UIItemCommonWindow>(true);
                    }
                    if (themsg.BagType == ItemType.BuyBackItemBagType)
                    {
                        ItemModeLocator.Instance.BuyBackItems = themsg;
                        ItemModeLocator.AlreadyBuyBackRequest = true;
                    }
                    if (ItemListInItemPanel != null)
                    {
                        ItemListInItemPanel();
                    }
                }
                else
                {
                    if (themsg.BagType == ItemType.MainItemBagType)
                    {
                        ItemModeLocator.Instance.ScAllItemInfos = themsg;
                        WindowManager.Instance.GetWindow<UIHeroCommonWindow>().HeroDetailHandler.RefreshCanEquipItems();
                        ItemModeLocator.AlreadyMainRequest = true;
                    }
                }
            }
        }

        public static void OnServerConfigMsg(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCServerConfigMsg;
            if (themsg != null)
            {
                ItemModeLocator.Instance.ServerConfigMsg = themsg;
                EnergyIncreaseControl.Instance.Init(themsg.RecoverEnergyMinutes);
                MailModelLocator.Instance.MailUpdateInterval = themsg.MailUpdateInterval;

                Debug.Log("Go to finish in itemHandler.");
                //WindowManager.Instance.Show<LoginAccountWindow>(false);
                //WindowManager.Instance.Show<LoginCreateRoleWindow>(false);
                //WindowManager.Instance.Show<LoginMainWindow>(false);
                //WindowManager.Instance.Show<LoginRegisterWindow>(false);
                //WindowManager.Instance.Show<LoginServersWindow>(false);
                //WindowManager.Instance.Show<LoadingWaitWindow>(false);
                //WindowManager.Instance.Show(WindowGroupType.Popup, false);

                GreenHandGuideHandler.Instance.ShowMainScreen();

                Debug.Log("Go to persistence way.");
                //BattlePersistence
                PersistenceHandler.Instance.GoToPersistenceWay();
            }
        }

        public static void OnAddItem(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCAddItem;
            if (themsg != null)
            {
                ChooseCardHandler.AddToCacheItemList(new List<ItemInfo> {themsg.Info});
                var itemWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
                if(NGUITools.GetActive(itemWindow))
                {
                    itemWindow.Refresh(ItemModeLocator.Instance.ScAllItemInfos.ItemInfos);
                }
            }
        }

        public static void OnItemDetail(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCItemDetail;
            if (themsg != null)
            {
                ItemModeLocator.Instance.ItemDetail = themsg;
                var itemDetailHandler = WindowManager.Instance.GetWindow<UIItemCommonWindow>().ItemDetailHandler;
                itemDetailHandler.Refresh();
                //WindowManager.Instance.Show<UIItemDetailHandler>(true);
            }
        }

        public static void OnStrengthItemSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCStrengthItemSucc;
            if (themsg != null)
            {
                var deleteIndexs = themsg.DelteItems.DeleteIndexes;
                var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                infos.RemoveAll(item => deleteIndexs.Contains(item.BagIndex));
                var operationInfo = ItemModeLocator.Instance.FindItem(themsg.OperItemIndex);
                operationInfo.ContribExp = themsg.UpdateContribExp;
                WindowManager.Instance.GetWindow<UIItemCommonWindow>().LevelUpItemHandler.ShowLevelOver();
            }
        }

        public static void OnItemSellSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCItemSellSucc;
            if (themsg != null)
            {
                var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                foreach (var indexeChange in themsg.ItemIndexeChanges)
                {
                    var bagIndex = indexeChange.Key;
                    var buyBackIndex = indexeChange.Value;
                    var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
                    infos.Remove(itemInfo);
                    if (buyBackIndex != ItemType.InvalidBuyBackIndex)
                    {
                        if (ItemModeLocator.Instance.BuyBackItems != null && ItemModeLocator.Instance.BuyBackItems.ItemInfos != null)
                        {
                            var buyBackInfos = ItemModeLocator.Instance.BuyBackItems.ItemInfos;
                            itemInfo.BagIndex = buyBackIndex;
                            var expireDateTime = DateTime.Now + new TimeSpan(0, ItemModeLocator.Instance.ServerConfigMsg.SellItemSaveHours, 0, 0);
                            itemInfo.ExpireTime = Utils.ConvertToJavaTimestamp(expireDateTime);
                            buyBackInfos.Add(itemInfo);
                        }
                    }
                }
                var commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
                var sellItem = commonWindow.ItemSellHandler;
                sellItem.SellOverUpdate();
                commonWindow.Refresh(infos);
            }
        }

        public static void OnItemLockOperSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCItemLockOperSucc;
            if (themsg != null)
            {
                //for (int i = 0; i < ItemLockHandler.ChangedLockList.Count; i++)
                //{
                //    var bagIndex = ItemLockHandler.ChangedLockList[i];
                //    var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
                //    itemInfo.BindStatus = (sbyte)(1 - itemInfo.BindStatus);
                //}
                //WindowManager.Instance.GetWindow<UItemsWindow>().Refresh(ItemModeLocator.Instance.ScAllItemInfos.ItemInfos);
            }
        }

        public static void OnBuyBackItemSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCBuyBackItemSucc;
            if (themsg != null)
            {
                var buyBackInfos = ItemModeLocator.Instance.BuyBackItems.ItemInfos;
                var needUpdateMain = ItemModeLocator.Instance.ScAllItemInfos != null &&
                                     ItemModeLocator.Instance.ScAllItemInfos.ItemInfos != null;
                foreach (var indexeChange in themsg.ItemIndexeChanges)
                {
                    var buyBackIndex = indexeChange.Key;
                    var bagIndex = indexeChange.Value;
                    var itemInfo = ItemModeLocator.Instance.FindBuyBackItem(buyBackIndex);
                    itemInfo.BagIndex = bagIndex;
                    buyBackInfos.Remove(itemInfo);
                    if(needUpdateMain)
                    {
                        var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ??
                                    (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos = new List<ItemInfo>());
                        infos.Add(itemInfo);
                    }
                }
                //WindowManager.Instance.GetWindow<UIBuyBackItemsWindow>().BuyBackItemSucc();
            }
        }

        public static void OnEvoluteItemSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCEvoluteItemSucc;
            if (themsg != null)
            {
                var deleteIndexs = themsg.DelteItems.DeleteIndexes;
                var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                var operationBag = themsg.EvolutedItemInfo.Info.BagIndex;
                var operationInfo = ItemModeLocator.Instance.FindItem(operationBag);
                var operationId = operationInfo.Id;
                var evolvedInfo = themsg.EvolutedItemInfo.Info;
                UpdateHeroEquipItem(operationId, evolvedInfo.Id);
                deleteIndexs.Add(operationBag);
                infos.RemoveAll(item => deleteIndexs.Contains(item.BagIndex));
                infos.Add(themsg.EvolutedItemInfo.Info);
                var commomWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
                commomWindow.EvolveItemHandler.ShowEvolveOver(themsg.EvolutedItemInfo.Info);
                commomWindow.Refresh(infos);
            }
        }

        private static void UpdateHeroEquipItem(string operationId, string newId)
        {
            HeroInfo heroEquiped = null;
            var heros = HeroModelLocator.Instance.SCHeroList != null ? HeroModelLocator.Instance.SCHeroList.HeroList : null;
            var index = 0;
            if(heros != null)
            {
                foreach(var hero in heros)
                {
                    if(hero.EquipUuid.Contains(operationId))
                    {
                        index = hero.EquipUuid.IndexOf(operationId);
                        heroEquiped = hero;
                        break;
                    }
                }
            }
            if(heroEquiped != null)
            {
                heroEquiped.EquipUuid[index] = newId;
            }
        }

        public static void OnExtendItemBagSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCExtendItemBagSucc;
            if (themsg != null)
            {
                ItemModeLocator.Instance.ScAllItemInfos.Capacity = themsg.Capacity;
                WindowManager.Instance.GetWindow<UIItemCommonWindow>().RefreshItemMaxNum();
            }
        }
    }
}

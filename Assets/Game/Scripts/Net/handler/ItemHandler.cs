using System;
using System.Collections.Generic;
using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    class ItemHandler
    {
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
                        Utils.ShowWithoutDestory(typeof(UIEquipDispTabWindow));
                    }
                    if (themsg.BagType == ItemType.BuyBackItemBagType)
                    {
                        ItemModeLocator.Instance.BuyBackItems = themsg;
                        WindowManager.Instance.Show<BuyBackDialogWindow>(true);
                    }
                }
                else
                {
                    if (themsg.BagType == ItemType.MainItemBagType)
                    {
                        ItemModeLocator.Instance.ScAllItemInfos = themsg;
                        WindowManager.Instance.GetWindow<UIHeroDetailWindow>().RefreshCanEquipItems();
                        //WindowManager.Instance.GetWindow<HeroBaseInfoWindow>().ShowHeroSelItems();
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
            }
        }

        public static void OnAddItem(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCAddItem;
            if (themsg != null)
            {
                if(ItemModeLocator.Instance.ScAllItemInfos.ItemInfos == null)
                {
                    ItemModeLocator.Instance.ScAllItemInfos.ItemInfos = new List<ItemInfo>();
                }
                var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                infos.Add(themsg.Info);
                var itemsWindow = WindowManager.Instance.GetWindow<UItemsWindow>();
                itemsWindow.Refresh(infos);
                var viewHandler = WindowManager.Instance.GetWindow<UIEquipDispTabWindow>().ItemViewHandler;
                itemsWindow.ItemClicked = viewHandler.ItemInfoClicked;
                viewHandler.Refresh();
            }
        }

        public static void OnItemDetail(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCItemDetail;
            if (themsg != null)
            {
                ItemBaseInfoWindow.ItemDetail = themsg;
                var itemInfo = ItemModeLocator.Instance.FindItem(themsg.BagIndex);
                var type = ItemModeLocator.Instance.GetItemType(itemInfo.TmplId);
                if (type == ItemModeLocator.EquipType.MaterialTempl)
                {
                    WindowManager.Instance.Show<UIMaterialInfoWindow>(true);
                }
                if(type == ItemModeLocator.EquipType.EquipTempl || type == ItemModeLocator.EquipType.ArmorTemplate)
                {
                    WindowManager.Instance.Show<ItemBaseInfoWindow>(true);
                    WindowManager.Instance.Show<UIItemInfoWindow>(true);
                }
            }
        }

        public static void OnStrengthItemSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCStrengthItemSucc;
            if (themsg != null)
            {
                var deleteIndexs = themsg.DelteItems.DeleteIndexes;
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.RemoveAll(item => deleteIndexs.Contains(item.BagIndex));
                WindowManager.Instance.GetWindow<UItemsWindow>().DespawnItems(deleteIndexs);
                WindowManager.Instance.GetWindow<UIItemLevelUpWindow>().ShowLevelOver();
            }
        }

        public static void OnItemSellSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCItemSellSucc;
            if (themsg != null)
            {
                WindowManager.Instance.GetWindow<UIEquipDispTabWindow>().ItemSellHandler.CleanUp();
                var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                foreach (var indexeChange in themsg.ItemIndexeChanges)
                {
                    var bagIndex = indexeChange.Key;
                    var buyBackIndex = indexeChange.Value;
                    var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
                    infos.Remove(itemInfo);
                    if (ItemModeLocator.Instance.BuyBackItems != null && ItemModeLocator.Instance.BuyBackItems.ItemInfos != null)
                    {
                        var buyBackInfos = ItemModeLocator.Instance.BuyBackItems.ItemInfos;
                        itemInfo.BagIndex = buyBackIndex;
                        var expireDateTime = DateTime.Now + new TimeSpan(0, ItemModeLocator.Instance.ServerConfigMsg.SellItemSaveHours, 0, 0);
                        itemInfo.ExpireTime = Utils.ConvertToJavaTimestamp(expireDateTime);
                        buyBackInfos.Add(itemInfo);       
                    }
                }
                var window = WindowManager.Instance.GetWindow<UItemsWindow>();
                window.Refresh(infos);
            }
        }

        public static void OnItemLockOperSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCItemLockOperSucc;
            if (themsg != null)
            {
                for (int i = 0; i < ItemLockHandler.ChangedLockList.Count; i++)
                {
                    var bagIndex = ItemLockHandler.ChangedLockList[i];
                    var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
                    itemInfo.BindStatus = (sbyte)(1 - itemInfo.BindStatus);
                }
                WindowManager.Instance.GetWindow<UItemsWindow>().Refresh(ItemModeLocator.Instance.ScAllItemInfos.ItemInfos);
            }
        }

        public static void OnBuyBackItemSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCBuyBackItemSucc;
            if (themsg != null)
            {
                WindowManager.Instance.GetWindow<BuyBackDialogWindow>().CleanUp();
                var buyBackInfos = ItemModeLocator.Instance.BuyBackItems.ItemInfos;
                var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ??
                            (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos = new List<ItemInfo>());
                foreach (var indexeChange in themsg.ItemIndexeChanges)
                {
                    var buyBackIndex = indexeChange.Key;
                    var bagIndex = indexeChange.Value;
                    var itemInfo = ItemModeLocator.Instance.FindBuyBackItem(buyBackIndex);
                    itemInfo.BagIndex = bagIndex;
                    infos.Add(itemInfo);
                    buyBackInfos.Remove(itemInfo);
                }
                
                var window = WindowManager.Instance.GetWindow<UItemsWindow>();
                window.Refresh(infos);
                WindowManager.Instance.GetWindow<UIEquipDispTabWindow>().ItemSellHandler.Refresh();
            }
        }

        public static void OnEvoluteItemSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCEvoluteItemSucc;
            if (themsg != null)
            {
                var deleteIndexs = themsg.DelteItems.DeleteIndexes;
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.RemoveAll(item => deleteIndexs.Contains(item.BagIndex));
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Add(themsg.EvolutedItemInfo.Info);
                ItemBaseInfoWindow.ItemDetail = themsg.EvolutedItemDetail;
                WindowManager.Instance.GetWindow<UItemsWindow>().DespawnItems(deleteIndexs);
                WindowManager.Instance.Show<ItemBaseInfoWindow>(true);
                WindowManager.Instance.Show<UIItemInfoWindow>(true);
            }
        }

        public static void OnExtendItemBagSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCExtendItemBagSucc;
            if (themsg != null)
            {
                ItemModeLocator.Instance.ScAllItemInfos.Capacity = themsg.Capacity;
                WindowManager.Instance.GetWindow<UIEquipDispTabWindow>().ItemViewHandler.Refresh();
            }
        }
    }
}

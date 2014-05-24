using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class ItemHandler
    {
        public static void OnAllItemInfos(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCAllItemInfos;
            if (themsg != null)
            {
                ItemModeLocator.Instance.ScAllItemInfos = themsg;
                Utils.ShowWithoutDestory(typeof(UIEquipsDisplayWindow));
            }
        }

        public static void OnAddItem(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCAddItem;
            if (themsg != null)
            {
                if (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos == null)
                {
                    ItemModeLocator.Instance.ScAllItemInfos.ItemInfos = new List<ItemInfo>();
                }
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Add(themsg.Info);
                WindowManager.Instance.GetWindow<UIEquipItemsPageWindow>().Refresh();
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
    }
}

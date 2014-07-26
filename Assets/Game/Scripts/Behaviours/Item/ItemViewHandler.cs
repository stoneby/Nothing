using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

public class ItemViewHandler : MonoBehaviour
{
    #region Private Fields

    private UIEventListener extendBagLis;
    private List<ItemInfo> infos;
    private UILabel equipNums;
    private ExtendBag itemExtendConfirm;
    private UItemsWindow itemsWindow;

    #endregion 

    #region Public Fields

    public GameObject ItemExtendConfirm;

    #endregion

    #region Private Methods

    private void Awake()
    {
        extendBagLis = UIEventListener.Get(Utils.FindChild(transform, "ExtendBag").gameObject);
        equipNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
        itemsWindow = WindowManager.Instance.Show<UItemsWindow>(true);
        itemsWindow.Items.onReposition += OnReposition;
    }

    private void OnEnable()
    {
        extendBagLis.onClick = OnExtenBag;
        Refresh();
    }

    private void OnDisable()
    {
        extendBagLis.onClick = null;
        extendBagLis.gameObject.SetActive(false);
    }

    /// <summary>
    /// The callback of clicking add button.
    /// </summary>
    private void OnAdd(GameObject go)
    {
        var csmsg = new CSAddItemTest();
        NetManager.SendMessage(csmsg);
    }

    private void OnExtenBag(GameObject go)
    {
        itemExtendConfirm = NGUITools.AddChild(transform.gameObject, ItemExtendConfirm).GetComponent<ExtendBag>();
        var bases = ItemModeLocator.Instance.Bag;
        var costDict = bases.ItemExtTmpls.ToDictionary(item => item.Value.Id, item => item.Value.ExtendCost);
        itemExtendConfirm.Init(PlayerModelLocator.Instance.ExtendItemTimes + 1, bases.BagBaseTmpls[1].ExtendItemCount,
                               costDict);
        itemExtendConfirm.OkClicked += OnExendBagOk;
    }

    private void OnExendBagOk(GameObject go)
    {
        var msg = new CSExtendItemBag { ExtendSize = itemExtendConfirm.ExtendSize };
        NetManager.SendMessage(msg);
    }

    private void OnReposition()
    {
        var items = itemsWindow.Items.transform;
        var childCount = items.childCount;
        Utils.MoveToParent(items, extendBagLis.transform);
        if (childCount != 0)
        {
            var maxPerLine = itemsWindow.Items.maxPerLine;
            if (childCount % maxPerLine != 0)
            {
                extendBagLis.transform.localPosition = new Vector3((childCount % maxPerLine) * itemsWindow.Items.cellWidth,
                                                                   -itemsWindow.Items.cellHeight * (childCount / maxPerLine),
                                                                   0);
            }
            else
            {
                extendBagLis.transform.localPosition = new Vector3(0,
                                                                   -itemsWindow.Items.cellHeight * (childCount / maxPerLine),
                                                                   0);
            }
        }
        extendBagLis.transform.parent = items.parent;
    }

    #endregion

    #region Public Methods

    public void Refresh()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        var capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        equipNums.text = string.Format("{0}/{1}", infos.Count, capacity);
        extendBagLis.gameObject.SetActive(true);
    }

    public void ItemInfoClicked(GameObject go)
    {
        ItemModeLocator.Instance.GetItemDetailPos = ItemType.GetItemDetailInPanel;
        var bagIndex = go.GetComponent<EquipItem>().BagIndex;
        var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
        NetManager.SendMessage(csmsg);
    }

    #endregion
}

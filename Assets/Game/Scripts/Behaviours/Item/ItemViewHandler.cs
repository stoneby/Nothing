using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class ItemViewHandler : MonoBehaviour 
{
    private UIEventListener extendBagLis;
    private UIEventListener addOneLis;
    private List<ItemInfo> infos;
    private UILabel equipNums;
    private ExtendBag itemExtendConfirm;
    private UItemsWindow itemsWindow;

    public GameObject ItemExtendConfirm;

    private void Awake()
    {
        extendBagLis = UIEventListener.Get(Utils.FindChild(transform, "ExtendBag").gameObject);
        addOneLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Add").gameObject);
        equipNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
        itemsWindow = WindowManager.Instance.Show<UItemsWindow>(true);
        itemsWindow.Items.onReposition += OnReposition;
    }

    private void OnEnable()
    {
        addOneLis.onClick += OnAdd;
        extendBagLis.onClick += OnExtenBag;
        Refresh();
    }

    private void OnDisable()
    {
        addOneLis.onClick -= OnAdd;
        extendBagLis.onClick -= OnExtenBag;
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
        var costDict = bases.ItemExtTmpl.ToDictionary(item => item.Value.Id, item => item.Value.Cost);
        itemExtendConfirm.Init(PlayerModelLocator.Instance.ExtendItemTimes + 1, bases.BagBaseTmpl[1].EachExtItemNum,
                               costDict);
        itemExtendConfirm.OkClicked += OnExendBagOk;
    }

    private void OnExendBagOk(GameObject go)
    {
        var msg = new CSExtendItemBag { ExtendSize = itemExtendConfirm.ExtendSize };
        NetManager.SendMessage(msg);
    }

    public void Refresh()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        var capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        equipNums.text = string.Format("{0}/{1}", infos.Count, capacity);
        extendBagLis.gameObject.SetActive(true);
    }

    private void OnReposition()
    {
        var items = itemsWindow.Items.transform;
        var childCount = items.childCount;
        Utils.MoveToParent(items, extendBagLis.transform);
        if(childCount != 0)
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
                                                                   -itemsWindow.Items.cellHeight *(childCount / maxPerLine),
                                                                   0);
            }
        }
        extendBagLis.transform.parent = items.parent;
    }

    public void ItemInfoClicked(GameObject go)
    {
        ItemModeLocator.Instance.GetItemDetailPos = ItemType.GetItemDetailInPanel;
        var bagIndex = go.GetComponent<EquipItem>().BagIndex;
        var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
        NetManager.SendMessage(csmsg);
    }
}

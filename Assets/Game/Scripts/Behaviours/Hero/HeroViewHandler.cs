using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

public class HeroViewHandler : MonoBehaviour
{
    #region Private Fields

    private UIEventListener extendBagLis;
    private List<HeroInfo> infos;
    private UILabel heroNums;
    private ExtendBag itemExtendConfirm;
    private UIHerosPageWindow herosWindow;

    #endregion

    #region Public Fields

    public GameObject ExtendBagConfirm;

    #endregion

    #region Private Methods

    private void Awake()
    {
        extendBagLis = UIEventListener.Get(Utils.FindChild(transform, "ExtendBag").gameObject);
        heroNums = Utils.FindChild(transform, "HeroNums").GetComponent<UILabel>();
        herosWindow = WindowManager.Instance.Show<UIHerosPageWindow>(true);
        herosWindow.Heros.onReposition += OnReposition;
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

    private void OnExtenBag(GameObject go)
    {
        itemExtendConfirm = NGUITools.AddChild(transform.gameObject, ExtendBagConfirm).GetComponent<ExtendBag>();
        var bases = ItemModeLocator.Instance.Bag;
        var costDict = bases.HeroExtTmpl.ToDictionary(item => item.Value.Id, item => item.Value.Cost);
        itemExtendConfirm.Init(PlayerModelLocator.Instance.ExtendHeroTimes + 1, bases.BagBaseTmpl[1].EachExtItemNum,
                               costDict);
        itemExtendConfirm.OkClicked += OnExendBagOk;
    }

    private void OnExendBagOk(GameObject go)
    {
        var msg = new CSHeroMaxExtend
                      {
                          ExtendTimes = itemExtendConfirm.ExtendSize
                      };
        NetManager.SendMessage(msg);
    }

    private void OnReposition()
    {
        var items = herosWindow.Heros.transform;
        var childCount = items.childCount;
        Utils.MoveToParent(items, extendBagLis.transform);
        if (childCount != 0)
        {
            var maxPerLine = herosWindow.Heros.maxPerLine;
            if (childCount % maxPerLine != 0)
            {
                extendBagLis.transform.localPosition = new Vector3((childCount % maxPerLine) * herosWindow.Heros.cellWidth,
                                                                   -herosWindow.Heros.cellHeight * (childCount / maxPerLine),
                                                                   0);
            }
            else
            {
                extendBagLis.transform.localPosition = new Vector3(0,
                                                                   -herosWindow.Heros.cellHeight * (childCount / maxPerLine),
                                                                   0);
            }
        }
        extendBagLis.transform.parent = items.parent;
    }

    #endregion

    #region Public Methods

    public void Refresh()
    {
        infos = HeroModelLocator.Instance.SCHeroList.HeroList ?? new List<HeroInfo>();
        var capacity = PlayerModelLocator.Instance.HeroMax;
        heroNums.text = string.Format("{0}/{1}", infos.Count, capacity);
        extendBagLis.gameObject.SetActive(true);
    }

    public void OnHeroItemClicked(GameObject go)
    {
        HeroBaseInfoWindow.CurUuid = go.GetComponent<HeroItem>().Uuid;
        WindowManager.Instance.Show(typeof(HeroBaseInfoWindow), true);
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    #endregion
}

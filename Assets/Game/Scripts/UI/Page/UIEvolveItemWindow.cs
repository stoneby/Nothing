using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Linq;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEvolveItemWindow : Window
{
    private readonly List<GameObject> mats = new List<GameObject>();
    private readonly List<UILabel> matsOwnCount = new List<UILabel>(); 
    private GameObject source;
    private GameObject curClicked;
    private UIDragDropContainer dragDropContainer;
    private Transform targetBg;
    private UIGrid evolveMats;
    private GameObject mask;
    private UIEventListener evolveLis;
    private UIEventListener backLis;
    private const int MaterialCount = 6;
    private UIItemCommonWindow itemsWindow;

    /// <summary>
    /// The item which will show on the right.
    /// </summary>
    public GameObject BaseItemPrefab;

    #region Window

    public override void OnEnter()
    {
        itemsWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        itemsWindow.NormalClicked = OnNormalClicked;
        Init();
        evolveLis.onClick = OnEvolve;
        backLis.onClick = OnBack;
    }

    public override void OnExit()
    {
        evolveLis.onClick = null;
        backLis.onClick = null;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
        targetBg = transform.Find("PreShow/Target");
        mask = transform.Find("Mask").gameObject;
        NGUITools.SetActive(mask, false);
        evolveMats = transform.Find("EvolveMats").GetComponent<UIGrid>();
        var matBgs = transform.Find("MatBgs");
        for (var i = 1; i <= matBgs.childCount; i++)
        {
            matsOwnCount.Add(matBgs.Find("Mat"+i+"/Label").GetComponent<UILabel>());
        }
        evolveLis = UIEventListener.Get(transform.Find("Buttons/Button-Evolve").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
    }

    private void Init()
    {
        if(mats.Count == 0)
        {
            for (var i = 0; i < MaterialCount; i++)
            {
                mats.Add(null);
            }
        }
        else
        {
            for (var i = 0; i < MaterialCount; i++)
            {
                mats[i] = null;
            }
        }
    }

    private void OnEvolve(GameObject go)
    {
        var msg = new CSEvoluteItem
        {
            OperItemIndex = source.GetComponent<ItemBase>().BagIndex
        };
        NetManager.SendMessage(msg);
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIEvolveItemWindow>(false);
        WindowManager.Instance.Show<UIItemCommonWindow>(false);
    }

    private void OnNormalClicked(GameObject go)
    {
        curClicked = go;
        var bagIndex = curClicked.GetComponent<ItemBase>().BagIndex;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        UIItemSnapShotWindow.ItemInfo = info;
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true);
        var isEnabled = ItemModeLocator.Instance.ItemConfig.ItemEvoluteTmpls.ContainsKey(info.TmplId);
        if (mats[0] == null)
        {
            snapShot.InitTemplate("ItemSnapShot.Evolve", GotoEvovle, isEnabled);
        }
        else if (bagIndex == source.GetComponent<ItemBase>().BagIndex)
        {
            snapShot.InitTemplate("ItemSnapShot.CancelEvolve", CancelEvovle, isEnabled);
        }
        else
        {
            snapShot.InitTemplate("ItemSnapShot.Evolve", GotoEvovle, false);
        }
    }

    private void CancelEvovle(GameObject go)
    {
        CleanUp();
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void CleanUp()
    {
        NGUITools.Destroy(source.transform.Find("Mask(Clone)").gameObject);
        for(var i = 0; i < mats.Count; i++)
        {
            var mat = mats[i];
            if(mat != null)
            {
                NGUITools.Destroy(mat);
                mats[i] = null;
            }
        }

        for (var i = 0; i < matsOwnCount.Count; i++)
        {
            matsOwnCount[i].text = "";
        }
    }

    private void GotoEvovle(GameObject go)
    {
        var bagIndex = curClicked.GetComponent<ItemBase>().BagIndex;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        source = curClicked;
        var maskClone = NGUITools.AddChild(source, mask);
        NGUITools.SetActive(maskClone, true);
        GameObject mat = NGUITools.AddChild(dragDropContainer.gameObject, BaseItemPrefab);
        mat.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClicked;
        mat.GetComponent<ItemBase>().InitItem(info);
        mats[0] = mat;

        var evoluteTmp = ItemModeLocator.Instance.ItemConfig.ItemEvoluteTmpls[info.TmplId];
        mat = NGUITools.AddChild(targetBg.gameObject, BaseItemPrefab);
        mat.GetComponent<ItemBase>().InitItem(evoluteTmp.TargetItemId);
        mats[1] = mat;
        var isDirty = false;
        for (var i = 0; i < evoluteTmp.NeedMaterials.Count; i++)
        {
            var evoluteParam = evoluteTmp.NeedMaterials[i];
            var count = FindMaterialCount(evoluteParam.NeedMaterialId);
            if (count > 0)
            {
                mat = NGUITools.AddChild(evolveMats.gameObject, BaseItemPrefab);
                mat.GetComponent<NGUILongPress>().OnNormalPress = NormalPressForMatInfo;
                mat.GetComponent<ItemBase>().InitItem(evoluteParam.NeedMaterialId);
                //The first two are used to store the main and target item. 
                mats[2+i] = mat;
                matsOwnCount[0].text = string.Format("{0}/{1}", count, evoluteParam.NeedMaterialCount);
                isDirty = true;
            }
        }
        if(isDirty)
        {
            evolveMats.repositionNow = true;
        }
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void NormalPressForMatInfo(GameObject go)
    {
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        var item = ItemModeLocator.Instance.FindItem(bagIndex);
        var showingWindow = WindowManager.Instance.Show<InfoShowingWindow>(true);
        showingWindow.Refresh(item, null);
    }

    private int FindMaterialCount(int id)
    {
        return ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Count(t => t.TmplId == id);
    }

    public void ShowEvolveOver()
    {
        CleanUp();
    }

    #endregion
}

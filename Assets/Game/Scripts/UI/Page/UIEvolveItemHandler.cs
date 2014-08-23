using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Linq;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEvolveItemHandler : MonoBehaviour
{
    private readonly List<GameObject> mats = new List<GameObject>();
    private readonly List<UILabel> matsOwnCount = new List<UILabel>(); 
    private GameObject source;
    private Transform evolveBg;
    private Transform targetBg;
    private UIGrid evolveMats;
    private UIEventListener evolveLis;
    private const int MaterialCount = 6;
    private UIItemCommonWindow commonWindow;

    /// <summary>
    /// The item which will show on the right.
    /// </summary>
    public GameObject BaseItemPrefab;

    #region Window

    private void OnEnable()
    {
        Init();
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.NormalClicked = OnNormalClicked;
        OnNormalClicked(commonWindow.CurSel);
        evolveLis.onClick = OnEvolve;
    }

    private void OnDisable()
    {
        evolveLis.onClick = null;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        evolveBg = transform.Find("PreShow/Evolve");
        targetBg = transform.Find("PreShow/Target");
        evolveMats = transform.Find("EvolveMats").GetComponent<UIGrid>();
        var matBgs = transform.Find("MatBgs");
        for (var i = 1; i <= matBgs.childCount; i++)
        {
            matsOwnCount.Add(matBgs.Find("Mat"+i+"/Label").GetComponent<UILabel>());
        }
        evolveLis = UIEventListener.Get(transform.Find("Button-Evolve").gameObject);
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

    private void OnNormalClicked(GameObject go)
    {
        commonWindow.CurSel = go;
        commonWindow.ShowSelMask(go.transform.position);
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        var mat = NGUITools.AddChild(evolveBg.gameObject, BaseItemPrefab);
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
                UIEventListener.Get(mat).onClick = NormalPressForMatInfo;
                mat.GetComponent<ItemBase>().InitItem(evoluteParam.NeedMaterialId);
                //The first two are used to store the main and target item. 
                mats[2 + i] = mat;
                matsOwnCount[0].text = string.Format("{0}/{1}", count, evoluteParam.NeedMaterialCount);
                isDirty = true;
            }
        }
        if (isDirty)
        {
            evolveMats.repositionNow = true;
        }
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

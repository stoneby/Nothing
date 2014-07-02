using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// The window to edit the hero team.
/// </summary>
public class UITeamEditWindow : Window
{
    #region Public Fields

    /// <summary>
    /// The hero prefab used in this window.
    /// </summary>
    public GameObject HeroPrefab;

    /// <summary>
    /// The depth of heros page's panel in this window showing.
    /// </summary>
    public int HerosPanelDepth = 10;

    #endregion

    #region Private Fields

    private UIEventListener backLis;
    private UIEventListener canceAlllLis;
    private UIEventListener okLis;
    private GameObject smallHero;
    private GameObject items;
    private GameObject mask;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private UILabel heroNums;
    private SCHeroList scHeroList;
    private int totalAttack;
    private int totalHp;
    private int totalRecover;
    private int totalMp;
    private readonly Vector3 maskOffset = new Vector3(0, 10, 0);
    private readonly List<int> minHeroIndex = new List<int> { 1, 2, 3 };

    /// <summary>
    /// The key is the uuid of the hero, the value is the position in the team.
    /// </summary>
    private readonly Dictionary<long, int> selectedItems = new Dictionary<long, int>();
    private UIHerosPageWindow cachedHerosWindow;
    private int cachedRow;
    private UIEventListener.VoidDelegate heroClickDelegate;
    private static bool isShuttingDown;

    private int TotalAttack
    {
        get { return totalAttack; }
        set
        {
            totalAttack = value;
            attack.text = totalAttack.ToString(CultureInfo.InvariantCulture);
        }
    }

    private int TotalHp
    {
        get { return totalHp; }
        set
        {
            totalHp = value;
            hp.text = totalHp.ToString(CultureInfo.InvariantCulture);
        }
    }  
    
    private int TotalRecover
    {
        get { return totalRecover; }
        set
        {
            totalRecover = value;
            recover.text = totalRecover.ToString(CultureInfo.InvariantCulture);
        }
    }

    private int TotalMp
    {
        get { return totalMp; }
        set
        {
            totalMp = value;
            mp.text = totalMp.ToString(CultureInfo.InvariantCulture);
        }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        Init();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        CleanUp();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        backLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        canceAlllLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CancelAll").gameObject);
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Ok").gameObject); 
        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "Attack-Value").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HP-Value").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "Recover-Value").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MP-Value").GetComponent<UILabel>();
        smallHero = Utils.FindChild(transform, "SmallHero").gameObject;
        items = Utils.FindChild(transform, "Items").gameObject;
        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
        smallHero.SetActive(false);
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        heroNums = Utils.FindChild(transform, "HeroNums").GetComponent<UILabel>();
    }

    /// <summary>
    /// Init some varibles.
    /// </summary>
    private void Init()
    {
        cachedHerosWindow = WindowManager.Instance.Show<UIHerosPageWindow>(true);
        cachedRow = cachedHerosWindow.RowToShow;
        cachedHerosWindow.RowToShow = HeroConstant.TwoRowsVisible;
        heroClickDelegate = cachedHerosWindow.ItemClicked;
        cachedHerosWindow.ItemClicked = OnHeroItemClicked;
        cachedHerosWindow.Depth = HerosPanelDepth;
        cachedHerosWindow.OnSortOrderChanged += SortOrderChanged;
        ResetProperty();
    }

    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    /// <param name="go">The sender of the event.</param>
    private void SortOrderChanged(GameObject go)
    {
        var herosTran = cachedHerosWindow.Heros.transform;
        for (int i = 0; i < herosTran.childCount; i++)
        {
            var child = herosTran.GetChild(i);
            var maskToDel = child.FindChild("Mask(Clone)");
            if(maskToDel != null)
            {
                Destroy(maskToDel.gameObject);
            }
        }
        var uuids = selectedItems.Keys.ToList();
        for (var i = 0; i < uuids.Count; i++)
        {
            var uuid = uuids[i];
            for (var j = 0; j < herosTran.childCount; j++)
            {
                var child = herosTran.GetChild(j);
                if (child.GetComponent<HeroItem>().Uuid == uuid)
                {
                    var maskToShow = NGUITools.AddChild(child.gameObject, mask);
                    maskToShow.SetActive(true);
                    maskToShow.transform.localPosition = maskOffset;
                    maskToShow.transform.GetChild(0).GetComponent<UISprite>().spriteName = selectedItems[uuid].ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }

    /// <summary>
    /// Do the clean up job.
    /// </summary>
    private void CleanUp()
    {
        if (!isShuttingDown)
        {
            cachedHerosWindow.RowToShow = cachedRow;
        }
        cachedHerosWindow.ItemClicked = heroClickDelegate;
        cachedHerosWindow.ResetDepth();
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.Show<UIHerosPageWindow>(false);
        }
        CleanEditItems();
        cachedHerosWindow.OnSortOrderChanged -= SortOrderChanged;
    }

    /// <summary>
    /// Reset the values of properties.
    /// </summary>
    private void ResetProperty()
    {
        TotalAttack = 0;
        TotalHp = 0;
        totalRecover = 0;
        TotalMp = 0;
    }

    /// <summary>
    /// The call back of application quit.
    /// </summary>
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        backLis.onClick = OnBackClick;
        canceAlllLis.onClick = OnCancelAllClick;
        okLis.onClick = OnOkClick;
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        backLis.onClick = null;
        canceAlllLis.onClick = null;
        okLis.onClick = null;
    }

    /// <summary>
    /// The callback of clicking back button.
    /// </summary>
    private void OnBackClick(GameObject go)
    {
        OnCancelAllClick(null);
        WindowManager.Instance.Show(typeof(UITeamEditWindow), false);
    }

    /// <summary>
    /// The callback of clicking cancel all button.
    /// </summary>
    private void OnCancelAllClick(GameObject go)
    {
        CleanEditItems();
    }

    /// <summary>
    /// Clean up the edited items.
    /// </summary>
    private void CleanEditItems()
    {
        var uuids = selectedItems.Keys.ToList();
        var herosTran = cachedHerosWindow.Heros.transform;
        for (var i = 0; i < uuids.Count; i++)
        {
            var uuid = uuids[i];
            for (var j = 0; j < herosTran.childCount; j++)
            {
                var child = herosTran.GetChild(j);
                if (child.GetComponent<HeroItem>().Uuid == uuid)
                {
                    var maskToDel = child.FindChild("Mask(Clone)");
                    Destroy(maskToDel.gameObject);

                    var item = items.transform.FindChild("Item" + selectedItems[uuid]);
                    var smallHeroToDel = item.FindChild("SmallHero(Clone)");
                    Destroy(smallHeroToDel.gameObject);
                }
            }   
        }
        selectedItems.Clear();
        okLis.GetComponent<UIButton>().isEnabled = false;
    }

    /// <summary>
    /// The callback of clicking ok button.
    /// </summary>
    private void OnOkClick(GameObject go)
    {
        Dictionary<int, long> temp = selectedItems.ToDictionary(item => item.Value, item => item.Key);
        var keys = temp.Keys.ToList();
        keys.Sort();
        var uUids = new List<long>();
        int index = 0;
        int count = keys.Count;
        for (int keyIndex = 0; keyIndex < HeroConstant.MaxHerosPerTeam; keyIndex++)
        {
            if (index < count && keyIndex + 1 == keys[index])
            {
                uUids.Add(temp[keyIndex + 1]);
                index++;
            }
            else
            {
                uUids.Add(HeroConstant.NoneInitHeroUuid);
            }
        }
        var csmsg = new CSHeroModifyTeam
                        {
                            TeamIndex = scHeroList.CurrentTeamIndex,
                            NewTeamList =uUids,
                        };
        NetManager.SendMessage(csmsg);
    }

    /// <summary>
    /// The callback of clicking ok each hero item.
    /// </summary>
    private void OnHeroItemClicked(GameObject go)
    {
        var uUid = go.GetComponent<HeroItem>().Uuid;
        if (selectedItems.ContainsKey(uUid))
        {
            var item = items.transform.FindChild("Item" + selectedItems[uUid]);
            Destroy(item.FindChild("SmallHero(Clone)").gameObject);
            Destroy(go.transform.FindChild("Mask(Clone)").gameObject);
            RemoveSelectedItem(uUid);
            RefreshProperty(uUid, false);
        }
        else
        {
            if (selectedItems.Count >= HeroConstant.MaxHerosPerTeam)
            {
                return;
            }
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var values = selectedItems.Values.ToList();
            var firstValue = list.Except(values).ToList()[0];

            var maskToShow = NGUITools.AddChild(go, mask);
            maskToShow.SetActive(true);
            maskToShow.transform.localPosition = maskOffset;
            maskToShow.transform.GetChild(0).GetComponent<UISprite>().spriteName = firstValue.ToString(CultureInfo.InvariantCulture);

            var item = items.transform.FindChild("Item" + firstValue);
            var child = NGUITools.AddChild(item.gameObject, smallHero);
            child.SetActive(true);
            AddSelectedItem(uUid, firstValue);
            RefreshProperty(uUid, true);
        }

    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Refresh()
    {
        var infos = HeroModelLocator.Instance.SCHeroList.HeroList ?? new List<HeroInfo>();
        var capacity = PlayerModelLocator.Instance.HeroMax;
        heroNums.text = string.Format("{0}/{1}", infos.Count, capacity);
        selectedItems.Clear();
        var index = scHeroList.CurrentTeamIndex;
        var curTeamUuids = scHeroList.TeamList[index].ListHeroUuid;
        var herosTran = cachedHerosWindow.Heros.transform;
        for (var heroIndex = 0; heroIndex < herosTran.childCount; heroIndex++)
        {
            var item = herosTran.GetChild(heroIndex);
            var uUid = item.GetComponent<HeroItem>().Uuid;
            if (curTeamUuids.Contains(uUid))
            {
                var numIndex = curTeamUuids.IndexOf(uUid) + 1;
                var maskToShow = NGUITools.AddChild(item.gameObject, mask);
                maskToShow.SetActive(true);
                maskToShow.transform.localPosition = maskOffset;
                maskToShow.transform.GetChild(0).GetComponent<UISprite>().spriteName = numIndex.ToString(CultureInfo.InvariantCulture);

                var smallItem = items.transform.FindChild("Item" + numIndex);
                var child = NGUITools.AddChild(smallItem.gameObject, smallHero);
                child.SetActive(true);
                AddSelectedItem(uUid, curTeamUuids.IndexOf(uUid) + 1);
                RefreshProperty(uUid, true);
            }
        }
    }

    /// <summary>
    /// Refresh the property of heros.
    /// </summary>
    /// <param name="uUid">The uUid of the hero to be used to update the property.</param>
    /// <param name="add">If true, all properties will be increased with corresponding attribute values of the special hero.</param>
    private void RefreshProperty(long uUid, bool add)
    {
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        int flag = add ? 1 : -1;

        TotalAttack += heroInfo.Prop[RoleProperties.ROLE_ATK] * flag;
        TotalHp += heroInfo.Prop[RoleProperties.ROLE_HP] * flag;
        TotalRecover += heroInfo.Prop[RoleProperties.ROLE_RECOVER] * flag;
        TotalMp += heroInfo.Prop[RoleProperties.ROLE_MP] * flag;

    }

    /// <summary>
    /// Add the key/value pair to the selected item dictionary.
    /// </summary>
    /// <param name="uuidToAdd">The key of the item pair to be added.</param>
    /// <param name="numIndex">The value of the item pair to be added.</param>
    private void AddSelectedItem(long uuidToAdd, int numIndex)
    {
        selectedItems.Add(uuidToAdd, numIndex);

        //Set the color to normal.
        if (selectedItems.Values.ToList().Intersect(minHeroIndex).Count() == minHeroIndex.Count)
        {
            okLis.GetComponent<UIButton>().isEnabled = true;
        }
    }

    /// <summary>
    /// Considering performance, we don't check the existence, the user need take care of it.
    /// </summary>
    /// <param name="uuidToAdd">The uuid of hero item need removed from the selected item dictionary.</param>
    private void RemoveSelectedItem(long uuidToAdd)
    {
        selectedItems.Remove(uuidToAdd);
        if (selectedItems.Values.ToList().Intersect(minHeroIndex).Count() < minHeroIndex.Count)
        {
            okLis.GetComponent<UIButton>().isEnabled = false;
        }
    }

    #endregion
}

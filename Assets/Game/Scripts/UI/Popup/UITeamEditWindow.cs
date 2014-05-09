using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller. Need optimized in the future.
/// </summary>
public class UITeamEditWindow : Window
{
    #region Public Fields

    public GameObject HeroPrefab;

    public const long DefaultNonHero = -1;
    public const int MaxHeroCount = 9;

    #endregion

    #region Private Fields

    private UIEventListener backLis;
    private UIEventListener canceAlllLis;
    private UIEventListener okLis;
    private UIEventListener sortBtnLis;
    private UIGrid herosGrid;
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

    private readonly Dictionary<GameObject, int> selectedItems = new Dictionary<GameObject, int>();
    private readonly Vector3 maskOffset = new Vector3(0, 10, 0);
    private readonly List<int> minHeroIndex = new List<int> { 1, 2, 3 };
    private readonly Color greyOkColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
    private UILabel sortLabel;

    #endregion

    #region Window

    public override void OnEnter()
    {
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        InstallHandlers();
        Reset();
        FillHeroList();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        backLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        canceAlllLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CancelAll").gameObject);
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Ok").gameObject); 
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();

        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "Attack-Value").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HP-Value").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "Recover-Value").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MP-Value").GetComponent<UILabel>();
        heroNums = Utils.FindChild(transform, "HeroNums").GetComponent<UILabel>();
        herosGrid = GetComponentInChildren<UIGrid>();

        smallHero = Utils.FindChild(transform, "SmallHero").gameObject;
        items = Utils.FindChild(transform, "Items").gameObject;
        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
        smallHero.SetActive(false);
        scHeroList = HeroModelLocator.Instance.SCHeroList;
    }

    private void InstallHandlers()
    {
        backLis.onClick += OnBackClick;
        canceAlllLis.onClick += OnCancelAllClick;
        okLis.onClick += OnOkClick;
        sortBtnLis.onClick += OnSortClicked;
    }

    private void UnInstallHandlers()
    {
        backLis.onClick -= OnBackClick;
        canceAlllLis.onClick -= OnCancelAllClick;
        okLis.onClick -= OnOkClick;
        sortBtnLis.onClick -= OnSortClicked;
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        orderType = (sbyte)((orderType + 1) % StringTable.SortStrings.Count);
        scHeroList.OrderType = orderType;
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        HeroModelLocator.Instance.SortHeroList(orderType, scHeroList.HeroList);
        for (int i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var item = herosGrid.transform.GetChild(i);
            var uUid = scHeroList.HeroList[i].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            ShowHero(orderType, item, uUid);
        }
    }

    private void OnBackClick(GameObject go)
    {
        OnCancelAllClick(null);
        WindowManager.Instance.Show(typeof(UITeamEditWindow), false);
    }

    private void OnCancelAllClick(GameObject go)
    {
        var objList = selectedItems.Keys.ToList();
        for (int index = 0; index < objList.Count; index++)
        {
            var mask = objList[index].transform.FindChild("Mask(Clone)");
            Destroy(mask.gameObject);

            var item = items.transform.FindChild("Item" + selectedItems[objList[index]]);
            var smallHero = item.FindChild("SmallHero(Clone)");
            Destroy(smallHero.gameObject);
        }
        selectedItems.Clear();
        okLis.GetComponent<UISprite>().color = Color.white;
        okLis.enabled = false;
        Reset();
    }

    private void OnOkClick(GameObject go)
    {
        Dictionary<int, GameObject> temp = selectedItems.ToDictionary(item => item.Value, item => item.Key);
        var keys = temp.Keys.ToList();
        keys.Sort();
        var uUids = new List<long>();
        int index = 0;
        int count = keys.Count;
        for(int keyIndex = 0; keyIndex < MaxHeroCount; keyIndex++)
        {
            if (index < count && keyIndex + 1 == keys[index])
            {
                uUids.Add(temp[keyIndex + 1].GetComponent<HeroInfoPack>().Uuid);
                index++;
            }
            else
            {
                uUids.Add(DefaultNonHero);
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
    /// This will be optimized in future, as this is done similar thing in ui hero items page window.
    /// Some other functions will be also optimized in the future.
    /// </summary> 
    private void FillHeroList()
    {
        var heroCount = scHeroList.HeroList.Count;
        for (int index = 0; index < heroCount; index++)
        {
            var item = NGUITools.AddChild(herosGrid.gameObject, HeroPrefab);
            item.name = item.name + index;
            UIEventListener.Get(item).onClick += OnHeroItemClicked;
        }
        herosGrid.Reposition();
    }

    private void OnHeroItemClicked(GameObject go)
    {
        var uUid = go.GetComponent<HeroInfoPack>().Uuid;
        if (selectedItems.ContainsKey(go))
        {
            var item = items.transform.FindChild("Item" + selectedItems[go]);
            Destroy(item.FindChild("SmallHero(Clone)").gameObject);
            Destroy(go.transform.FindChild("Mask(Clone)").gameObject);
            RemoveSelectedItem(go);
            RefreshProperty(uUid, false);
        }
        else
        {
            if (selectedItems.Count >= MaxHeroCount)
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
            AddSelectedItem(go, firstValue);
            RefreshProperty(uUid, true);
        }

    }

    private void Reset()
    {
        attack.text = "0";
        hp.text = "0";
        recover.text = "0";
        mp.text = "0";

        totalAttack = 0;
        totalHp = 0;
        totalRecover = 0;
        totalMp = 0;
    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Refresh()
    {
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        selectedItems.Clear();
        var index = scHeroList.CurrentTeamIndex;
        var uUids = scHeroList.TeamList[index].ListHeroUuid;
        var orderType = scHeroList.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, scHeroList.HeroList);
        for (int heroIndex = 0; heroIndex < scHeroList.HeroList.Count; heroIndex++)
        {
            var item = herosGrid.transform.GetChild(heroIndex);
            var uUid = scHeroList.HeroList[heroIndex].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            ShowHero(orderType, item, uUid);
            if (uUids.Contains(uUid))
            {
                var numIndex = uUids.IndexOf(uUid) + 1;
                var maskToShow = NGUITools.AddChild(item.gameObject, mask);
                maskToShow.SetActive(true);
                maskToShow.transform.localPosition = maskOffset;
                maskToShow.transform.GetChild(0).GetComponent<UISprite>().spriteName = numIndex.ToString(CultureInfo.InvariantCulture);

                var smallItem = items.transform.FindChild("Item" + numIndex);
                var child = NGUITools.AddChild(smallItem.gameObject, smallHero);
                child.SetActive(true);
                AddSelectedItem(item.gameObject, uUids.IndexOf(uUid) + 1);
                RefreshProperty(uUid, true);
            }
        }
    }

    private void RefreshProperty(long uUid, bool add)
    {
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        int flag = add ? 1 : -1;

        totalAttack += heroInfo.Prop[RoleProperties.HERO_ATK] * flag;
        totalHp += heroInfo.Prop[RoleProperties.HERO_HP] * flag;
        totalRecover += heroInfo.Prop[RoleProperties.HERO_RECOVER] * flag;
        totalMp += heroInfo.Prop[RoleProperties.HERO_MP] * flag;

        attack.text = totalAttack.ToString(CultureInfo.InvariantCulture);
        hp.text = totalHp.ToString(CultureInfo.InvariantCulture);
        recover.GetComponent<UILabel>().text = totalRecover.ToString(CultureInfo.InvariantCulture);
        mp.text = totalMp.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Add the key/value pair to the selected item dictionary.
    /// </summary>
    /// <param name="itemToAdd">The key of the item pair to be added.</param>
    /// <param name="numIndex">The value of the item pair to be added.</param>
    private void AddSelectedItem(GameObject itemToAdd, int numIndex)
    {
        selectedItems.Add(itemToAdd, numIndex);

        //Set the color to normal.
        if (selectedItems.Values.ToList().Intersect(minHeroIndex).Count() == minHeroIndex.Count)
        {
            okLis.GetComponent<UISprite>().color = Color.white;
            okLis.GetComponent<BoxCollider>().enabled = true;
        }
    }

    /// <summary>
    /// Considering performance, we don't check the existence, the user need take care of it.
    /// </summary>
    /// <param name="itemToAdd">The game object need removed from the selected item dictionary.</param>
    private void RemoveSelectedItem(GameObject itemToAdd)
    {
        selectedItems.Remove(itemToAdd);
        //Set the color to grey.
        if (selectedItems.Values.ToList().Intersect(minHeroIndex).Count() < minHeroIndex.Count)
        {
            okLis.GetComponent<UISprite>().color = greyOkColor;
            okLis.GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// Show the info of the hero.
    /// </summary>
    /// <param name="orderType">The order type of </param>
    /// <param name="heroTran">The transform of hero.</param>
    /// <param name="uUid">The template id of hero.</param>
    private void ShowHero(short orderType, Transform heroTran, long uUid)
    {
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        var sortRelated = Utils.FindChild(heroTran, "SortRelated");
        var stars = Utils.FindChild(heroTran, "Rarity");
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = heroTemplate.Star; index < stars.transform.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        switch (orderType)
        {
            //ÈëÊÖË³ÐòÅÅÐò
            case 0:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //Îä½«Ö°ÒµÅÅÐò
            case 1:
                ShowByJob(sortRelated, heroInfo, heroTemplate);
                break;

            //Îä½«Ï¡ÓÐ¶ÈÅÅÐò
            case 2:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //ÕÕ¶ÓÎéË³ÐòÅÅÐò
            case 3:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //¹¥»÷Á¦ÅÅÐò
            case 4:
                ShowByJob(sortRelated, heroInfo, heroTemplate);
                break;

            //HPÅÅÐò
            case 5:
                ShowByHp(sortRelated, heroInfo);
                break;

            //»Ø¸´Á¦ÅÅÐò
            case 6:
                ShowByRecover(sortRelated, heroInfo);
                break;

            //µÈ¼¶ÅÅÐò
            case 7:
                ShowByLvl(sortRelated, heroInfo);
                break;
        }
    }

    private void ShowByJob(Transform sortRelated, HeroInfo heroInfo, HeroTemplate heroTemplate)
    {
        var jobSymobl = Utils.FindChild(sortRelated, "JobSymbol").GetComponent<UISprite>();
        var attack = Utils.FindChild(sortRelated, "Attack").GetComponent<UILabel>();
        jobSymobl.spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
        attack.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(jobSymobl.gameObject, true);
        NGUITools.SetActive(attack.gameObject, true);
    }
    private void ShowByHp(Transform sortRelated, HeroInfo heroInfo)
    {
        var hp = Utils.FindChild(sortRelated, "HP-Title");
        var hpValue = Utils.FindChild(sortRelated, "HP-Value").GetComponent<UILabel>();
        hpValue.text = heroInfo.Prop[RoleProperties.HERO_HP].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(hp.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
    }
    private void ShowByRecover(Transform sortRelated, HeroInfo heroInfo)
    {
        var recover = Utils.FindChild(sortRelated, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelated, "Recover-Value").GetComponent<UILabel>();
        recoverValue.text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(recover.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
    }
    private void ShowByLvl(Transform sortRelated, HeroInfo heroInfo)
    {
        var lvTitle = Utils.FindChild(sortRelated, "LV-Title");
        var lvValue = Utils.FindChild(sortRelated, "LV-Value").GetComponent<UILabel>();
        lvValue.text = heroInfo.Lvl.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(lvTitle.gameObject, true);
        NGUITools.SetActive(lvValue.gameObject, true);
    }

    #endregion
}

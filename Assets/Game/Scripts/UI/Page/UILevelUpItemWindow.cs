using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UILevelUpItemWindow : Window
{
    #region Public Fields

    /// <summary>
    /// The particle system to show the level up.
    /// </summary>
    public GameObject LevelUpEffect;

    /// <summary>
    /// The item which will show on the right.
    /// </summary>
    public GameObject BaseItemPrefab;

    #endregion

    private UIEventListener lvlUpLis;
    private UIEventListener backLis;
    private UIDragDropContainer mainContainer;
    private UIDragDropContainer matsContainer;
    private readonly List<GameObject> mats = new List<GameObject>();
    private readonly List<GameObject> sources =  new List<GameObject>();
    private GameObject curClicked;
    private const int MaxMatCount = 5;
    private UIItemCommonWindow itemsWindow;
    private UILabel mainItemName;
    private UILabel atk;
    private UILabel hp;
    private UILabel recover;
    private UILabel lvl;
    private UILabel changedAtk;
    private UILabel changedHp;
    private UILabel changedRecover;
    private UILabel changedMp;
    private UISlider atkForeShowSlider;
    private UISlider atkSlider;
    private UISlider hpForeShowSlider;
    private UISlider hpSlider;
    private UISlider recoverForeShowSlider;
    private UISlider recoverSlider;
    private UISlider lvlForeShowSlider;
    private UISlider lvlSlider;
    private GameObject mask;


    private int maxAtk;
    private int maxHp;
    private int maxRecover;
    private short maxLvl;

    private ItemInfo mainInfo;

    private ItemInfo MainInfo
    {
        get { return mainInfo;}
        set
        {
            ResetData();
            mainInfo = value;
            if (value != null)
            {
                maxLvl = value.MaxLvl;
                maxAtk = ItemModeLocator.Instance.GetAttack(value.TmplId, maxLvl);
                maxHp = ItemModeLocator.Instance.GetHp(value.TmplId, maxLvl);
                maxRecover = ItemModeLocator.Instance.GetRecover(value.TmplId, maxLvl);
                mainItemName.text = ItemModeLocator.Instance.GetName(value.TmplId);
                Refresh(value.Level);
            }
        }
    }

    private void Refresh(short level)
    {
        if(level <= maxLvl)
        {
            lvl.text = level.ToString(CultureInfo.InvariantCulture);
            lvlSlider.value = (float)level / maxLvl;
            var attack = ItemModeLocator.Instance.GetAttack(MainInfo.TmplId, level);
            atk.text = attack.ToString(CultureInfo.InvariantCulture);
            atkSlider.value = (float)attack / maxAtk;
            var hpTemp = ItemModeLocator.Instance.GetHp(MainInfo.TmplId, level);
            hp.text = hpTemp.ToString(CultureInfo.InvariantCulture);
            hpSlider.value = (float)hpTemp / maxHp;
            var recoverTemp = ItemModeLocator.Instance.GetRecover(MainInfo.TmplId, level);
            recover.text = recoverTemp.ToString(CultureInfo.InvariantCulture);
            recoverSlider.value = (float)recoverTemp / maxRecover;
        }
    }

    private int expCanGet;

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.LevelUpItemWindow);
        itemsWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        itemsWindow.NormalClicked = OnNormalClicked;
        Init();
        InstallHandlers();
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.LevelUpItemWindow);
        UnInstallHandlers();
        CleanMats();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        mainContainer = transform.Find("MainContainer").GetComponent<UIDragDropContainer>();
        matsContainer = transform.Find("MatsContainer").GetComponent<UIDragDropContainer>();
        lvlUpLis = UIEventListener.Get(transform.Find("Buttons/Button-LevelUp").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        mainItemName = transform.Find("LvlUpName").GetComponent<UILabel>();
        var property = transform.Find("Property");
        atk = property.Find("Attack/AttackValue").GetComponent<UILabel>();
        changedAtk = property.Find("Attack/ChangedAtk").GetComponent<UILabel>();
        atkForeShowSlider = property.Find("Attack/ForeshowBar").GetComponent<UISlider>();
        atkSlider = property.Find("Attack/NormalBar").GetComponent<UISlider>();

        hp = property.Find("HP/HPValue").GetComponent<UILabel>();
        changedHp = property.Find("HP/ChangedHp").GetComponent<UILabel>();
        hpForeShowSlider = property.Find("HP/ForeshowBar").GetComponent<UISlider>();
        hpSlider = property.Find("HP/NormalBar").GetComponent<UISlider>();

        recover = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        changedRecover = property.Find("Recover/ChangedRecover").GetComponent<UILabel>();
        recoverForeShowSlider = property.Find("Recover/ForeshowBar").GetComponent<UISlider>();
        recoverSlider = property.Find("Recover/NormalBar").GetComponent<UISlider>();

        lvl = property.Find("Lvl/LvlValue").GetComponent<UILabel>();
        changedMp = property.Find("Lvl/ChangedLvl").GetComponent<UILabel>();
        lvlForeShowSlider = property.Find("Lvl/ForeshowBar").GetComponent<UISlider>();
        lvlSlider = property.Find("Lvl/NormalBar").GetComponent<UISlider>();

        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
    }

    private void InstallHandlers()
    {
        lvlUpLis.onClick = OnLevelUp;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        lvlUpLis.onClick = null;
        backLis.onClick = null;
    }

    private void OnLevelUp(GameObject go)
    {
        var choiceItemIndexs = new List<short>();
        for(var i = 1; i < mats.Count; i++)
        {
            if (mats[i] != null)
            {
                choiceItemIndexs.Add(mats[i].GetComponent<ItemBase>().BagIndex);
            }
        }
        if (choiceItemIndexs.Count > 0)
        {
            var msg = new CSStrengthItem
            {
                OperItemIndex = mainInfo.BagIndex,
                ChoiceItemIndexes = choiceItemIndexs
            };
            NetManager.SendMessage(msg); 
        }
    }

    private void OnNormalClicked(GameObject go)
    {
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        curClicked = go;
        UIItemSnapShotWindow.ItemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
        //已装备或已绑定的话，不响应
        bool isEnabled = (UIItemSnapShotWindow.ItemInfo.BindStatus == 0 &&
                          UIItemSnapShotWindow.ItemInfo.EquipStatus == 0);
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true);
        //If main item is not selected.
        if(mats[0] == null)
        {
            snapShot.InitTemplate("HeroOrItemSnapShot.Levelup", GotoLevelUp, isEnabled);
            return;
        }
        var index = -1;
        for(var i = 0; i < mats.Count; i++)
        {
            var mat = mats[i];
            if(mat != null && mat.GetComponent<ItemBase>().BagIndex == bagIndex)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            snapShot.InitTemplate("ItemSnapShot.LvlUpMat", GoToLvlMat, isEnabled);
        }
        else if(index == 0)
        {
            snapShot.InitTemplate("HeroOrItemSnapShot.CancelLevelup", CancelLevelUp, isEnabled);
        }
        else
        {
            snapShot.InitTemplate("ItemSnapShot.CancelLvlUpMat", CancelLvlMat, isEnabled);
        }
    }

    private void CancelLvlMat(GameObject go)
    {
        var index = -1;
        var bagIndex = curClicked.GetComponent<ItemBase>().BagIndex;
        for (var i = 1; i < sources.Count; i++)
        {
            var source = sources[i];
            if (source != null && source.GetComponent<ItemBase>().BagIndex == bagIndex)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            NGUITools.Destroy(mats[index].gameObject);
            mats[index] = null;
            NGUITools.Destroy(sources[index].transform.Find("Mask(Clone)").gameObject);
            sources[index] = null;
            WindowManager.Instance.Show<UIItemSnapShotWindow>(false);

            var mainType = ItemModeLocator.Instance.GetItemType(MainInfo.TmplId);
            expCanGet -= MainInfo.ContribExp * (ItemHelper.IsSameJobType(mainType, MainInfo.TmplId, bagIndex) ? 5 : 1);
            var changedLvl = GetLevelChanged();
            Refresh((short)(mainInfo.Level + changedLvl));
        }
    }

    private void GoToLvlMat(GameObject go)
    {
        var index = -1;
        for (var i = 1; i < mats.Count; i++)
        {
            var mat = mats[i];
            if (mat == null)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            var matsGrid = matsContainer.reparentTarget.gameObject;
            var child = NGUITools.AddChild(matsGrid, BaseItemPrefab);
            matsGrid.GetComponent<UIGrid>().repositionNow = true;
            child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClicked;
            mats[index] = child;
            sources[index] = curClicked;
            var bagIndex = curClicked.GetComponent<ItemBase>().BagIndex;
            var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
            child.GetComponent<ItemBase>().InitItem(itemInfo);
            var maskClone = NGUITools.AddChild(curClicked, mask);
            NGUITools.SetActive(maskClone, true);
            WindowManager.Instance.Show<UIItemSnapShotWindow>(false);

            var mainType = ItemModeLocator.Instance.GetItemType(MainInfo.TmplId);
            expCanGet += MainInfo.ContribExp * (ItemHelper.IsSameJobType(mainType, MainInfo.TmplId, bagIndex) ? 5 : 1);
            var changedLvl = GetLevelChanged();
            Refresh((short)(mainInfo.Level + changedLvl));
        }
    }

    private int GetLevelChanged()
    {
        var exp = expCanGet + MainInfo.CurExp;
        var itemLvlTmpls = ItemModeLocator.Instance.ItemConfig.ItemLvlTmpls;
        var level = MainInfo.Level;
        while (exp >= itemLvlTmpls[level].MaxExp)
        {
            exp -= itemLvlTmpls[level].MaxExp;
            level++;
        }
        return level - MainInfo.Level;
    }


    private void GotoLevelUp(GameObject go)
    {
        var child = NGUITools.AddChild(mainContainer.gameObject, BaseItemPrefab);
        child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClicked;
        mats[0] = child;
        sources[0] = curClicked;
        var bagIndex = curClicked.GetComponent<ItemBase>().BagIndex;
        MainInfo = ItemModeLocator.Instance.FindItem(bagIndex);
        child.GetComponent<ItemBase>().InitItem(MainInfo);
        var maskClone = NGUITools.AddChild(curClicked, mask);
        NGUITools.SetActive(maskClone, true);
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void Init()
    {
        mats.Clear();
        sources.Clear();
        for(var i = 0; i < MaxMatCount; i++)
        {
            mats.Add(null);
            sources.Add(null);
        }
    }

    private void CancelLevelUp(GameObject go)
    {
        CleanMats();
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void CleanMats()
    {
        for(var i = mats.Count - 1; i >= 0; i--)
        {
            if(mats[i] != null)
            {
                NGUITools.Destroy(mats[i].gameObject);
                mats[i] = null;
            }
            if(sources[i] != null)
            {
                NGUITools.Destroy(sources[i].transform.Find("Mask(Clone)").gameObject);
                sources[i] = null;
            }
        }
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UILevelUpItemWindow>(false);
        WindowManager.Instance.Show<UIItemCommonWindow>(false);
    }

    private void ResetData()
    {
        atkForeShowSlider.value = 0;
        hpForeShowSlider.value = 0;
        recoverForeShowSlider.value = 0;
        lvlForeShowSlider.value = 0;
        expCanGet = 0;
        changedAtk.text = "";
        changedHp.text = "";
        changedRecover.text = "";
        changedMp.text = "";
        mainItemName.text = "";
    }

    private IEnumerator PlayEffect(float time)
    {
        var pss = LevelUpEffect.GetComponents<ParticleSystem>();
        foreach (var system in pss)
        {
            system.Play();
        }
        yield return new WaitForSeconds(time);
        foreach (var system in pss)
        {
            system.Stop();
        }
    }

    public void ShowLevelOver()
    {
        StartCoroutine("PlayEffect", 1.5f);
        ResetData();
        CleanMats();
    }

    #endregion
}

using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroSelItemWindow : Window
{
    #region Private Fields

    private UIEventListener backLis;
    private UIEventListener unLoadLis;
    private UIEventListener oKLis;
    private Transform properties;
    private HeroInfo heroInfo;
    private UILabel equipNums;
    private UILabel atkValue;
    private UILabel hpValue;
    private UILabel recoverValue;
    private UILabel mpValue;

    private UItemsWindow cachedItemsWindow;
    private HeroBaseInfoWindow heroBaseInfoWindow;
    private int cachedRowToShow;
    private UIEventListener.VoidDelegate itemClickDelegate;
    private static bool isShuttingDown;
    private EquipItem currentEquipItem;
    private List<ItemInfo> nonMatItems;

    private const string IncColorStr = "[56ff0b](+";
    private const string DesColorStr = "[ff0000](-";
    private const string ColorEndString = ")[-]";
    private ItemInfo infoBeforeChange;
 
    #endregion

    #region Public

    /// <summary>
    /// The depth of the items window's panel. 
    /// </summary>
    public int ItemsWindowDepth = 10;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Init();
        InstallHandlers();
        heroInfo = HeroModelLocator.Instance.FindHero(HeroBaseInfoWindow.CurUuid);
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
        var buttons = Utils.FindChild(transform, "Buttons");
        backLis = UIEventListener.Get(buttons.FindChild("Button-Back").gameObject);
        unLoadLis = UIEventListener.Get(buttons.FindChild("Button-Unload").gameObject); 
        oKLis = UIEventListener.Get(buttons.FindChild("Button-Ok").gameObject);
        properties = Utils.FindChild(transform, "Properties");
        equipNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
        atkValue = Utils.FindChild(properties, "Attack-Value").GetComponent<UILabel>();
        hpValue = Utils.FindChild(properties, "HP-Value").GetComponent<UILabel>();  
        recoverValue = Utils.FindChild(properties, "Recover-Value").GetComponent<UILabel>();
        mpValue = Utils.FindChild(properties, "MP-Value").GetComponent<UILabel>();
    }

    /// <summary>
    /// Install all handlers in the window.
    /// </summary>
    private void InstallHandlers()
    {
        backLis.onClick = OnBack;
        unLoadLis.onClick = OnUnload;
        oKLis.onClick = OnOk;
        CommonHandler.HeroPropertyChanged += OnHeroPropertyChanged;
    }

    /// <summary>
    /// Uninstall all handlers in the window.
    /// </summary>
    private void UnInstallHandlers()
    {
        backLis.onClick = null;
        unLoadLis.onClick = null;
        oKLis.onClick = null;
        CommonHandler.HeroPropertyChanged -= OnHeroPropertyChanged;
    }

    private void OnHeroPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        WindowManager.Instance.Show<UIHeroSelItemWindow>(false);
        WindowManager.Instance.Show<HeroBaseInfoWindow>(true);
        WindowManager.Instance.Show<UIHeroInfoWindow>(true);
        WindowManager.Instance.Show<UItemsWindow>(false);
    }

    /// <summary>
    /// The call back of the back button clicked.
    /// </summary>
    /// <param name="go">The sender of the click event.</param>
    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroSelItemWindow>(false);
        WindowManager.Instance.Show<HeroBaseInfoWindow>(true);
        WindowManager.Instance.Show<UIHeroInfoWindow>(true);
        WindowManager.Instance.Show<UItemsWindow>(false);
    }

    /// <summary>
    /// The call back of the unload button clicked.
    /// </summary>
    /// <param name="go">The sender of the click event.</param>
    private void OnUnload(GameObject go)
    {
        if (currentEquipItem != null)
        {
            currentEquipItem.ShowEquipMask(false);
        }
        currentEquipItem = null;
    }

    /// <summary>
    /// The call back of the ok button clicked.
    /// </summary>
    /// <param name="go">The sender of the click event.</param>
    private void OnOk(GameObject go)
    {
        ItemInfo info = null;
        if(currentEquipItem != null)
        {
            info = ItemModeLocator.Instance.FindItem(currentEquipItem.BagIndex);
        }
        if (heroBaseInfoWindow.HeroInfo.EquipUuid[heroBaseInfoWindow.CurEquipIndex] != info.Id)
        {
            var msg = new CSHeroChangeEquip()
            {
                EquipUuid = info.Id,
                HeroUuid = HeroBaseInfoWindow.CurUuid,
                Index = heroBaseInfoWindow.CurEquipIndex
            };
            NetManager.SendMessage(msg);
        }
    }

    /// <summary>
    /// Init some varibles when we enter this window.
    /// </summary>
    private void Init()
    {
        cachedItemsWindow = WindowManager.Instance.Show<UItemsWindow>(true);
        nonMatItems = FilterItems();
        cachedItemsWindow.Refresh(nonMatItems);
        cachedRowToShow = cachedItemsWindow.RowToShow;
        cachedItemsWindow.RowToShow = 2;
        cachedItemsWindow.Depth = ItemsWindowDepth;
        itemClickDelegate = cachedItemsWindow.ItemClicked;
        cachedItemsWindow.ItemClicked = null;
        RegisterLongPress();
        InitCurEquipItem();
    }

    /// <summary>
    /// Init the current equip item.
    /// </summary>
    private void InitCurEquipItem()
    {
        heroBaseInfoWindow = WindowManager.Instance.GetWindow<HeroBaseInfoWindow>();
        var currentItemUuid = heroBaseInfoWindow.HeroInfo.EquipUuid[heroBaseInfoWindow.CurEquipIndex];
        var itemsTran = cachedItemsWindow.Items.transform;
        infoBeforeChange = ItemModeLocator.Instance.FindItem(currentItemUuid);
        if (infoBeforeChange != null)
        {
            for (var i = 0; i < itemsTran.childCount; i++)
            {
                var equipItem = itemsTran.GetChild(i).GetComponent<EquipItem>();
                if (equipItem != null && equipItem.BagIndex == infoBeforeChange.BagIndex)
                {
                    currentEquipItem = equipItem;
                    currentEquipItem.ShowEquipMask(true);
                }
            }
        }
    }

    /// <summary>
    /// Register the long press event.
    /// </summary>
    private void RegisterLongPress()
    {
       if(cachedItemsWindow != null)
       {
           var items = cachedItemsWindow.Items.transform;
           for (int i = 0; i < items.childCount; i++)
           {
               var child = items.GetChild(i);
               var longPress = child.GetComponent<NGUILongPress>();
               if(longPress == null)
               {
                   longPress = child.gameObject.AddComponent<NGUILongPress>();
                   longPress.LongClickDuration = 1f;
                   longPress.OnLongPress = OnLongPress;
                   longPress.OnNormalPress = OnNormalPress;
               }
           }
       }
    }

    /// <summary>
    /// Unregister the long press event.
    /// </summary>
    private void UnRegisterLongPress()
    {
        if (cachedItemsWindow != null)
        {
            var items = cachedItemsWindow.Items.transform;
            for (int i = 0; i < items.childCount; i++)
            {
                var child = items.GetChild(i);
                var longPress = child.GetComponent<NGUILongPress>();
                if (longPress != null)
                {
                    longPress.OnLongPress = null;
                    longPress.OnNormalPress = null;
                    Destroy(longPress);
                }
            }
        }
    }

    /// <summary>
    /// Filter all items to exclude material items.
    /// </summary>
    /// <returns>The item info list of all items after filtering.</returns>
    private List<ItemInfo> FilterItems()
    {
        if (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos != null)
        {
            return
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.FindAll(
                    item => ItemModeLocator.Instance.GetItemType(item.TmplId) != ItemModeLocator.EquipType.MaterialTempl);
        }
        return new List<ItemInfo>();
    }

    /// <summary>
    /// Used to clean up some varibles.
    /// </summary>
    private void CleanUp()
    {
        cachedItemsWindow.ItemClicked = itemClickDelegate;
        UnRegisterLongPress();
        cachedItemsWindow.ResetDepth();
        if (!isShuttingDown)
        {
            cachedItemsWindow.RowToShow = cachedRowToShow;
            WindowManager.Instance.Show<UItemsWindow>(false);
        }
        if (currentEquipItem != null)
        {
            currentEquipItem.ShowEquipMask(false);
            currentEquipItem = null;
        }
    }

    /// <summary>
    /// The call back of application quit.
    /// </summary>
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    /// <summary>
    /// The handler of long press.
    /// </summary>
    /// <param name="go">The sender of the long press.</param>
    private void OnLongPress(GameObject go)
    {
        ItemModeLocator.Instance.GetItemDetailPos = ItemType.GetItemDetailInHeroInfo;
        var bagIndex = go.GetComponent<EquipItem>().BagIndex;
        var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
        NetManager.SendMessage(csmsg);
    }

    /// <summary>
    /// The handler of the normal press.
    /// </summary>
    /// <param name="go">The sender of the normal press.</param>
    private void OnNormalPress(GameObject go)
    {
        if(currentEquipItem != null)
        {
            currentEquipItem.ShowEquipMask(false);
        }
        var item = go.GetComponent<EquipItem>();
        var newInfo = (item != null) ? ItemModeLocator.Instance.FindItem(item.BagIndex) : null;
        RefreshProperties(infoBeforeChange, newInfo);
        if(item == currentEquipItem)
        {
            if(item != null)
            {
                item.ShowEquipMask(false);
            }
            currentEquipItem = null;
        }
        else
        {
            currentEquipItem = item;
            if(currentEquipItem != null)
            {
                currentEquipItem.ShowEquipMask(true);
            }
        }
    }

    private void RefreshProperties(ItemInfo oldInfo, ItemInfo newInfo)
    {
        var oldAtk = 0;
        var oldHp = 0;
        var oldRecover = 0;
        var oldMp = 0;   
        var newAtk = 0;
        var newHp = 0;
        var newRecover = 0;
        var newMp = 0;
        if(oldInfo != null)
        {
            GetProproties(oldInfo, out oldAtk, out oldHp, out oldRecover, out oldMp);
        }
        if (newInfo != null)
        {
            GetProproties(newInfo, out newAtk, out newHp, out newRecover, out newMp);
        }
        var changedAtk = newAtk - oldAtk;
        if(changedAtk != 0)
        {
            atkValue.text = string.Format("{0}{1}{2}{3}", heroInfo.Prop[RoleProperties.ROLE_ATK],
                                  changedAtk > 0 ? IncColorStr : DesColorStr, Mathf.Abs(changedAtk),
                                  ColorEndString);  
        }
        var changedHp = newHp - oldHp;
        if(changedHp != 0)
        {
            hpValue.text = string.Format("{0}{1}{2}{3}", heroInfo.Prop[RoleProperties.ROLE_HP],
                                  changedHp > 0 ? IncColorStr : DesColorStr, Mathf.Abs(changedHp),
                                  ColorEndString); 
        }
        var changedRecover = newRecover - oldRecover;
        if (changedRecover != 0)
        {
            recoverValue.text = string.Format("{0}{1}{2}{3}", heroInfo.Prop[RoleProperties.ROLE_RECOVER],
                                  changedRecover > 0 ? IncColorStr : DesColorStr, Mathf.Abs(changedRecover),
                                  ColorEndString);  
        }
        var changedMp = newMp - oldMp;
        if (changedMp != 0)
        {
            mpValue.text = string.Format("{0}{1}{2}{3}", heroInfo.Prop[RoleProperties.ROLE_MP],
                                  changedMp > 0 ? IncColorStr : DesColorStr, Mathf.Abs(changedMp),
                                  ColorEndString);
        }
    }

    private void GetProproties(ItemInfo itemInfo, out int atk, out int hp, out int recover, out int mp)
    {
        atk = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level);
        hp = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level);
        recover = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level);
        mp = ItemModeLocator.Instance.GetMp(itemInfo.TmplId);
    }

    /// <summary>
    /// Refresh of ui data of this window.
    /// </summary>
    private void Refresh()
    {
        atkValue.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
        hpValue.text = heroInfo.Prop[RoleProperties.ROLE_HP].ToString(CultureInfo.InvariantCulture);
        recoverValue.text = heroInfo.Prop[RoleProperties.ROLE_RECOVER].ToString(CultureInfo.InvariantCulture);
        mpValue.text = heroInfo.Prop[RoleProperties.ROLE_MP].ToString(CultureInfo.InvariantCulture);
        var capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        equipNums.text = string.Format("{0}/{1}", nonMatItems.Count, capacity);
    }
    
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Property;
using UnityEngine;

public class BattleWinControl : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The initial position of the tween.
    /// </summary>
    public Transform From;

    /// <summary>
    /// The target position of the tween.
    /// </summary>
    public Transform To;

    /// <summary>
    /// The slider of the experience bar.
    /// </summary>
    public UISlider ExpSlider;

    /// <summary>
    /// The tranfrom of template game object.
    /// </summary>
    public UISprite Template;

    /// <summary>
    /// The transform of coin sprites game object's the parent.
    /// </summary>
    public Transform CoinValues;

    /// <summary>
    /// The transform of soul sprites game object's the parent.
    /// </summary>
    public Transform SoulValues;

    /// <summary>
    ///The transform of reputation sprites game object's the parent.
    /// </summary>
    public Transform RepValues;

    /// <summary>
    /// The transform of experience sprites game object's the parent.
    /// </summary>
    public Transform ExpValues;

    /// <summary>
    /// The coin value.
    /// </summary>
    public int CoinValue = 25000;

    /// <summary>
    /// The soul value.
    /// </summary>
    public int SoulValue = 25000;

    /// <summary>
    ///  The reputation value.
    /// </summary>
    public int RepValue = 25000;

    /// <summary>
    ///  The experience value.
    /// </summary>
    public int ExpValue = 25000;

    /// <summary>
    /// The time interval between different labels showing.
    /// </summary>
    public float Delay = 0.2f;

    #endregion

    private GameObject ItemPrefab;

    private GameObject InfoContainer;
    private GameObject GetContainer;

    private GameObject SpriteClick;

    private GameObject ExpBar;

    private int CurrentStep;
    private bool IsPlaying = false;
    private int CurrItemIndex;

    private List<GameObject> Items; 

    #region Private Methods

    /// <summary>
    /// This is used to test the battle win when we enable the prefab game object.
    /// </summary>
    private void OnEnable()
    {
        Show();
    }

    private List<GameObject> GoldSprites;
    private List<GameObject> SpiritSprites;
    private List<GameObject> RepuSprites;
    private List<GameObject> ExpSprites;

    /// <summary>
    /// Show coins, soul, reputation and experience text in the battle win window.
    /// </summary>
    private IEnumerator ShowBattleWin()
    {
        IsPlaying = true;
        ShowExp();

        yield return new WaitForSeconds(0.2f);
        SpriteClick.SetActive(false);
        CurrentStep = 0;
        CurrItemIndex = 0;
        InfoContainer.SetActive(true);
        GetContainer.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        GoldSprites = Int2Sprite.Show(CoinValues, Template, GetValueByKey(RoleProperties.ROLEBASE_GOLD), GoldSprites);
        yield return new WaitForSeconds(Delay);
        SpiritSprites = Int2Sprite.Show(SoulValues, Template, GetValueByKey(RoleProperties.ROLEBASE_HERO_SPIRIT), SpiritSprites);
        yield return new WaitForSeconds(Delay);
        RepuSprites = Int2Sprite.Show(RepValues, Template, GetValueByKey(RoleProperties.ROLEBASE_REPUTATION), RepuSprites);
 
        yield return new WaitForSeconds(Delay);
        ExpSprites = Int2Sprite.Show(ExpValues, Template, MissionModelLocator.Instance.BattleReward.Exp, ExpSprites);

        var bar = ExpBar.GetComponent<UIProgressBar>();
        var temp = LevelModelLocator.Instance.GetLevelByTemplateId(MissionModelLocator.Instance.OldLevel + 1);
        bar.value = (float)(MissionModelLocator.Instance.OldExp + MissionModelLocator.Instance.BattleReward.Exp) / temp.MaxExp;
        IsPlaying = false;
        SpriteClick.SetActive(true);
    }

    private int GetValueByKey(int thekey)
    {
        int k = 0;
        if (MissionModelLocator.Instance.BattleReward.Money.ContainsKey(thekey))
        {
            k = MissionModelLocator.Instance.BattleReward.Money[thekey];
        }
        return k;
    }

    private void ShowExp()
    {
        if (ExpBar != null)
        {
            var bar = ExpBar.GetComponent<UIProgressBar>();
            var temp = LevelModelLocator.Instance.GetLevelByTemplateId(MissionModelLocator.Instance.OldLevel + 1);
            bar.value = (float)MissionModelLocator.Instance.OldExp / temp.MaxExp;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show battle win screen.
    /// </summary>
    public void Show()
    {
        StartCoroutine("ShowBattleWin");
    }

    private IEnumerator ShowItemStep()
    {
        IsPlaying = true;
        while (CurrItemIndex < MissionModelLocator.Instance.BattleReward.RewardItem.Count)
        {
            yield return new WaitForSeconds(0.8f);
            var control = Items[CurrItemIndex].GetComponent<GetItemControl>();
            CurrItemIndex++;
            control.Open();
            if (control.Data.IsNew)
            {
                IsPlaying = false;
                SpriteClick.SetActive(true);
                break;
            }
        }
        SpriteClick.SetActive(true);
        IsPlaying = false;
    }

    void OnClick()
    {
        if (IsPlaying) return;
        if (CurrentStep == 0)
        {
            SetItems();

            SpriteClick.SetActive(false);
            InfoContainer.SetActive(false);
            GetContainer.SetActive(true);
            CurrentStep = 1;
            StartCoroutine(ShowItemStep());
        }
        else if (CurrItemIndex < MissionModelLocator.Instance.BattleReward.RewardItem.Count)
        {
            StartCoroutine(ShowItemStep());
        }
        else
        {
            ShowEnd();
        }
    }

    private void SetItems()
    {
        if (Items == null)
        {
            Items = new List<GameObject>();
        }
        else
        {
            while (Items.Count > 0)
            {
                var obj = Items[0];
                Items.RemoveAt(0);
                Destroy(obj);
            }
        }
        int k = (MissionModelLocator.Instance.BattleReward.RewardItem.Count >= 8)
            ? 8
            : MissionModelLocator.Instance.BattleReward.RewardItem.Count;
        int basex = - 140 * k / 2;
        int basey = 70;
        int offsetx = 140;
        int offsety = 140;
        for (int i = 0; i < MissionModelLocator.Instance.BattleReward.RewardItem.Count; i ++)
        {
            var v = i%8;
            var m = (i - v)/8;
            var item = NGUITools.AddChild(GetContainer, ItemPrefab);
            var control = item.GetComponent<GetItemControl>();
			var d = MissionModelLocator.Instance.BattleReward.RewardItem[i];
			control.SetData(d);
            item.transform.localPosition = new Vector3(basex + offsetx * v, basey - offsety * m, 0);
            Items.Add(item);
        }
    }

    private void ShowEnd()
    {
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        battlemanager.ResetBattle();

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
        WindowManager.Instance.Show(typeof(MissionTabWindow), true);
    }


    void Start()
    {
        InfoContainer = transform.FindChild("Container info").gameObject;
        GetContainer = transform.FindChild("Container get").gameObject;
        SpriteClick = transform.FindChild("Sprite click").gameObject;
        ExpBar = transform.FindChild("Container info/Progress Bar").gameObject;

        ItemPrefab = Resources.Load("Prefabs/Component/BattleGetItem") as GameObject;
        GetContainer.SetActive(false);
        ShowExp();
    }

    #endregion
}

using System.Globalization;
using Property;
using UnityEngine;
using System.Collections;

public class UIHeroItemsPageWindow : Window
{
    private UIPanel panel;
    private UIGrid grid;
    private GameObject offset;

    public GameObject HeroPrefab;

    private int rowToShow;
    public int RowToShow
    {
        get
        {
            return rowToShow;
        }
        set
        {
            if (rowToShow != value)
            {
                rowToShow = value;
                var clipHeight = panel.baseClipRegion.w;
                panel.baseClipRegion = new Vector4(panel.baseClipRegion.x, panel.baseClipRegion.y, panel.baseClipRegion.z, rowToShow * grid.cellHeight);
                var pos = offset.transform.localPosition;
                offset.transform.localPosition = new Vector3(pos.x, pos.y - 0.5f * (panel.baseClipRegion.w - clipHeight), pos.z);
                pos = grid.transform.localPosition;
                grid.transform.localPosition = new Vector3(pos.x, pos.y + 0.5f * (panel.baseClipRegion.w - clipHeight), pos.z);
            }
        }
    }

    #region Window

    public override void OnEnter()
    {
        StartCoroutine(FillHeroList());
        Refresh();
    }

    public override void OnExit()
    {

    }

    #endregion

    // Use this for initialization
    void Awake()
    {
        panel = GetComponentInChildren<UIPanel>();
        grid = GetComponentInChildren<UIGrid>();
        offset = Utils.FindChild(transform, "Offset").gameObject;
    }

    /// <summary>
    /// Fill in the hero game objects.
    /// </summary> 
    private IEnumerator FillHeroList()
    {
        var heroCount = HeroModelLocator.Instance.SCHeroList.HeroList.Count;
        for (int i = 0; i < heroCount; i++)
        {
            var item = NGUITools.AddChild(grid.gameObject, HeroPrefab);
            UIEventListener.Get(item).onClick += OnHeroInfoClicked;
        }
        grid.Reposition();
        yield return new WaitForEndOfFrame();
    }

    private void OnHeroInfoClicked(GameObject go)
    {
        UIHeroInfoWindow.Uuid = go.GetComponent<HeroInfoPack>().Uuid;
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Refresh()
    {
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, HeroModelLocator.Instance.SCHeroList.HeroList);
        for (int i = 0; i < HeroModelLocator.Instance.SCHeroList.HeroList.Count; i++)
        {
            var item = grid.transform.GetChild(i);
            var uUid = HeroModelLocator.Instance.SCHeroList.HeroList[i].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            ShowHero(orderType, item, uUid);
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
        var jobSymobl = Utils.FindChild(heroTran, "JobSymbol").GetComponent<UISprite>();
        var attack = Utils.FindChild(heroTran, "Attack").GetComponent<UILabel>();
        jobSymobl.spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
        attack.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
        switch (orderType)
        {
            //»Î ÷À≥–Ú≈≈–Ú
            case 0:
                break;

            //Œ‰Ω´÷∞“µ≈≈–Ú
            case 1:
                break;

            //Œ‰Ω´œ°”–∂»≈≈–Ú
            case 3:

                break;

            //’’∂”ŒÈÀ≥–Ú≈≈–Ú
            case 4:
                break;

            //π•ª˜¡¶≈≈–Ú
            case 5:

                break;

            //HP≈≈–Ú
            case 6:

                break;

            //ªÿ∏¥¡¶≈≈–Ú
            case 7:

                break;

            //µ»º∂≈≈–Ú
            case 8:
                break;
        }
    }
}

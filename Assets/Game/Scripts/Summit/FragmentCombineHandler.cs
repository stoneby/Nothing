using System.Linq;
using KXSGCodec;
using System.Collections.Generic;
using Template.Auto.Hero;
using UnityEngine;

public class FragmentCombineHandler : MonoBehaviour
{
    #region Public Fields

    public GameObject FragmentHero;
    public EffectSpawner FragmentEffectController;

    #endregion

    #region Private Fields

    //Interface info.
    private UILabel fragmentNum;
    private GameObject grid3;
    private GameObject grid4;
    private GameObject grid5;
    private GameObject starBar3;
    private GameObject starBar4;

    //Refresh interface info.
    private List<int> templateID5;
    private List<int> templateID4;
    private List<int> templateID3;
    private int superChip;

    //Combine info from sever.
    private SCLotteryComposeList scLotteryComposeList;
    private SCLotteryComposeSucc scLotteryComposeSucc;

    //Const interface info from Unity editor.
    private const double MaxSuperChipUse = 0.2;
    private const int StarBar5PosY = 180;
    private const int StarBarHeight = 50;
    private const int GridColumn = 9;
    private const int GridItemHeight = 125;
    private const int GapDistance = 15;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize FragmentList info.
    /// </summary>
    /// <param name="msg"></param>
    public void Refresh(SCLotteryComposeList msg)
    {
        scLotteryComposeList = msg;

        //Initialize the SuperChip info.
        fragmentNum.text = scLotteryComposeList.SuperChip.ToString();
        PlayerModelLocator.Instance.SuperChip = scLotteryComposeList.SuperChip;

        //Set interface info.
        TransferToTemplateIDList(5, out templateID5);
        TransferToTemplateIDList(4, out templateID4);
        TransferToTemplateIDList(3, out templateID3);
        superChip = scLotteryComposeList.SuperChip;

        //Initialize grid5 hero info.
        AddedOrDelFragmentHero(grid5, templateID5.Count);
        grid5.GetComponent<UIGrid>().Reposition();
        SetHeroItem(grid5, templateID5, superChip);

        //Initialize grid4 hero info.
        AddedOrDelFragmentHero(grid4, templateID4.Count);
        grid4.GetComponent<UIGrid>().Reposition();
        SetHeroItem(grid4, templateID4, superChip);

        //Initialize grid3 hero info.
        AddedOrDelFragmentHero(grid3, templateID3.Count);
        grid3.GetComponent<UIGrid>().Reposition();
        SetHeroItem(grid3, templateID3, superChip);

        //Initialize the starbar4 position.
        if (templateID5.Count == 0)
        {
            starBar4.transform.localPosition = new Vector3(0, StarBar5PosY - StarBarHeight - GapDistance, 0);
        }
        else
        {
            starBar4.transform.localPosition = new Vector3(0, StarBar5PosY - StarBarHeight - ((templateID5.Count - 1) / GridColumn + 1) * GridItemHeight - GapDistance, 0);
        }

        //Initialize the starbar3 position.
        if (templateID4.Count == 0)
        {
            starBar3.transform.localPosition = new Vector3(0, starBar4.transform.localPosition.y - StarBarHeight - GapDistance, 0);
        }
        else
        {
            starBar3.transform.localPosition = new Vector3(0, starBar4.transform.localPosition.y - StarBarHeight - ((templateID4.Count - 1) / GridColumn + 1) * GridItemHeight - GapDistance, 0);
        }
    }

    /// <summary>
    /// Override Refresh function for refresh FragmentList info, called when fragment combine succeed.
    /// </summary>
    /// <param name="msg"></param>
    public void Refresh(SCLotteryComposeSucc msg)
    {
        scLotteryComposeSucc = msg;
        FragmentEffectController.Play();

        //Refresh changed FragmentHero info.
        FragmentHero[] tempFragmentHeros = transform.GetComponentsInChildren<FragmentHero>();
        foreach (var item in tempFragmentHeros)
        {
            if (item.TemplateId == scLotteryComposeSucc.TemplateId)
            {
                int composeCount = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[item.TemplateId].ComposeCount;
                item.MaterialCount = scLotteryComposeSucc.ChipCount;

                if (scLotteryComposeList != null)
                {
                    if (scLotteryComposeList.HeroChip.ContainsKey(scLotteryComposeSucc.TemplateId))
                    {
                        scLotteryComposeList.HeroChip[scLotteryComposeSucc.TemplateId] = scLotteryComposeSucc.ChipCount;
                    }
                    else
                    {
                        Logger.LogError("Template id not found in scLotteryComposeList in FragmentCombineHandler.Refresh(), fragment combine UI not refreshed.");
                    }
                }

                if (item.MaterialCount < composeCount * 0.8 || item.MaterialCount + PlayerModelLocator.Instance.SuperChip < composeCount)
                {
                    item.SwitchColor(Color.grey);
                }
                else
                {
                    item.SwitchColor(Color.white);
                }
                item.gameObject.SetActive(true);
                item.Refresh();
            }
        }
        //PopTextManager.PopTip("Hero fragment combine success.");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Select hero with specific star and compose count not equal to 0.
    /// </summary>
    /// <param name="star"></param>
    /// <param name="templateID"></param>
    private void TransferToTemplateIDList(int star, out List<int> templateID)
    {
        templateID = HeroModelLocator.Instance.HeroTemplates.HeroTmpls.Where(item => item.Value.Star == star && item.Value.ComposeCount != 0).Select(pair => pair.Key).ToList();
    }

    /// <summary>
    /// Add or delete FragmentHero prefab to specific num in a star bar.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="totalNum"></param>
    private void AddedOrDelFragmentHero(GameObject parent, int totalNum)
    {
        while (parent.transform.childCount > totalNum)
        {
            var herotemp = parent.transform.GetChild(0);
            herotemp.parent = null;
            Destroy(herotemp);
        }
        while (parent.transform.childCount < totalNum)
        {
            var herotemp = Instantiate(FragmentHero) as GameObject;
            herotemp.SetActive(true);
            Utils.MoveToParent(parent.transform, herotemp.transform);
        }
    }

    /// <summary>
    /// Set hero item's data in a star bar.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="templateIdList"></param>
    /// <param name="superChipSetStarBar"></param>
    private void SetHeroItem(GameObject parent, List<int> templateIdList, int superChipSetStarBar)
    {
        for (int i = 0; i < templateIdList.Count; i++)
        {
            var heroTemp = parent.transform.GetChild(i);
            var fragmentHeroTemp = parent.transform.GetChild(i).GetComponent<FragmentHero>();
            fragmentHeroTemp.TemplateId = templateIdList[i];
            //Set material count to message's num if possible, or 0.
            int tempMaterialCount;
            scLotteryComposeList.HeroChip.TryGetValue(templateIdList[i], out tempMaterialCount);
            fragmentHeroTemp.MaterialCount = tempMaterialCount;

            int composeCount = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[fragmentHeroTemp.TemplateId].ComposeCount;

            if (fragmentHeroTemp.MaterialCount < composeCount * (1 - MaxSuperChipUse) || fragmentHeroTemp.MaterialCount + superChipSetStarBar < composeCount)
            {
                fragmentHeroTemp.CanCombine = false;
                fragmentHeroTemp.SwitchColor(Color.grey);
            }
            else
            {
                fragmentHeroTemp.CanCombine = true;
                fragmentHeroTemp.SwitchColor(Color.white);
            }

            heroTemp.gameObject.SetActive(true);
            fragmentHeroTemp.Refresh();
        }
    }

    private void RefreshSuperChip(SCPropertyChangedNumber scPropertyChanged)
    {
        //Refresh fragmentNum info.
        fragmentNum.text = PlayerModelLocator.Instance.SuperChip.ToString();
        if (scLotteryComposeList != null)
        {
            scLotteryComposeList.SuperChip = PlayerModelLocator.Instance.SuperChip;
        }
    }

    private void InstallHandlers()
    {
        Logger.Log("!!!!!!Install handlers in FragmentCombineHandler.");
        CommonHandler.PlayerPropertyChanged += RefreshSuperChip;
    }

    private void UnInstallHandlers()
    {
        Logger.Log("!!!!!!UnInstall handlers in FragmentCombineHandler.");
        CommonHandler.PlayerPropertyChanged -= RefreshSuperChip;
    }

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        fragmentNum = transform.Find("Frame1/Frame2/SpriteInfo/Label").gameObject.GetComponent<UILabel>();
        grid3 = transform.Find("Frame1/Frame2/ScrollView/StarBar3/Grid").gameObject;
        grid4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4/Grid").gameObject;
        grid5 = transform.Find("Frame1/Frame2/ScrollView/StarBar5/Grid").gameObject;
        starBar3 = transform.Find("Frame1/Frame2/ScrollView/StarBar3").gameObject;
        starBar4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4").gameObject;

        grid3.GetComponent<UIGrid>().enabled = true;
        grid4.GetComponent<UIGrid>().enabled = true;
        grid5.GetComponent<UIGrid>().enabled = true;
    }

    void Start()
    {
        InstallHandlers();
    }

    void OnDestory()
    {
        UnInstallHandlers();
    }

    #endregion
}

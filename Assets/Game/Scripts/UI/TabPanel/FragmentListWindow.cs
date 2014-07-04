using KXSGCodec;
using System.Collections.Generic;
using UnityEngine;

public class FragmentListWindow : Window
{
    #region Private Fields

    //Interface info.
    private UILabel fragmentNum;
    private GameObject grid4;
    private GameObject grid5;
    private GameObject starBar4;

    //Combine info from sever.
    private SCLotteryComposeList scLotteryComposeList;
    private SCLotteryComposeSucc scLotteryComposeSucc;

    //Const interface info from Unity editor.
    private const double MaxSuperChipUse = 0.2;
    private const int StarBar5PosY = 180;
    private const int StarBarHeight = 61;
    private const int GridColumn = 8;
    private const int GridItemHeight = 119;

    #endregion

    #region Private Methods

    /// <summary>
    /// Add or delete FragmentHero prefab to specific num.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="starChip"></param>
    /// <param name="superChip"></param>
    private void AddedOrDelFragmentHero(GameObject parent, List<Dictionary<int,int>> starChip, int superChip)
    {
        while (parent.transform.childCount > starChip.Count)
        {
            var herotemp = parent.transform.GetChild(0);
            herotemp.parent = null;
            Destroy(herotemp);
        }
        while (parent.transform.childCount < starChip.Count)
        {
            var herotemp = Instantiate(FragmentHero) as GameObject;
            herotemp.SetActive(true);
            Utils.MoveToParent(parent.transform, herotemp.transform);
        }
        for (int i = 0; i < starChip.Count; i++)
        {
            var heroTemp = parent.transform.GetChild(i);
            var fragmentHeroTemp = parent.transform.GetChild(i).GetComponent<FragmentHero>();
            Dictionary<int, int>.KeyCollection keyCol = starChip[i].Keys;
            if (keyCol.Count != 1)
            {
                Logger.LogError("Incorrect Star5Chip info, check the server info.");
                return;
            }
            foreach (int item in keyCol)
            {
                fragmentHeroTemp.TemplateId = item;
                fragmentHeroTemp.MaterialCount = starChip[i][item];
            }
            int composeCount =
                HeroModelLocator.Instance.HeroTemplates.HeroTmpl[fragmentHeroTemp.TemplateId].ComposeCount;
            if (fragmentHeroTemp.MaterialCount < composeCount * (1-MaxSuperChipUse) || fragmentHeroTemp.MaterialCount + superChip < composeCount)
            {
                fragmentHeroTemp.SwitchColor(Color.grey);
            }
            else
            {
                fragmentHeroTemp.SwitchColor(Color.white);
            }

            heroTemp.gameObject.SetActive(true);
            fragmentHeroTemp.Refresh();
        }
    }

    #endregion

    #region Public Fields

    public GameObject FragmentHero;

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

        //Initialize grid5 hero info.
        AddedOrDelFragmentHero(grid5, scLotteryComposeList.Star5Chip, scLotteryComposeList.SuperChip);
        
        //Initialize grid4 hero info.
        AddedOrDelFragmentHero(grid4, scLotteryComposeList.Star4Chip, scLotteryComposeList.SuperChip);

        //Initialize the starbar4 position.
        if (scLotteryComposeList.Star5Chip.Count == 0)
        {
            starBar4.transform.localPosition = new Vector3(0, StarBar5PosY - StarBarHeight, 0);
        }
        else
        {
            starBar4.transform.localPosition = new Vector3(0, StarBar5PosY - StarBarHeight - ((scLotteryComposeList.Star5Chip.Count - 1) / GridColumn + 1) * GridItemHeight, 0);
        }
    }

    /// <summary>
    /// Override Refresh function for refresh FragmentList info.
    /// </summary>
    /// <param name="msg"></param>
    public void Refresh(SCLotteryComposeSucc msg)
    {
        scLotteryComposeSucc = msg;

        //Refresh fragmentNum info.
        fragmentNum.text = PlayerModelLocator.Instance.SuperChip.ToString();

        //Refresh changed FragmentHero info.
        FragmentHero[] tempFragmentHeros= transform.GetComponentsInChildren<FragmentHero>();
        foreach (var item in tempFragmentHeros)
        {
            if (item.TemplateId == scLotteryComposeSucc.TemplateId)
            {
                int composeCount = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[item.TemplateId].ComposeCount;
                item.MaterialCount = scLotteryComposeSucc.ChipCount;
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
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        fragmentNum = transform.Find("Frame1/Frame2/SpriteInfo/Label").gameObject.GetComponent<UILabel>();              
        grid4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4/Grid").gameObject;
        grid5 = transform.Find("Frame1/Frame2/ScrollView/StarBar5/Grid").gameObject;
        starBar4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4").gameObject;
    }

    #endregion
}

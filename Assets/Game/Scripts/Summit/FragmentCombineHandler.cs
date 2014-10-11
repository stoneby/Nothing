using KXSGCodec;
using System.Collections.Generic;
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

        //Initialize grid5 hero info.
        AddedOrDelFragmentHero(grid5, scLotteryComposeList.Star5Chip, scLotteryComposeList.SuperChip);
        grid5.GetComponent<UIGrid>().Reposition();

        //Initialize grid4 hero info.
        AddedOrDelFragmentHero(grid4, scLotteryComposeList.Star4Chip, scLotteryComposeList.SuperChip);
        grid4.GetComponent<UIGrid>().Reposition();

        //Initialize grid3 hero info.
        AddedOrDelFragmentHero(grid3, scLotteryComposeList.Star3Chip, scLotteryComposeList.SuperChip);
        grid3.GetComponent<UIGrid>().Reposition();

        //Initialize the starbar4 position.
        if (scLotteryComposeList.Star5Chip.Count == 0)
        {
            starBar4.transform.localPosition = new Vector3(0, StarBar5PosY - StarBarHeight - GapDistance, 0);
        }
        else
        {
            starBar4.transform.localPosition = new Vector3(0, StarBar5PosY - StarBarHeight - ((scLotteryComposeList.Star5Chip.Count - 1) / GridColumn + 1) * GridItemHeight - GapDistance, 0);
        }

        //Initialize the starbar3 position.
        if (scLotteryComposeList.Star4Chip.Count == 0)
        {
            starBar3.transform.localPosition = new Vector3(0, starBar4.transform.localPosition.y - StarBarHeight - GapDistance, 0);
        }
        else
        {
            starBar3.transform.localPosition = new Vector3(0, starBar4.transform.localPosition.y - StarBarHeight - ((scLotteryComposeList.Star4Chip.Count - 1) / GridColumn + 1) * GridItemHeight - GapDistance, 0);
        }
    }

    /// <summary>
    /// Override Refresh function for refresh FragmentList info.
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
                    bool isFind = false;
                    foreach (var listItem in scLotteryComposeList.Star5Chip)
                    {
                        if (isFind)
                        {
                            break;
                        }
                        if (listItem.ContainsKey(scLotteryComposeSucc.TemplateId))
                        {
                            listItem[scLotteryComposeSucc.TemplateId] = scLotteryComposeSucc.ChipCount;
                            isFind = true;
                        }
                    }

                    foreach (var listItem in scLotteryComposeList.Star4Chip)
                    {
                        if (isFind)
                        {
                            break;
                        }
                        if (listItem.ContainsKey(scLotteryComposeSucc.TemplateId))
                        {
                            listItem[scLotteryComposeSucc.TemplateId] = scLotteryComposeSucc.ChipCount;
                            isFind = true;
                        }
                    }

                    foreach (var listItem in scLotteryComposeList.Star3Chip)
                    {
                        if (isFind)
                        {
                            break;
                        }
                        if (listItem.ContainsKey(scLotteryComposeSucc.TemplateId))
                        {
                            listItem[scLotteryComposeSucc.TemplateId] = scLotteryComposeSucc.ChipCount;
                            isFind = true;
                        }
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
    /// Add or delete FragmentHero prefab to specific num.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="starChip"></param>
    /// <param name="superChip"></param>
    private void AddedOrDelFragmentHero(GameObject parent, List<Dictionary<int, int>> starChip, int superChip)
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
                Logger.LogError("Incorrect StarChip info, check the server info.");
                return;
            }
            foreach (int item in keyCol)
            {
                fragmentHeroTemp.TemplateId = item;
                fragmentHeroTemp.MaterialCount = starChip[i][item];
            }

            int composeCount =
                HeroModelLocator.Instance.HeroTemplates.HeroTmpls[fragmentHeroTemp.TemplateId].ComposeCount;

            if (fragmentHeroTemp.MaterialCount < composeCount * (1 - MaxSuperChipUse) || fragmentHeroTemp.MaterialCount + superChip < composeCount)
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

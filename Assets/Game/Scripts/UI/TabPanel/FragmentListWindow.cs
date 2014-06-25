using System.Collections.Generic;
using KXSGCodec;
using Template;
using UnityEngine;

public class FragmentListWindow : Window
{
    #region Private Fields

    private UILabel fragmentNum;
    private GameObject grid4;
    private GameObject grid5;
    private GameObject starBar4;

    private SCLotteryComposeList scLotteryComposeList;
    private SCLotteryComposeSucc scLotteryComposeSucc;

    #endregion

    #region Private Methods

    #endregion

    #region Public Fields

    public GameObject FragmentHero;

    #endregion

    #region Public Methods

    //initialize FragmentList info.
    public void Refresh(SCLotteryComposeList msg)
    {
        //initialize the SuperChip info.
        scLotteryComposeList = msg;
        fragmentNum.text = scLotteryComposeList.SuperChip.ToString();

        #region initialize grid5 hero info

        while (grid5.transform.childCount > scLotteryComposeList.Star5Chip.Count)
        {
            var herotemp= grid5.transform.GetChild(0);
            herotemp.parent = null;
            Destroy(herotemp);
        }
        while (grid5.transform.childCount < scLotteryComposeList.Star5Chip.Count)
        {
            var herotemp = Instantiate(FragmentHero) as GameObject;
            herotemp.SetActive(true);
            Utils.MoveToParent(grid5.transform, herotemp.transform);
        }
        for (int i = 0; i < scLotteryComposeList.Star5Chip.Count; i++)
        {
            var herotemp = grid5.transform.GetChild(i);
            Dictionary<int, int>.KeyCollection keyCol = scLotteryComposeList.Star5Chip[i].Keys;
            if (keyCol.Count != 1)
            {
                Debug.LogError("Incorrect Star5Chip info, check the server info.");
                return;
            }
            foreach (int item in keyCol)
            {
                herotemp.GetComponent<FragmentHero>().TemplateID = item;
                herotemp.GetComponent<FragmentHero>().MaterialCount =
                    scLotteryComposeList.Star5Chip[i][item];
            }
            int composeCount =
                HeroModelLocator.Instance.HeroTemplates.HeroTmpl[herotemp.GetComponent<FragmentHero>().TemplateID].ComposeCount;
            if (herotemp.GetComponent<FragmentHero>().MaterialCount < composeCount*0.8 ||
                herotemp.GetComponent<FragmentHero>().MaterialCount + scLotteryComposeList.SuperChip < composeCount)
            {
                herotemp.GetComponent<FragmentHero>().SwitchColorGrey();
            }
            else
            {
                herotemp.GetComponent<FragmentHero>().SwitchColorWhite();
            }

            herotemp.gameObject.SetActive(true);                                 
            herotemp.GetComponent<FragmentHero>().Refresh();
        }

        #endregion

        #region initialize grid4 hero info

        while (grid4.transform.childCount > scLotteryComposeList.Star4Chip.Count)
        {
            var herotemp = grid4.transform.GetChild(0);
            herotemp.parent = null;
            Destroy(herotemp);
        }
        while (grid4.transform.childCount < scLotteryComposeList.Star4Chip.Count)
        {
            var herotemp = Instantiate(FragmentHero) as GameObject;
            herotemp.SetActive(true);
            Utils.MoveToParent(grid4.transform, herotemp.transform);
        }
        for (int i = 0; i < scLotteryComposeList.Star4Chip.Count; i++)
        {
            var herotemp = grid4.transform.GetChild(i);
            Dictionary<int, int>.KeyCollection keyCol = scLotteryComposeList.Star4Chip[i].Keys;
            if (keyCol.Count != 1)
            {
                Debug.LogError("Incorrect Star4Chip info, check the server info.");
                return;
            }
            foreach (int item in keyCol)
            {
                herotemp.GetComponent<FragmentHero>().TemplateID = item;
                herotemp.GetComponent<FragmentHero>().MaterialCount =
                    scLotteryComposeList.Star4Chip[i][item];
            }
            int composeCount =
                HeroModelLocator.Instance.HeroTemplates.HeroTmpl[herotemp.GetComponent<FragmentHero>().TemplateID].ComposeCount;
            if (herotemp.GetComponent<FragmentHero>().MaterialCount < composeCount*0.8 ||
                herotemp.GetComponent<FragmentHero>().MaterialCount + scLotteryComposeList.SuperChip < composeCount)
            {
                herotemp.GetComponent<FragmentHero>().SwitchColorGrey();
            }
            else
            {
                herotemp.GetComponent<FragmentHero>().SwitchColorWhite();
            }

            herotemp.gameObject.SetActive(true);
            herotemp.GetComponent<FragmentHero>().Refresh();
        }

        #endregion

        //initialize the starbar4 position.
        starBar4.transform.localPosition = new Vector3(0, 119 - ((scLotteryComposeList.Star5Chip.Count - 1) / 8 + 1) * 119,0);
    }

    //override Refresh function for changing FragmentList info.
    public void Refresh(SCLotteryComposeSucc msg)
    {
        scLotteryComposeSucc = msg;
        fragmentNum.text = PlayerModelLocator.Instance.SuperChip.ToString();
        FragmentHero[] tempFragmentHeros= transform.GetComponentsInChildren<FragmentHero>();
        foreach (var item in tempFragmentHeros)
        {
            if (item.TemplateID == msg.TemplateId)
            {
                int composeCount = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[item.TemplateID].ComposeCount;
                item.MaterialCount = msg.ChipCount;
                if (item.MaterialCount < composeCount * 0.8 || item.MaterialCount + PlayerModelLocator.Instance.SuperChip < composeCount)
                {
                    item.SwitchColorGrey();
                }
                else
                {
                    item.SwitchColorWhite();
                }
                item.gameObject.SetActive(true);
                item.Refresh();
            }
        }
        PopTextManager.PopTip("合成成功！");
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

    void Awake()
    {
        fragmentNum = transform.Find("Frame1/Frame2/SpriteInfo/Label").gameObject.GetComponent<UILabel>();              
        grid4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4/Grid").gameObject;
        grid5 = transform.Find("Frame1/Frame2/ScrollView/StarBar5/Grid").gameObject;
        starBar4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4").gameObject;
    }

    #endregion
}

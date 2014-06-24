using System.Collections.Generic;
using System.Security.Cryptography;
using KXSGCodec;
using UnityEngine;
using System.Collections;

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

    public string FragmentNum;
    public string FragmentInfo;

    #endregion

    #region Public Methods

    //initialize FragmentList info.
    public void Refresh(SCLotteryComposeList msg)
    {
        //initialize the supermaterialcount info.
        scLotteryComposeList = msg;
        fragmentNum.text = scLotteryComposeList.SuperMaterialCount.ToString();

        //initialize the hero info.
        for (int i = 0; i < scLotteryComposeList.Star5Material.Count; i++)
        {
            var herotemp = Instantiate(FragmentHero) as GameObject;
            Dictionary<int, int>.KeyCollection keyCol = scLotteryComposeList.Star5Material[i].Keys;
            if (keyCol.Count != 1)
            {
                Debug.LogError("Incorrect Star5Material info, check the server info.");
                return;
            }
            foreach (int item in keyCol)
            {
                herotemp.GetComponent<FragmentHero>().TemplateID = item;
                herotemp.GetComponent<FragmentHero>().MaterialCount =
                    scLotteryComposeList.Star5Material[i][item];
            }
            if (herotemp.GetComponent<FragmentHero>().MaterialCount < 40 ||
                herotemp.GetComponent<FragmentHero>().MaterialCount + scLotteryComposeList.SuperMaterialCount < 50)
            {
                herotemp.GetComponent<FragmentHero>().UnInstallHandlers();
            }
            else
            {
                herotemp.GetComponent<FragmentHero>().InstallHandlers();
            }

            herotemp.SetActive(true);           
            Utils.MoveToParent(grid5.transform, herotemp.transform);           
            herotemp.GetComponent<FragmentHero>().Refresh();
        }

        for (int i = 0; i < scLotteryComposeList.Star4Material.Count; i++)
        {
            var herotemp = Instantiate(FragmentHero) as GameObject;
            Dictionary<int, int>.KeyCollection keyCol = scLotteryComposeList.Star4Material[i].Keys;
            if (keyCol.Count != 1)
            {
                Debug.LogError("Incorrect Star4Material info, check the server info.");
                return;
            }
            foreach (int item in keyCol)
            {
                herotemp.GetComponent<FragmentHero>().TemplateID = item;
                herotemp.GetComponent<FragmentHero>().MaterialCount =
                    scLotteryComposeList.Star4Material[i][item];
            }
            if (herotemp.GetComponent<FragmentHero>().MaterialCount < 40 ||
                herotemp.GetComponent<FragmentHero>().MaterialCount + scLotteryComposeList.SuperMaterialCount < 50)
            {
                herotemp.GetComponent<FragmentHero>().UnInstallHandlers();
            }
            else
            {
                herotemp.GetComponent<FragmentHero>().InstallHandlers();
            }

            herotemp.SetActive(true);
            Utils.MoveToParent(grid4.transform, herotemp.transform);
            herotemp.GetComponent<FragmentHero>().Refresh();
        }

        //initialize the starbar4 position.
        grid4.transform.position = new Vector3(0, 118 - ((scLotteryComposeList.Star5Material.Count - 1) / 8 + 1) * 119,0);
    }

    //override Refresh function for changing FragmentList info.
    public void Refresh(SCLotteryComposeSucc msg)
    {
        scLotteryComposeSucc = msg;
        fragmentNum.text = scLotteryComposeSucc.SuperMaterialCount.ToString();
        FragmentHero[] tempFragmentHeros= transform.GetComponentsInChildren<FragmentHero>();
        foreach (var item in tempFragmentHeros)
        {
            if (item.TemplateID == msg.TemplateId)
            {
                item.MaterialCount = msg.MaterialCount;
                if (item.MaterialCount < 40 ||
                item.MaterialCount + scLotteryComposeSucc.SuperMaterialCount < 50)
                {
                    item.UnInstallHandlers();
                }
                else
                {
                    item.InstallHandlers();
                }
                Debug.Log("Combine succeed, FragmentList changed.");
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

    void Awake()
    {
        fragmentNum = transform.Find("Frame1/Frame2/SpriteInfo/Label").gameObject.GetComponent<UILabel>();              
        grid4 = transform.Find("Frame1/Frame2/ScrollView/StarBar4/Grid").gameObject;
        grid5 = transform.Find("Frame1/Frame2/ScrollView/StarBar5/Grid").gameObject;      
    }

    #endregion
}

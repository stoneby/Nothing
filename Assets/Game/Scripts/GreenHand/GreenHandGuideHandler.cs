using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class GreenHandGuideHandler : BattleMode
{
    #region Public Fields

    public bool IsGreenHand = false;
    public int CurrentConfig = 0;
    public List<string> TextList = new List<string>();
    public List<int> CanClickIndexList = new List<int>();
    public List<int> CanSelectIndexList = new List<int>();
    public List<int> ValidateIndexList = new List<int>();
    public List<int> MoveTraceIndexList=new List<int>(); 

    #endregion

    #region Private Fields

    private List<Vector3> moveTrace = new List<Vector3>();

    #endregion

    #region Public Methods

    /// <summary>
    /// Read CanSelectIndex list and ValidateIndex list config.
    /// </summary>
    public int ReadGuideConfig()
    {
        var configReader = new TestConfigReader();
        var result = configReader.ReadConfig(this, CurrentConfig);
        CurrentConfig++;
        return result;
    }

    /// <summary>
    /// Set a series of hero character can or can't selected.
    /// </summary>
    /// <param name="teamController"></param>
    /// <param name="enemyController"></param>
    public void SetCanSelect(TeamSelectController teamController, TeamSimpleController enemyController)
    {
        var canSelectPositions = new Position[CanSelectIndexList.Count];

        for (int i = 0; i < CanSelectIndexList.Count; i++)
        {
            canSelectPositions[i] = teamController.OneDimensionToTwo(CanSelectIndexList[i]);
        }

        for (int i = 0; i < 12; i++)
        {
            teamController.CharacterList[i].CanSelected = canSelectPositions.Contains(teamController.CharacterList[i].Location);
        }

        for (int i = 0; i < enemyController.CharacterList.Count; i++)
        {
            enemyController.CharacterList[i].CanSelected = CanSelectIndexList.Contains(i + 12);
        }
    }

    public void ShieldGuideWindowButtons()
    {
        var window = WindowManager.Instance.GetWindow<GreenHandGuideWindow>();
        if (window)
        {
            var isShieldList = new List<bool>();
            for (int i = 0; i < 5; i++)
            {
                if (CanClickIndexList.Contains(i))
                {
                    isShieldList.Add(false);
                }
                else
                {
                    isShieldList.Add(true);
                }
            }
            window.ShieldButtons(isShieldList);
        }
    }

    public void ShowAPeriodInfos(TeamSelectController teamController, Character[,] charactersLeft, TeamSimpleController monsterController, BattleFaceController faceController)
    {
        //Set moveTrace by MoveTraceIndex.
        var moveTraceArrangePos = new Position[MoveTraceIndexList.Count];
        for (int i = 0; i < MoveTraceIndexList.Count; i++)
        {
            moveTraceArrangePos[i] = teamController.OneDimensionToTwo(MoveTraceIndexList[i]);
        }
        moveTrace = new List<Vector3>();
        foreach (var position in moveTraceArrangePos)
        {
            moveTrace.Add(charactersLeft[position.X, position.Y].transform.position);
        }

        //Set moveTrace by canSelectIndex if it's null.
        if (moveTrace.Count == 0)
        {
            foreach (var item in CanSelectIndexList)
            {
                if (item > 11)
                {
                    moveTrace.Add(monsterController.CharacterList[item - 12].transform.position);
                }
            }
        }

        //Set moveTrace by canClickIndex if it's null.
        if (moveTrace.Count == 0)
        {
            for (int i = 0; i < CanClickIndexList.Count; i++)
            {
                if (CanClickIndexList[i] == 0)
                {
                    moveTrace.Add(faceController.transform.Find("ButtomLeftContainer/Menu").position);
                }
                else
                {
                    moveTrace.Add(faceController.LeaderController.LeaderList[CanClickIndexList[i] - 1].transform.position);
                }
            }
        }

        //Output moveTrace's null or not.
        if (moveTrace.Count == 0)
        {
            Logger.LogWarning("No moveTrace in configindex:" + (CurrentConfig - 1));
        }

        //Set greenHand window.
        var window = WindowManager.Instance.Show<GreenHandGuideWindow>(true);
        window.MoveTrace = moveTrace;
        if (TextList != null)
        {
            window.ShowComponents(true, true, false,false);
        }
        else
        {
            Logger.LogWarning("TextList in greenHandGuideHandler in null. Disable all operation.");
            window.ShowComponents(true, false, false,false);
        }
        if (moveTrace.Count > 1)
        {
            window.ShowAPeriodInfos(TextList, window.OnMoveFinger);
        }
        else if(moveTrace.Count==1)
        {
            window.ShowAPeriodInfos(TextList, window.OnBlinkFinger);
        }
        else
        {
            window.ShowAPeriodInfos(TextList, null);
        }
    }

    #endregion

    #region Inherit From Battle Mode

    public void ResetCurrentConfig()
    {
        CurrentConfig = 0;
    }

    public void SetBattleField(TeamSelectController teamController, TeamSimpleController enemyController, Character[,] characters, BattleFaceController faceController, string mode)
    {
        var flag = false;
        switch (mode)
        {
            case "MakeUpOneByOne":
                flag = (CurrentConfig != 5 - 1);
                break;
            case "ActiveSkill":
                flag = true;
                break;
            case "MonsterSelect":
                flag = (CurrentConfig == 2 - 1);
                break;
            case "UnderAttack":
                flag = (CurrentConfig == 5 - 1);
                break;
            default:
                Logger.LogError("Try to call SetBattleField fun");
                break;
        }
        if (flag)
        {
            Logger.Log("Read config.");
            if (ReadGuideConfig() == -1)
            {
                Logger.LogWarning("Read config fail, setting battle field cancelled.");
                return;
            }
            ShowAPeriodInfos(teamController, characters, enemyController, faceController);
            ShieldGuideWindowButtons();
            SetCanSelect(teamController, enemyController);
        }
    }

    /// <summary>
    /// Check selectedCharacterList can or can't attack
    /// </summary>
    /// <param name="teamController"></param>
    /// <returns></returns>
    public int CheckCanAttack(TeamSelectController teamController)
    {
        if (teamController.SelectedCharacterList.Count != ValidateIndexList.Count) return 0;

        for (int i = 0; i < ValidateIndexList.Count; i++)
        {
            int oneDimension = teamController.TwoDimensionToOne(teamController.SelectedCharacterList[i].Location);
            Logger.Log("!!!!!!!!!!!!OneDimension:" + oneDimension);
            if (oneDimension != ValidateIndexList[i])
            {
                return 0;
            }
        }
        return 1;
    }

    public void StopFingerMove()
    {
        //Stop MoveFinger translation.
        var window = WindowManager.Instance.GetWindow<GreenHandGuideWindow>();
        if (window)
        {
            window.ObjectMove.StopMove();
        }
    }

    #endregion
}

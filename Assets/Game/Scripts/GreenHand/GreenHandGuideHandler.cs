using System;
using Assets.Game.Scripts.Net.handler;
using KXSGCodec;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GreenHandGuideHandler : Singleton<GreenHandGuideHandler>, IBattleMode
{
    #region Public Fields

    public bool IsFlagAfterLogin = true;
    public bool IsGreenHand = false;
    public string ConfigMode;
    public List<string> TextList = new List<string>();
    public string NextConfigTriggerObjectTag;
    public int TagObjectIndex;
    public Vector3 NormalMoveVec;
    public bool IsWait;
    public float WaitSec;

    public bool BattleFinishFlag = true;
    public bool GiveHeroFinishFlag = true;
    public bool RaidFinishFlag = true;
    public bool SummitFinishFlag = true;
    public bool TeamFinishFlag = true;

    public GameObject TagObject;

    #region Battle Variables

    public List<int> CanSelectIndexList = new List<int>();
    public List<int> ValidateIndexList = new List<int>();
    public List<int> MoveTraceIndexList = new List<int>();

    /// <summary>
    /// Green hand guide finish flag.
    /// </summary>
    public bool IsGreenHandGuideFinish
    {
        get { return BattleFinishFlag && GiveHeroFinishFlag && RaidFinishFlag && SummitFinishFlag && TeamFinishFlag; }
    }

    #endregion

    #endregion

    #region Private Fields

    private int currentConfigIndex;

    private List<Vector3> moveTrace = new List<Vector3>();

    private readonly GreeenHandConfigReader configReader = new TemplateConfigReader();

    #endregion

    #region Public Methods

    public void ShowMainScreen()
    {
        StartCoroutine("DoShowMainScreen");
    }

    public void SetGreenHandGuideFlag(ThriftSCMessage msg)
    {
        var greenHandMsg = msg.GetContent() as SCGreenhandFlagMsg;
        var flagInfo = greenHandMsg.GreenhandPassFlag;
        BattleFinishFlag = (flagInfo & 1) != 0;
        GiveHeroFinishFlag = (flagInfo & 2) != 0;
        RaidFinishFlag = (flagInfo & 4) != 0;
        SummitFinishFlag = (flagInfo & 8) != 0;
        TeamFinishFlag = (flagInfo & 16) != 0;
        Debug.Log("!!!!!!!!!!!!flag info:" + flagInfo);
        Debug.Log("Flags:" + BattleFinishFlag + ", " + GiveHeroFinishFlag + ", " + RaidFinishFlag + ", " + SummitFinishFlag + ", " + TeamFinishFlag);

        if (!BattleFinishFlag)
        {
            Debug.Log("Send battle start msg in flag response.");
            StartCoroutine("SendStartBattleMessage");
        }
    }

    #endregion

    #region Inherit From IBattleMode

    public void ResetCurrentConfig()
    {
        currentConfigIndex = 0;
    }

    public void SetBattleField(TeamSelectController teamController, TeamSimpleController enemyController, Character[,] characters, string mode)
    {
        var flag = false;
        switch (mode)
        {
            case "Start":
                flag = (GetGuideBattleType(currentConfigIndex) == "Start");
                break;
            case "LeftAttack":
                flag = (GetGuideBattleType(currentConfigIndex) == "LeftAttack");
                break;
            case "Outter":
                flag = true;
                break;
            case "MonsterSelect":
                flag = (GetGuideBattleType(currentConfigIndex) == "MonsterSelect");
                break;
            case "UnderAttack":
                flag = (GetGuideBattleType(currentConfigIndex) == "UnderAttack");
                break;
            default:
                Logger.LogError("Try to call SetBattleField with inCorrect mode.");
                break;
        }
        if (flag)
        {
            //Read config info.
            Logger.Log("Read config.");
            if (!ReadGuideConfig())
            {
                Logger.LogWarning("Read config fail, setting battle field cancelled.");
                return;
            }

            SetGreenHandConfig();
        }
    }

    /// <summary>
    /// Check selectedCharacterList can or can't attack
    /// </summary>
    /// <param name="teamController"></param>
    /// <returns></returns>
    public int CheckCanAttack(TeamSelectController teamController)
    {
        if (teamController.SelectedCharacterList == null || ValidateIndexList == null)
        {
            Logger.LogError("List is null in CheckCanAttack.");
            return 0;
        }

        if (teamController.SelectedCharacterList.Count != ValidateIndexList.Count)
        {
            return 0;
        }

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
        if (WindowManager.Instance.ContainWindow<GreenHandGuideWindow>())
        {
            var window = WindowManager.Instance.GetWindow<GreenHandGuideWindow>();
            window.ObjectMove.StopMove();
        }
    }

    public void StopAll()
    {
        GlobalDimmerController.Instance.Show(false);

        if (WindowManager.Instance.ContainWindow<GreenHandGuideWindow>())
        {
            var window = WindowManager.Instance.GetWindow<GreenHandGuideWindow>();
            window.ObjectMove.StopMove();
            window.ShowComponents(false, false, false, false);
            window.ShowDimmerButtom(false);

            window.ClearAll();
        }
        WindowManager.Instance.Show<GreenHandGuideWindow>(false);

        currentConfigIndex = 0;
    }

    #endregion

    #region Private Methods

    private string GetGuideBattleType(int index)
    {
        return configReader.GetBattleType(index);
    }

    /// <summary>
    /// Read CanSelectIndex list and ValidateIndex list config.
    /// </summary>
    private bool ReadGuideConfig()
    {
        var result = configReader.ReadConfig(this, currentConfigIndex);

        //MTA track Read Guide Config.
        if (result)
        {
            //var dict = new Dictionary<string, string>
            //{
            //    {"Index", CurrentConfigIndex.ToString()},
            //};
            //MtaManager.TrackCustomKVEvent(MtaType.KVEventReadGreenHandConfig, dict);
            MtaManager.TrackCustomKVEvent(MtaType.KVEventReadGreenHandConfig + currentConfigIndex.ToString(), new Dictionary<string, string>());
        }

        Logger.Log("!!!!!!!!!!!!!!Read config in index: " + currentConfigIndex + ", result is: " + result);
        currentConfigIndex++;
        return result;
    }

    private IEnumerator DoShowMainScreen()
    {
        var checkCount = 0;
        while (!HttpResourceManager.Instance.IsLoadTemplateFinished)
        {
            checkCount++;
            if (checkCount > 20)
            {
                Debug.LogError("Check template data loaded time out.");
                yield break;
            }
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("!!!!!!!!!!!Show main screen.");
        WindowManager.Instance.GetWindow<LoginWindow>().GreenHandLoading.SetActive(false);
        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        WindowManager.Instance.Show<UIMainScreenWindow>(true);

        MemoryStrategy.Instance.HandleUIBegin();
    }

    private void FindTagObject(string tagString)
    {
        StartCoroutine(DoFindTagObject(tagString));
    }

    private IEnumerator DoFindTagObject(string tagString)
    {
        if (IsWait)
        {
            yield return new WaitForSeconds(WaitSec);
        }

        var objects = GameObject.FindGameObjectsWithTag(tagString);
        var findCount = 1;
        while (objects.Length == 0)
        {
            Logger.LogWarning("!!!!!!!!Specified tagObject not found, continue finding after 0.5 sec.");
            yield return new WaitForSeconds(0.5f);
            objects = GameObject.FindGameObjectsWithTag(tagString);
            findCount++;
            if (findCount == 20 && objects.Length == 0)
            {
                Logger.LogWarning("!!!!!!!!!!!!Can't find object after 10 seconds, cancel finding.");
                yield break;
            }
        }

        Logger.Log("!!!!!!!!!!Find tagObject, num:" + objects.Length + ", name in first:" + objects[0].name);

        // yield one more frame utill dimmer get closed.
        yield return null;

        if (objects.Length == 1)
        {
            TagObject = objects[0];
            ShowAPeriodInfos();
            yield break;
        }
        if (objects.Length > 1)
        {
            TagObject = objects[TagObjectIndex];
            ShowAPeriodInfos();
            yield break;
        }
    }

    private void SetNextTrigger()
    {
        Logger.Log("!!!!!!!!!!!Set next trigger in showAPeriodInfo.");
        SetNextTrigger(TagObject);
    }

    public void SetNextTrigger(GameObject go)
    {
        if (go == null)
        {
            Logger.LogError("tagObject is null in GreenHandGuideHandler.");
            return;
        }

        Logger.Log("!!!!!!!!!!!Set next trigger.");
        var listener = go.GetComponent<UIEventListener>() ?? go.AddComponent<UIEventListener>();
        if (ConfigMode != "NormalMove")
        {
            Logger.Log("!!!!!!Bind onclick lis in SetNextTrigger.");
            listener.onClick += GreenHandGuideCall;
        }
        else
        {
            Logger.Log("!!!!!!Bind onDragEnd lis in SetNextTrigger.");
            listener.onDragEnd += GreenHandGuideCall;
            GameObject.FindGameObjectWithTag("TeamCloseBTN").GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void GreenHandGuideCall(GameObject go)
    {
        Logger.Log("!!!!!!!!!!!!Go to GreenHandGuideCall.");
        //Subtract listener void delegate after call.
        TagObject.GetComponent<UIEventListener>().onClick -= GreenHandGuideCall;

        var greenHandGuideWindow = WindowManager.Instance.GetWindow<GreenHandGuideWindow>();
        if (greenHandGuideWindow)
        {
            Logger.Log("!!!!!!!!!!!!StopMove.");
            greenHandGuideWindow.ObjectMove.StopMove();
        }

        RestoreBlinkButtonDepth(TagObject);

        if (!ReadGuideConfig())
        {
            Logger.LogWarning("Read config fail, GreenHandGuideCall cancelled.");
            return;
        }

        SetGreenHandConfig();
    }

    private void RestoreBlinkButtonDepth(GameObject target)
    {
        //var panel = target.GetComponent<UIPanel>();
        //if (!panel)
        //{
        //    Logger.LogError("No panel found in blink button:" + target.name);
        //    return;
        //}
        //Destroy(panel);
    }

    /// <summary>
    /// Set a series of hero character can or can't selected.
    /// </summary>
    /// <param name="teamController"></param>
    /// <param name="enemyController"></param>
    private void SetCanSelect(TeamSelectController teamController, TeamSimpleController enemyController)
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

    private void ShowAPeriodInfos(TeamSelectController teamController, Character[,] charactersLeft, TeamSimpleController monsterController)
    {
        StartCoroutine(DoShowAPeriodInfos(teamController, charactersLeft, monsterController));
    }

    private IEnumerator DoShowAPeriodInfos(TeamSelectController teamController, Character[,] charactersLeft, TeamSimpleController monsterController)
    {
        if (IsWait)
        {
            yield return new WaitForSeconds(WaitSec);
        }

        //Set moveTrace by MoveTraceIndex.
        if (ConfigMode == "BattleMove")
        {
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
        }

        //Set moveTrace by canSelectIndex.
        else if (ConfigMode == "BattleBlink")
        {
            moveTrace = new List<Vector3>();
            foreach (var item in CanSelectIndexList)
            {
                if (item > 11)
                {
                    moveTrace.Add(monsterController.CharacterList[item - 12].transform.position);
                }
            }
        }

        //Set greenHand window.
        var window = WindowManager.Instance.Show<GreenHandGuideWindow>(true);
        window.MoveTrace = moveTrace;
        window.ShowDimmerButtom(true);
        if (TextList != null)
        {
            window.ShowComponents(true, true, false, false);
        }
        else
        {
            Logger.LogWarning("TextList in greenHandGuideHandler in null. Deactive the frame.");
            window.ShowComponents(true, false, false, false);
        }

        if (moveTrace.Count > 1)
        {
            window.ShowAPeriodInfos(TextList, window.OnMoveFinger);
        }
        else if (moveTrace.Count == 1)
        {
            window.ShowAPeriodInfos(TextList, window.OnBlinkFinger);
        }
        else
        {
            window.ShowAPeriodInfos(TextList, null);
        }

        //Set battleField can selected.
        SetCanSelect(teamController, monsterController);
    }

    private void ShowAPeriodInfos()
    {
        SetNextTrigger();

        //Set moveTrace.
        if (ConfigMode == "NormalBlink")
        {
            moveTrace = new List<Vector3> { TagObject.transform.position };
        }

        if (ConfigMode == "NormalMove")
        {
            moveTrace = new List<Vector3>() { TagObject.transform.position };
            moveTrace.Add(moveTrace[0] + NormalMoveVec);
        }

        //Set greenHand window.
        var window = WindowManager.Instance.Show<GreenHandGuideWindow>(true);
        window.MoveTrace = moveTrace;
        Logger.Log("!!!!!!!!GreenHand window movetrace count:" + moveTrace.Count);
        window.FingerBlinkButton = TagObject;
        window.ShowDimmerButtom(false);

        if (TextList != null)
        {
            window.ShowComponents(true, true, false, false);
        }
        else
        {
            Logger.LogWarning("TextList in greenHandGuideHandler in null. Deactive the frame.");
            window.ShowComponents(true, false, false, false);
        }

        if (moveTrace.Count > 1)
        {
            window.ShowAPeriodInfos(TextList, window.OnMoveFinger);
        }
        else if (moveTrace.Count == 1)
        {
            window.ShowAPeriodInfos(TextList, window.OnBlinkFinger);
        }
        else
        {
            window.ShowAPeriodInfos(TextList, null);
        }
    }

    private void DoGiveHero(GameObject go)
    {
        //FirstLoginGiveHero
        var message = new CSGreenhandStartMsg { GreenhandType = 2 };
        Debug.Log("Send greenhandGiveHero msg to server.");
        NetManager.SendMessage(message);
    }

    private void DoCreatePlayer(GameObject go)
    {
        PlayerHandler.OnCreatePlayer();
    }

    private void SetGreenHandConfig()
    {
        if (ConfigMode == "BattleBlink" || ConfigMode == "BattleMove")
        {
            if (WindowManager.Instance.ContainWindow<BattleWindow>())
            {
                var battleWindow = WindowManager.Instance.GetWindow<BattleWindow>();
                var initBattleField = battleWindow.gameObject.GetComponent<InitBattleField>();
                ShowAPeriodInfos(initBattleField.TeamController, initBattleField.charactersLeft, initBattleField.MonsterController);
            }
            else
            {
                Logger.LogError("BattleWindow not found, can't operate ShowAPeriodInfos(teamController, characters, enemyController).");
            }
        }
        else if (ConfigMode == "NormalBlink" || ConfigMode == "NormalMove")
        {
            FindTagObject(NextConfigTriggerObjectTag);
        }
        else if (ConfigMode == "CreatePlayer")
        {
            var window = WindowManager.Instance.Show<GreenHandGuideWindow>(true);
            window.ShowAPeriodInfos(TextList, DoCreatePlayer);
            window.ShowDimmerButtom(false);
            window.ShowComponents(true, true, false, false);
        }
        else if (ConfigMode == "GiveHero")
        {
            var window = WindowManager.Instance.Show<GreenHandGuideWindow>(true);
            window.ShowAPeriodInfos(TextList, DoGiveHero);
            window.ShowDimmerButtom(false);
            window.ShowComponents(true, true, false, false);
        }
        else
        {
            Logger.LogError("Not correct configMode in GreenHandGuideHandler.");
        }
    }

    #endregion

    #region Start GreenHand Module Interface

    public void ExecuteGreenHandFlag()
    {
        if (!GiveHeroFinishFlag)
        {
            StartCoroutine("GoToGiveHero");
        }
        else if (!RaidFinishFlag)
        {
            StartCoroutine("GoToScene");
        }
        else if (!SummitFinishFlag)
        {
            StartCoroutine("GoToChooseCard");
        }
        else if (!TeamFinishFlag)
        {
            Logger.Log("!!!!!!!!!!Go to team greenHandGuide.");
            StartCoroutine("GoToTeam");
        }
    }

    public void SendEndMessage(int typeID)
    {
        GlobalDimmerController.Instance.Show(false);

        var message = new CSGreenhandFinishMsg { GreenhandType = (sbyte)typeID };
        Logger.Log("!!!!!!!!!Send greenhandEnd msg to server, type:" + typeID);
        NetManager.SendMessage(message);
    }

    private IEnumerator SendStartBattleMessage()
    {
        var checkCount = 0;
        while (!HttpResourceManager.Instance.IsLoadTemplateFinished)
        {
            checkCount++;
            if (checkCount > 20)
            {
                Debug.LogError("Check template data loaded time out.");
                yield break;
            }
            yield return new WaitForSeconds(1.0f);
        }

        var message = new CSGreenhandStartMsg { GreenhandType = 1 };
        Debug.Log("Send greenhandBattleStart msg to server.");
        NetManager.SendMessage(message);

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
    }

    public void GoToCreatePlayer()
    {
        Debug.Log("Go to CreatePlayer.");
        currentConfigIndex = 10;
        if (!ReadGuideConfig())
        {
            Logger.LogWarning("Read config fail, setting battle field cancelled.");
            return;
        }

        SetGreenHandConfig();
    }

    public IEnumerator GoToGiveHero()
    {
        yield return null;

        Debug.Log("Go to HeroFirstLoginGive.");
        currentConfigIndex = 20;
        if (!ReadGuideConfig())
        {
            Logger.LogWarning("Read config fail, setting battle field cancelled.");
            yield break;
        }

        SetGreenHandConfig();
    }

    public IEnumerator GoToScene()
    {
        yield return null;

        currentConfigIndex = 30;
        if (!ReadGuideConfig())
        {
            Logger.LogWarning("Read config fail, setting battle field cancelled.");
            yield break;
        }

        SetGreenHandConfig();
    }

    public IEnumerator GoToChooseCard()
    {
        yield return null;

        currentConfigIndex = 40;
        if (!ReadGuideConfig())
        {
            Logger.LogWarning("Read config fail, setting battle field cancelled.");
            yield break;
        }

        SetGreenHandConfig();
    }

    public IEnumerator GoToTeam()
    {
        yield return null;

        currentConfigIndex = 50;
        if (!ReadGuideConfig())
        {
            Logger.LogWarning("Read config fail, setting battle field cancelled.");
            yield break;
        }

        SetGreenHandConfig();
    }

    #endregion
}

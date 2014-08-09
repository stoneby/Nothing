using Assets.Game.Scripts.Common.Model;
using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PersistenceHandler : MonoBehaviour
{
    #region Private Fields

    private bool isOpenPersistence = true;

    //Waiting seconds
    private const int WaitingSeconds = 3;

    private bool IsReSendMessageNowClickDown = false;

    #endregion

    #region Private Methods

    private void OnReStratBattle(GameObject go)
    {
        WindowManager.Instance.Show<SimpleConfirmWindow>(false);

        LoadMissionModelLocator();
        var battleStartMsg = LoadStartBattleMessage();

        PopTextManager.PopTip("返回战斗数据");
        BattleModelLocator.Instance.BattleType = battleStartMsg.BattleType;
        BattleModelLocator.Instance.RaidID = battleStartMsg.RaidID;
        BattleModelLocator.Instance.Uuid = battleStartMsg.Uuid;

        // server logic data.
        BattleCreateUtils.initBattleModeLocator(BattleModelLocator.Instance, battleStartMsg);


        var factory = BattleModelLocator.Instance.Source.BattleType.Factory;

        BattleModelLocator.Instance.MainBattle = factory.createBattle(BattleModelLocator.Instance.Source);

        Dictionary<string, float> tempPersistence = new Dictionary<string, float>();
        if (Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            tempPersistence = LoadPersistence();
            BattleModelLocator.Instance.MainBattle.startOnSceneIndex((int)tempPersistence["TopData"]);
        }
        else
        {
            BattleModelLocator.Instance.MainBattle.start();
        }

        BattleModelLocator.Instance.MonsterIndex = 0;

        // client side show.
        var window = Singleton<WindowManager>.Instance.Show(typeof(BattleWindow), true).gameObject;
        Singleton<WindowManager>.Instance.Show(typeof(MissionTabWindow), false);
        Singleton<WindowManager>.Instance.Show(typeof(MainMenuBarWindow), false);
        Singleton<WindowManager>.Instance.Show(typeof(BattleConfirmTabWindow), false);

        if (Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            window.GetComponent<BattleWindow>().Battle.PersisitenceSet(tempPersistence);
        }
    }

    private void OnReSendMessage(GameObject go)
    {
        if (Mode == PersistenceMode.ReSendMessageNext)
        {
            LoadMissionModelLocator();
            var battleEndMsg = LoadBattleEndMessage();

            NetManager.SendMessage(battleEndMsg);
            MtaManager.TrackEndPage(MtaType.BattleScreen);

            //Check battle end succeed.
            StartCoroutine(MakeBattleEndSucceed(battleEndMsg));
        }

        if (Mode == PersistenceMode.ReSendMessageNow)
        {
            IsReSendMessageNowClickDown = true;
        }
    }

    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<SimpleConfirmWindow>(false);
        if (Mode != PersistenceMode.ReSendMessageNow)
        {
            new FileInfo(MissionModelLocatorPath).Delete();
            new FileInfo(StartBattleMessagePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(BattleEndMessagePath).Delete();
        }
    }

    private void SetConfirmWindow(SimpleConfirmWindow window)
    {
        if (Mode == PersistenceMode.ReStartBattle || Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            window.SetLabelAndLis("在当前账号下检测到未完成的副本，是否继续副本？", OnReStratBattle, OnCancel);
        }
        else if (Mode == PersistenceMode.ReSendMessageNext)
        {
            window.SetLabelAndLis("在当前账号下检测到未发送的战斗结束消息，是否继续发送？", OnReSendMessage, OnCancel);
        }
        else if (Mode == PersistenceMode.ReSendMessageNow)
        {
            window.SetLabelAndLis("发送消息失败，是否继续发送？", OnReSendMessage, OnCancel);
        }
        else
        {
            Logger.LogError("Error to persistence way.");
        }
    }

    #endregion

    #region Public Fields

#if UNITY_EDITOR

    public static string LoginInfoPath = Application.dataPath + "/LoginInfo.txt";

    public static string StartBattleMessagePath = Application.dataPath + "/StartBattleMessageInfo.txt";

    public static string PersistencePath = Application.dataPath + "/PersistenceInfo.txt";

    public static string MissionModelLocatorPath = Application.dataPath + "/MissionModelLocatorInfo.txt";

    public static string BattleEndMessagePath = Application.dataPath + "/BattleEndMessageInfo.txt";

#else

    public static string LoginInfoPath = Application.persistentDataPath + "/LoginInfo.txt";

    public static string StartBattleMessagePath = Application.persistentDataPath + "/StartBattleMessageInfo.txt";

    public static string PersistencePath = Application.persistentDataPath + "/PersistenceInfo.txt";

    public static string MissionModelLocatorPath = Application.persistentDataPath + "/MissionModelLocatorInfo.txt";

    public static string BattleEndMessagePath = Application.persistentDataPath + "/BattleEndMessageInfo.txt";

#endif

    public static bool isRaidReward = false;

    public enum PersistenceMode
    {
        ReSendMessageNow,
        ReStartBattle,
        ReStartBattleWithPersistence,
        ReSendMessageNext,
        Normal
    }

    public static PersistenceMode Mode = PersistenceMode.Normal;

    #endregion

    #region Public Methods

    private void StoreLoginInfo(Dictionary<string, string> value)
    {
        StartCoroutine(DoStoreLoginInfo(value));
    }

    private IEnumerator DoStoreLoginInfo(Dictionary<string, string> value)
    {
        StreamWriter writer;
        var fileInfo = new FileInfo(LoginInfoPath);

        if (fileInfo.Exists == true)
        {
            fileInfo.Delete();
        }

        writer = fileInfo.CreateText();

        foreach (var pair in value)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value);
        }

        writer.Close();
        Logger.Log("LoginInfo file written");

        yield return null;
    }

    private Dictionary<string, string> LoadLoginInfo()
    {
        StreamReader reader;
        var fileInfo = new FileInfo(LoginInfoPath);

        if (fileInfo.Exists == false)
        {
            Debug.LogError("LoginInfo file not existed! Load file cancelled!");
            return null;
        }

        reader = fileInfo.OpenText();
        var loginInfo = new Dictionary<string, string>();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
            }
            loginInfo.Add(splitedStrings[0], splitedStrings[1]);
        }

        reader.Close();
        Logger.Log("LoginInfo file loaded");

        return loginInfo;
    }

    public void StoreMissionModelLocator()
    {
        if (isOpenPersistence)
        {
            StartCoroutine(DoStoreMissionModelLocator());
        }
    }

    private IEnumerator DoStoreMissionModelLocator()
    {
        Dictionary<string, string> valueMissionModelLocator = new Dictionary<string, string>();
        var missionModelLocator = MissionModelLocator.Instance;

        valueMissionModelLocator.Add("Raid.templateID", missionModelLocator.Raid.TemplateId.ToString());
        valueMissionModelLocator.Add("Raid.isSet.templateID", missionModelLocator.Raid.__isset.templateId.ToString());
        valueMissionModelLocator.Add("Raid.isSet.stateInfo", missionModelLocator.Raid.__isset.stateInfo.ToString());

        valueMissionModelLocator.Add("FriendData.data.friendUuid", missionModelLocator.FriendData.Data.FriendUuid.ToString());
        valueMissionModelLocator.Add("FriendData.data.friendName", missionModelLocator.FriendData.Data.FriendName);
        valueMissionModelLocator.Add("FriendData.data.friendLvl", missionModelLocator.FriendData.Data.FriendLvl.ToString());

        for (int i = 0; i < missionModelLocator.FriendData.Data.HeroProp.Count; i++)
        {
            valueMissionModelLocator.Add("FriendData.data.heroProp[" + i + "].heroTemplateID", missionModelLocator.FriendData.Data.HeroProp[i].HeroTemplateId.ToString());
        }
        valueMissionModelLocator.Add("FriendData.data.heroProp.ListCount", missionModelLocator.FriendData.Data.HeroProp.Count.ToString());

        valueMissionModelLocator.Add("FriendData.data.isSet.friendUuid", missionModelLocator.FriendData.Data.__isset.friendUuid.ToString());
        valueMissionModelLocator.Add("FriendData.data.isSet.friendName", missionModelLocator.FriendData.Data.__isset.friendName.ToString());
        valueMissionModelLocator.Add("FriendData.data.isSet.friendLvl", missionModelLocator.FriendData.Data.__isset.friendLvl.ToString());
        valueMissionModelLocator.Add("FriendData.data.isSet.heroProp", missionModelLocator.FriendData.Data.__isset.heroProp.ToString());

        valueMissionModelLocator.Add("SelectedStageID", missionModelLocator.SelectedStageId.ToString());
        valueMissionModelLocator.Add("OldExp", missionModelLocator.OldExp.ToString());
        valueMissionModelLocator.Add("OldLevel", missionModelLocator.OldLevel.ToString());
        valueMissionModelLocator.Add("StarCount", missionModelLocator.StarCount.ToString());

        int pairCount = 0;
        foreach (var pair in missionModelLocator.TotalStarCount)
        {
            valueMissionModelLocator.Add("TotalStarCount.Keys[" + pairCount + "]", pair.Key.ToString());
            valueMissionModelLocator.Add("TotalStarCount.Values[" + pairCount + "]", pair.Value.ToString());
            pairCount++;
        }
        valueMissionModelLocator.Add("TotalStarCount.DictionaryCount", pairCount.ToString());

        StreamWriter writer;
        var fileInfo = new FileInfo(MissionModelLocatorPath);

        if (fileInfo.Exists == true)
        {
            fileInfo.Delete();
        }

        writer = fileInfo.CreateText();

        foreach (var pair in valueMissionModelLocator)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value);
        }

        writer.Close();
        Logger.Log("MissionModelLocator file written");

        yield return null;
    }

    private void LoadMissionModelLocator()
    {
        StreamReader reader;
        var fileInfo = new FileInfo(MissionModelLocatorPath);

        if (fileInfo.Exists == false)
        {
            Debug.LogError("MissionModelLocator file not existed! Load file cancelled!");
            return;
        }

        reader = fileInfo.OpenText();

        Dictionary<string, string> missionModelLocatorInfo = new Dictionary<string, string>();
        missionModelLocatorInfo.Clear();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
            }
            missionModelLocatorInfo.Add(splitedStrings[0], splitedStrings[1]);
        }
        reader.Close();
        Logger.Log("MissionModelLocator file loaded");

        var missionModelLocator = MissionModelLocator.Instance;

        if (missionModelLocator.Raid == null)
        {
            missionModelLocator.Raid = new RaidInfo();
        }
        missionModelLocator.Raid.TemplateId = int.Parse(missionModelLocatorInfo["Raid.templateID"]);
        //if (missionModelLocator.Raid.__isset == null)
        //{
        //    missionModelLocator.Raid = new RaidInfo();
        //}
        missionModelLocator.Raid.__isset.templateId = bool.Parse(missionModelLocatorInfo["Raid.isSet.templateID"]);
        missionModelLocator.Raid.__isset.stateInfo = bool.Parse(missionModelLocatorInfo["Raid.isSet.stateInfo"]);

        if (missionModelLocator.FriendData == null)
        {
            missionModelLocator.FriendData = new FriendVO();
        }
        if (missionModelLocator.FriendData.Data == null)
        {
            missionModelLocator.FriendData.Data = new FriendInfo();
        }
        missionModelLocator.FriendData.Data.FriendUuid = long.Parse(missionModelLocatorInfo["FriendData.data.friendUuid"]);
        missionModelLocator.FriendData.Data.FriendName = missionModelLocatorInfo["FriendData.data.friendName"];
        missionModelLocator.FriendData.Data.FriendLvl = int.Parse(missionModelLocatorInfo["FriendData.data.friendLvl"]);

        int listCount = int.Parse(missionModelLocatorInfo["FriendData.data.heroProp.ListCount"]);
        if (missionModelLocator.FriendData.Data.HeroProp == null)
        {
            missionModelLocator.FriendData.Data.HeroProp = new List<HeroPropInfo>();
        }
        while (missionModelLocator.FriendData.Data.HeroProp.Count < listCount)
        {
            missionModelLocator.FriendData.Data.HeroProp.Add(new HeroPropInfo());
        }
        for (int i = 0; i < missionModelLocator.FriendData.Data.HeroProp.Count; i++)
        {
            if (i < listCount)
            {
                missionModelLocator.FriendData.Data.HeroProp[i].HeroTemplateId = int.Parse(missionModelLocatorInfo["FriendData.data.heroProp[" + i + "].heroTemplateID"]);
            }
        }

        missionModelLocator.FriendData.Data.__isset.friendUuid = bool.Parse(missionModelLocatorInfo["FriendData.data.isSet.friendUuid"]);
        missionModelLocator.FriendData.Data.__isset.friendName = bool.Parse(missionModelLocatorInfo["FriendData.data.isSet.friendName"]);
        missionModelLocator.FriendData.Data.__isset.friendLvl = bool.Parse(missionModelLocatorInfo["FriendData.data.isSet.friendLvl"]);
        missionModelLocator.FriendData.Data.__isset.heroProp = bool.Parse(missionModelLocatorInfo["FriendData.data.isSet.heroProp"]);

        missionModelLocator.SelectedStageId = int.Parse(missionModelLocatorInfo["SelectedStageID"]);
        missionModelLocator.OldExp = int.Parse(missionModelLocatorInfo["OldExp"]);
        missionModelLocator.OldLevel = int.Parse(missionModelLocatorInfo["OldLevel"]);
        missionModelLocator.StarCount = int.Parse(missionModelLocatorInfo["StarCount"]);

        if (missionModelLocator.TotalStarCount == null)
        {
            missionModelLocator.TotalStarCount = new Dictionary<int, int>();
        }
        missionModelLocator.TotalStarCount.Clear();
        for (int i = 0; i < int.Parse(missionModelLocatorInfo["TotalStarCount.DictionaryCount"]); i++)
        {
            missionModelLocator.TotalStarCount.Add(int.Parse(missionModelLocatorInfo["TotalStarCount.Keys[" + i + "]"]), int.Parse(missionModelLocatorInfo["TotalStarCount.Values[" + i + "]"]));
        }

        return;
    }

    public void StoreStartBattleMessage(SCBattlePveStartMsg battlestartmsg)
    {
        if (isOpenPersistence)
        {
            StartCoroutine(DoStoreStartBattleMessage(battlestartmsg));
        }
    }

    private IEnumerator DoStoreStartBattleMessage(SCBattlePveStartMsg battlestartmsg)
    {
        var valueBattleStartMsg = new Dictionary<string, string>();
        valueBattleStartMsg.Add("Uuid", battlestartmsg.Uuid.ToString());
        valueBattleStartMsg.Add("BattleType", battlestartmsg.BattleType.ToString());
        valueBattleStartMsg.Add("SpMaxBuffId", battlestartmsg.SpMaxBuffId.ToString());
        valueBattleStartMsg.Add("RaidID", battlestartmsg.RaidID.ToString());

        for (int i = 0; i < battlestartmsg.FighterList.Count; i++)
        {
            valueBattleStartMsg.Add("FighterList[" + i + "].Index", battlestartmsg.FighterList[i].Index.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].TemplateId", battlestartmsg.FighterList[i].TemplateId.ToString());

            int fighterPropPairCount = 0;
            foreach (var pair in battlestartmsg.FighterList[i].FighteProp)
            {
                valueBattleStartMsg.Add("FighterList[" + i + "].FighteProp.Keys[" + fighterPropPairCount + "]", pair.Key.ToString());
                valueBattleStartMsg.Add("FighterList[" + i + "].FighteProp.Values[" + fighterPropPairCount + "]", pair.Value.ToString());
                fighterPropPairCount++;
            }
            valueBattleStartMsg.Add("FighterList[" + i + "].FighteProp.PairCount", fighterPropPairCount.ToString());

            if (battlestartmsg.FighterList[i].OtherProp != null)
            {
                int otherPropPairCount = 0;
                foreach (var pair in battlestartmsg.FighterList[i].OtherProp)
                {
                    valueBattleStartMsg.Add("FighterList[" + i + "].OtherProp.Keys[" + otherPropPairCount + "]", pair.Key.ToString());
                    valueBattleStartMsg.Add("FighterList[" + i + "].OtherProp.Values[" + otherPropPairCount + "]", pair.Value.ToString());
                }
                valueBattleStartMsg.Add("FighterList[" + i + "].OtherProp.PropPairCount", otherPropPairCount.ToString());
            }
            else
            {
                valueBattleStartMsg.Add("FighterList[" + i + "].OtherProp.PropPairCount", 0.ToString());
            }

            valueBattleStartMsg.Add("FighterList[" + i + "].ActiveSkillId", battlestartmsg.FighterList[i].ActiveSkillId.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].LeaderSkill", battlestartmsg.FighterList[i].LeaderSkill.ToString());

            for (int j = 0; j < battlestartmsg.FighterList[i].AllSkill.Count; j++)
            {
                valueBattleStartMsg.Add("FighterList[" + i + "].AllSkill[" + j + "]", battlestartmsg.FighterList[i].AllSkill[j].ToString());
            }
            valueBattleStartMsg.Add("FighterList[" + i + "].AllSkill.ListCount", battlestartmsg.FighterList[i].AllSkill.Count.ToString());

            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.activeSkillID", battlestartmsg.FighterList[i].__isset.activeSkillId.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.allSkill", battlestartmsg.FighterList[i].__isset.allSkill.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.fighteProp", battlestartmsg.FighterList[i].__isset.fighteProp.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.heroType", battlestartmsg.FighterList[i].__isset.heroType.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.index", battlestartmsg.FighterList[i].__isset.index.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.jobId", battlestartmsg.FighterList[i].__isset.jobId.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.leaderSkill", battlestartmsg.FighterList[i].__isset.leaderSkill.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.otherProp", battlestartmsg.FighterList[i].__isset.otherProp.ToString());
            valueBattleStartMsg.Add("FighterList[" + i + "].isSet.templateId", battlestartmsg.FighterList[i].__isset.templateId.ToString());
        }
        valueBattleStartMsg.Add("FighterList.ListCount", battlestartmsg.FighterList.Count.ToString());

        for (int i = 0; i < battlestartmsg.MonsterList.Count; i++)
        {
            valueBattleStartMsg.Add("MonsterList[" + i + "].Index", battlestartmsg.MonsterList[i].Index.ToString());
            valueBattleStartMsg.Add("MonsterList[" + i + "].TemplateId", battlestartmsg.MonsterList[i].TemplateId.ToString());

            int DropMapPairCount = 0;
            foreach (var pair in battlestartmsg.MonsterList[i].DropMap)
            {
                valueBattleStartMsg.Add("MonsterList[" + i + "].DropMap.Keys[" + DropMapPairCount + "]", pair.Key.ToString());
                valueBattleStartMsg.Add("MonsterList[" + i + "].DropMap.Values[" + DropMapPairCount + "]", pair.Value.ToString());
                DropMapPairCount++;
            }
            valueBattleStartMsg.Add("MonsterList[" + i + "].DropMap.PairCount", DropMapPairCount.ToString());

            valueBattleStartMsg.Add("MonsterList[" + i + "].IsSet.DropMap", battlestartmsg.MonsterList[i].__isset.dropMap.ToString());
            valueBattleStartMsg.Add("MonsterList[" + i + "].IsSet.Index", battlestartmsg.MonsterList[i].__isset.index.ToString());
            valueBattleStartMsg.Add("MonsterList[" + i + "].IsSet.TemplateID", battlestartmsg.MonsterList[i].__isset.templateId.ToString());
        }
        valueBattleStartMsg.Add("MonsterList.ListCount", battlestartmsg.MonsterList.Count.ToString());

        valueBattleStartMsg.Add("IsSet.battleType", battlestartmsg.__isset.battleType.ToString());
        valueBattleStartMsg.Add("IsSet.fighterList", battlestartmsg.__isset.fighterList.ToString());
        valueBattleStartMsg.Add("IsSet.monsterList", battlestartmsg.__isset.monsterList.ToString());
        valueBattleStartMsg.Add("IsSet.raidID", battlestartmsg.__isset.raidID.ToString());
        valueBattleStartMsg.Add("IsSet.spMaxBuffId", battlestartmsg.__isset.spMaxBuffId.ToString());
        valueBattleStartMsg.Add("IsSet.uuid", battlestartmsg.__isset.uuid.ToString());

        StreamWriter writer;
        var fileInfo = new FileInfo(StartBattleMessagePath);

        if (fileInfo.Exists == true)
        {
            fileInfo.Delete();
        }

        writer = fileInfo.CreateText();

        foreach (var pair in valueBattleStartMsg)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value);
        }

        writer.Close();
        Logger.Log("StartBattleMessage file written");

        yield return null;
    }

    private SCBattlePveStartMsg LoadStartBattleMessage()
    {
        StreamReader reader;
        var fileInfo = new FileInfo(StartBattleMessagePath);

        if (fileInfo.Exists == false)
        {
            Debug.LogError("StartBattleMessage file not existed! Load file cancelled!");
            return null;
        }

        reader = fileInfo.OpenText();

        Dictionary<string, string> startBattleMessageInfo = new Dictionary<string, string>();
        startBattleMessageInfo.Clear();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
            }
            startBattleMessageInfo.Add(splitedStrings[0], splitedStrings[1]);
        }
        reader.Close();
        Logger.Log("StartBattleMessage file loaded");

        SCBattlePveStartMsg battlestartmsg = new SCBattlePveStartMsg()
        {
            Uuid = long.Parse(startBattleMessageInfo["Uuid"]),
            BattleType = sbyte.Parse(startBattleMessageInfo["BattleType"]),
            SpMaxBuffId = int.Parse(startBattleMessageInfo["SpMaxBuffId"]),
            RaidID = int.Parse(startBattleMessageInfo["RaidID"]),
            __isset = new SCBattlePveStartMsg.Isset()
            {
                battleType = bool.Parse(startBattleMessageInfo["IsSet.battleType"]),
                fighterList = bool.Parse(startBattleMessageInfo["IsSet.fighterList"]),
                monsterList = bool.Parse(startBattleMessageInfo["IsSet.monsterList"]),
                raidID = bool.Parse(startBattleMessageInfo["IsSet.raidID"]),
                spMaxBuffId = bool.Parse(startBattleMessageInfo["IsSet.spMaxBuffId"]),
                uuid = bool.Parse(startBattleMessageInfo["IsSet.uuid"]),
            }
        };

        var fighterList = new List<BattleMsgHero>();
        for (int i = 0; i < int.Parse(startBattleMessageInfo["FighterList.ListCount"]); i++)
        {
            BattleMsgHero battleMsgHero=new BattleMsgHero()
            {
                Index = int.Parse(startBattleMessageInfo["FighterList[" + i + "].Index"]),
                TemplateId = int.Parse(startBattleMessageInfo["FighterList[" + i + "].TemplateId"]),
                ActiveSkillId = int.Parse(startBattleMessageInfo["FighterList[" + i + "].ActiveSkillId"]),
                LeaderSkill = int.Parse(startBattleMessageInfo["FighterList[" + i + "].LeaderSkill"]),
                __isset = new BattleMsgHero.Isset()
                {
                    activeSkillId = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.activeSkillID"]),
                    allSkill = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.allSkill"]),
                    fighteProp = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.fighteProp"]),
                    heroType = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.heroType"]),
                    index = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.index"]),
                    jobId = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.jobId"]),
                    leaderSkill = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.leaderSkill"]),
                    otherProp = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.otherProp"]),
                    templateId = bool.Parse(startBattleMessageInfo["FighterList[" + i + "].isSet.templateId"]),
                }
            };

            var fighterProp = new Dictionary<int, int>();
            for (int j = 0; j < int.Parse(startBattleMessageInfo["FighterList[" + i + "].FighteProp.PairCount"]); j++)
            {
                fighterProp.Add(int.Parse(startBattleMessageInfo["FighterList[" + i + "].FighteProp.Keys[" + j + "]"]),int.Parse(startBattleMessageInfo["FighterList[" + i + "].FighteProp.Values[" + j + "]"]));
            }
            battleMsgHero.FighteProp = fighterProp;

            var otherProp = new Dictionary<int, int>();
            for (int j = 0; j < int.Parse(startBattleMessageInfo["FighterList[" + i + "].OtherProp.PropPairCount"]); j++)
            {
                otherProp.Add(int.Parse(startBattleMessageInfo["FighterList[" + i + "].OtherProp.Keys[" + j + "]"]),int.Parse(startBattleMessageInfo["FighterList[" + i + "].OtherProp.Values[" + j + "]"]));
            }
            battleMsgHero.OtherProp = otherProp;

            var allSkill = new List<int>();
            for (int j = 0; j < int.Parse(startBattleMessageInfo["FighterList[" + i + "].AllSkill.ListCount"]); j++)
            {
                allSkill.Add(int.Parse(startBattleMessageInfo["FighterList[" + i + "].AllSkill[" + j + "]"]));
            }
            battleMsgHero.AllSkill = allSkill;

            fighterList.Add(battleMsgHero);
        }

        var monsterList = new List<BattleMsgMonster>();
        for (int i = 0; i < int.Parse(startBattleMessageInfo["MonsterList.ListCount"]); i++)
        {
            BattleMsgMonster battleMsgMonster=new BattleMsgMonster()
            {
                Index = int.Parse(startBattleMessageInfo["MonsterList[" + i + "].Index"]),
                TemplateId = int.Parse(startBattleMessageInfo["MonsterList[" + i + "].TemplateId"]),
                __isset = new BattleMsgMonster.Isset()
                {
                    dropMap = bool.Parse(startBattleMessageInfo["MonsterList[" + i + "].IsSet.DropMap"]),
                    index = bool.Parse(startBattleMessageInfo["MonsterList[" + i + "].IsSet.Index"]),
                    templateId = bool.Parse(startBattleMessageInfo["MonsterList[" + i + "].IsSet.TemplateID"]),
                },
            };

            var dropMap = new Dictionary<sbyte, int>();
            for (int j = 0; j < int.Parse(startBattleMessageInfo["MonsterList[" + i + "].DropMap.PairCount"]); j++)
            {
                dropMap.Add(sbyte.Parse(startBattleMessageInfo["MonsterList[" + i + "].DropMap.Keys[" + j + "]"]),int.Parse(startBattleMessageInfo["MonsterList[" + i + "].DropMap.Values[" + j + "]"]));
            }
            battleMsgMonster.DropMap = dropMap;

            monsterList.Add(battleMsgMonster);
        }

        battlestartmsg.FighterList = fighterList;
        battlestartmsg.MonsterList = monsterList;

        return battlestartmsg;
    }

    public void StorePersistence(Dictionary<string, float> value)
    {
        if (isOpenPersistence)
        {
            DoStorePersistence(value);
        }
    }

    private void DoStorePersistence(Dictionary<string, float> value)
    {
        StreamWriter writer;
        var fileInfo = new FileInfo(PersistencePath);

        if (fileInfo.Exists == true)
        {
            fileInfo.Delete();
        }

        writer = fileInfo.CreateText();

        foreach (var pair in value)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value.ToString());
        }

        writer.Close();
        Logger.Log("Persistence file written");
    }

    private Dictionary<string, float> LoadPersistence()
    {
        StreamReader reader;
        var fileInfo = new FileInfo(PersistencePath);

        if (fileInfo.Exists == false)
        {
            Debug.LogError("Persistence file not existed! Load file cancelled!");
            return null;
        }

        reader = fileInfo.OpenText();

        Dictionary<string, float> persistenceInfo = new Dictionary<string, float>();
        persistenceInfo.Clear();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
            }
            persistenceInfo.Add(splitedStrings[0], Single.Parse(splitedStrings[1]));
        }
        reader.Close();
        Logger.Log("Persistence file loaded");

        return persistenceInfo;
    }

    public void StoreBattleEndMessage(Dictionary<string, float> value)
    {
        if (isOpenPersistence)
        {
            StartCoroutine(DoStoreBattleEndMessage(value));
        }
    }

    private IEnumerator DoStoreBattleEndMessage(Dictionary<string, float> value)
    {
        StreamWriter writer;
        var fileInfo = new FileInfo(BattleEndMessagePath);

        if (fileInfo.Exists == true)
        {
            fileInfo.Delete();
        }

        writer = fileInfo.CreateText();

        foreach (var pair in value)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value.ToString());
        }

        writer.Close();
        Logger.Log("BattleEndMessage file written");

        yield return null;
    }

    private CSBattlePveFinishMsg LoadBattleEndMessage()
    {
        StreamReader reader;
        var fileInfo = new FileInfo(BattleEndMessagePath);

        if (fileInfo.Exists == false)
        {
            Debug.LogError("BattleEndMessage file not existed! Load file cancelled!");
            return null;
        }

        reader = fileInfo.OpenText();

        Dictionary<string, float> battleEndMessageInfo = new Dictionary<string, float>();
        battleEndMessageInfo.Clear();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
            }
            battleEndMessageInfo.Add(splitedStrings[0], Single.Parse(splitedStrings[1]));
        }
        reader.Close();
        Logger.Log("BattleEndMessage file loaded");

        var msg = new CSBattlePveFinishMsg();
        msg.Uuid = (long)battleEndMessageInfo["Uuid"];
        msg.BattleResult = (int)battleEndMessageInfo["BattleResult"];
        msg.Star = (sbyte)battleEndMessageInfo["Star"];

        return msg;
    }

    public IEnumerator MakeBattleEndSucceed(CSBattlePveFinishMsg msg)
    {
        if (isOpenPersistence)
        {
            yield return new WaitForSeconds(WaitingSeconds);
            while (isRaidReward == false)
            {
                if (WindowManager.Instance.GetWindow<SimpleConfirmWindow>() == null)
                {
                    WindowManager.Instance.Show<SimpleConfirmWindow>(true);
                }
                Mode = PersistenceMode.ReSendMessageNow;
                var window = WindowManager.Instance.GetWindow<SimpleConfirmWindow>();
                window.gameObject.SetActive(true);
                SetConfirmWindow(window);

                if (IsReSendMessageNowClickDown == true)
                {
                    WindowManager.Instance.Show<SimpleConfirmWindow>(false);

                    IsReSendMessageNowClickDown = false;
                    Logger.Log("Resend Battle End Message now.");
                    NetManager.SendMessage(msg);
                }
                yield return new WaitForSeconds(WaitingSeconds);
            }

            //Delete file.
            new FileInfo(MissionModelLocatorPath).Delete();
            new FileInfo(StartBattleMessagePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(BattleEndMessagePath).Delete();
        }
    }

    public void GoToPersistenceWay()
    {
        if (isOpenPersistence)
        {
            StartCoroutine(PersistenceExecute());
        }
    }

    private IEnumerator PersistenceExecute()
    {
        //Load perisitence file
        if (new FileInfo(LoginInfoPath).Exists == true)
        {
            if (long.Parse(LoadLoginInfo()["AccountID"]) == ServiceManager.UserID && LoadLoginInfo()["ServerID"] == ServiceManager.ServerData.ID)
            {
                if (new FileInfo(StartBattleMessagePath).Exists == true && new FileInfo(BattleEndMessagePath).Exists == false)
                {
                    Logger.Log("Persistence mode: ReStartBattle.");

                    if (new FileInfo(PersistencePath).Exists == false)
                    {
                        Mode = PersistenceMode.ReStartBattle;
                    }
                    else
                    {
                        Mode = PersistenceMode.ReStartBattleWithPersistence;
                    }
                    var window = WindowManager.Instance.Show<SimpleConfirmWindow>(true);
                    window.gameObject.SetActive(true);
                    SetConfirmWindow(window);
                }
                else if (new FileInfo(StartBattleMessagePath).Exists == true && new FileInfo(BattleEndMessagePath).Exists == true)
                {
                    Logger.Log("Persistence mode: ReSendMessage.");

                    Mode = PersistenceMode.ReSendMessageNext;
                    var window = WindowManager.Instance.Show<SimpleConfirmWindow>(true);
                    window.gameObject.SetActive(true);
                    SetConfirmWindow(window);
                }
            }
            else
            {
                //Delete file if switch account or server.
                new FileInfo(MissionModelLocatorPath).Delete();
                new FileInfo(StartBattleMessagePath).Delete();
                new FileInfo(PersistencePath).Delete();
                new FileInfo(BattleEndMessagePath).Delete();
            }
        }

        //Store loginaccount file 
        var tempDictionary = new Dictionary<string, string>
        {
                {"AccountID", ServiceManager.UserID.ToString()},
                {"ServerID", ServiceManager.ServerData.ID}
        };
        StoreLoginInfo(tempDictionary);

        yield return null;
    }

    #endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#if !SILVERLIGHT
[Serializable]
#endif
public class BattleWindow : Window
{
    #region Private Fields

    //Waiting seconds
    private const int WaitingSeconds = 3;

    #endregion

    #region Public Fields

    /// <summary>
    /// Battle controller.
    /// </summary>
    public InitBattleField Battle;

    public bool EditMode;

    /// <summary>
    /// Rectangle team controller on left side.
    /// </summary>
    public TeamSelectController TeamLeft;

    /// <summary>
    /// Rectangle team controller on right side.
    /// </summary>
    public TeamSelectController TeamRight;

    /// <summary>
    /// Simple team controller.
    /// </summary>
    public TeamSimpleController TeamSimpleRight;

#if UNITY_EDITOR
    public static string LoginInfoPath = Application.dataPath + "/LoginInfo.txt";

    public static string StartBattleMessagePath = Application.dataPath + "/StartBattleMessageInfo.txt";

    public static string PersistencePath = Application.dataPath + "/PersistenceInfo.txt";

    public static string MissionModelLocatorPath = Application.dataPath + "/MissionModelLocatorInfo.txt";

    public static string BattleEndMessagePath = Application.dataPath + "/BattleEndMessageInfo.txt";
#else
    public static string LoginInfoPath = Application.persistentDataPath + "/LoginInfo.txt";

    public static string StartBattleMessagePath = Application.persistentDataPath + "/StartBattleMessageInfo.txt";

    public static string PersistencePath = Application.persistentDataPath + "/PersisitenceInfo.txt";

    public static string MissionModelLocatorPath = Application.persistentDataPath + "/MissionModelLocatorInfo.txt";

    public static string BattleEndMessagePath = Application.persistentDataPath + "/BattleEndMessageInfo.txt";
#endif

    public static bool isRaidReward = false;

    #endregion

    #region Public Methods

    public static void StoreLoginInfo(Dictionary<string, string> value)
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
    }

    public static Dictionary<string,string> LoadLoginInfo()
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

    public static void StoreMissionModelLocator(object value)
    {
        Stream writer;

        writer = File.Create(MissionModelLocatorPath);

        BinaryFormatter binFormatter = new BinaryFormatter();
        binFormatter.Serialize(writer, value);

        writer.Close();
        Logger.Log("MissionModelLocator file written");
    }

    public static void LoadMissionModelLocator()
    {
        Stream reader;

        if (File.Exists(MissionModelLocatorPath) == false)
        {
            Debug.LogError("MissionModelLocator file not existed! Load file cancelled!");
            return;
        }
        reader = File.OpenRead(MissionModelLocatorPath);

        BinaryFormatter binFormatter = new BinaryFormatter();
        MissionModelLocator missionModelLocator = (MissionModelLocator)binFormatter.Deserialize(reader);

        reader.Close();
        Logger.Log("!!!!!!!!!!!!!MissionModelLocator read, Uuid:" + missionModelLocator.NextRaidType + ", FighterIndex:");

        MissionModelLocator.Instance.RaidLoadingAll = missionModelLocator.RaidLoadingAll;
        //MissionModelLocator.Instance.RaidAddition = missionModelLocator.RaidAddition;
        //MissionModelLocator.Instance.NextRaidType = missionModelLocator.NextRaidType;
        //MissionModelLocator.Instance.CurrRaidType = missionModelLocator.CurrRaidType;
        MissionModelLocator.Instance.Raid = missionModelLocator.Raid;
        //MissionModelLocator.Instance.FriendsMsg = missionModelLocator.FriendsMsg;
        if (MissionModelLocator.Instance.FriendData == null)
        {
            MissionModelLocator.Instance.FriendData = missionModelLocator.FriendData;
        }
        else if (MissionModelLocator.Instance.FriendData.Data == null)
        {
            MissionModelLocator.Instance.FriendData.Data = missionModelLocator.FriendData.Data;
        }
        else
        {
            MissionModelLocator.Instance.FriendData.Data.FriendUuid = missionModelLocator.FriendData.Data.FriendUuid;
            MissionModelLocator.Instance.FriendData.Data.FriendName = missionModelLocator.FriendData.Data.FriendName;
            MissionModelLocator.Instance.FriendData.Data.FriendLvl = missionModelLocator.FriendData.Data.FriendLvl;
            for (int i = 0; i < missionModelLocator.FriendData.Data.HeroProp.Count; i++)
            {
                if (MissionModelLocator.Instance.FriendData.Data.HeroProp[i] == null)
                {
                    MissionModelLocator.Instance.FriendData.Data.HeroProp[i] =
                        missionModelLocator.FriendData.Data.HeroProp[i];
                }
                MissionModelLocator.Instance.FriendData.Data.HeroProp[i].HeroTemplateId =
                    missionModelLocator.FriendData.Data.HeroProp[i].HeroTemplateId;
            }
        }
        //MissionModelLocator.Instance.SelectedStageId = missionModelLocator.SelectedStageId;
        //MissionModelLocator.Instance.MissionStep = missionModelLocator.MissionStep;
        MissionModelLocator.Instance.OldExp = missionModelLocator.OldExp;
        MissionModelLocator.Instance.OldLevel = missionModelLocator.OldLevel;
        MissionModelLocator.Instance.StarCount = missionModelLocator.StarCount;
        //MissionModelLocator.Instance.ShowAddFriendAlert = missionModelLocator.ShowAddFriendAlert;
        //MissionModelLocator.Instance.TotalStageCount = missionModelLocator.TotalStageCount;
        MissionModelLocator.Instance.TotalStarCount = missionModelLocator.TotalStarCount;
    }

    public static void StoreStartBattleMessage(object value)
    {
        Stream writer;

        writer = File.Create(StartBattleMessagePath);

        BinaryFormatter binFormatter = new BinaryFormatter();
        binFormatter.Serialize(writer, value);

        writer.Close();
        Logger.Log("StartBattleMessage file written");
    }

    public static SCBattlePveStartMsg LoadStartBattleMessage()
    {
        Stream reader;

        if (File.Exists(StartBattleMessagePath) == false)
        {
            Debug.LogError("StartBattleMessage file not existed! Load file cancelled!");
            return null;
        }
        reader = File.OpenRead(StartBattleMessagePath);

        BinaryFormatter binFormatter = new BinaryFormatter();
        SCBattlePveStartMsg scBattlePveStartMsg = (SCBattlePveStartMsg)binFormatter.Deserialize(reader);

        reader.Close();
        Logger.Log("!!!!!!!!!!!!!StartBattleMessage read, Uuid:" + scBattlePveStartMsg.Uuid + ", BattleType:" + scBattlePveStartMsg.BattleType + ", RaidID:" + scBattlePveStartMsg.RaidID + ", FighterIndex:" + scBattlePveStartMsg.FighterList[0].FighteProp[2]);

        return scBattlePveStartMsg;
    }

    public static void StorePersistence(Dictionary<string, float> value)
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

    public static Dictionary<string, float> LoadPersistence()
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

    public static void StoreBattleEndMessage(Dictionary<string, float> value)
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
    }

    public static CSBattlePveFinishMsg LoadBattleEndMessage()
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

    public static IEnumerator MakeBattleEndSucceed(CSBattlePveFinishMsg msg)
    {
        yield return new WaitForSeconds(WaitingSeconds);
        while (isRaidReward == false)
        {
            if (WindowManager.Instance.GetWindow<PersistenceConfirmWindow>() == null)
            {
                WindowManager.Instance.Show<PersistenceConfirmWindow>(true);
            }
            PersistenceConfirmWindow.Mode = PersistenceConfirmWindow.PersistenceMode.ReSendMessageNow;
            var window = WindowManager.Instance.GetWindow<PersistenceConfirmWindow>();
            window.gameObject.SetActive(true);
            window.SetLabel();

            if (PersistenceConfirmWindow.IsReSendMessageNowClickDown == true)
            {
                PersistenceConfirmWindow.IsReSendMessageNowClickDown = false;
                Logger.Log("Resend Battle End Message now.");
                NetManager.SendMessage(msg);
            }
            yield return new WaitForSeconds(WaitingSeconds);
        }

        ////Delete file.
        //new FileInfo(MissionModelLocatorPath).Delete();
        //new FileInfo(StartBattleMessagePath).Delete();
        //new FileInfo(PersisitencePath).Delete();
        //new FileInfo(BattleEndMessagePath).Delete();

        WindowManager.Instance.Show<PersistenceConfirmWindow>(false);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        Logger.Log("I am OnEnter with type - " + GetType().Name);

        Battle.Init();
        Battle.StartBattle();
    }

    public override void OnExit()
    {
        Logger.Log("I am OnExit with type - " + GetType().Name);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Save position.
    /// </summary>
    private void SavePosition()
    {
        var formationController = TeamLeft.FormationController;
        formationController.SpawnList.Clear();
        formationController.SpawnList.AddRange(TeamLeft.CharacterList.Select(character => character.gameObject));
        formationController.Description = "RunningGameEdit";
        formationController.WriteXml();
    }

    #endregion

    #region Mono

#if UNITY_EDITOR

    private void OnGUI()
    {
        var x = Screen.width / 20;
        var y = Screen.height / 20;
        var width = Screen.width / 8;
        var height = Screen.height / 10;
        if (!EditMode)
        {
            if (GUI.Button(new Rect(x, y, width, height), "Edit Mode On"))
            {
                EditMode = true;
                TeamLeft.EditMode = true;
                TeamSimpleRight.EditMode = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(x, y, width, height), "Edit Mode Off"))
            {
                EditMode = false;
                TeamLeft.EditMode = false;
                TeamSimpleRight.EditMode = false;
            }
        }

        if (EditMode)
        {
            if (GUI.Button(new Rect(Screen.width - x - width, y, width, height), "Save Position"))
            {
                SavePosition();
            }
        }
    }

#endif

    private void Awake()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
    }

    #endregion
}


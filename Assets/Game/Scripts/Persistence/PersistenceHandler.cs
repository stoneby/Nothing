using Assets.Game.Scripts.Common.Model;
using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistenceHandler : Singleton<PersistenceHandler>
{
    #region Private Fields

    private bool isOpenPersistence = true;

    //Waiting seconds
    private const int WaitingSeconds = 3;

    private bool IsReSendMessageNowClickDown = false;

    private const int loginInfoCount = 2;
    private const int persistenceInfoCount = 9;

    #endregion

    #region Private Methods

    private void OnReStratBattle(GameObject go)
    {
        StartCoroutine(DoReStartBattle());
    }

    private IEnumerator DoReStartBattle()
    {
        try
        {
            WindowManager.Instance.Show<SimpleConfirmWindow>(false);

            var battleStartMsg = LoadStartBattle();

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
            Singleton<WindowManager>.Instance.Show(typeof(RaidsWindow), false);
            //Singleton<WindowManager>.Instance.Show(typeof(MainMenuBarWindow), false);
            Singleton<WindowManager>.Instance.Show(typeof(SetBattleWindow), false);

            if (Mode == PersistenceMode.ReStartBattleWithPersistence)
            {
                window.GetComponent<BattleWindow>().Battle.PersisitenceSet(tempPersistence);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Catch Exception in DoReStartBattle, delete all persistence file to initialize.");
            Debug.LogException(e);
            PopTextManager.PopTip("执行持久化操作时出现异常，已经删除存储的持久化信息, 请重启游戏。");
            new FileInfo(LoginInfoPath).Delete();
            new FileInfo(StartBattlePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(EndBattlePath).Delete();
        }

        yield return null;
    }

    private void OnReSendMessage(GameObject go)
    {
        StartCoroutine(DoReSendMessage());
    }

    private IEnumerator DoReSendMessage()
    {
        try
        {
            if (Mode == PersistenceMode.ReSendMessageNext)
            {
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
        catch (Exception e)
        {
            Debug.LogError("Catch Exception in DoReSendMessage, delete all persistence file to initialize.");
            Debug.LogException(e);
            PopTextManager.PopTip("执行持久化操作时出现异常，已经删除存储的持久化信息, 请重启游戏。");
            new FileInfo(LoginInfoPath).Delete();
            new FileInfo(StartBattlePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(EndBattlePath).Delete();
        }

        yield return null;
    }

    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<SimpleConfirmWindow>(false);
        if (Mode != PersistenceMode.ReSendMessageNow)
        {
            new FileInfo(StartBattlePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(EndBattlePath).Delete();
        }
        //StopCoroutine(MakeBattleEndSucceed(null));
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

    private void StoreLoginInfo(Dictionary<string, string> value)
    {
        if (isOpenPersistence)
        {
            var fileInfo = new FileInfo(LoginInfoPath);

            if (fileInfo.Exists == true)
            {
                fileInfo.Delete();
            }
#if false
            StreamWriter writer = File.AppendText(LoginInfoPath);
#else
            StreamWriter writer = new StreamWriter(EncryptionManagement.Encrypt(LoginInfoPath));
#endif

            foreach (var pair in value)
            {
                writer.WriteLine(pair.Key + "\t" + pair.Value);
            }

            writer.Close();
            Logger.Log("LoginInfo file written");
        }
    }

    private Dictionary<string, string> LoadLoginInfo()
    {
        var fileInfo = new FileInfo(LoginInfoPath);

        if (fileInfo.Exists == false)
        {
            Logger.LogError("LoginInfo file not existed! Load file cancelled!");
            throw new Exception("LoginInfo file not existed.");
        }

#if false
        StreamReader reader = File.OpenText(LoginInfoPath);
#else
        StreamReader reader = new StreamReader(EncryptionManagement.Decrypt(LoginInfoPath));
#endif

        var loginInfo = new Dictionary<string, string>();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
                reader.Close();
                throw new Exception("Not key,value pair in loading logininfo file.");
            }
            loginInfo.Add(splitedStrings[0], splitedStrings[1]);
        }

        if (loginInfo.Count != loginInfoCount)
        {
            Logger.LogError("Not correct key,value pair in file. Loading file aborted!");
            reader.Close();
            throw new Exception("Not correct key,value pair count in loading logininfo file.");
        }

        reader.Close();
        Logger.Log("LoginInfo file loaded");

        return loginInfo;
    }

    private IEnumerator DoStoreStartBattle(SCBattlePveStartMsg battlestartmsg)
    {
        StartBattleSerialize battleStart = new StartBattleSerialize();
        battleStart.BattleStartMsg = battlestartmsg;

#if false
        StreamWriter writer = File.AppendText(StartBattlePath);
#else
        StreamWriter writer = new StreamWriter(EncryptionManagement.Encrypt(StartBattlePath));
#endif

        battleStart.WriteClass(writer, "BattleStart:");
        writer.Close();

        Logger.Log("StartBattle file written");

        yield return null;
    }

    private SCBattlePveStartMsg LoadStartBattle()
    {
        if (new FileInfo(StartBattlePath).Exists == false)
        {
            Logger.LogError("Start battle file not existed! Check the file.");
            throw new Exception("Start battle file not existed!");
        }

#if false
        StreamReader reader = File.OpenText(StartBattlePath);
#else
        StreamReader reader = new StreamReader(EncryptionManagement.Decrypt(StartBattlePath));
#endif

        string value = reader.ReadToEnd();
        string[] outStrings = value.Split(new string[] { "BattleStart:" }, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, 1);
        reader.Close();

        StartBattleSerialize battleStart = new StartBattleSerialize();
        battleStart.ReadClass(outStrings[0]);

        return battleStart.BattleStartMsg;
    }

    private void DoStorePersistence(Dictionary<string, float> value)
    {
        var fileInfo = new FileInfo(PersistencePath);

        if (fileInfo.Exists == true)
        {
            fileInfo.Delete();
        }
#if false
        StreamWriter writer = File.AppendText(PersistencePath);
#else
        StreamWriter writer = new StreamWriter(EncryptionManagement.Encrypt(PersistencePath));
#endif

        foreach (var pair in value)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value.ToString());
        }

        writer.Close();
        Logger.Log("Persistence file written");
    }

    private Dictionary<string, float> LoadPersistence()
    {
        var fileInfo = new FileInfo(PersistencePath);

        if (fileInfo.Exists == false)
        {
            Logger.LogError("Persistence file not existed! Load file cancelled!");
            throw new Exception("Persistence file not existed!");
        }

#if false
        StreamReader reader = File.OpenText(PersistencePath);
#else
        StreamReader reader = new StreamReader(EncryptionManagement.Decrypt(PersistencePath));
#endif

        Dictionary<string, float> persistenceInfo = new Dictionary<string, float>();
        persistenceInfo.Clear();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            if (splitedStrings.Length != 2)
            {
                reader.Close();
                throw new Exception("ReadPersistenceDic: not couple strings.");
            }
            persistenceInfo.Add(splitedStrings[0], Single.Parse(splitedStrings[1]));
        }

        if (persistenceInfo.Count != persistenceInfoCount)
        {
            Logger.LogError("Not correct key,value pair in file. Loading file aborted!");
            reader.Close();
            throw new Exception("Not correct key,value pair count in loading Persistence file.");
        }

        reader.Close();
        Logger.Log("Persistence file loaded");

        return persistenceInfo;
    }

    private IEnumerator DoStoreBattleEndMessage(CSBattlePveFinishMsg msg)
    {
        EndBattleSerialize battleEnd = new EndBattleSerialize();
        battleEnd.BattleEndMsg = msg;

#if false
        StreamWriter writer = File.AppendText(EndBattlePath);
#else
        StreamWriter writer = new StreamWriter(EncryptionManagement.Encrypt(EndBattlePath));
#endif

        battleEnd.WriteClass(writer, "BattleEnd:");
        writer.Close();
        Logger.Log("EndBattle file written");

        new FileInfo(StartBattlePath).Delete();
        new FileInfo(PersistencePath).Delete();

        yield return null;
    }

    private CSBattlePveFinishMsg LoadBattleEndMessage()
    {
        if (new FileInfo(EndBattlePath).Exists == false)
        {
            Logger.LogError("End battle file not existed! Check the file.");
            throw new Exception("End battle file not existed!");
        }

#if false
        StreamReader reader = File.OpenText(EndBattlePath);
#else
        StreamReader reader = new StreamReader(EncryptionManagement.Decrypt(EndBattlePath));
#endif

        string value = reader.ReadToEnd();
        string[] outStrings = value.Split(new string[] { "BattleEnd:" }, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, 1);
        reader.Close();

        EndBattleSerialize battleEnd = new EndBattleSerialize();
        battleEnd.ReadClass(outStrings[0]);

        return battleEnd.BattleEndMsg;
    }

    private IEnumerator PersistenceExecute()
    {
        //Load perisitence file
        try
        {
            if (new FileInfo(LoginInfoPath).Exists == true)
            {
                if (long.Parse(LoadLoginInfo()["AccountID"]) == ServiceManager.UserID && LoadLoginInfo()["ServerID"] == ServiceManager.ServerData.ID)
                {
                    if (new FileInfo(StartBattlePath).Exists == true && new FileInfo(EndBattlePath).Exists == false)
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
                    else if (new FileInfo(StartBattlePath).Exists == false && new FileInfo(EndBattlePath).Exists == true)
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
                    PopTextManager.PopTip("更换账号/服务器，或持久化信息不满足战斗要求，已经删除存储的持久化信息。");
                    //Delete file if switch account or server.
                    new FileInfo(StartBattlePath).Delete();
                    new FileInfo(PersistencePath).Delete();
                    new FileInfo(EndBattlePath).Delete();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Catch Exception in PersistenceExecute, delete all persistence file to initialize.");
            Debug.LogException(e);
            PopTextManager.PopTip("执行持久化操作时出现异常，已经删除存储的持久化信息, 请重启游戏。");
            new FileInfo(LoginInfoPath).Delete();
            new FileInfo(StartBattlePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(EndBattlePath).Delete();
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

    #region Public Fields

#if UNITY_EDITOR

    public static string LoginInfoPath = Application.dataPath + "/LoginInfo.txt";
    public static string StartBattlePath = Application.dataPath + "/StartBattleInfo.txt";
    public static string PersistencePath = Application.dataPath + "/PersistenceInfo.txt";
    public static string EndBattlePath = Application.dataPath + "/EndBattleInfo.txt";

#else

    public static string LoginInfoPath = Application.persistentDataPath + "/LoginInfo.txt";
    public static string StartBattlePath = Application.persistentDataPath + "/StartBattleInfo.txt";
    public static string PersistencePath = Application.persistentDataPath + "/PersistenceInfo.txt";
    public static string EndBattlePath = Application.persistentDataPath + "/EndBattleInfo.txt";

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

    public void StoreStartBattle(SCBattlePveStartMsg battlestartmsg)
    {
        if (isOpenPersistence)
        {
            StartCoroutine(DoStoreStartBattle(battlestartmsg));
        }
    }

    public void StorePersistence(Dictionary<string, float> value)
    {
        if (isOpenPersistence)
        {
            DoStorePersistence(value);
        }
    }

    public void StoreBattleEndMessage(CSBattlePveFinishMsg msg)
    {
        if (isOpenPersistence)
        {
            StartCoroutine(DoStoreBattleEndMessage(msg));
        }
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
            new FileInfo(StartBattlePath).Delete();
            new FileInfo(PersistencePath).Delete();
            new FileInfo(EndBattlePath).Delete();
        }
    }

    public void GoToPersistenceWay()
    {
        if (isOpenPersistence)
        {
            StartCoroutine(PersistenceExecute());
        }
    }

    #endregion
}

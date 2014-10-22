using Assets.Game.Scripts.Common.Model;
using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistenceHandler : Singleton<PersistenceHandler>
{
    #region Public Fields

    //Switcher to open or close Battle Persistence function.
    public bool Enabled = true;

    // Stored persistence file path.
    private static readonly string LoginInfoPath = Application.persistentDataPath + "/LoginInfo.txt";
    private static readonly string StartBattlePath = Application.persistentDataPath + "/StartBattleInfo.txt";
    private static readonly string PersistencePath = Application.persistentDataPath + "/PersistenceInfo.txt";
    private static readonly string EndBattlePath = Application.persistentDataPath + "/EndBattleInfo.txt";

    /// <summary>
    /// Flag to verifying BattleEndMessage sent succeed or not.
    /// </summary>
    public static bool IsRaidFinish;

    public enum PersistenceMode
    {
        ReSendMessageNow,
        ReStartBattle,
        ReStartBattleWithPersistence,
        ReSendMessageNext,
        Normal
    }
    /// <summary>
    /// Mode for managing SimpleConfirmWindow.
    /// </summary>
    public PersistenceMode Mode = PersistenceMode.Normal;

    public bool IsBattlePersistent
    {
        get { return Mode == PersistenceMode.ReStartBattle || Mode == PersistenceMode.ReStartBattleWithPersistence; }
    }

    public Dictionary<string, string> PersistentInfor { get; private set; }

    #endregion

    #region Private Fields

    //Check BattleEndMessage sent succeed or not after waiting seconds.
    private const int WaitingSeconds = 3;

    //Nums count for verifying stored data.
    private const int LoginInfoCount = 2;
    private const int PersistenceInfoCount = 8;

    //Store BattleEndMessage for resending if send failed.
    private CSBattlePveFinishMsg tempEndBattleMsg = new CSBattlePveFinishMsg();

    #endregion

    #region Public Methods

    /// <summary>
    ///  Store Start Battle data for persistence.
    /// </summary>
    public void StoreStartBattle(SCBattlePveStartMsg battlestartmsg)
    {
        if (Enabled)
        {
            StartCoroutine(DoStoreStartBattle(battlestartmsg));
        }
    }

    /// <summary>
    ///  Store Persistence data for persistence.
    /// </summary>
    public void StorePersistence()
    {
        if (Enabled)
        {
            if (PersistentInfor == null)
            {
                PersistentInfor = new Dictionary<string, string>();
            }
            PersistentInfor.Clear();

            // store LevelManager persistent infor.
            BattleModelLocator.Instance.LevelManager.StorePersistent(PersistentInfor);

            // store InitBattleField persistent infor.
            var battleWindow = WindowManager.Instance.Show<BattleWindow>(true);
            battleWindow.Battle.PersistenceStore(PersistentInfor);

            DoStorePersistence(PersistentInfor);
        }
    }

    /// <summary>
    ///  Store Battle End data for persistence.
    /// </summary>
    public void StoreBattleEndMessage(CSBattlePveFinishMsg msg)
    {
        if (Enabled)
        {
            StartCoroutine(DoStoreBattleEndMessage(msg));
        }
    }

    /// <summary>
    /// Check BattleEndMessage sent succeed function, is called after sending BattleEndMessage.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns>IEnumerator</returns>
    public IEnumerator CheckBattleEndSucceed(CSBattlePveFinishMsg msg)
    {
        if (Enabled)
        {
            //wait several seconds.
            yield return new WaitForSeconds(WaitingSeconds);

            if (IsRaidFinish == false && msg != null)
            {
                //Show ConfirmWindow if battle end msg sent failed.
                tempEndBattleMsg = msg;

                if (!WindowManager.Instance.ContainWindow<SimpleConfirmWindow>())
                {
                    WindowManager.Instance.Show<SimpleConfirmWindow>(true);
                }
                Mode = PersistenceMode.ReSendMessageNow;
                var window = WindowManager.Instance.GetWindow<SimpleConfirmWindow>();
                window.gameObject.SetActive(true);
                SetConfirmWindow(window);

                yield break;
            }

            Cleanup();
        }
        else
        {
            yield return null;
        }
    }

    /// <summary>
    /// Check persistence state, is called when returning playerinfo.
    /// </summary>
    public void GoToPersistenceWay()
    {
        if (Enabled)
        {
            StartCoroutine(PersistenceExecute());
        }
    }

    public void Cleanup()
    {
        //Delete file if battle end msg sent succeed.
        new FileInfo(StartBattlePath).Delete();
        new FileInfo(PersistencePath).Delete();
        new FileInfo(EndBattlePath).Delete();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// ReStart Battle function, assigned to OKBtn listener in SimpleConfirmWindow
    /// </summary>
    /// <param name="go"></param>
    private void OnReStratBattle(GameObject go)
    {
        StartCoroutine(DoReStartBattle());
    }

    private IEnumerator DoReStartBattle()
    {
        try
        {
            //Close GreenHand module if enabled.
            GreenHandGuideHandler.Instance.StopAll();

            WindowManager.Instance.Show<SimpleConfirmWindow>(false);

            var battleStartMsg = LoadStartBattle();

            PopTextManager.PopTip("返回战斗数据");

            if (Mode == PersistenceMode.ReStartBattleWithPersistence)
            {
                PersistentInfor = LoadPersistence();
            }

            BattleModelLocator.Instance.Init(battleStartMsg);

            //Start battle scene accroding to different Mode.
            if (Mode == PersistenceMode.ReStartBattleWithPersistence)
            {
                BattleModelLocator.Instance.MainBattle.startFromDataStore(PersistentInfor["ServerLogicKey"]);
            }
            else
            {
                BattleModelLocator.Instance.MainBattle.start();
            }

            // client side show.
            var battleWindow = WindowManager.Instance.Show<BattleWindow>(true);
            Singleton<WindowManager>.Instance.Show<RaidsWindow>(false);
            Singleton<WindowManager>.Instance.Show<MainMenuBarWindow>(false);
            Singleton<WindowManager>.Instance.Show<SetBattleWindow>(false);

            if (Mode == PersistenceMode.ReStartBattleWithPersistence)
            {
                battleWindow.Battle.PersisitenceSet(PersistentInfor);
            }
        }
        catch (Exception e)
        {
            //Delete all stored file if catch exception.
            Debug.LogError("Catch Exception in DoReStartBattle, delete all persistence file to initialize." + e.Message);
            Debug.LogException(e);
            PopTextManager.PopTip("执行持久化操作时出现异常，已经删除存储的持久化信息。");

            if (File.Exists(LoginInfoPath))
            {
                File.Move(LoginInfoPath, LoginInfoPath + "Rename");
                new FileInfo(LoginInfoPath + "Rename").Delete();
            }

            if (File.Exists(StartBattlePath))
            {
                File.Move(StartBattlePath, StartBattlePath + "Rename");
                new FileInfo(StartBattlePath + "Rename").Delete();
            }

            if (File.Exists(PersistencePath))
            {
                File.Move(PersistencePath, PersistencePath + "Rename");
                new FileInfo(PersistencePath + "Rename").Delete();
            }

            if (File.Exists(LoginInfoPath))
            {
                File.Move(EndBattlePath, EndBattlePath + "Rename");
                new FileInfo(EndBattlePath + "Rename").Delete();
            }

            Mode = PersistenceMode.Normal;

            //Store loginaccount file 
            var tempDictionary = new Dictionary<string, string>
            {
                {"AccountID", ServiceManager.UserID.ToString()},
                {"ServerID", ServiceManager.ServerData.ID}
            };
            StoreLoginInfo(tempDictionary);
        }

        yield return null;
    }

    /// <summary>
    /// ReSendMessage function, assigned to OKBtn listener in SimpleConfirmWindow
    /// </summary>
    /// <param name="go"></param>
    private void OnReSendMessage(GameObject go)
    {
        StartCoroutine(DoReSendMessage());
    }

    private IEnumerator DoReSendMessage()
    {
        try
        {
            //Close GreenHand module if enabled.
            GreenHandGuideHandler.Instance.StopAll();

            WindowManager.Instance.Show<SimpleConfirmWindow>(false);

            if (Mode == PersistenceMode.ReSendMessageNext)
            {
                var battleEndMsg = LoadBattleEndMessage();

                NetManager.SendMessage(battleEndMsg);
                MtaManager.TrackEndPage(MtaType.BattleScreen);

                //Check battle end succeed.
                StartCoroutine(CheckBattleEndSucceed(battleEndMsg));
            }

            if (Mode == PersistenceMode.ReSendMessageNow)
            {
                Logger.Log("Resend Battle End Message now.");
                NetManager.SendMessage(tempEndBattleMsg);
                StartCoroutine(CheckBattleEndSucceed(tempEndBattleMsg));
            }
        }
        catch (Exception e)
        {
            //Delete all stored file if catch exception.
            Debug.LogError("Catch Exception in DoReSendMessage, delete all persistence file to initialize.");
            Debug.LogException(e);
            PopTextManager.PopTip("执行持久化操作时出现异常，已经删除存储的持久化信息。");

            if (File.Exists(LoginInfoPath))
            {
                File.Move(LoginInfoPath, LoginInfoPath + "Rename");
                new FileInfo(LoginInfoPath + "Rename").Delete();
            }

            if (File.Exists(StartBattlePath))
            {
                File.Move(StartBattlePath, StartBattlePath + "Rename");
                new FileInfo(StartBattlePath + "Rename").Delete();
            }

            if (File.Exists(PersistencePath))
            {
                File.Move(PersistencePath, PersistencePath + "Rename");
                new FileInfo(PersistencePath + "Rename").Delete();
            }

            if (File.Exists(LoginInfoPath))
            {
                File.Move(EndBattlePath, EndBattlePath + "Rename");
                new FileInfo(EndBattlePath + "Rename").Delete();
            }

            Mode = PersistenceMode.Normal;

            //Store loginaccount file 
            var tempDictionary = new Dictionary<string, string>
            {
                {"AccountID", ServiceManager.UserID.ToString()},
                {"ServerID", ServiceManager.ServerData.ID}
            };
            StoreLoginInfo(tempDictionary);
        }

        yield return null;
    }

    /// <summary>
    /// Cancel operation, assigned to CancelBtn.OnClick delegate in SimpleConfirmWindow
    /// </summary>
    /// <param name="go"></param>
    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<SimpleConfirmWindow>(false);
        Mode = PersistenceMode.Normal;
        //if (Mode != PersistenceMode.ReSendMessageNow)
        //{
            Cleanup();
        //}
    }

    /// <summary>
    /// Set SimpleConfirmWindow's label and eventlistener accroding to different Mode.
    /// </summary>
    /// <param name="window"></param>
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

    //Store and Load persistence data functions.
    private void StoreLoginInfo(Dictionary<string, string> value)
    {
        if (Enabled)
        {
            var fileInfo = new FileInfo(LoginInfoPath);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            var writer = new StreamWriter(EncryptionManagement.Encrypt(LoginInfoPath));
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

        var reader = new StreamReader(EncryptionManagement.Decrypt(LoginInfoPath));
        var loginInfo = new Dictionary<string, string>();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);

            //Check string array's num.
            if (splitedStrings.Length != 2)
            {
                Logger.LogError("Not key,value pair in file. Loading file aborted!");
                reader.Close();
                throw new Exception("Not key,value pair in loading logininfo file.");
            }
            loginInfo.Add(splitedStrings[0], splitedStrings[1]);
        }

        //Check string array's num.
        if (loginInfo.Count != LoginInfoCount)
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
        var battleStart = new StartBattleSerialize { BattleStartMsg = battlestartmsg };

        var writer = new StreamWriter(EncryptionManagement.Encrypt(StartBattlePath));
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

        var reader = new StreamReader(EncryptionManagement.Decrypt(StartBattlePath));
        string value = reader.ReadToEnd();
        string[] outStrings = value.Split(new[] { "BattleStart:" }, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, 1);
        reader.Close();

        var battleStart = new StartBattleSerialize();
        battleStart.ReadClass(outStrings[0]);

        return battleStart.BattleStartMsg;
    }

    private void DoStorePersistence(Dictionary<string, string> value)
    {
        var fileInfo = new FileInfo(PersistencePath);

        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        var writer = new StreamWriter(EncryptionManagement.Encrypt(PersistencePath));
        foreach (var pair in value)
        {
            writer.WriteLine(pair.Key + "\t" + pair.Value);
        }
        var tempString = BattleModelLocator.Instance.MainBattle.StoreData.toStoreStr();
        writer.WriteLine("ServerLogicKey" + "\t" + tempString);

        writer.Close();
        Logger.Log("Persistence file written");
    }

    private Dictionary<string, string> LoadPersistence()
    {
        var fileInfo = new FileInfo(PersistencePath);

        if (fileInfo.Exists == false)
        {
            Logger.LogError("Persistence file not existed! Load file cancelled!");
            throw new Exception("Persistence file not existed!");
        }

        var reader = new StreamReader(EncryptionManagement.Decrypt(PersistencePath));
        var persistenceInfo = new Dictionary<string, string>();
        persistenceInfo.Clear();

        string tempString;
        char[] splitChars = { '\t', '\n' };
        while ((tempString = reader.ReadLine()) != null)
        {
            string[] splitedStrings = tempString.Split(splitChars);
            //Check string array's num.
            if (splitedStrings.Length != 2)
            {
                reader.Close();
                throw new Exception("ReadPersistenceDic: not couple strings.");
            }
            persistenceInfo.Add(splitedStrings[0], splitedStrings[1]);
        }

        //Check string array's num.
        if (persistenceInfo.Count != PersistenceInfoCount)
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
        var battleEnd = new EndBattleSerialize { BattleEndMsg = msg };

        var writer = new StreamWriter(EncryptionManagement.Encrypt(EndBattlePath));
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

        var reader = new StreamReader(EncryptionManagement.Decrypt(EndBattlePath));
        string value = reader.ReadToEnd();
        string[] outStrings = value.Split(new[] { "BattleEnd:" }, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, 1);
        reader.Close();

        var battleEnd = new EndBattleSerialize();
        battleEnd.ReadClass(outStrings[0]);

        return battleEnd.BattleEndMsg;
    }

    /// <summary>
    /// Check persistence state, is called in GoToPersistenceWay by another coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PersistenceExecute()
    {
        try
        {
            //Check login file exist
            if (new FileInfo(LoginInfoPath).Exists)
            {
                //Check login file consistent
                if (long.Parse(LoadLoginInfo()["AccountID"]) == ServiceManager.UserID && LoadLoginInfo()["ServerID"] == ServiceManager.ServerData.ID)
                {
                    //Verifying persistence Mode
                    if (new FileInfo(StartBattlePath).Exists && new FileInfo(EndBattlePath).Exists == false)
                    {
                        Logger.Log("Persistence mode: ReStartBattle.");

                        Mode = new FileInfo(PersistencePath).Exists == false ? PersistenceMode.ReStartBattle : PersistenceMode.ReStartBattleWithPersistence;
                        var window = WindowManager.Instance.Show<SimpleConfirmWindow>(true);
                        window.gameObject.SetActive(true);
                        SetConfirmWindow(window);
                    }
                    else if (new FileInfo(StartBattlePath).Exists == false && new FileInfo(EndBattlePath).Exists)
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
                    PopTextManager.PopTip("更换账号/服务器，或持久化信息不满足战斗要求，已经删除存储的持久化信息。");
                    Cleanup();
                }
            }
        }
        catch (Exception e)
        {
            //Delete all stored files if catch exception.
            Debug.LogError("Catch Exception in PersistenceExecute, delete all persistence file to initialize.");
            Debug.LogException(e);
            PopTextManager.PopTip("执行持久化操作时出现异常，已经删除存储的持久化信息。");

            if (File.Exists(LoginInfoPath))
            {
                File.Move(LoginInfoPath, LoginInfoPath + "Rename");
                new FileInfo(LoginInfoPath + "Rename").Delete();
            }

            if (File.Exists(StartBattlePath))
            {
                File.Move(StartBattlePath, StartBattlePath + "Rename");
                new FileInfo(StartBattlePath + "Rename").Delete();
            }

            if (File.Exists(PersistencePath))
            {
                File.Move(PersistencePath, PersistencePath + "Rename");
                new FileInfo(PersistencePath + "Rename").Delete();
            }

            if (File.Exists(LoginInfoPath))
            {
                File.Move(EndBattlePath, EndBattlePath + "Rename");
                new FileInfo(EndBattlePath + "Rename").Delete();
            }

            Mode = PersistenceMode.Normal;
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
}

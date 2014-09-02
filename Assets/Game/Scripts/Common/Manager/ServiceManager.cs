using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
//using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class ServiceManager
{
    //global
    public static bool QuikeLoginOffen = false;
    public static bool AutoLoginOfften = false;
    public static string ResourceUrl;
    public static string ResourceVersion;
    public static string UpdateUrl;
    public static string UpdateInfo;
    public static string GameID;
    public static bool OpenTestAccount = false;
    public static bool OpenBattlePersistence = true;
    //public static int GameVersionValue;

    //FValue
    public static string FValue;

    //iosState
    public static bool IsCheck = false;
    public static string CheckServerUrl;
    public static List<AppStateVO> IosStateArray;

    //当前版本是否为技术测试版本
    public static bool IsTest
    {
        get
        {
            return isTest;
        }
        set
        {
            isTest = value;
            //if (UICamera.currentCamera != null)
            //{
            //    NGUIDebug.debugRaycast = isTest;
            //}
        }
    }

    private static bool isTest;

    //appMap
    public static AppVO AppData;
    public static List<AppVO> AppArray;

    //servers
    public static ServerVO ServerData;
    public static List<ServerVO> AllServerArray;
    public static List<ServerVO> UsedServerArray;

    //account
    //public static AccountVO AccountData;
    //public static List<AccountVO> AccountArray;//最后一个是最新的
    public static long UserID;
    public static string UserName = "";
    public static string DebugUserID = "";
    public static string DebugUserName = "";
    public static string DebugPassword = "";
    public static int IsDebugAccount = 0;
    public static List<string> ServerNames = null; 

    /// <summary>
    /// Initialize user account.
    /// </summary>
    public static void InitAccount()
    {
        //AccountArray = new List<AccountVO>();
        //return;
        //PlayerPrefs.
        //if (PlayerPrefs.HasKey("UserID")) UserID = PlayerPrefs.GetString("UserID");
        if (PlayerPrefs.HasKey("TheUserName")) UserName = PlayerPrefs.GetString("TheUserName");
        if (PlayerPrefs.HasKey("DebugUserID")) DebugUserID = PlayerPrefs.GetString("DebugUserID");
        if (PlayerPrefs.HasKey("DebugUserName")) DebugUserName = PlayerPrefs.GetString("DebugUserName");
        if (PlayerPrefs.HasKey("DebugPassword")) DebugPassword = PlayerPrefs.GetString("DebugPassword");
        if (PlayerPrefs.HasKey("IsDebugAccount")) IsDebugAccount = PlayerPrefs.GetInt("IsDebugAccount");
        ServerNames = new List<string>();
        //Logger.Log("UserID=" + UserID);
        //Logger.Log("UserName=" + UserName);
//        if (IsDebugAccount == 1)
//        {
//            PopTextManager.PopTip("玩家账号：" + DebugUserName);
//        }
//        else
//        {
//            PopTextManager.PopTip("玩家账号：" + UserName);
//        }

        if (PlayerPrefs.HasKey("ServerNames"))
        {
            var content = PlayerPrefs.GetString("ServerNames");

            Logger.Log(content);
            int index = content.IndexOf(",");
            string[] arr = null;
            if (index >= 0)
            {
                arr = content.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                Logger.Log(arr.Length);
            }
            else if (content != "")
            {
                arr = new string[1];
                arr[0] = content;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                ServerNames.Add(arr[i]);
            }
        }
    }

    /// <summary>
    /// Save account to disk.
    /// </summary>
    private static void SaveAccount()
    {
        //return;
        //PlayerPrefs.SetString("UserID", UserID);
        PlayerPrefs.SetString("TheUserName", UserName);
        PlayerPrefs.SetString("DebugUserID", DebugUserID);
        PlayerPrefs.SetString("DebugUserName", DebugUserName);
        PlayerPrefs.SetString("DebugPassword", DebugPassword);
        PlayerPrefs.SetInt("IsDebugAccount", IsDebugAccount);

        var str = "";
        for (int i = 0; i < ServerNames.Count; i++)
        {
            if (i != ServerNames.Count - 1)
            {
                if (ServerNames[i] != "") str += ServerNames[i] + ",";
            }
            else
            {
                str += ServerNames[i];
            }
            
        }
        PlayerPrefs.SetString("ServerNames", str);
        PlayerPrefs.Save();
    }

    public static void SetAccount(string username)
    {
        UserName = username;
        SaveAccount();
    }

    public static void SetDebugAccount(string username, string password)
    {
        DebugUserName = username;
        DebugPassword = password;
        SaveAccount();
    }

    public static void AddServer(string server)
    {
        var flag = true;
        for (int i = 0; i < ServerNames.Count; i++)
        {
            if (ServerNames[i] == server)
            {
                ServerNames[i] = "";
                break;
            }
        }

        ServerNames.Add(server);
        SaveAccount();
    }

   

    /// <summary>
    /// Set server list.
    /// </summary>
    /// <param name="serverMap">Server xml element.</param>
    public static void SetServers(XElement serverMap)
    {
        Logger.Log(serverMap);
        //GameVersionValue = GetVersionValue(GameConfig.);

        var servers = serverMap.Elements("server");
        var temp = new List<ServerVO>();
        UsedServerArray = new List<ServerVO>();
        int suggestcount = 0;
        if (IsCheck)
        {
            foreach (var xElement in servers)
            {
                var item = ServerVO.Parse(xElement);
                if (item.Url == CheckServerUrl)
                {
                    temp.Add(item);
                }
            }
        }
        else
        {
            foreach (var server in servers.Select(item => ServerVO.Parse(item)))
            {
                if (server.ServerState == 0)continue;

                if (SystemInfo.deviceType == DeviceType.Desktop)
                {
                    temp.Add(server);
                    if (server.ServerState == 1) suggestcount++;
                }
                else if (!server.IsTest && GameConfig.VersionValue >= server.RequestClientVersion && CanNotInCheckList(server.Url))
                {
                    temp.Add(server);
                    if (server.ServerState == 1) suggestcount++;
                }
            }
        }
        //stages.OrderByDescending(stageinfo => stageinfo.TemplateId)
        //var temp = 
        AllServerArray = new List<ServerVO>(temp.OrderBy(stageinfo => stageinfo.ServerState));

        UsedServerArray = GetUsedServers();

        if (UsedServerArray.Count > 0)
        {
            ServerData = UsedServerArray[0];
        }
        else if (suggestcount > 0)
        {
            float v = Random.Range(0, suggestcount);
            var  k = (int) Mathf.Floor(v);
            ServerData = AllServerArray[k];
        }
        else if (AllServerArray.Count > 0)
        {
            ServerData = AllServerArray[0];
        }
    }

    private static bool CanNotInCheckList(string theurl)
    {
        for (int i = 0; i < IosStateArray.Count; i++)
        {
            if (IosStateArray[i].CheckServerUrl == theurl)
            {
                return false;
            }
        }
        return true;
    }

    private static List<ServerVO> GetUsedServers()
    {
        var arr = new List<ServerVO>();
        for (int i = ServerNames.Count - 1; i >= 0; i--)
        {
            var item = GetServerByUrl(ServerNames[i]);
            if (item != null)
            {
                arr.Add(item);
            }
        }
        return arr;
    }

    public static ServerVO GetServerByUrl(string url)
    {
        return AllServerArray.FirstOrDefault(t => t.Url == url);
    }

    /// <summary>
    /// Get default server.
    /// </summary>
    /// <returns>The default server.</returns>
    public static ServerVO GetDefaultServer()
    {
        return ServerData;
    }

    public static int GetVersionValue(string str)
    {
        Logger.Log(str);
        string[] arr = str.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
        int k = 0;
        int offset = 1;
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            Logger.Log(arr[i]);
            var  v = Int32.Parse(arr[i]);
            k += v * offset;
            offset *= 10;
        }
        Logger.Log(k);
        return k;
    }
}

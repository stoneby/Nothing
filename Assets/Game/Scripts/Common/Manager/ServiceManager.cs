using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;


public class ServiceManager
{
    //global
    public static string PassWordRegex;
    public static bool QuikeLoginOffen = false;
    public static bool AutoLoginOfften = false;
    public static string PlatformDomain;
    public static string ResourceUrl;
    public static string ResourceVersion;
    public static string UpdateUrl;
    public static string UpdateInfo;
    public static string UserNameRegex;
    public static string UserNameInfo;
    public static string GameID;

    //FValue
    public static string FValue;

    //iosState
    public static bool IsCheck = false;
    public static string CheckServerUrl;
    public static List<AppStateVO> IosStateArray;

    //appMap
    public static AppVO AppData;
    public static List<AppVO> AppArray;

    //servers
    public static ServerVO ServerData;
    public static List<ServerVO> AllServerArray;
    public static List<ServerVO> UsedServerArray;

    //account
    public static AccountVO AccountData;
    public static List<AccountVO> AccountArray;//最后一个是最新的

    /// <summary>
    /// Initialize user account.
    /// </summary>
    public static void InitAccount()
    {
        AccountArray = new List<AccountVO>();
        //return;
        Logger.Log(GameConfig.CookieAddress);
        try
        {
            var f = new FileInfo(GameConfig.CookieAddress);
            if (!f.Exists)
            {
                if (f.Directory == null || !f.Directory.Exists)
                {
                    if (f.DirectoryName != null) Directory.CreateDirectory(f.DirectoryName);
                }

                f.Create();

                return;
            }
            var content = File.ReadAllText(GameConfig.CookieAddress);

            Logger.Log(content);
            int index = content.IndexOf("<GameAccount>");
            string[] arr = null;
            if (index >= 0)
            {
                arr = content.Split(new string[] { "<GameAccount>" }, StringSplitOptions.RemoveEmptyEntries);
                Logger.Log(arr.Length);
            }
            else if (content != "")
            {
                arr = new string[1];
                arr[0] = content;
            }

            if (arr == null) return;
            for (int i = 0; i < arr.Length; i++)
            {
                var obj = AccountVO.CreateData(arr[i]);
                AccountArray.Add(obj);
            }
        }
        catch (Exception)
        {
            Logger.LogWarning("InitAccount Exception ===============");
        }
        
    }

    /// <summary>
    /// Save account to disk.
    /// </summary>
    public static void SaveAccount()
    {
        //return;
        var str = "";
        for (int i = 0; i < AccountArray.Count; i++)
        {
            if (i != AccountArray.Count - 1)
            {
                str += AccountArray[i].GetInfo() + "<GameAccount>";
            }
            else
            {
                str += AccountArray[i].GetInfo();
            }
            
        }
        
        File.WriteAllText(GameConfig.CookieAddress, str);
    }

    public static void AddAccount(string account, string password, string server)
    {
        var flag = true;
        for (int i = 0; i < AccountArray.Count; i++)
        {
            if (AccountArray[i].Account == account)
            {
                var obj = AccountArray[i];
                AccountArray.RemoveAt(i);
                obj.Password = password;
                obj.AddServer(server);
                AccountArray.Add(obj);
                flag = false;
                break;
            }
        }

        if (flag)
        {
            AccountArray.Add(AccountVO.CreateData(account, password, server));
        }
    }

    public static void AddAccount(AccountVO obj)
    {
        var flag = true;
        Logger.Log(AccountArray.Count);
        for (int i = 0; i < AccountArray.Count; i++)
        {
            if (AccountArray[i].Account == obj.Account)
            {
                var item = AccountArray[i];
                AccountArray.RemoveAt(i);
                item.Password = obj.Password;
                for (int j = 0; j < obj.Servers.Count; j++)
                {
                    item.AddServer(obj.Servers[j]);
                }
                AccountArray.Add(item);
                flag = false;
                break;
            }
        }

        if (flag)
        {
            AccountArray.Add(obj);
        }
    }

    public static void DeleteAccount(AccountVO obj)
    {
        for (int i = 0; i < AccountArray.Count; i++)
        {
            if (AccountArray[i].Account == obj.Account)
            {
                AccountArray.RemoveAt(i);
                break;
            }
        }
        SaveAccount();
    }

    public static AccountVO GetDefaultAccount()
    {
        if (AccountArray != null && AccountArray.Count > 0)
        {
            return AccountArray[AccountArray.Count - 1];
        }
        return null;
    }

    /// <summary>
    /// Set server list.
    /// </summary>
    /// <param name="serverMap">Server xml element.</param>
    public static void SetServers(XElement serverMap)
    {
        Logger.Log(serverMap);
        var servers = serverMap.Elements("server");
        AllServerArray = new List<ServerVO>();
        UsedServerArray = new List<ServerVO>();
        foreach (var server in servers.Select(item => ServerVO.Parse(item)))
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                AllServerArray.Add(server);
            }
            else if (!server.IsTest)
            {
                AllServerArray.Add(server);
            }
        }
        ServerData = AllServerArray[0];
    }

    public static List<ServerVO> GetUsedServers()
    {
        var arr = new List<ServerVO>();
        var obj = GetDefaultAccount();
        if (obj == null) return null;
        for (int i = obj.Servers.Count - 1; i >= 0; i--)
        {
            var item = GetServerByUrl(obj.Servers[i]);
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
}

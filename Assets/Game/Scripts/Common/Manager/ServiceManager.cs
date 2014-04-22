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

    //初始化账号信息
    public static void InitAccount()
    {
        
        AccountArray = new List<AccountVO>();
        //return;
        Debug.Log(GameConfig.CookieAddress);
        var f = new FileInfo(GameConfig.CookieAddress);
        if (!f.Exists)
        {
            if (f.Directory == null || !f.Directory.Exists)
            {
                if (f.DirectoryName != null) Directory.CreateDirectory(f.DirectoryName);
            }
           
            f.Create();
            //File.WriteAllText(GameConfig.CookieAddress, "");
            //File.Create(GameConfig.CookieAddress);
            
            return;
        }
        var content = File.ReadAllText(GameConfig.CookieAddress);
            
        Debug.Log(content);
		int index = content.IndexOf ("<GameAccount>");
        string[] arr = null;
        if (index >= 0)
		{
			arr = content.Split(new string[] {"<GameAccount>"}, StringSplitOptions.RemoveEmptyEntries);
			Debug.Log(arr.Length);
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

    //本地保存账号信息
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
        Debug.Log(AccountArray.Count);
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
        var flag = true;
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
        else
        {
            return null;
        }
    }

    //设置服务器列表
    public static void SetServers(XElement serverMap)
    {
        Debug.Log(serverMap);
        var servers = serverMap.Elements("server");
        ServiceManager.AllServerArray = new List<ServerVO>();
        ServiceManager.UsedServerArray = new List<ServerVO>();
        foreach (XElement node in servers)
        {
            var server = ServerVO.Parse(node);
            ServiceManager.AllServerArray.Add(server);
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
        for (int i = 0; i < AllServerArray.Count; i++)
        {
            if (AllServerArray[i].Url == url)
            {
                return AllServerArray[i];
            }
        }
        return null;
    }

    //获取当前默认登录的服务器
    public static ServerVO GetDefaultServer()
    {
        return ServerData;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Game configuration files parser.
/// </summary>
public class GameConfiguration : Singleton<GameConfiguration>
{
    #region Public Fields

    public TextAsset FeatureConfigurationText;
    public TextAsset GameConfigurationText;

    public ClickEffectHandler ClickEffect;

    public bool SingleMode;

    #endregion

    private void ReadFeatureConfigurationXml()
    {
        var doc = XElement.Parse(FeatureConfigurationText.text);
        var clickElement = doc.Element("ClickBehaviour");
        var continousMode = clickElement.Element("ContinousMode").Value;
        var timeInterval = clickElement.Element("TimeInterval").Value;
        ClickEffect.ContinousMode = bool.Parse(continousMode);
        ClickEffect.TimeInterval = float.Parse(timeInterval);
    }

    private void ReadGameConfigurationXml()
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(GameConfigurationText.text);
        var nodeList = xmlDoc.SelectNodes("config");

        if (nodeList == null)
        {
            return;
        }

        foreach (XmlNode node in nodeList)
        {
            if (node["fName"] != null)
            {
                GameConfig.FName = node["fName"].InnerText;
            }

            if (node["GameVersion"] != null)
            {
                GameConfig.Version = node["GameVersion"].InnerText;
            }

            if (node["ShowLog"] != null)
            {
                GameConfig.ShowLog = bool.Parse(node["ShowLog"].InnerText);
            }

            if (node["Language"] != null)
            {
                GameConfig.Language = node["Language"].InnerText;
            }

            if (node["ServicePath"] != null)
            {
                GameConfig.ServicePath = node["ServicePath"].InnerText;
            }

            if (node["LocalServicePath"] != null)
            {
                // Warning: DO NOT USE THIS PATH. Currently we do not need local service path, which will be used as local testing usage.
                GameConfig.LocalServicePath = string.Format("{0}/{1}", Application.persistentDataPath,
                    node["LocalServicePath"].InnerText);
            }

            if (node["CookieAddress"] != null)
            {
                GameConfig.CookieAddress = string.Format("{0}/{1}", Application.persistentDataPath, node["CookieAddress"].InnerText);
                Debug.LogWarning("cookie path" + GameConfig.CookieAddress);
                if (!File.Exists(GameConfig.CookieAddress))
                {
                    var path = new FileInfo(GameConfig.CookieAddress).DirectoryName;
                    if (!File.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    File.Create(GameConfig.CookieAddress);
                }
            } 
        }
    }

    private void ReadServiceConfigurationXml()
    {        
        StartCoroutine(DoReadRemoteServiceXml());
    }

    private IEnumerator DoReadRemoteServiceXml()
    {
        Debug.Log("加载Service.xml=" + GameConfig.ServicePath);
        var www = new WWW(GameConfig.ServicePath);
        yield return www;
        Debug.Log("加载Service.xml成功");
        var doc = XElement.Parse(www.text, LoadOptions.None);
        ParseService(doc);

        WindowManager.Instance.Show(typeof(LoginWindow), true);
    }

    private static void ParseService(XContainer doc)
    {
        Debug.Log("解析Service.xml");
        if (doc != null)
        {
            var global = doc.Element("global");
            var fValue = doc.Element("fValue");
            var iosStates = doc.Element("iosStates");
            var appMap = doc.Element("appMap");
            var serverMap = doc.Element("servers");

            //global
            var temp = global.Attribute("AutoLoginOfften").Value;
            ServiceManager.AutoLoginOfften = bool.Parse(temp);

            temp = global.Attribute("QuikeLoginOffen").Value;
            ServiceManager.QuikeLoginOffen = bool.Parse(temp);

            ServiceManager.GameID = global.Attribute("GameID").Value;
            ServiceManager.PassWordRegex = global.Attribute("PassWordRegex").Value;
            ServiceManager.PlatformDomain = global.Attribute("PlatformDomain").Value;
            ServiceManager.ResourceUrl = global.Attribute("ResourceUrl").Value;
            ServiceManager.ResourceVersion = global.Attribute("ResourceVersion").Value;
            ServiceManager.UpdateInfo = global.Attribute("UpdateInfo").Value;
            ServiceManager.UpdateUrl = global.Attribute("UpdateUrl").Value;
            ServiceManager.UserNameInfo = global.Attribute("UserNameInfo").Value;
            ServiceManager.UserNameRegex = global.Attribute("UserNameRegex").Value;

            //fValue
            ServiceManager.FValue = fValue.Element(GameConfig.FName).Value;

            //iosStates
            var states = iosStates.Elements("state");
            ServiceManager.IosStateArray = new List<AppStateVO>();
            foreach (XElement node in states)
            {
                var app = AppStateVO.Parse(node);
                ServiceManager.IosStateArray.Add(app);
                if (app.IsCheck && app.CheckVersion == Application.unityVersion)
                {
                    ServiceManager.IsCheck = true;
                    ServiceManager.CheckServerUrl = app.CheckServerUrl;
                }
            }

            //appMap
            var apps = appMap.Elements("app");
            ServiceManager.AppArray = new List<AppVO>();
            foreach (XElement node in apps)
            {
                var app = AppVO.Parse(node);
                ServiceManager.AppArray.Add(app);

                if (app.Version == Application.unityVersion)
                {
                    ServiceManager.AppData = app;
                }
            }

            //account
            ServiceManager.InitAccount();

            //servers
            ServiceManager.SetServers(serverMap);
            Debug.Log("解析Service.xml成功");
        }
    }

    #region Mono

    private void Start()
    {
        ReadFeatureConfigurationXml();
        ReadGameConfigurationXml();
        ReadServiceConfigurationXml();
    }

    #endregion
}

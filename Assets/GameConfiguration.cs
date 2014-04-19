using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class GameConfiguration : Singleton<GameConfiguration>
{
    private const string GameConfigurationFile = "GameConfiguration.xml";
    private string gameConfigurationPath;

    private const string GameConfigPath = "Config/GameConfig";

    public ClickEffectHandler ClickEffect;

    private void WriteToXml()
    {
        var doc = new XDocument(new XElement("Root",
                                       new XElement("ClickBehaviour",
                                           new XElement("ContinousMode", "false"),
                                           new XElement("TimeInterval", "0"))));
        doc.Save(gameConfigurationPath);
    }

    private void ReadXml()
    {
        var doc = XElement.Load(gameConfigurationPath);
        var clickElement = doc.Element("ClickBehaviour");
        var continousMode = clickElement.Element("ContinousMode").Value;
        var timeInterval = clickElement.Element("TimeInterval").Value;
        ClickEffect.ContinousMode = bool.Parse(continousMode);
        ClickEffect.TimeInterval = float.Parse(timeInterval);
    }

    private void ReadGameConfigXml()
    {
        var data = Resources.Load(GameConfigPath).ToString();

        var xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(data);
        var nodeList = xmlDoc.SelectNodes("config");

        if (nodeList != null)
        {
            foreach (XmlNode node in nodeList)
            {
                if (node["fName"] != null) GameConfig.FName = node["fName"].InnerText;
                if (node["GameVersion"] != null) GameConfig.Version = node["GameVersion"].InnerText;
                if (node["ShowLog"] != null) GameConfig.ShowLog = bool.Parse(node["ShowLog"].InnerText);
                if (node["Language"] != null) GameConfig.Language = node["Language"].InnerText;
                if (node["ServicePath"] != null) GameConfig.ServicePath = node["ServicePath"].InnerText;
                
                if (node["LocalServicePath"] != null) 
                    GameConfig.LocalServicePath = string.Format("{0}/{1}", Application.streamingAssetsPath, node["LocalServicePath"].InnerText);
                if (node["CookieAddress"] != null) 
                    GameConfig.CookieAddress = string.Format("{0}/{1}", Application.dataPath, node["CookieAddress"].InnerText); 
            }
        }
    }

    private void ReadServiceXml()
    {        
#if UNITY_EDITOR
        ReadLocalServiceXml();
#else
        StartCoroutine(ReadRemoteServiceXml());
#endif
    }

    private void ReadLocalServiceXml()
    {
        var doc = XElement.Load(GameConfig.LocalServicePath);
        ParseService(doc);
        WindowManager.Instance.Show(typeof(LoginWindow), true);
    }

    private IEnumerator ReadRemoteServiceXml()
    {
        var www = new WWW(GameConfig.ServicePath);
        yield return www;
        var doc = XElement.Parse(www.text, LoadOptions.None);
        ParseService(doc);
        WindowManager.Instance.Show(typeof(LoginWindow), true);
    }

    private void ParseService(XElement doc)
    {
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

        }
    }

    #region Mono
    private void Start()
    {
        gameConfigurationPath = string.Format("{0}/{1}", Application.streamingAssetsPath, GameConfigurationFile);

        ReadXml();

        ReadGameConfigXml();

        ReadServiceXml();
    }

    #endregion
}

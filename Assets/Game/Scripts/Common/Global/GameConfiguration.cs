using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
//using UnityEditor;
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

    public float LoadingTestTime = 1f;

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
                GameConfig.VersionValue = ServiceManager.GetVersionValue(GameConfig.Version);
            }
            
            if (node["BundleID"] != null)
            {
                GameConfig.BundleID = node["BundleID"].InnerText;
            }

            if (node["GameName"] != null)
            {
                GameConfig.GameName = node["GameName"].InnerText;
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

            GameConfig.ServerIpAddress = node["ServerIpAddress"].InnerText;

            if (node["BattleConfig"] != null)
            {
                GameConfig.BattleConfig = node["BattleConfig"].InnerText;
            }

            if (node["LocalServicePath"] != null)
            {
                // Warning: DO NOT USE THIS PATH. Currently we do not need local service path, which will be used as local testing usage.
                GameConfig.LocalServicePath = string.Format("{0}/{1}", Application.persistentDataPath,
                    node["LocalServicePath"].InnerText);
            }

//            if (node["CookieAddress"] != null)
//            {
//                GameConfig.CookieAddress = string.Format("{0}/{1}", Application.persistentDataPath, node["CookieAddress"].InnerText);
//                Logger.LogWarning("cookie path" + GameConfig.CookieAddress);
//                if (!File.Exists(GameConfig.CookieAddress))
//                {
//                    var path = new FileInfo(GameConfig.CookieAddress).DirectoryName;
//                    if (!File.Exists(path))
//                    {
//                        Directory.CreateDirectory(path);
//                    }
//                    File.Create(GameConfig.CookieAddress);
//                }
//            }
        }
    }

    private IEnumerator DoReadConfigXml()
    {
        yield return StartCoroutine(DoReadRemoteServiceXml());
        yield return StartCoroutine(DoReadBattleConfigXml());

        yield return new WaitForSeconds(LoadingTestTime);

        WindowManager.Instance.Show(typeof(LoginWindow), true);
    }

    private IEnumerator DoReadRemoteServiceXml()
    {
        var str = GameConfig.ServicePath + "?" + DateTime.Now.ToFileTime();
        Logger.Log("加载Service.xml=" + str);

        var www = new WWW(str);
        yield return www;

        if (!String.IsNullOrEmpty(www.error))
        {
            Logger.LogError(www.error);
            Alert.Show(AssertionWindow.Type.Ok, "系统提示", "网络连接失败，请您接入网络后再试", OnAssertButtonClicked);
        }
        else
        {
            Logger.Log("加载Service.xml成功");
            //Logger.Log(www.text);

            var doc = XElement.Parse(www.text, LoadOptions.None);

            ParseService(doc);
        }
    }

    private static void ParseService(XContainer doc)
    {
        Logger.Log("解析Service.xml");
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
            ServiceManager.ResourceUrl = global.Attribute("ResourceUrl").Value;
            ServiceManager.ResourceVersion = global.Attribute("ResourceVersion").Value;
            ServiceManager.UpdateInfo = global.Attribute("UpdateInfo").Value;
            ServiceManager.UpdateUrl = global.Attribute("UpdateUrl").Value;
            ServiceManager.QuikeLoginOffen = bool.Parse(global.Attribute("QuikeLoginOffen").Value);
            ServiceManager.AutoLoginOfften = bool.Parse(global.Attribute("AutoLoginOfften").Value);

            //fValue
            ServiceManager.FValue = fValue.Element(GameConfig.FName).Value;

            //iosStates
            var states = iosStates.Elements("state");
            ServiceManager.IosStateArray = new List<AppStateVO>();
            foreach (XElement node in states)
            {
                var app = AppStateVO.Parse(node);
                ServiceManager.IosStateArray.Add(app);
                if (app.IsCheck && app.CheckVersion == GameConfig.Version)
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

                if (app.BundleID == GameConfig.BundleID)
                {
                    ServiceManager.AppData = app;
                    if ((ServiceManager.AppData.Version == GameConfig.Version && ServiceManager.AppData.IsTest) || (SystemInfo.deviceType == DeviceType.Desktop))
                    {
                        ServiceManager.IsTest = true;
                        ServiceManager.OpenTestAccount = ServiceManager.AppData.OpenTestAccount;
                    }
                }
            }

            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                ServiceManager.IsTest = true;
                ServiceManager.OpenTestAccount = true;
            }

            if (ServiceManager.AppData != null && GameConfig.VersionValue < ServiceManager.AppData.VersionValue && !ServiceManager.AppData.IsTest)
            {
                DoUpdate();
            }

            //account
            ServiceManager.InitAccount();

            //servers
            ServiceManager.SetServers(serverMap);
            Logger.Log("解析Service.xml成功");
        }
    }

    private static void DoUpdate()
    {
        if (ServiceManager.AppData.ForceUpdate)
        {
            Alert.Show(AssertionWindow.Type.Ok, "系统提示", ServiceManager.UpdateInfo, OnUpdateConfirmHandler);
        }
        else
        {
            Alert.Show(AssertionWindow.Type.OkCancel, "系统提示", ServiceManager.UpdateInfo, OnUpdateConfirmHandler, OnUpdateCancelHandler);
        }
    }

    private static void OnUpdateConfirmHandler(GameObject sender)
    {
        Application.OpenURL(ServiceManager.AppData.UpdateUrl);
        DoUpdate();
    }

    private static void OnUpdateCancelHandler(GameObject sender)
    {
        
    }

    private IEnumerator DoReadBattleConfigXml()
    {
        Logger.Log("加载BattleConfig.xml=" + GameConfig.BattleConfig);
        var www = new WWW(GameConfig.BattleConfig);
        yield return www;
        Logger.Log("加载BattleConfig.xml成功");
        //Logger.Log(www.text);
        var doc = XElement.Parse(www.text, LoadOptions.PreserveWhitespace);

        ParseBattleConfig(doc);
    }

    private static void ParseBattleConfig(XContainer doc)
    {
        Logger.Log("解析BattleConfig.xml");
        if (doc != null)
        {
            GameConfig.HeroRunReturnTime = GetValueByName(doc, "HeroRunReturnTime");
            GameConfig.ShortTime = GetValueByName(doc, "ShortTime");
            GameConfig.PlayAttrackTime = GetValueByName(doc, "PlayAttrackTime");
            GameConfig.RunStepNeedTime = GetValueByName(doc, "RunStepNeedTime");
            GameConfig.NextRunWaitTime = GetValueByName(doc, "NextRunWaitTime");

            GameConfig.PlayRecoverEffectTime = GetValueByName(doc, "PlayRecoverEffectTime");
            GameConfig.RunRoNextMonstersTime = GetValueByName(doc, "RunRoNextMonstersTime");
            GameConfig.RunToAttrackPosTime = GetValueByName(doc, "RunToAttrackPosTime");
            GameConfig.NextAttrackWaitTime = GetValueByName(doc, "NextAttrackWaitTime");
            GameConfig.TotalHeroAttrackTime = GetValueByName(doc, "TotalHeroAttrackTime");

            GameConfig.MoveCameraTime = GetValueByName(doc, "MoveCameraTime");
            GameConfig.Attrack9PlayEffectTime = GetValueByName(doc, "Attrack9PlayEffectTime");
            GameConfig.Attrack9HeroInTime = GetValueByName(doc, "Attrack9HeroInTime");
            GameConfig.Attrack9TextInTime = GetValueByName(doc, "Attrack9TextInTime");
            GameConfig.Attrack9TextShowTime = GetValueByName(doc, "Attrack9TextShowTime");

            GameConfig.Attrack9TextFadeTime = GetValueByName(doc, "Attrack9TextFadeTime");
            GameConfig.Attrack9HeroFadeTime = GetValueByName(doc, "Attrack9HeroFadeTime");
            GameConfig.Attrack9SwardEffectTime = GetValueByName(doc, "Attrack9SwardEffectTime");
            GameConfig.Attrack9SwardWaitTime = GetValueByName(doc, "Attrack9SwardWaitTime");
            GameConfig.Attrack9TotalSwardTime = GetValueByName(doc, "Attrack9TotalSwardTime");

            GameConfig.PlayMonsterEffectTime = GetValueByName(doc, "PlayMonsterEffectTime");
            GameConfig.MonsterAttrackStepTime = GetValueByName(doc, "MonsterAttrackStepTime");
            GameConfig.HeroBeenAttrackTime = GetValueByName(doc, "HeroBeenAttrackTime");

            Logger.Log("解析BattleConfig.xml成功");
        }
    }

    private static float GetValueByName(XContainer doc, string name)
    {
        var value = doc.Element(name);
        if (value != null)
        {
            return Convert.ToSingle(value.Value);
        }
        return 0.01f;
    }

    private void HandleAsyncOperation()
    {
        StartCoroutine(DoAsyncOperation());
    }

    private IEnumerator DoAsyncOperation()
    {
        yield return StartCoroutine(PingTest.Instance.TestConnection(GameConfig.ServerIpAddress));
        if (PingTest.Instance.HasConnection)
        {
            StartCoroutine(DoReadConfigXml());
        }
        else
        {
            // Show assert window.
            Alert.Show(AssertionWindow.Type.Ok, "系统提示", "网络连接失败，请您接入网络后再试", OnAssertButtonClicked);
        }
    }

    private void OnAssertButtonClicked(GameObject sender)
    {
        HandleAsyncOperation();
    }

    private void InitMta()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        // 设置渠道（android可以使用配置manifest.xml形式配置）
        MtaService.SetInstallChannel("feidou");
        // !!!!! 重要 !!!!!!!
        // MTA的appkey在android和ios系统上不同，请为根据不同平台设置不同appkey，否则统计结果可能会有问题。
        string mta_appkey = null;
#if UNITY_IPHONE
	    mta_appkey = "I86MH45ENQZP";
#elif UNITY_ANDROID
        mta_appkey = "ACR76WS35WPQ";
#endif
        MtaService.StartStatServiceWithAppKey(mta_appkey);
        MtaManager.TestServiceSpeed();
    }

    #region Mono

    private void Start()
    {
        InitMta();

        ReadFeatureConfigurationXml();
        ReadGameConfigurationXml();
        HandleAsyncOperation();
    }

    #endregion
}

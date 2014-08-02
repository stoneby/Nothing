using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using System.Collections;

public class AppVO 
{
    //<app BundleID="cn.kx.yttxsougou" ForceUpdate="false" RechargeType="2"  Version="3.0.0" UpdateUrl="http://www.feidou.com/download/yttx.html"/>
    public string BundleID;
    public bool ForceUpdate = false;
    public string RechargeType;
    public string Version;
    public int VersionValue;
    public bool IsTest;
    public string UpdateUrl;
    public bool OpenTestAccount;

    public static AppVO Parse(XElement data)
    {
        var app = new AppVO();
        app.BundleID = data.Attribute("BundleID").Value;
        app.RechargeType = data.Attribute("RechargeType").Value;
        app.Version = data.Attribute("Version").Value;
        app.VersionValue = ServiceManager.GetVersionValue(app.Version);
        app.UpdateUrl = data.Attribute("UpdateUrl").Value;
        app.ForceUpdate = bool.Parse(data.Attribute("ForceUpdate").Value);
        app.IsTest = bool.Parse(data.Attribute("IsTest").Value);
        app.OpenTestAccount = bool.Parse(data.Attribute("OpenTestAccount").Value);
        return app;
    }
}

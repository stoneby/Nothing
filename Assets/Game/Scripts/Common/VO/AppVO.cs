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
    public string UpdateUrl;

    public static AppVO Parse(XElement data)
    {
        var app = new AppVO();
        app.BundleID = data.Attribute("BundleID").Value;
        app.RechargeType = data.Attribute("RechargeType").Value;
        app.Version = data.Attribute("Version").Value;
        app.UpdateUrl = data.Attribute("UpdateUrl").Value;
        app.ForceUpdate = bool.Parse(data.Attribute("ForceUpdate").Value);

        return app;
    }
}

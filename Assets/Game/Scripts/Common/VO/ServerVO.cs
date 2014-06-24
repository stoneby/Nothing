using System.Xml.Linq;
using UnityEngine;
using System.Collections;

public class ServerVO
{
    //<server ID="3" RequestClientVersion="1.1.0" ServerName="本地测试(S3)" ServerState="1" Url="http://s3.yt.feidou.com"/>
    public string ID;
    public string ServerState;
    public string RequestClientVersion;
    public string ServerName;
    public string Url;
    public bool IsTest;
    public string DataUrl;

    public static ServerVO Parse(XElement data)
    {
        var app = new ServerVO();
        app.ID = data.Attribute("ID").Value;
        app.RequestClientVersion = data.Attribute("RequestClientVersion").Value;
        app.ServerState = data.Attribute("ServerState").Value;
        app.ServerName = data.Attribute("ServerName").Value;
        app.Url = data.Attribute("Url").Value;
        app.IsTest = bool.Parse(data.Attribute("IsTest").Value);
        app.DataUrl = data.Attribute("DataUrl").Value;

//        Logger.Log(app.ID);
//        Logger.Log(app.RequestClientVersion);
//        Logger.Log(app.ServerState);
//        Logger.Log(app.ServerName);
//        Logger.Log(app.Url);

        return app;
    }
	
}

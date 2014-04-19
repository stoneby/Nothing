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

    public static ServerVO Parse(XElement data)
    {
        var app = new ServerVO();
        app.ID = data.Attribute("ID").Value;
        app.RequestClientVersion = data.Attribute("RequestClientVersion").Value;
        app.ServerState = data.Attribute("ServerState").Value;
        app.ServerName = data.Attribute("ServerName").Value;
        app.Url = data.Attribute("Url").Value;

        Debug.Log(app.ID);
        Debug.Log(app.RequestClientVersion);
        Debug.Log(app.ServerState);
        Debug.Log(app.ServerName);
        Debug.Log(app.Url);

        return app;
    }
	
}

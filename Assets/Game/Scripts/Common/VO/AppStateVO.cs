using System.Xml.Linq;

public class AppStateVO
{
    //<state CheckServerUrl="http://ss.yt.feidou.com" checkVersion="0.0.5" isCheck="false"/>
    public string CheckServerUrl;
    public string CheckVersion;
    public bool IsCheck;

    public static AppStateVO Parse(XElement data)
    {
        var app = new AppStateVO();
        app.CheckServerUrl = data.Attribute("CheckServerUrl").Value;
        app.CheckVersion = data.Attribute("CheckVersion").Value;
        app.IsCheck = bool.Parse(data.Attribute("IsCheck").Value);
        return app;
    }
}

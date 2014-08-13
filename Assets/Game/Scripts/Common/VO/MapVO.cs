using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using System.Collections;

public class MapVO  {
    //<map id="3" x="223" y="-190" w="349" h="260" points=""/>
    public string id;
    public int x;
    public int y;
    public int w;
    public int h;
    public Vector2[] points;

    public static MapVO Parse(string datastr)
    {
        //var data = new XmlElement();
        var data = XElement.Parse(datastr, LoadOptions.None);
        //Logger.Log("=======================================" + data.ToString());
        var app = new MapVO();
        app.id = data.Attribute("id").Value;
        app.x = int.Parse(data.Attribute("x").Value);
        app.y = int.Parse(data.Attribute("y").Value);
        app.w = int.Parse(data.Attribute("w").Value);
        app.h = int.Parse(data.Attribute("h").Value);

        string str = data.Attribute("points").Value;
        string[] ps = str.Split(',');
        if (ps.Length > 1)
        {
            int count = ps.Length / 2;
            app.points = new Vector2[count];
            for (int i = 0; i < ps.Length; i += 2)
            {
                app.points[i / 2] = new Vector2(float.Parse(ps[i]), float.Parse(ps[i + 1]));
            }
        }
            
        return app;
    }
}

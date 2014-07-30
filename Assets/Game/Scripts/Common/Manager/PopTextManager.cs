using UnityEngine;
using Object = UnityEngine.Object;

public class PopTextManager
{
    static GameObject textContainer;
    static GameObject textPrefab;
    static GameObject shakePrefab;
    private static bool haveInit;

    public static void Init()
    {
        textContainer = GameObject.Find("Tip");
        textPrefab = Resources.Load("Prefabs/Component/PopLabel") as GameObject;
        shakePrefab = Resources.Load("Prefabs/Component/ShakeLabel") as GameObject;
    }

    public static void ShowText(string str, float showtime, float offsetx, float offsety, float popheight, Vector3 pos)
    {
        if (!haveInit)
        {
            Init();
            haveInit = true;
        }
        var obj = NGUITools.AddChild(textContainer, textPrefab);
        var pt = obj.GetComponent<PopTextControl>();
        pt.ShowText(str, popheight, new Vector3(pos.x + offsetx, pos.y + offsety, pos.z));
        obj.transform.localPosition = pos;
        Object.Destroy(obj, showtime);
        
    }

    private static float LastTime = -10;
    private static float LastPos = 0;
    public static void PopTip(string str)
    {
        Logger.Log(str);
        var t = Time.time;
        if (t - LastTime < 3 && LastPos < 300)
        {
            LastPos += 40;
        }
        else
        {
            LastPos = 0;
        }
        LastTime = t;
        //Logger.Log("time = " + Time.time);
        //var now = DateTime.Now;
        //Logger.Log(Time.time);
        ShowText(str, 3, 0, 0, 100, new Vector3(0, 250 - LastPos, 0));
    }

    public static GameObject ShakeText(string str, float offsetx, float offsety, Vector3 pos)
    {
        if (!haveInit)
        {
            Init();
            haveInit = true;
        }
        var obj = NGUITools.AddChild(textContainer, shakePrefab);
        var lb = obj.GetComponent<UILabel>();
        lb.text = str;
        iTweenEvent.GetEvent(obj, "ShakeAttrackLabel").Play();
        obj.transform.localPosition = new Vector3(pos.x + offsetx, pos.y + offsety, pos.z);
        return obj;
    }
}

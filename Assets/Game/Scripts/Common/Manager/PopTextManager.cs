using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class PopTextManager
{
    static GameObject textContainer;
    static GameObject textPrefab;
    private static bool haveInit = false;

    public static void Init()
    {
        textContainer = GameObject.Find("Tip");
        textPrefab = Resources.Load("Prefabs/Component/PopLabel") as GameObject;
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

    public static void PopTip(string str)
    {
        
        //var now = DateTime.Now;
        //Debug.Log(Time.time);
        ShowText(str, 3, 0, 0, 100, new Vector3(0,250,0));
    }
}

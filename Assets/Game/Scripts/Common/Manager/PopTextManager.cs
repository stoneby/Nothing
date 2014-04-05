using UnityEngine;

public class PopTextManager
{
    static GameObject textContainer;
    static GameObject textPrefab;

    public static void Init(GameObject rootobj, GameObject textprefab)
    {
        textContainer = rootobj;
        textPrefab = textprefab;
    }

    public static void ShowText(string str, float showtime, float offsetx, float offsety, float popheight, Vector3 pos)
    {
        var obj = NGUITools.AddChild(textContainer, textPrefab);
        var pt = obj.GetComponent<PopTextControl>();
        pt.ShowText(str, popheight, new Vector3(pos.x + offsetx, pos.y + offsety, pos.z));
        obj.transform.localPosition = pos;
        Object.Destroy(obj, showtime);
    }
}

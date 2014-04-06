using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public class EffectManager
{
    private static ArrayList effectFabs;
    private static ArrayList names;
    private static GameObject rootGameObject;

    public static double Tolerance = 0.1f;

    public static void InitEffects(GameObject rootobj)
    {
        effectFabs = new ArrayList();
        names = new ArrayList();
        rootGameObject = rootobj;
    }

    public static GameObject ShowEffect(string effecttype, float offsetx, float offsety, Vector3 pos)
    {
        var obj = GetEffectPrefab(effecttype);
        Vector3 v3 = Camera.main.WorldToScreenPoint(pos);
        v3.x += offsetx;
        v3.y += offsety;
        var sam = rootGameObject.GetComponent<Camera>();
        v3 = sam.ScreenToWorldPoint(v3);
        v3.z = 5;
        var effectobj = MonoBehaviour.Instantiate(obj, v3, obj.transform.rotation) as GameObject;
        effectobj.transform.parent = rootGameObject.transform;
        SetAllLayers(effectobj, 9);
        return effectobj;
    }

    public static void PlayEffect(string effecttype, float showtime, float offsetx, float offsety, Vector3 pos, float rotatevalue = 0, bool isbig = false)
    {
        var sam = rootGameObject.GetComponent<Camera>();
        if (isbig)
        {
            sam.orthographicSize = 0.5f;
        }
        else
        {
            sam.orthographicSize = 2.5f;

        }
        GameObject obj = GetEffectPrefab(effecttype);

        //Transform trans = transforms;
        Vector3 v3 = Camera.main.WorldToScreenPoint(pos);
        v3.x += offsetx;
        v3.y += offsety;
       
        v3 = sam.ScreenToWorldPoint(v3);
        v3.z = 5;
        //obj.transform.rotation
        var effectobj =
            Object.Instantiate(obj, v3, obj.transform.rotation) as GameObject;

        SetAllRotation(effectobj, rotatevalue);
        effectobj.transform.parent = rootGameObject.transform;
        SetAllLayers(effectobj, 9);
        Object.Destroy(effectobj, showtime);
    }

    private static void SetAllRotation(GameObject obj, float rotatevalue)
    {
        if (Math.Abs(rotatevalue) < Tolerance)
        {
            return;
        }
        var ps = obj.GetComponent<ParticleSystem>();
        ps.startRotation += rotatevalue;
        foreach (var item in obj.transform)
        {
            SetAllRotation(((Transform)item).gameObject, rotatevalue);
        }
    }

    public static void PlayMoreEffect(string effecttype, float showtime, Vector3 pos, float offsetx = 0, float offsety = 0)
    {
        Debug.Log(pos);
        const float offset = 0.2f;

        PlayEffect(effecttype, showtime, offsetx, offsety, new Vector3(pos.x + offset, pos.y - offset, pos.z));
        PlayEffect(effecttype, showtime, offsetx, offsety, new Vector3(pos.x - offset, pos.y + offset, pos.z));
        PlayEffect(effecttype, showtime, offsetx, offsety, new Vector3(pos.x + offset, pos.y + offset, pos.z));
        PlayEffect(effecttype, showtime, offsetx, offsety, new Vector3(pos.x - offset, pos.y - offset, pos.z));
    }

    public static void FlyEffect(string effecttype, float showtime, float offsetx, float offsety, Vector3 frompos, Vector3 topos)
    {
        var obj = GetEffectPrefab(effecttype);
        Vector3 v1 = Camera.main.WorldToScreenPoint(frompos);
        v1.x += offsetx;
        v1.y += offsety;
        var sam = rootGameObject.GetComponent<Camera>();
        v1 = sam.ScreenToWorldPoint(v1);
        v1.z = 5;

        Vector3 v2 = topos;
        v2.x += offsetx + (int)(Screen.width / 2);
        v2.y += offsety + (int)(Screen.height / 2);
        v2 = sam.ScreenToWorldPoint(v2);
        v2.z = 5;
        //_effects.Add(rootGameObject.Instantiate(obj, trans.position, trans.rotation));
        var effectobj = Object.Instantiate(obj, v1, new Quaternion(v2.x - v1.x, v2.y - v1.y, v2.z - v1.z, 0)) as GameObject;
        effectobj.transform.parent = rootGameObject.transform;
        SetAllLayers(effectobj, 9);

        var tp = effectobj.AddComponent<TweenPosition>();

        tp.from = v1;
        tp.to = v2;
        tp.duration = showtime;
        tp.PlayForward();

        Object.Destroy(effectobj, showtime);
    }

    private static void SetAllLayers(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (var item in obj.transform)
        {
            SetAllLayers(((Transform)item).gameObject, layer);
        }
    }

    private static void SetAllScales(GameObject obj, float scale)
    {
        obj.transform.localScale = new Vector3(scale, scale, scale);
        foreach (var item in obj.transform)
        {
            SetAllScales(((Transform)item).gameObject, scale);
        }
    }

    private static GameObject GetEffectPrefab(string effecttype)
    {
        for (var i = 0; i < names.Count; i++)
        {
            var str = names[i] as string;
            if (str == effecttype)
            {
                return effectFabs[i] as GameObject;
            }
        }
        var obj = Resources.Load(effecttype) as GameObject;
        effectFabs.Add(obj);
        names.Add(effecttype);
        return obj;
    }

    public static void PlayAllEffect(bool isplay)
    {
        var gamecamera = GameObject.Find("Camera");
        foreach (var item in gamecamera.transform)
        {
            ((Transform)item).gameObject.SetActive(isplay);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpResourceManager : MonoBehaviour 
{
    private static GameObject LoadEffect;

    public static GameObject MainObj;

    private static Dictionary<string, byte[]> FileDatas;

    void Start()
    {
        MainObj = gameObject;
    }

    public static void LoadAll()
    {
        if (string.IsNullOrEmpty(ServiceManager.ServerData.DataUrl)) return;
        FileDatas = new Dictionary<string, byte[]>();
        var m = MainObj.GetComponent<HttpResourceManager>();
        m.DoLoadData();
    }

    public static byte[] LoadData(string theurl)
    {
        return FileDatas.ContainsKey(theurl) ? FileDatas[theurl] : null;
    }

    public void DoLoadData()
    {
        StartCoroutine(StartLoad());
    }

    private IEnumerator StartLoad()
    {
        var str = ".bytes?" + DateTime.Now.ToFileTime();
        foreach (var t in ResourcePath.ByteFiles)
        {
            var theurl = ServiceManager.ServerData.DataUrl + t + str;
            Logger.Log(theurl);
            var www = new WWW(theurl);
            
            // Wait for download to complete
            yield return www;
            FileDatas[t] = www.bytes;
        }
    }
}


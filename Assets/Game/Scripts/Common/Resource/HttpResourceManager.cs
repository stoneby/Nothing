using System;
using System.Collections.Generic;
using Thrift.Protocol;
using Thrift.Transport;
using UnityEngine;
using System.Collections;

public class HttpResourceManager : MonoBehaviour 
{
    private static GameObject LoadEffect;

    public static GameObject MainObj;

    private static Dictionary<string, byte[]> FileDatas;

    void Start()
    {
        MainObj = gameObject;
        //LoadAll();
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
        if (FileDatas.ContainsKey(theurl))
        {
            return FileDatas[theurl];
        }
        else
        {
            return null;
        }
    }

    public void DoLoadData()
    {
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad()
    {
        // Start a download of the given URL
        LoadEffect = EffectManager.ShowEffect(EffectType.Loading, 0, 0, new Vector3(0, 0, 0));
        var str = ".bytes?" + DateTime.Now.ToFileTime();
        for (int i = 0; i < ResourcePath.ByteFiles.Length; i++)
        {
            var theurl = ServiceManager.ServerData.DataUrl + ResourcePath.ByteFiles[i] + str;
            Logger.Log(theurl);
            WWW www = new WWW(theurl);
            
            // Wait for download to complete
            yield return www;
            FileDatas[ResourcePath.ByteFiles[i]] = www.bytes;
        }

        if (LoadEffect != null)
        {
            Destroy(LoadEffect);
            LoadEffect = null;
        }
    }
}


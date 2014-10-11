using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;

public class ResoucesManager : Singleton<ResoucesManager>
{
    #region Private Fields

    private readonly Dictionary<string, WWW> resoucesWwws = new Dictionary<string, WWW>();
    private const string VersionFileName = "VersionNum.xml";
    private bool isFullPackage;
    private readonly Dictionary<string, int> versions = new Dictionary<string, int>();
    private List<string> needDownFilesCached; 

    #endregion

    #region public Fields

    public delegate void ResourcesDownLoaded();

    public ResourcesDownLoaded OnResourcesDownLoaded;

    public bool IsFullPackage
    {
        get { return isFullPackage; }
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        GameConfiguration.Instance.OnParseXmlFinish += OnParseServiceFinish;
    }

    private void OnParseServiceFinish()
    {
        isFullPackage = GameConfig.IsFullAssetBundles;
        if(isFullPackage == false)
        {
            var baseUrl = AssetBundlePathProvider.GetBundleBaseUrl();
            Debug.Log("DownLoadAllBundles ========================== " + baseUrl);
            StartCoroutine(DownLoadAllBundles());
        }
        else
        {
            if (OnResourcesDownLoaded != null)
            {
                OnResourcesDownLoaded();
            }
        }
    }

    private IEnumerator DownLoadAllBundles()
    {
        yield return StartCoroutine(DownLoadVersions());
        DownLoadResources();
    }

    private IEnumerator DownLoadVersions()
    {
        var baseUrl = AssetBundlePathProvider.GetBundleBaseUrl();
        var versionPath = baseUrl + VersionFileName;
        var versionWww = new WWW(versionPath);
        yield return versionWww;
        var xmlDoc = new XmlDocument();
        Debug.Log("version down load sucuss ========================== " );
        xmlDoc.LoadXml(versionWww.text);
        var xmlRoot = xmlDoc.DocumentElement;
        foreach (var node in xmlRoot.ChildNodes)
        {
            if((node is XmlElement) == false)
                continue;
            var file = (node as XmlElement).GetAttribute("FileName");
            var num = XmlConvert.ToInt32((node as XmlElement).GetAttribute("Num"));
            versions.Add(file, num);
        }
        needDownFilesCached = versions.Keys.ToList();
    }

    private void DownLoadResources()
    {
        if (needDownFilesCached.Count > 0)
        {
            var file = needDownFilesCached[0];
            needDownFilesCached.RemoveAt(0);
            StartCoroutine(DownLoad(file, versions[file]));
        }
    }

    private IEnumerator DownLoad(string file, int num)
    {
        var baseUrl = AssetBundlePathProvider.GetBundleBaseUrl();
        var path = baseUrl + file;
        var www = WWW.LoadFromCacheOrDownload(path, num);
        yield return www;
        resoucesWwws.Add(file, www);
        DownLoadResources();
        if (needDownFilesCached.Count == 0)
        {
            if (OnResourcesDownLoaded != null)
            {
                OnResourcesDownLoaded();
            }
            Debug.Log(" Asset bundle down load is finished *********************    "+Time.time); 
        }
    }

    #endregion

    #region AssetBundle Load Interface

    public Object Load(string path)
    {
        //读下载资源   
        if (IsFullPackage == false)
        {
            var bundleName = Utils.ConvertToAssetBundleName(path) + ".assetbundle";
            if (resoucesWwws.ContainsKey(bundleName))
            {
                var bundle = resoucesWwws[bundleName].assetBundle;
                var obj = bundle.mainAsset;
                bundle.Unload(false);
                return obj;
            }
        }
        return Resources.Load(path);
    }

    public T Load<T>(string path) where T : Object
    {
        return Load(path) as T;
    }

    public Object[] LoadAll(string path)
    {
        if (IsFullPackage == false)
        {
            var bundleFolderName = Utils.ConvertToAssetBundleName(path) + ".assetbundle";
            if (resoucesWwws.ContainsKey(bundleFolderName))
            {
                var bundle = resoucesWwws[bundleFolderName].assetBundle;
                var objs = bundle.LoadAll(typeof(Object));
                bundle.Unload(false);
                return objs;
            }
        }
        return Resources.LoadAll(path);
    }

    public T[] LoadAll<T>(string path) where T : Object
    {
        var att = LoadAll(path).Where(item => item is T).ToList();
        var result = new T[att.Count];
        int index = 0;
        foreach(var at in att)
        {
            result[index] = (T) at;
            index++;
        }
        return result;
    }

    #endregion
}

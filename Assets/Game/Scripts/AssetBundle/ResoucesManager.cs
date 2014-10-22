using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using VersionInfo = BundleUtils.VersionInfo;

public class ResoucesManager : Singleton<ResoucesManager>
{
    #region Private Fields

    private readonly Dictionary<string, WWW> resoucesWwws = new Dictionary<string, WWW>();
    public static string VersionFileName = "VersionNum.xml";
    private bool isFullPackage;
    private Dictionary<string, VersionInfo> versions = new Dictionary<string, VersionInfo>();
    private Dictionary<string, VersionInfo> fullPackageOriginVersions = new Dictionary<string, VersionInfo>(); 
    private List<string> needDownFilesCached = new List<string>();
    private int needDownLoadTotalCount;
    private float startDownloadTime;
    private int curLoadingCount;

    #endregion

    #region public Fields

    public delegate void ResourcesDownLoaded();
    public delegate void ResourcesDownLoadStarted();
    public delegate void DownLoadProgessChanged(float progess);

    public DownLoadProgessChanged OnDownLoadProgessChanged;
    public ResourcesDownLoadStarted OnResourcesDownLoadStarted;
    public ResourcesDownLoaded OnResourcesDownLoaded;

    public int LoadingCountAtSameTime = 6;

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
        PingTest.Instance.CheckConnection();
        isFullPackage = GameConfig.IsFullAssetBundles;
        var baseUrl = AssetBundlePathProvider.GetBundleBaseUrl();
        Debug.Log("Start DownLoadAllBundles from " + baseUrl + " ==========================  ");
        StartCoroutine(DownLoadAllBundles());
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
        Debug.Log("version down load sucuss ========================== ");
        versions = ParseVersionNums(versionWww.text);
        if (isFullPackage)
        {
            var versionPathInResources = AssetBundlePathProvider.GetVerionPathInResource();
            var versionTextAsset = Resources.Load(versionPathInResources) as TextAsset;
            if(versionTextAsset != null)
            {
                fullPackageOriginVersions = ParseVersionNums(versionTextAsset.text);
            }
            needDownFilesCached =
                versions.Keys.Where(
                    key =>
                    (!fullPackageOriginVersions.ContainsKey(key) || versions[key].Version > fullPackageOriginVersions[key].Version)).
                    ToList();
        }
        else
        {
            needDownFilesCached = versions.Keys.ToList();
        }
        CheckAssetBundlesLoaded();
        needDownLoadTotalCount = needDownFilesCached.Count;
    }

    private Dictionary<string, VersionInfo> ParseVersionNums(string versionXml)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(versionXml);
        var xmlRoot = xmlDoc.DocumentElement;
        return BundleUtils.ReadVersionNumFile(xmlRoot);
    }

    private void DownLoadResources()
    {
        if (needDownFilesCached.Count > 0 && curLoadingCount < LoadingCountAtSameTime)
        {
            if (needDownFilesCached.Count == needDownLoadTotalCount)
            {
                startDownloadTime = Time.realtimeSinceStartup;
                if(OnResourcesDownLoadStarted != null)
                {
                    OnResourcesDownLoadStarted();
                }
            }
            var file = needDownFilesCached[0];
            needDownFilesCached.RemoveAt(0);
            StartCoroutine(DownLoad(file, versions[file].Version));
            curLoadingCount++;
        }
    }

    private IEnumerator DownLoad(string file, int num)
    {
        var baseUrl = AssetBundlePathProvider.GetBundleBaseUrl();
        var path = baseUrl + file;
        var www = WWW.LoadFromCacheOrDownload(path, num);
        if (BundleUtils.ComputeMd5(www.bytes) != versions[file].Md5)
        {
            Logger.LogWarning(
                string.Format(
                    "Down loaded asset bundle {0}'s md5 value is different from the version xml of the asset.", file));
        }
        yield return www;
        resoucesWwws.Add(file, www);
        if (OnDownLoadProgessChanged != null)
        {
            OnDownLoadProgessChanged(1 - (float)needDownFilesCached.Count / needDownLoadTotalCount);
        }
        curLoadingCount--;
        DownLoadResources();
        CheckAssetBundlesLoaded();
    }

    private void CheckAssetBundlesLoaded()
    {
        if(needDownFilesCached.Count == 0)
        {
            Debug.Log(" Asset bundle down load is finished *********************, it costs time   " +
                      (Time.realtimeSinceStartup - startDownloadTime));
            if(OnResourcesDownLoaded != null)
            {
                OnResourcesDownLoaded();
            }
        }
    }

    #endregion

    #region AssetBundle Load Interface

    public Object Load(string path)
    {
        var bundleName = Utils.ConvertToAssetBundleName(path) + ".assetbundle";
        //读下载资源   
        if (resoucesWwws.ContainsKey(bundleName))
        {
            var bundle = resoucesWwws[bundleName].assetBundle;
            var obj = bundle.mainAsset;
            return obj;
        }
        return Resources.Load(path);
    }

    public T Load<T>(string path) where T : Object
    {
        return Load(path) as T;
    }

    public Object[] LoadAll(string path)
    {
        var bundleFolderName = Utils.ConvertToAssetBundleName(path) + ".assetbundle";
        if (resoucesWwws.ContainsKey(bundleFolderName))
        {
            var bundle = resoucesWwws[bundleFolderName].assetBundle;
            var objs = bundle.LoadAll(typeof(Object));
            return objs;
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

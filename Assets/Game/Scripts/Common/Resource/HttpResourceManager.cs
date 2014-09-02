using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HttpResourceManager : Singleton<HttpResourceManager>
{
    private Dictionary<string, byte[]> fileDatas;

    private ZipHelper zipHelper;

    public delegate void LoadFinish();
    public LoadFinish OnLoadFinish;

    public void LoadTemplate()
    {
        if (string.IsNullOrEmpty(ServiceManager.ServerData.DataUrl))
        {
            return;
        }

        if (fileDatas == null)
        {
            fileDatas = new Dictionary<string, byte[]>();
        }

        zipHelper.AutoMode = false;
        zipHelper.UrlPath = string.Format("{0}{1}", ServiceManager.ServerData.DataUrl, ResourcePath.TemplatePath);
        zipHelper.Download();
    }

    public byte[] GetTemplateData(string type)
    {
        return fileDatas.ContainsKey(type) ? fileDatas[type] : null;
    }

    private void OnResourceLoadFinish()
    {
        LoadAllTemplates();
    }

    private void LoadAllTemplates()
    {
        StartCoroutine(DoLoadAllTemplates());
    }

    private IEnumerator DoLoadAllTemplates()
    {
        foreach (var entryPath in zipHelper.EntryList)
        {
            var fileInfor = new FileInfo(entryPath);
            var index = fileInfor.Name.LastIndexOf('.');
            if (index != -1)
            {
                // [WARNING] This is very important building this key, refers to ResourcePath.cs.
                var key = string.Format("Templates/{0}", fileInfor.Name.Substring(0, index));
                fileDatas[key] = File.ReadAllBytes(entryPath);
            }
        }

        yield return null;

        if (OnLoadFinish != null)
        {
            OnLoadFinish();
        }
    }

    private void Awake()
    {
        zipHelper = GetComponent<ZipHelper>() ?? gameObject.AddComponent<ZipHelper>();
        zipHelper.OnLoadFinish += OnResourceLoadFinish;
    }
}


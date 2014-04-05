using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Automatically map between path and layer.
/// </summary>
public class AutoPathLayerMapping : AbstractPathLayerMapping
{
    private const string PrefabExtension = ".prefab";
    private const string BasePath = "Prefabs/UI";
    private const string SvnFolder = ".svn";

    private string absolutePath;

    #region AbstractPathLayerMapping

    public override bool Load()
    {
        var folderNameList =
            Directory.GetDirectories(absolutePath)
                .Select(folder => new FileInfo(folder).Name)
                .ToList();
        folderNameList.Remove(SvnFolder);

        var layerList = folderNameList.Select(folderName => LayerMask.NameToLayer(folderName)).Where(layer => layer != Utils.Invalid).ToList();
        if (folderNameList.Count != layerList.Count())
        {
            Debug.LogError("Subfolder in path - " + absolutePath +
                           " does not match layer exactly from layer manager. Folder count - " + folderNameList.Count +
                           ", layer count - " + layerList.Count);
            return false;
        }

        PathLayerMap.Clear();
        LayerPathMap.Clear();
        for (var i = 0; i < folderNameList.Count; ++i)
        {
            var path = string.Format("{0}/{1}", absolutePath, folderNameList[i]);
            var prefabList =
                Directory.GetFiles(path)
                    .Select(file => new FileInfo(file))
                    .Where(fileInfor => fileInfor.Extension.Equals(PrefabExtension))
                    .Select(fileInfor => fileInfor.Name.Remove(fileInfor.Name.IndexOf(fileInfor.Extension, StringComparison.Ordinal)))
                    .ToList();
            foreach (var prefabPath in prefabList.Select(prefabName => string.Format("{0}/{1}/{2}", BasePath, folderNameList[i], prefabName)))
            {
                var typeName = GetNameFromPath(prefabPath);
                var windowType = (WindowType)Enum.Parse(typeof(WindowType), typeName);
                PathTypeMap[prefabPath] = windowType;
                TypePathMap[windowType] = prefabPath;

                var windowGroupType = (WindowGroupType)layerList[i];
                PathLayerMap[prefabPath] = windowGroupType;
                if (!LayerPathMap.ContainsKey(windowGroupType))
                {
                    LayerPathMap[windowGroupType] = new List<string>();
                }
                LayerPathMap[windowGroupType].Add(prefabPath);
            }
        }

        Display();
        return true;
    }

    #endregion

    /// <summary>
    /// Get file or folder name from a path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>File or folder name</returns>
    private static string GetNameFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("path should not be null or empty.");
            return string.Empty;
        }

        if (path[path.Length - 1] == '/')
        {
            path.Remove(path.Length - 1);
        }
        return path.Substring(path.LastIndexOf('/') + 1);
    }

    private void Display()
    {
        foreach (var pair in PathLayerMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }

        foreach (var pair in LayerPathMap)
        {
            foreach (var value in pair.Value)
            {
                Debug.Log("pair: key-" + pair.Key + ", value-" + value);
            }
        }

        foreach (var pair in PathTypeMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }

        foreach (var pair in TypePathMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }
    }

    #region Mono

    void Awake()
    {
        PathLayerMap = new Dictionary<string, WindowGroupType>();
        LayerPathMap = new Dictionary<WindowGroupType, List<string>>();
        TypePathMap = new Dictionary<WindowType, string>();
        PathTypeMap = new Dictionary<string, WindowType>();

        absolutePath = string.Format("{0}/Game/Resources/{1}", Application.dataPath, BasePath);
        if (!Directory.Exists(absolutePath))
        {
            Debug.LogError("Window base path - " + absolutePath + " does not exist, please make sure we have this kind of folder structure to work properly.");
            return;
        }

        Load();
    }

    #endregion
}

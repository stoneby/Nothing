
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Window configurator
/// </summary>
/// <remarks>
/// Write essential informations to xml, including:
/// - Window path and layer configuration.
/// </remarks>
public class WindnowMappingXmlGenerator
{
    #region Private Fields

    private string absolutePath = string.Format("{0}/Game/Resources/{1}", Application.dataPath, Utils.UIBasePath);
    private List<string> folderNameList = new List<string>();

    private Dictionary<string, List<string>> prefabDict = new Dictionary<string, List<string>>();

    private const string SvnFolder = ".svn";
    private const string WindowMapPath = "Game/Resources/Config/WindowMap.xml";

    #endregion

    #region Private Methods

    private void GetFilePath()
    {
        if (string.IsNullOrEmpty(absolutePath))
        {
            absolutePath = string.Format("{0}/Game/Resources/{1}", Application.dataPath, Utils.UIBasePath);
        }
    }

    private bool IsValid()
    {
        folderNameList =
            Directory.GetDirectories(absolutePath)
                .Select(folder => new FileInfo(folder).Name)
                .ToList();
        folderNameList.Remove(SvnFolder);

        var windowGroupCount = Enum.GetNames(typeof(WindowGroupType)).Length;
        if (folderNameList.Count != windowGroupCount)
        {
            Logger.LogError("Subfolder in path - " + absolutePath +
                           " does not match window group type exactly from WindowGroupType class. Folder count - " + folderNameList.Count +
                           ", window group count - " + windowGroupCount);
            return false;
        }
        return true;
    }

    #endregion

    #region Public Methods

    public bool GenerateMappingFile()
    {
        if (!IsValid())
        {
            return false;
        }

        prefabDict.Clear();
        foreach (var folder in folderNameList)
        {
            prefabDict[folder] = new List<string>();
            var path = string.Format("{0}/{1}", absolutePath, folder);
            var prefabList =
                Directory.GetFiles(path)
                    .Select(file => new FileInfo(file))
                    .Where(fileInfor => fileInfor.Extension.Equals(Utils.PrefabExtension))
                    .Select(fileInfor => fileInfor.Name.Remove(fileInfor.Name.IndexOf(fileInfor.Extension, StringComparison.Ordinal)))
                    .ToList();
            foreach (var prefabPath in prefabList.Select(prefabName => string.Format("{0}/{1}/{2}", Utils.UIBasePath, folder, prefabName)))
            {
                prefabDict[folder].Add(prefabPath);
            }
        }

        var filePath = Path.Combine(Application.dataPath, WindowMapPath);
        AutoPathLayerMapping.WriteWindowMapToXml(prefabDict, filePath);

        return true;
    }

    #endregion
}

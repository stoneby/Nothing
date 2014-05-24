using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Window configurator
/// </summary>
/// <remarks>
/// Write essential informations to xml, including:
/// - Window path and layer configuration.
/// </remarks>
public class WindowMappingXmlGeneratorWindow : EditorWindow
{
    #region Private Fields

    private string absolutePath;
    private List<string> folderNameList = new List<string>();

    private Dictionary<string, List<string>> prefabDict = new Dictionary<string, List<string>>();

    private const string SvnFolder = ".svn";
    private const string WindowMapPath = "Game/Resources/Config/WindowMap.xml";

    private WindnowMappingXmlGenerator xmlGenerator = new WindnowMappingXmlGenerator();

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

        var windowGroupTypeCount = Enum.GetNames(typeof(WindowGroupType)).Length;
        if (folderNameList.Count != windowGroupTypeCount)
        {
            Logger.LogError("Subfolder in path - " + absolutePath +
                           " does not match window group types from WindowGroupType class. Folder count - " + folderNameList.Count +
                           ", window group count - " + windowGroupTypeCount);
            return false;
        }
        return true;
    }

    private void GenerateMappingFile()
    {
        if (!IsValid())
        {
            return;
        }

        GetFilePath();
        
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
        AssetDatabase.Refresh();
    }

    #endregion

    #region Mono

    void OnGUI()
    {
        GUILayout.TextArea(
            "UserManual: This tool to generator window path layer mapping to path - " + WindowMapPath,
            "Label");

        if (GUILayout.Button("Generate"))
        {
            xmlGenerator.GenerateMappingFile();
            AssetDatabase.Refresh();
        }
    }

    #endregion
}

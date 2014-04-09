﻿using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// Utilitity class used for helper functions and constances.
/// </summary>
public class Utils
{
    /// <summary>
    /// Invalid value.
    /// </summary>
    public const int Invalid = -1;

    /// <summary>
    /// Screen Layer from layer manager.
    /// </summary>
    /// <remarks>
    /// It's important that Screen locates at 12 from layer manager
    /// Also make sure the sequence is Screen->TabPanel->Popup->Face.
    /// </remarks>
    public const int ScreenLayer = 12;

    /// <summary>
    /// Prefab extension.
    /// </summary>
    public const string PrefabExtension = ".prefab";

    /// <summary>
    /// Script extension.
    /// </summary>
    public const string ScriptExtension = ".cs";

    /// <summary>
    /// UI resouces base path.
    /// </summary>
    public const string UIBasePath = "Prefabs/UI";

    public const string WindowMapName = "WindowMap.xml";

    public static string WindowMapPath = string.Format("{0}/{1}", Application.streamingAssetsPath, WindowMapName);

    /// <summary>
    /// Get file or folder name from a path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>File or folder name</returns>
    public static string GetNameFromPath(string path)
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

    /// <summary>
    /// Window prefab name to window script name.
    /// </summary>
    /// <param name="prefabName">Prefab name</param>
    /// <returns>Window script name</returns>
    /// <remarks>
    /// For example, Battle.prefab to BattleWindow.cs.
    /// </remarks>
    public static string PrefabNameToWindow(string prefabName)
    {
        return string.Format("{0}Window", prefabName);
    }

    /// <summary>
    /// Window script name to window prefab name.
    /// </summary>
    /// <param name="windowName">Window script name</param>
    /// <returns>Prefab name</returns>
    /// <remarks>
    /// For example, BattleWindow.cs to Battle.prefab.
    /// </remarks>
    public static string WindowNameToPrefab(string windowName)
    {
        return windowName.Replace("Window", string.Empty);
    }
    public static void WriteWindowMapToXml(Dictionary<string, List<string>> prefabDict)
    {
        var doc = new XmlDocument();
        var root = doc.CreateElement("Root");
        doc.AppendChild(root);
        foreach (var pair in prefabDict)
        {
            var element = doc.CreateElement("Group");
            element.SetAttribute("name", pair.Key);
            foreach (var path in pair.Value)
            {
                var subElement = doc.CreateElement("Path");
                subElement.InnerText = path;
                element.AppendChild(subElement);
            }
            root.AppendChild(element);
        }
        doc.Save(WindowMapPath);

        Debug.Log("Save window map file to " + WindowMapPath);
    }

    public static Dictionary<string, List<string>> ReadWindowMapFromXml()
    {
        var dict = new Dictionary<string, List<string>>();
        var doc = new XmlDocument();
        try
        {
            doc.Load(Utils.WindowMapPath);
            var root = doc.DocumentElement;
            foreach (XmlElement element in root.ChildNodes)
            {
                var name = element.Attributes[0].Value;
                dict[name] = new List<string>();
                foreach (XmlElement subElement in element.ChildNodes)
                {
                    dict[name].Add(subElement.InnerText);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Load window map xml fail - " + WindowMapPath + " " + e.Message);
            throw;
        }
        return dict;
    }
}

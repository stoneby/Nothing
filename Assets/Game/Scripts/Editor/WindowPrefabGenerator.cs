﻿using System;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// Window prefab generator tool.
/// </summary>
public class WindowPrefabGenerator : EditorWindow
{
    #region Private Fields

    private string prefabName;
    private WindowGroupType windowGroup = WindowGroupType.Screen;
    private GameObject prefabGameObject;

    private string userManual =
        "User manual: Please fill in prefab name and choose a window group, then click generate button.";

    private string log = string.Empty;

    private static readonly string UIBasePathByAsset = string.Format("Assets/Game/Resources/{0}", Utils.UIBasePath);
    private static readonly string UIBasePathBySystem = string.Format("{0}/Game/Resources/{1}", Application.dataPath, Utils.UIBasePath);
    private static readonly string UIScriptBasePath = string.Format("{0}/Game/Scripts/UI", Application.dataPath);

    private const string TemplateWindow = "TemplateWindow";

    private bool buttonPressed;
    private GameObject generatedPrefab;

    #endregion

    #region Private Methods

    private void GeneratePrefab()
    {
        if (NullCheck())
        {
            return;
        }

        // create empty prefab need a unique file name.
        if (PrefabExistedCheck())
        {
            return;
        }

        GenerateWindowScript();

        var prefabPath = string.Format("{0}/{1}/{2}{3}", UIBasePathByAsset, windowGroup, prefabName, Utils.PrefabExtension);
        // create prefab.
        generatedPrefab = PrefabUtility.CreatePrefab(prefabPath, prefabGameObject, ReplacePrefabOptions.ReplaceNameBased);

        log =
            string.Format("Generate prefab - {0}, to path - {1}, with game object in scene - {2}", prefabName,
                string.Format("{0}/{1}", Utils.UIBasePath, prefabName), prefabGameObject.name);
    }

    private bool PrefabExistedCheck()
    {
        var prefabPathEnum =
            Directory.GetFiles(UIBasePathBySystem, "*.prefab", SearchOption.AllDirectories)
                .Where(file => file.Contains(prefabName));
        var prefabPaths = prefabPathEnum as string[] ?? prefabPathEnum.ToArray();
        var prefabExisted = prefabPaths.Any();
        if (prefabExisted)
        {
            var prefabPath = prefabPaths.ElementAt(0);
            log = string.Format("Prefab with name - {0} already existed in path - {1}, please try another name.",
                prefabName, prefabPath);
            return true;
        }
        return false;
    }

    private bool NullCheck()
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            log = string.Format("Prefab name should not be null.");
            return true;
        }

        if (prefabGameObject == null)
        {
            log =
                string.Format(
                    "Game object should not be null, please select the game object you want to attach to prefab.");
            return true;
        }
        return false;
    }

    private void GenerateWindowScript()
    {
        var templateWindowPath = string.Format("{0}/Game/Scripts/Template/{1}{2}", Application.dataPath, TemplateWindow, Utils.ScriptExtension);
        var specificWindowName = Utils.PrefabNameToWindow(prefabName);
        var generatedFilePath = string.Format("{0}/{1}/{2}{3}", UIScriptBasePath, windowGroup, specificWindowName, Utils.ScriptExtension);

        Debug.Log("template path: " + templateWindowPath);
        Debug.Log("generated file path: " + generatedFilePath);

        var fullText = File.ReadAllText(templateWindowPath).Replace(TemplateWindow, specificWindowName);
        File.WriteAllText(generatedFilePath, fullText);

        AssetDatabase.Refresh();
    }

    #endregion

    #region Mono

    void OnGUI()
    {
        userManual = GUILayout.TextArea(userManual, "Label");
        
        EditorGUILayout.Space();

        prefabName = EditorGUILayout.TextField("Name: ", prefabName);
        windowGroup = (WindowGroupType)EditorGUILayout.EnumPopup("Window Group: ", windowGroup);
        const bool allowSceneObjects = true;
        prefabGameObject = EditorGUILayout.ObjectField("Game Object: ", prefabGameObject, typeof(GameObject), allowSceneObjects) as GameObject;

        if (GUILayout.Button("Generator Prefab"))
        {
            buttonPressed = true;

            GeneratePrefab();
        }

        if (buttonPressed && !EditorApplication.isCompiling)
        {
            buttonPressed = false;

            var specificWindowName = string.Format("{0}Window", prefabName);
            generatedPrefab.AddComponent(specificWindowName);
        }

        EditorGUILayout.Space();

        log = GUILayout.TextArea(log, "Label");
    }

    #endregion
}

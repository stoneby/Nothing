using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterGeneratorWindow : EditorWindow
{
    private GameObject templatePrefab;

    private bool toggle;

    private const string TemplatePath = "Assets/Game/Arts/Prefabs/NewBattle/CharacterTemplate";

    #region Single Maker

    private GameObject characterPrefab;
    private string prefabName;

    private const string SingleOutputPath = "Assets/Game/Arts/Prefabs/NewBattle/Character";

    #endregion

    #region Group Maker

    private TextAsset characterText;
    private Dictionary<string, int> characterIndexMap = new Dictionary<string, int>();

    private static readonly string GroupInputPathBase = string.Format("{0}/Development/HeroTest/Character/character_rename", Application.dataPath);
    private const string CharacterDataPath = "Assets/Game/Scripts/Editor/Tool/CharacterData.txt";
    private const string GroupOutputPath = "Assets/Game/Resources/AssetBundles/Prefabs/NewBattle/Character";

    #endregion

    private void GeneratePrefab()
    {
        if (templatePrefab == null || characterPrefab == null)
        {
            ShowNotification(new GUIContent("Prefab should not be null."));
            return;
        }

        if (string.IsNullOrEmpty(prefabName))
        {
            ShowNotification(new GUIContent("Prefab name should not be empty."));
            return;
        }

        var path = string.Format("{0}/{1}{2}", SingleOutputPath, prefabName, Utils.PrefabExtension);
        GeneratePrefab(characterPrefab, path);
    }

    private void GeneratePrefab(GameObject prefab, string outputPath)
    {
        var templateObject = Instantiate(templatePrefab) as GameObject;
        var locationTrans = templateObject.transform.GetChild(0);

        var characterObject = Instantiate(prefab) as GameObject;
        characterObject.transform.parent = locationTrans;
        characterObject.transform.localPosition = Vector3.zero;
        characterObject.transform.localScale = Vector3.one;
        characterObject.transform.localEulerAngles = Vector3.zero;

        PrefabUtility.CreatePrefab(outputPath, templateObject, ReplacePrefabOptions.ReplaceNameBased);

        DestroyImmediate(templateObject);
        DestroyImmediate(characterObject);

        ShowNotification(new GUIContent("Character prefab generate complete."));
    }

    private void OnGUI()
    {
        GUI.enabled = false;
        templatePrefab = EditorGUILayout.ObjectField("Template Prefab: ", templatePrefab, typeof(GameObject), true) as GameObject;
        GUI.enabled = true;

        toggle = EditorGUILayout.BeginToggleGroup("Single Output", toggle);
        DrawSingleOutput();
        EditorGUILayout.EndToggleGroup();

        toggle = !EditorGUILayout.BeginToggleGroup("Group Output", !toggle);
        DrawGroupOutput();
        EditorGUILayout.EndToggleGroup();
    }

    private void DrawSingleOutput()
    {
        templatePrefab =
            AssetDatabase.LoadMainAssetAtPath(string.Format("{0}{1}", TemplatePath, Utils.PrefabExtension)) as GameObject;

        characterPrefab =
            EditorGUILayout.ObjectField("Character Prefab: ", characterPrefab, typeof(GameObject), true) as GameObject;
        prefabName = EditorGUILayout.TextField("Name: ", prefabName);

        if (GUILayout.Button("Generate"))
        {
            GeneratePrefab();
        }
    }

    private void DrawGroupOutput()
    {
        GUI.enabled = false;
        characterText = EditorGUILayout.ObjectField("Character Text:", characterText, typeof(TextAsset), true) as TextAsset;
        characterText = AssetDatabase.LoadMainAssetAtPath(CharacterDataPath) as TextAsset;
        GUI.enabled = true;

        if (GUILayout.Button("Generate"))
        {
            GenerateCharacterMap();

            GenerateGroupPrefabs();
        }
    }

    private void GenerateGroupPrefabs()
    {
        var assetPaths =
            Directory.GetDirectories(GroupInputPathBase)
                .Where(
                    path => File.Exists(string.Format("{0}/{1}{2}", path, new FileInfo(path).Name, Utils.PrefabExtension)))
                .Select(
                    path =>
                        string.Format("{0}/{1}{2}", path.Replace(Application.dataPath, "Assets"), new FileInfo(path).Name,
                            Utils.PrefabExtension));

        Debug.LogWarning("Total character count: " + assetPaths.Count());

        foreach (var path in assetPaths)
        {
            var go = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
            int index;
            characterIndexMap.TryGetValue(go.name, out index);
            // make character name as three bit. say "001" to "999".
            var goName = index.ToString("D3");
            var filePath = string.Format("{0}/{1}{2}", GroupOutputPath, goName, Utils.PrefabExtension);

            GeneratePrefab(go, filePath);
        }
    }

    private void GenerateCharacterMap()
    {
        var lines = characterText.text.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        // 1 based character index map.
        for (var i = 0; i < lines.Count(); ++i)
        {
            characterIndexMap[lines[i]] = (i + 1);
        }
    }
}

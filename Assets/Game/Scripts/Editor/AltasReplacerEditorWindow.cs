using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AltasReplacerEditorWindow : EditorWindow
{
    private UIAtlas targetAtlas;
    private bool findSprites;
    private bool repaceAtlas;
    private bool startFind;
    private Vector2 scrollPos;
    private List<GameObject> sourcePrefabs = new List<GameObject>();
    private List<UISprite> sourceSprites = new List<UISprite>();
    private List<UISprite> allSprites;
    private readonly List<UIAtlas> sourceAtlas = new List<UIAtlas>();
    private readonly Dictionary<GameObject, bool> activeStatus = new Dictionary<GameObject, bool>();

    private const string PrefabExt = ".prefab";

    private static readonly List<string> PathFilterList = new List<string>
    {
        "NGUI",
        "Development",
        "SmartLocalization"
    };

    private static void AddWindow()
    {
        var wr = new Rect(0, 0, 300, 300);
        var window =
            (AltasReplacerEditorWindow)GetWindowWithRect(typeof(AltasReplacerEditorWindow), wr, true, "Altas Replacer");
        window.Show();
    }

    private void OnEnable()
    {
        FindAllSprites();
    }

    private void OnDestory()
    {
        FindWidget.RestoreActiveStatus(activeStatus);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Resources.UnloadUnusedAssets();
    }

    private void FindAllSprites()
    {
        Logger.LogWarning("Find all prefabs begins.");
        var prefabList = FindWidget.FindAllPrefabs(PrefabExt, PathFilterList);
        Logger.LogWarning("Find all prefabs ends, count: " + prefabList.Count);

        Logger.LogWarning("Load all prefabs begins.");
        FindWidget.LoadAllPrefabs(prefabList, activeStatus);
        Logger.LogWarning("Load all prefabs ends.");

        Logger.LogWarning("Active all prefabs begins.");
        FindWidget.ActiveAllPrefabs(activeStatus);
        Debug.LogWarning("Active all prefabs ends.");

        allSprites = FindWidget.FindAllWidgets<UISprite>();
    }

    private void OnGUI()
    {
        if (!findSprites && !repaceAtlas)
        {
            GUILayout.Space(10f);
            if (GUILayout.Button("查找使用某些Atlas的所有精灵"))
            {
                findSprites = true;
            }
            GUILayout.Space(10f);
            if (GUILayout.Button("用某个Atlas替换某些Atlas"))
            {
                repaceAtlas = true;
            }
        }
        else
        {
            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Find What： ");
            var isDirty = Field(sourceAtlas);
            if (isDirty)
            {
                if (allSprites != null)
                {
                    sourceSprites = allSprites.FindAll(sprite => sourceAtlas.Contains(sprite.atlas)).ToList();
                    sourcePrefabs = sourceSprites.Select(item =>
                    {
                        var rootObject = item.transform.root.gameObject;
                        var assetPath = AssetDatabase.GetAssetPath(rootObject);
                        if (String.IsNullOrEmpty(assetPath))
                        {
                            return item.gameObject;
                        }
                        return rootObject;
                    }).Distinct().ToList();
                }
            }

            if (findSprites)
            {
                ShowFindSprites(sourcePrefabs);
            }

            if (repaceAtlas)
            {
                ShowReplaceAltas(sourceSprites);
            }
        }
    }

    private void ShowFindSprites(IList<GameObject> sprites)
    {
        if (startFind)
        {
            if (sprites == null || sprites.Count == 0)
            {
                ShowNotification(new GUIContent("没有查找到精灵！"));
                return;
            }
            EditorGUILayout.LabelField("此Atlas所有相关联的精灵有：");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            var guiEnabled = GUI.enabled;
            GUI.enabled = false;
            foreach (var sourceSprite in sprites)
            {
                EditorGUILayout.ObjectField("", sourceSprite, typeof(object), true);
            }
            GUI.enabled = guiEnabled;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        GUILayout.Space(10f);
        if (GUILayout.Button("Start Find"))
        {
            startFind = true;
        }
        GUILayout.Space(10f);
        if (GUILayout.Button("Back"))
        {
            findSprites = false;
            startFind = false;
            sourceAtlas.Clear();
            sourceSprites.Clear();
            sourcePrefabs.Clear();
        }
    }

    private void ShowReplaceAltas(IList<UISprite> sprites)
    {
        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Replace With： ");
        targetAtlas = (UIAtlas)EditorGUILayout.ObjectField("", targetAtlas, typeof(UIAtlas), true);

        GUILayout.Space(10f);
        if (GUILayout.Button("Start Replace"))
        {
            ExcuteReplace(sprites, targetAtlas);
        }
        GUILayout.Space(10f);
        if (GUILayout.Button("Back"))
        {
            repaceAtlas = false;
            sourceAtlas.Clear();
            targetAtlas = null;
        }
    }

    private void ExcuteReplace(IList<UISprite> sprites, UIAtlas target)
    {
        if (sprites == null || sprites.Count == 0)
        {
            ShowNotification(new GUIContent("没有要替换的精灵，替换失败！"));
            return;
        }
        if (target != null || EditorUtility.DisplayDialog("目标atlas为空",
                                                         "目标atlas为空，确定要替换吗? ", "Replace", "Do Not Replace"))
        {
            foreach (var sourceSprite in sprites)
            {
                sourceSprite.atlas = target;
                ShowNotification(new GUIContent("Altas 替换成功！"));
            }
        }
    }

    private bool Field(List<UIAtlas> list)
    {
        var isDirty = false;
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (item == null)
            {
                list.RemoveAt(i);
                isDirty = true;
                continue;
            }
            var temp = Field(item);
            if (temp == null)
            {
                list.RemoveAt(i);
                isDirty = true;
                continue;
            }
            list[i] = temp;
            isDirty = true;
            ++i;
        }
        list.Add(null);
        list[list.Count - 1] = Field(list[list.Count - 1]);

        if (list[list.Count - 1] != null)
        {
            isDirty = true;
        }
        return isDirty;
    }

    private UIAtlas Field(UIAtlas item)
    {
        return (UIAtlas)EditorGUILayout.ObjectField("", item, typeof(UIAtlas), true);
    }
}
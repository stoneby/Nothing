using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwitchFontEditorWindow : EditorWindow
{
    #region Private Fields

    private enum FontType
    {
        Unity,
        NGUI
    }

    private enum Overflow
    {
        ShrinkContent,
        ClampContent,
        ResizeFreely,
        ResizeHeight,
    }

    private enum Crisp
    {
        Always,
        Never,
        OnDesktop
    }

    private const string PrefabExt = ".prefab";

    private static readonly List<string> PathFilterList = new List<string>
    {
        "NGUI",
        "Development",
        "SmartLocalization"
    };

    private string userManual = "Please select the font.";

    private FontType fontType;

    private Overflow overflow;
    private Crisp crisp;

    private UIFont nguiFont;
    private Font unityFont;

    #endregion

    #region Private Methods

    /// <summary>
    /// Apply font the whole process.
    /// </summary>
    private void ApplyFont()
    {
        Debug.LogWarning("Find all prefabs begins.");
        var prefabList = FindWidget.FindAllPrefabs(PrefabExt, PathFilterList);
        Debug.LogWarning("Find all prefabs ends, count: " + prefabList.Count);

        Debug.LogWarning("Load all prefabs begins.");
        var activeStatus = new Dictionary<GameObject, bool>();
        FindWidget.LoadAllPrefabs(prefabList, activeStatus);
        Debug.LogWarning("Load all prefabs ends.");

        Debug.LogWarning("Active all prefabs begins.");
        FindWidget.ActiveAllPrefabs(activeStatus);
        Debug.LogWarning("Active all prefabs ends.");

        Debug.LogWarning("Find all labels begins.");
        var labelList = FindWidget.FindAllWidgets<UILabel>();
        FindWidget.DisplayWidgets<UILabel>(labelList);
        Debug.LogWarning("Find all labels ends, count: " + labelList.Count);

        Debug.LogWarning("Apply font settings begins.");
        ApplyFontSettings(labelList);
        Debug.LogWarning("Apply font settings ends.");

        Debug.LogWarning("Restore active status begins.");
        FindWidget.RestoreActiveStatus(activeStatus);
        Debug.LogWarning("Restore active status ends.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ApplyFontSettings(IEnumerable<UILabel> labelList)
    {
        foreach (var item in labelList)
        {
            switch (fontType)
            {
                case FontType.Unity:
                    item.trueTypeFont = unityFont;
                    break;
                case FontType.NGUI:
                    item.bitmapFont = nguiFont;
                    switch (overflow)
                    {
                        case Overflow.ShrinkContent:
                            item.overflowMethod = UILabel.Overflow.ShrinkContent;
                            break;
                        case Overflow.ClampContent:
                            item.overflowMethod = UILabel.Overflow.ClampContent;
                            break;
                        case Overflow.ResizeFreely:
                            item.overflowMethod = UILabel.Overflow.ResizeFreely;
                            break;
                        case Overflow.ResizeHeight:
                            item.overflowMethod = UILabel.Overflow.ResizeHeight;
                            break;
                    }
                    switch (crisp)
                    {
                        case Crisp.Always:
                            item.keepCrispWhenShrunk = UILabel.Crispness.Always;
                            break;
                        case Crisp.Never:
                            item.keepCrispWhenShrunk = UILabel.Crispness.Never;
                            break;
                        case Crisp.OnDesktop:
                            item.keepCrispWhenShrunk = UILabel.Crispness.OnDesktop;
                            break;
                    }
                    break;
            }
        }
    }

    private bool Validate()
    {
        if (fontType == FontType.Unity && unityFont == null)
        {
            return false;
        }

        if (fontType == FontType.NGUI && nguiFont == null)
        {
            return false;
        }
        return true;
    }

    #endregion

    #region Mono

    private void OnGUI()
    {
        userManual = GUILayout.TextArea(userManual, "Label");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("FontType:");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        fontType = (FontType)EditorGUILayout.EnumPopup(fontType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (fontType == FontType.Unity)
        {
            unityFont = (Font)EditorGUILayout.ObjectField("UnityFont", unityFont, typeof(Font), false);
        }

        if (fontType == FontType.NGUI)
        {

            var fontObject = EditorGUILayout.ObjectField("NGUIFont", nguiFont, typeof(GameObject), false) as GameObject;
            if (fontObject != null)
            {
                nguiFont = fontObject.GetComponent<UIFont>();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Overflow:");
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            overflow = (Overflow)EditorGUILayout.EnumPopup(overflow);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Crisp:");
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            crisp = (Crisp)EditorGUILayout.EnumPopup(crisp);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        var validated = Validate();
        GUI.enabled = validated;
        if (GUILayout.Button("ApplyFont"))
        {
            ApplyFont();
        }
        GUI.enabled = true;
    }

    #endregion
}


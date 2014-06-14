using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwitchFontWizard : EditorWindow
{
    #region Private Fields

    private string usermanual = "Please select the font.";

    private enum FontType
    {
        unity,
        NGUI
    }
    private enum Crisp
    {
        Always,
        Never,
        OnDesktop
    }

    private FontType fontType;

    private Crisp crisp;

    private UIFont nguiFont;
    private Font unityFont;

    #endregion

    #region Private Methods

    //Apply choosed font.
    private void ApplyFont()
    {
        if (fontType == FontType.unity && unityFont == null)
        {
            return;
        }
        if (fontType == FontType.NGUI && nguiFont == null)
        {
            return;
        }
        string[] paths = AssetDatabase.GetAllAssetPaths();
        foreach (string path in paths)
        {
            GameObject path_gameobject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (path_gameobject != null)
            {
                ActiveAllObject(path_gameobject, true);
                List<UILabel> temp_uilabel = new List<UILabel>();
                FindUILabelInAllObject(path_gameobject, temp_uilabel);
                if (temp_uilabel != null)
                {
                    foreach (UILabel item in temp_uilabel)
                    {
                        if (fontType == FontType.unity)
                        {
                            item.trueTypeFont = unityFont;
                        }
                        else if (fontType == FontType.NGUI)
                        {
                            item.bitmapFont = nguiFont;
                            item.applyGradient = false;

                            switch(crisp)
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
                        }

                        Debug.Log("SwitchFont:" + path.ToString());
                    }
                }
            }
        }
    }

    //Find UILabel Components in parent and any child of parent.
    private void FindUILabelInAllObject(GameObject parent, List<UILabel> output)
    {
        UILabel[] result;

        result = parent.GetComponents<UILabel>();
        output.AddRange(result);
        if (parent.transform.childCount != 0)
        {
            int childCount = parent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                FindUILabelInAllObject(parent.transform.GetChild(i).gameObject, output);
            }
        }
    }

    //Active/Deactive parent and any child of parent.
    private void ActiveAllObject(GameObject parent, bool value)
    {
        parent.SetActive(value);
        if (parent.transform.childCount != 0)
        {
            int childCount = parent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                ActiveAllObject(parent.transform.GetChild(i).gameObject, value);
            }
        }
    }

    #endregion

    #region Mono

    void OnGUI()
    {
        usermanual = GUILayout.TextArea(usermanual, "Label");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("FontType:");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        fontType = (FontType)EditorGUILayout.EnumPopup(fontType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (fontType == FontType.unity)
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

            EditorGUILayout.LabelField("Crisp:");
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            crisp = (Crisp)EditorGUILayout.EnumPopup(crisp);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("ApplyFont"))
        {
            ApplyFont();
        }
    }

    #endregion
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class LocalizationManagerEditorWindow : EditorWindow {

    #region Private Fields

    private const string PrefabExt = ".prefab";

    private bool isGenerate=false;

    private static readonly List<string> PathFilterList = new List<string>
    {
        "NGUI",
        "Development",
        "SmartLocalization"
    };

    private Dictionary<GameObject, bool> activeStatus= new Dictionary<GameObject, bool>();
    private List<UILabel> labelCollection = new List<UILabel>();

    private class localUnit : IComparable
    {
        public bool Confirm;
        public string Key;
        public string Value;
        public UILabel Label;
        public bool State;

        public int CompareTo(object obj)
        {
            return System.String.CompareOrdinal(Key, ((localUnit)obj).Key);
        }
    }

    //Restore and manage 
    private List<localUnit> localManager=new List<localUnit>();

    private bool isSet = false;

    #region Private Fields from EditRootLanguageFileWindow.cs
         
    /// <summary>Containing the original keys and the changes to them, if any.</summary>
    [SerializeField]
    private List<SerializableStringPair> changedRootKeys = new List<SerializableStringPair>();

    /// <summary>Containing the original values and any changes to them</summary>
    [SerializeField]
    private List<SerializableLocalizationObjectPair> changedRootValues = new List<SerializableLocalizationObjectPair>();

    /// <summary>The parsed root values. This is used to check root key duplicates</summary>
    [SerializeField]
    private Dictionary<string, LocalizedObject> parsedRootValues = new Dictionary<string, LocalizedObject>();

    #endregion

    private Vector2 scrollViewPos1;
    private Vector2 scrollViewPos2;

    #endregion

    #region Private Methods

    /// <summary>
    /// Apply font the whole process.
    /// </summary>
    private void GenerateFromObject()
    {
        Debug.LogWarning("Find all prefabs begins.");
        var prefabList = FindAllPrefabs();
        Debug.LogWarning("Find all prefabs ends, count: " + prefabList.Count);

        Debug.LogWarning("Load all prefabs begins.");
        activeStatus = new Dictionary<GameObject, bool>();
        LoadAllPrefabs(prefabList, activeStatus);
        Debug.LogWarning("Load all prefabs ends.");

        Debug.LogWarning("Active all prefabs begins.");
        ActiveAllPrefabs(activeStatus);
        Debug.LogWarning("Active all prefabs ends.");

        Debug.LogWarning("Find all labels begins.");
        labelCollection.Clear();
        labelCollection = FindAllLabels();
        DisplayLabels(labelCollection);
        Debug.LogWarning("Find all labels ends, count: " + labelCollection.Count);

        Debug.LogWarning("Generate localization begins.");
        GenerateLocal(labelCollection);
        isGenerate = true;
        isSet = false;
        Debug.LogWarning("Generate localization ends.");

        Debug.LogWarning("Sort localization begins.");
        localManager.Sort();
        Debug.LogWarning("Sort localization ends.");

        Debug.LogWarning("Restore active status begins.");
        RestoreActiveStatus(activeStatus);
        Debug.LogWarning("Restore active status ends.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SubmitFromObject()
    {
        Debug.LogWarning("Confirm localization begins.");
        ConfirmLocal();
        Debug.LogWarning("Confirm localization ends.");

        Debug.LogWarning("Update Smart localization begins.");
        UpdateSmartLocal();
        Debug.LogWarning("Update Smart localization ends.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void GenerateFromSmartLocal()
    {
        Debug.Log("Generate form Smart Localization start.");
        SetRootValues(LocFileUtility.LoadParsedLanguageFile(null));
        isSet = true;
        isGenerate = false;
        Debug.Log("Generate form Smart Localization end.");
    }

    private void SubmitFromSL_EXCEL()
    {
        Debug.Log("Submit from SL/EXCEL start.");
        SaveRootLanguageFile();
        Debug.Log("Submit from SL/EXCEL end.");
    }

    private void ImportFromExcel()
    {
        //OpenFileDialog fdlg = new OpenFileDialog();
        //fdlg.Title = "Select file";
        //fdlg.InitialDirectory = @"c:\";
        //fdlg.FileName = txtFileName.Text;
        //fdlg.Filter = "Excel Sheet(*.xls)|*.xls|All Files(*.*)|*.*";
        //fdlg.FilterIndex = 1;
        //fdlg.RestoreDirectory = true;
        //if (fdlg.ShowDialog() == DialogResult.OK)
        //{
        //    txtFileName.Text = fdlg.FileName;
        //    Import();
        //    Application.DoEvents();
        //}
    }

    private void ExportToExcel()
    {
        //// creating Excel Application
        //var app = new Microsoft.Office.Interop.Excel.Application();

        //// creating new WorkBook within Excel application
        //var workbook = app.Workbooks.Add(Type.Missing);

        //// see the excel sheet behind the program
        //app.Visible = true;

        //// get the reference of first sheet. By default its name is Sheet1.
        //// store its reference to worksheet
        //var worksheet = workbook.Sheets["Sheet1"] as Microsoft.Office.Interop.Excel.Worksheet;
        //worksheet = workbook.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

        //// changing the name of active sheet
        //worksheet.Name = "Exported from Localization Manager";

        //for (int i = 0; i < changedRootKeys.Count;i++)
        //{
        //    worksheet.Cells[i + 1, 1] = changedRootKeys[i].changedValue;
        //    worksheet.Cells[i + 1, 2] = changedRootValues[i].changedValue.TextValue;
        //}

        //// save the application
        //workbook.SaveAs(
        //    "c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing
        //    );

        //// Exit from the application
        //app.Quit();

    }

    /// <summary>
    /// Add LocalizeWidget script for every UILabel gameObject in the list.
    /// </summary>
    /// <param name="labelList"></param>
    private void GenerateLocal(IEnumerable<UILabel> labelList)
    {
        localManager.Clear();
        foreach (var item in labelList)
        {
            var localSetting = item.gameObject.GetComponent<LocalizeWidget>();
            if (localSetting != null)
            {
                var keyTemp = localSetting.Key;
                localManager.Add(new localUnit()
                {
                    Confirm = false,
                    Key = keyTemp,
                    Value = item.text,
                    Label = item,
                    State = true
                });
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(item);
                char[] splitChars = { '/', '.' };
                string[] splitPaths = path.Split(splitChars);
                if (splitPaths.Length < 2)
                {
                    Debug.LogError("UILabel path error!Stop operation and check assets.");
                }
                var keyTemp = splitPaths[splitPaths.Length - 2] + "." + item.gameObject.name;
                localManager.Add(new localUnit()
                {
                    Confirm = false,
                    Key = keyTemp,
                    Value = item.text,
                    Label = item,
                    State = false
                });
            }
        }
    }

    private void ConfirmLocal()
    {
        foreach (var item in localManager)
        {
            if (item.Label == null)
            {
                Debug.LogError("Fail in GenerateLocal!Operation has been stopped.");
                return;
            }
            if (item.Confirm == true)
            {
                item.Label.gameObject.AddComponent<LocalizeWidget>();
                item.Label.gameObject.GetComponent<LocalizeWidget>().Key = item.Key;
            }
        }
    }
    
    private void UpdateSmartLocal()
    {
        SetRootValues(LocFileUtility.LoadParsedLanguageFile(null));
        Debug.Log("Load localization file ends.");

        int count = 0;
        foreach (var item in localManager)
        {
            if (item.Confirm == true)
            {
                AddNewKey(item.Key, item.Value);
                count++;
            }
        }
        Debug.Log("Add new key ends, count:"+count+".");

        SaveRootLanguageFile();
        Debug.Log("");
    }

    #region Private Methods from EditRootLanguageFileWindow.cs

    private void SetRootValues(Dictionary<string, LocalizedObject> rootValues)
    {
        changedRootValues.Clear();
        changedRootKeys.Clear();
        parsedRootValues.Clear();

        foreach (KeyValuePair<string, LocalizedObject> rootValue in rootValues)
        {
            changedRootKeys.Add(new SerializableStringPair(rootValue.Key, rootValue.Key));
            changedRootValues.Add(new SerializableLocalizationObjectPair(rootValue.Key, rootValue.Value));

            LocalizedObject copyObject = new LocalizedObject();
            copyObject.ObjectType = rootValue.Value.ObjectType;
            copyObject.TextValue = rootValue.Value.TextValue;
            parsedRootValues.Add(rootValue.Key, copyObject);
        }
    }

    private void AddNewKey(string newKey, string newValue)
    {
        LocalizedObject dummyObject = new LocalizedObject();
        dummyObject.ObjectType = LocalizedObjectType.STRING;
        dummyObject.TextValue = newKey;

        string addedKey = LocFileUtility.AddNewKeyPersistent(parsedRootValues, newKey, dummyObject);

        LocalizedObject copyObject = new LocalizedObject();
        copyObject.ObjectType = LocalizedObjectType.STRING;
        copyObject.TextValue = newValue;
        changedRootKeys.Add(new SerializableStringPair(addedKey, addedKey));
        changedRootValues.Add(new SerializableLocalizationObjectPair(addedKey, copyObject));
    }

    private void DeleteKey(int index)
    {
        parsedRootValues.Remove(changedRootKeys[index].originalValue);
        changedRootKeys.RemoveAt(index);
        changedRootValues.RemoveAt(index);
    }

    private void SaveRootLanguageFile()
    {
        Dictionary<string, string> changeNewRootKeys = new Dictionary<string, string>();
        Dictionary<string, string> changeNewRootValues = new Dictionary<string, string>();

        for (int i = 0; i < changedRootKeys.Count; i++)
        {
            SerializableStringPair rootKey = changedRootKeys[i];
            SerializableLocalizationObjectPair rootValue = changedRootValues[i];
            //Check for possible duplicates and rename them
            string newKeyValue = LocFileUtility.AddNewKeyPersistent(changeNewRootKeys, rootKey.originalValue, rootValue.changedValue.GetFullKey(rootKey.changedValue));

            //Check for possible duplicates and rename them(same as above)
            LocFileUtility.AddNewKeyPersistent(changeNewRootValues, newKeyValue, rootValue.changedValue.TextValue);
        }

        //Add the full values before saving
        Dictionary<string, string> changeNewRootKeysToSave = new Dictionary<string, string>();
        Dictionary<string, string> changeNewRootValuesToSave = new Dictionary<string, string>();

        foreach (KeyValuePair<string, string> rootKey in changeNewRootKeys)
        {
            LocalizedObject thisLocalizedObject = parsedRootValues[rootKey.Key];
            changeNewRootKeysToSave.Add(thisLocalizedObject.GetFullKey(rootKey.Key), rootKey.Value);
            changeNewRootValuesToSave.Add(thisLocalizedObject.GetFullKey(rootKey.Key), changeNewRootValues[rootKey.Key]);
        }

        LocFileUtility.SaveRootLanguageFile(changeNewRootKeysToSave, changeNewRootValuesToSave);
    }

    #endregion

    #region Private Methods from SwitchFontEditorWindow.cs

    private static List<string> FindAllPrefabs()
    {
        var result = new List<string>();
        var paths = AssetDatabase.GetAllAssetPaths();
        foreach (var path in paths.Where(path => path.EndsWith(PrefabExt)))
        {
            var filtered = PathFilterList.Any(filter => path.Contains(filter));
            if (filtered)
            {
                continue;
            }
            result.Add(path);
            Debug.Log("Find prefab with path: " + path);
        }
        return result;
    }

    private static void LoadAllPrefabs(List<string> prefabList, IDictionary<GameObject, bool> activeStatus)
    {
        prefabList.ForEach(path =>
        {
            var prefabObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            activeStatus[prefabObject] = prefabObject.activeSelf;
        });
    }

    private static void ActiveAllPrefabs(IEnumerable<KeyValuePair<GameObject, bool>> activeStatus)
    {
        foreach (var pair in activeStatus.Where(pair => !pair.Value))
        {
            pair.Key.SetActive(true);
        }
    }

    private static List<UILabel> FindAllLabels()
    {
        return Resources.FindObjectsOfTypeAll<UILabel>().ToList();
    }

    private static void DisplayLabels(IEnumerable<UILabel> labelList)
    {
        foreach (var label in labelList)
        {
            var path = AssetDatabase.GetAssetPath(label);
            Debug.Log("Find label: " + label.gameObject.name + ", path: " + path);
        }
    }

    private static void RestoreActiveStatus(Dictionary<GameObject, bool> activeStatus)
    {
        foreach (var pair in activeStatus)
        {
            pair.Key.SetActive(pair.Value);
        }
    }

    #endregion

    #endregion

    #region Mono

    private void OnGUI()
    {

#region Object-SL function

        if (GUILayout.Button("Generate key,value from gameobject"))
        {
            GenerateFromObject();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Confirm and Update to Smart Localization"))
        {
            SubmitFromObject();
        }

        EditorGUILayout.Space();

        GUILayout.Label("You can add key,value to smart localization here.");

        EditorGUILayout.Space();

        if (isGenerate == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Key", GUILayout.Width(100));
            GUILayout.Label("Base Value/Comment");

            if (GUILayout.Button("Add all"))
            {
                foreach (var item in localManager)
                {
                    if (item.State == false)
                    {
                        item.Confirm = true;
                    }
                }
            }

            if (GUILayout.Button("De-add all"))
            {
                foreach (var item in localManager)
                {
                    if (item.State == false)
                    {
                        item.Confirm = false;
                    }
                }
            }

            GUILayout.Label("Check/State", EditorStyles.miniLabel, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            scrollViewPos1 = EditorGUILayout.BeginScrollView(scrollViewPos1);

            foreach(var item in localManager)
            {
                EditorGUILayout.BeginHorizontal();

                item.Key = EditorGUILayout.TextField(item.Key);
                item.Value = EditorGUILayout.TextField(item.Value);
                if (item.State == true)
                {
                    GUILayout.Label("Added", EditorStyles.miniLabel, GUILayout.Width(100));
                }
                else
                {
                    item.Confirm = GUILayout.Toggle(item.Confirm, "Add");
                }

                if (GUILayout.Button("Instantiate"))
                {
                    var path = AssetDatabase.GetAssetPath(item.Label);                   
                    var go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    Instantiate(go);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

#endregion

#region SL/Excel-SL function

        if (GUILayout.Button("Generate key,value from Smart Localization"))
        {
            GenerateFromSmartLocal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Submit key,value from Smart Localization/EXCEL"))
        {
            SubmitFromSL_EXCEL();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Import key,value from Excel"))
        {
            ImportFromExcel();
        }

        if (GUILayout.Button("Export key,value to Excel"))
        {
            ExportToExcel();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Label("You can edit or delete key,value to smart localization and import/export to Excel file here.");

        if (isSet == true)
        {
            scrollViewPos2 = EditorGUILayout.BeginScrollView(scrollViewPos2);

            for (int i = 0; i < changedRootKeys.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                //This part code is from EditRootLanguageFileWindow.cs, key type is set to string.

                changedRootKeys[i].changedValue = EditorGUILayout.TextField(changedRootKeys[i].changedValue);
                changedRootValues[i].changedValue.TextValue = EditorGUILayout.TextField(changedRootValues[i].changedValue.TextValue);
                if (GUILayout.Button("Delete", GUILayout.Width(50)))
                {
                    DeleteKey(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

#endregion

    }

    #endregion
}

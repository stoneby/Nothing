using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LocalizationManagerEditorWindow : EditorWindow
{
    #region Private Fields

    private const string PrefabExt = ".prefab";

    private bool isGenerate = false;

    private static readonly List<string> PathFilterList = new List<string>
    {
        "NGUI",
        "Development",
        "SmartLocalization"
    };

    private Dictionary<GameObject, bool> activeStatus = new Dictionary<GameObject, bool>();
    private List<UILabel> labelCollection = new List<UILabel>();

    private class LocalUnit : IComparable
    {
        public bool Confirm;
        public string Key;
        public string Value;
        public UILabel Label;
        public bool State;

        public int CompareTo(object obj)
        {
            return System.String.CompareOrdinal(Key, ((LocalUnit)obj).Key);
        }
    }

    //Restore and manage 
    private List<LocalUnit> localManager = new List<LocalUnit>();

    private bool isSet = false;

    #region Private Fields from SmartLocalizationWindow.cs

    /// <summary>A list of all the available languages </summary>
    private List<CultureInfo> availableLanguages = new List<CultureInfo>();
    /// <summary>A list of all the languages not available</summary>
    private List<CultureInfo> notAvailableLanguages = new List<CultureInfo>();
    /// <summary>For the create new languages popup</summary>
    private List<string> notAvailableLanguagesEnglishNames = new List<string>();

    #endregion

    #region Private Fields from TranslateLanguageWindow.cs

    [SerializeField]
    private Dictionary<string, List<SerializableLocalizationObjectPair>> thisLanguageValues = new Dictionary<string, List<SerializableLocalizationObjectPair>>();

    #endregion

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

    //File from XLS
    private List<List<string>> parsedXLS = new List<List<string>>();
    private const string ImportFileName = "/LocalizationImport.xls";
    private const string ExportFileName = "/LocalizationExport.xml";

    //Mono private fields
    private Vector2 scrollViewPos1;
    private Vector2 scrollViewPos2;

    #endregion

    #region Public Methods

    /// <summary>
    /// Transform value to display in game.
    /// Use '{' and '}' to split string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string LocalizateFormat(string value, params object[] args)
    {
        //Formatted string.
        string result = "";

        char[] splitCharsLeft = { '{' };
        char[] splitCharsRight = { '}' };

        //Split by '{'
        string[] splitLeft = value.Split(splitCharsLeft);
        //Fail check
        if (splitLeft.Length != args.Length + 1)
        {
            Debug.LogError("LocalizateFormatError: Value doesn't fit args, operation stopped, check the data!");
            return null;
        }

        //Split by '}'
        var tempSplit = splitLeft[0].Split(splitCharsRight);
        //Fail check
        if (tempSplit.Length != 1)
        {
            Debug.LogError("LocalizateFormatError: Value doesn't fit args, operation stopped, check the data!");
            return null;
        }
        result += tempSplit[0];

        for (int i = 1; i < splitLeft.Length; i++)
        {
            //Split by '}'
            tempSplit = splitLeft[i].Split(splitCharsRight);

            //Fail check
            if (tempSplit.Length != 2 || tempSplit[0] != (i - 1).ToString())
            {
                Debug.LogError("LocalizateFormatError: Value doesn't fit args, operation stopped, check the data!");
                return null;
            }
            result = result + args[i - 1].ToString() + tempSplit[1];
        }

        return result;
    }

    #endregion

    #region Private Methods

    private void GenerateFromObject()
    {
        Debug.LogWarning("Find all prefabs begins.");
        var prefabList = FindWidget.FindAllPrefabs(PrefabExt, PathFilterList);
        Debug.LogWarning("Find all prefabs ends, count: " + prefabList.Count);

        Debug.LogWarning("Load all prefabs begins.");
        activeStatus = new Dictionary<GameObject, bool>();
        FindWidget.LoadAllPrefabs(prefabList, activeStatus);
        Debug.LogWarning("Load all prefabs ends.");

        Debug.LogWarning("Active all prefabs begins.");
        FindWidget.ActiveAllPrefabs(activeStatus);
        Debug.LogWarning("Active all prefabs ends.");

        Debug.LogWarning("Find all labels begins.");
        labelCollection.Clear();
        labelCollection = FindWidget.FindAllWidgets<UILabel>();
        FindWidget.DisplayWidgets<UILabel>(labelCollection);
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
        FindWidget.RestoreActiveStatus(activeStatus);
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

        LocFileUtility.CheckAvailableLanguages(availableLanguages, notAvailableLanguages, notAvailableLanguagesEnglishNames);

        this.thisLanguageValues.Clear();
        foreach (var info in availableLanguages)
        {
            this.thisLanguageValues.Add(info.Name, LocFileUtility.CreateSerializableLocalizationList(LocFileUtility.LoadParsedLanguageFile(info.Name)));
            //Load assets
            LocFileUtility.LoadAllAssets(this.thisLanguageValues[info.Name]);
        }

        isSet = true;
        isGenerate = false;
        Debug.Log("Generate form Smart Localization end.");
        Debug.Log("Key count:" + changedRootKeys.Count + "Value count:" + changedRootValues.Count +
            "Parsed count:" + parsedRootValues.Count + "ParsedXLS count:" + parsedXLS.Count +
            availableLanguages[0].Name + " count:" + thisLanguageValues[availableLanguages[0].Name].Count + 
            availableLanguages[1].Name + " count:" + thisLanguageValues[availableLanguages[1].Name].Count);
    }

    private void SubmitFromSL_EXCEL()
    {
        Debug.Log("Submit from SL/EXCEL start.");
        SaveRootLanguageFile();
        Debug.Log("Submit from SL/EXCEL end.");
    }

    private void ImportFromExcel()
    {
        parsedXLS = FileIO.ReadXLS(Application.dataPath + ImportFileName);
        Debug.Log("ParsedXLS count:" + parsedXLS.Count);

        for (int row=1;row<parsedXLS.Count;row++)
        {
            Debug.Log("NewKey:" + parsedXLS[row][0]);
            bool result = false;
            for (int i = 0; i < changedRootKeys.Count; i++)
            {
                //Edit value of existed key.
                if (parsedXLS[row][0] == changedRootKeys[i].changedValue)
                {
                    if (changedRootValues[i].changedValue.TextValue != parsedXLS[row][1])
                    {
                        Debug.Log("++++++ Change key:" + parsedXLS[row][0] + ", value from:" + changedRootValues[i].changedValue.TextValue + ", to:" + parsedXLS[row][1]);
                        changedRootValues[i].changedValue.TextValue = parsedXLS[row][1];
                    }
                    else
                    {
                        Debug.Log("------ Same key:" + parsedXLS[row][0] + ", Same value :" + changedRootValues[i].changedValue.TextValue);
                    }
                    result = true;
                    break;
                }
            }
            
            //Add key,value of no-existed key.
            if (!result)
            {
                Debug.Log("********* Add key:" + parsedXLS[row][0] + ", value:" + parsedXLS[row][1]);
                AddNewKey(parsedXLS[row][0], parsedXLS[row][1]);
            }

            //Edit localizated language value
            for (int i = 2; i < parsedXLS[row].Count; i++)
            {
                thisLanguageValues[parsedXLS[0][i]][row-1].changedValue.TextValue = parsedXLS[row][i];
                Debug.Log("Name:" + parsedXLS[0][i] + "Key:" + parsedXLS[row][0] + "Base Value:" + parsedXLS[row][1] + "Value from:" + thisLanguageValues[parsedXLS[0][i]][row - 1].changedValue.TextValue+"to:"+ parsedXLS[row][i]);
            }
        }
    }

    private void ExportToExcel()
    {
        //StreamWriter writer;
        var parsedXML = new List<List<string>>();
        for (int i = 0; i < changedRootKeys.Count; i++)
        {
            var tempList = new List<string>();
            tempList.Add(changedRootKeys[i].changedValue);
            tempList.Add(changedRootValues[i].changedValue.TextValue);
            foreach (var info in availableLanguages)
            {
                tempList.Add(thisLanguageValues[info.Name][i].changedValue.TextValue);
            }
            parsedXML.Add(tempList);
        }

        var fileinfo = new FileInfo(Application.dataPath + ExportFileName);
        FileIO.WriteXML(fileinfo, parsedXML);
    }

    private void GenerateLocal(IEnumerable<UILabel> labelList)
    {
        localManager.Clear();
        foreach (var item in labelList)
        {
            var localSetting = item.gameObject.GetComponent<LocalizeWidget>();
            if (localSetting != null)
            {
                var keyTemp = localSetting.Key;
                localManager.Add(new LocalUnit()
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
                localManager.Add(new LocalUnit()
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
        Debug.Log("Add new key ends, count:" + count + ".");

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

    /// <summary>
    /// Save Key, Value and Localizated Value of parsed data.
    /// </summary>
    private void SaveRootLanguageFile()
    {
        //Save Key, Value
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

        //Save Localizated Value
        foreach (var pair in thisLanguageValues)
        {
            //Copy everything into a dictionary
            Dictionary<string, string> newLanguageValues = new Dictionary<string, string>();
            int languangCount = 0;
            foreach (SerializableLocalizationObjectPair objectPair in pair.Value)
            {
                if (objectPair.changedValue.ObjectType == LocalizedObjectType.STRING)
                {
                    newLanguageValues.Add(objectPair.changedValue.GetFullKey(objectPair.keyValue), objectPair.changedValue.TextValue);
                }
                else
                {
                    //Delete the file in case there was a file there previously
                    LocFileUtility.DeleteFileFromResources(objectPair.changedValue.GetFullKey(objectPair.keyValue), availableLanguages[languangCount]);

                    //Store the path to the file
                    string pathValue = LocFileUtility.CopyFileIntoResources(objectPair, availableLanguages[languangCount]);
                    newLanguageValues.Add(objectPair.changedValue.GetFullKey(objectPair.keyValue), pathValue);
                }
                languangCount++;
            }
            LocFileUtility.SaveLanguageFile(newLanguageValues, LocFileUtility.rootLanguageFilePath + "." + pair.Key + LocFileUtility.resXFileEnding);
        }
    }

    #endregion

    #endregion

    #region Mono

    private void OnGUI()
    {
        GUILayout.Label("You can use LocalizationManagerEditorWindow.LocalizateFormat(string value, params object[] args) to change your dynamic value to the string that display in game.");

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

            foreach (var item in localManager)
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

        if (isSet == true)
        {
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
        }

        GUILayout.Label("You can edit or delete key,value to smart localization and import/export to Excel file here.");

        EditorGUILayout.Space();

        GUILayout.Label("Import:Program will import data from Assets/LocalizationImport.xls. You can only edit values in this way.");
        GUILayout.Label("Export:Program will export data to Assets/LocalizationExport.xml.");

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

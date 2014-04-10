using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// Automatically map between path and layer.
/// </summary>
public class AutoPathLayerMapping : AbstractPathLayerMapping
{
    private string absolutePath;

    private Dictionary<string, List<string>> windowMapDict;

	private const string windowMapName = "WindowMap.xml";
	private static string windowMapPath = System.IO.Path.Combine(Application.streamingAssetsPath, windowMapName);

    #region AbstractPathLayerMapping

    public override bool Load()
    {
        PathLayerMap.Clear();
        LayerPathMap.Clear();

        foreach (var pair in windowMapDict)
        {
            var group = pair.Key;
            var prefabList = pair.Value;
            foreach (var prefabPath in prefabList)
            {
                var className = Utils.GetNameFromPath(prefabPath);
                var windowType = Type.GetType(Utils.PrefabNameToWindow(className));

                Debug.LogWarning("Window type - " + windowType + " with name " + className);

                PathTypeMap[prefabPath] = windowType;
                TypePathMap[windowType] = prefabPath;

                var windowGroupType = (WindowGroupType)Enum.Parse(typeof(WindowGroupType), group);
                PathLayerMap[prefabPath] = windowGroupType;
                if (!LayerPathMap.ContainsKey(windowGroupType))
                {
                    LayerPathMap[windowGroupType] = new List<string>();
                }
                LayerPathMap[windowGroupType].Add(prefabPath);
            }
        }

        Display();
        return true;
    }

    #endregion

    private void Display()
    {
        foreach (var pair in PathLayerMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }

        foreach (var pair in LayerPathMap)
        {
            foreach (var value in pair.Value)
            {
                Debug.Log("pair: key-" + pair.Key + ", value-" + value);
            }
        }

        foreach (var pair in PathTypeMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }

        foreach (var pair in TypePathMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }
    }

	IEnumerator DoReadXml ()
	{
		// print the path to the streaming assets folder
		var result = "";
		if (windowMapPath.Contains("://"))
		{
			var www = new WWW (windowMapPath);
			yield return www;
			result = www.text;
		}
		else
		{
			result = System.IO.File.ReadAllText(windowMapPath);
		}
		windowMapDict = ReadWindowMapFromXml(result);
		Load();
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
		doc.Save(windowMapPath);
		
		Debug.Log("Save window map file to " + windowMapPath);
	}
	
	public static Dictionary<string, List<string>> ReadWindowMapFromXml(string text)
	{
		var dict = new Dictionary<string, List<string>>();
		var doc = new XmlDocument();
		doc.LoadXml(text);
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
		return dict;
	}

    #region Mono

    void Awake()
    {
        PathLayerMap = new Dictionary<string, WindowGroupType>();
        LayerPathMap = new Dictionary<WindowGroupType, List<string>>();
        TypePathMap = new Dictionary<Type, string>();
        PathTypeMap = new Dictionary<string, Type>();

        //windowMapDict = Utils.ReadWindowMapFromXml();
		StartCoroutine(DoReadXml());
    }

    #endregion
}

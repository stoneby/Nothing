using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class FindWidget : MonoBehaviour
{
    public static List<string> FindAllPrefabs(string PrefabExt, List<string> PathFilterList)
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

    public static void LoadAllPrefabs(List<string> prefabList, IDictionary<GameObject, bool> activeStatus)
    {
        foreach (var path in prefabList)
        {
            var prefabObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefabObject == null)
            {
                continue;
            }
            activeStatus[prefabObject] = prefabObject.activeSelf;
        }
    }

    public static void ActiveAllPrefabs(IEnumerable<KeyValuePair<GameObject, bool>> activeStatus)
    {
        foreach (var pair in activeStatus.Where(pair => !pair.Value))
        {
            pair.Key.SetActive(true);
        }
    }

    public static List<T> FindAllWidgets<T>() where T : UIWidget
    {
        return Resources.FindObjectsOfTypeAll<T>().ToList();
    }

    public static void DisplayWidgets<T>(IEnumerable<T> widgetList) where T : UIWidget
    {
        foreach (var widget in widgetList)
        {
            var path = AssetDatabase.GetAssetPath(widget);
            Debug.Log("Find label: " + widget.gameObject.name + ", path: " + path);
        }
    }

    public static void RestoreActiveStatus(Dictionary<GameObject, bool> activeStatus)
    {
        foreach (var pair in activeStatus)
        {
            pair.Key.SetActive(pair.Value);
        }
    }
}

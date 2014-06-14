using System;
using System.IO;
using System.Linq;
using Thrift.Protocol;
using Thrift.Transport;
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

    #region Window Manager Methods

    /// <summary>
    /// Get file or folder name from a path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>File or folder name</returns>
    public static string GetNameFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Logger.LogError("path should not be null or empty.");
            return string.Empty;
        }

        if (path[path.Length - 1] == '/')
        {
            path.Remove(path.Length - 1);
        }
        return path.Substring(path.LastIndexOf('/') + 1);
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

    #endregion

    #region Rotate

    /// <summary>
    /// Get rotation in between source and target.
    /// </summary>
    /// <param name="source">Source position</param>
    /// <param name="target">Target position</param>
    /// <returns>Rotation quaternion</returns>
    public static Quaternion GetRotation(Vector3 source, Vector3 target)
    {
        var delta = target - source;
        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Get rotation in between source and target.
    /// </summary>
    /// <param name="source">Source position</param>
    /// <param name="target">Target position</param>
    /// <returns>Rotation quaternion</returns>
    public static Quaternion GetRotation(Vector2 source, Vector2 target)
    {
        return GetRotation(new Vector3(source.x, source.y, 0), new Vector3(target.x, target.y, 0));
    }

    #endregion

    /// <summary>
    /// Find the child transform with special name. 
    /// </summary>
    /// <param name="parent">The parent tranfrom of the child which will be found.</param>
    /// <param name="objName">The name of the child transfrom.</param>
    /// <returns>The transfrom to be found.</returns>
    public static Transform FindChild(Transform parent, string objName)
    {
        if (parent.name == objName)
        {
            return parent;
        }
        return (from Transform item in parent select FindChild(item, objName)).FirstOrDefault(child => null != child);
    }

    public static void MoveToParent(Transform parent, Transform instance)
    {
        instance.parent = parent.transform;
        instance.localPosition = Vector3.zero;
        instance.localRotation = Quaternion.identity;
        instance.localScale = Vector3.one;
        instance.gameObject.layer = parent.gameObject.layer;
    }

    // Convert from screen-space coordinates to world-space coordinates on the Z = 0 plane
    public static Vector3 GetWorldPos(Camera cam, Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        // we solve for intersection with z = 0 plane
        float t = -ray.origin.z / ray.direction.z;

        return ray.GetPoint(t);
    }

    public static T Decode<T>(string path) where T :  TBase, new()
    {
        var membuffer = new TMemoryBuffer(Resources.Load<TextAsset>(path).bytes);
        TProtocol protocol = (new TCompactProtocol(membuffer));
        var temp = new T();
        temp.Read(protocol);
        return temp;
    }


    /// <summary>
    /// Deactive the last tab window then open the current window. 
    /// </summary>
    /// <param name="type">The type of current window to be opened.</param>
    public static void ShowWithoutDestory(Type type)
    {
        var destroyLastWindow = WindowManager.Instance.DestroyLastWindow;
        WindowManager.Instance.DestroyLastWindow = false;
        WindowManager.Instance.Show(type, true);
        WindowManager.Instance.DestroyLastWindow = destroyLastWindow;
    }

    /// <summary>
    /// Spawn or despawn the new game object, and install or uninstall handler. 
    /// </summary>
    /// <param name="parent">The parent of all items.</param>
    /// <param name="childPrefab">The prefab of child item.</param>
    /// <param name="isAdd">If true, add child to the parent.</param>
    /// <param name="count">The number of item to be added or deleted.</param>
    /// <param name="poolName">The name of pool.</param>
    /// <param name="dDelegate">The handler to install or uninstall.</param>
    public static void AddOrDelItems(Transform parent, Transform childPrefab, bool isAdd, int count, string poolName, UIEventListener.VoidDelegate dDelegate)
    {
        if (isAdd)
        {
            for (int i = 0; i < count; i++)
            {
                var item = PoolManager.Pools[poolName].Spawn(childPrefab);
                MoveToParent(parent, item);
                NGUITools.SetActive(item.gameObject, true);
                UIEventListener.Get(item.gameObject).onClick += dDelegate;
            }
        }
        else
        {
            if (PoolManager.Pools.ContainsKey(poolName))
            {
                var list = parent.Cast<Transform>().ToList();
                for (int index = 0; index < count; index++)
                {
                    var item = list[index];
                    UIEventListener.Get(item.gameObject).onClick -= dDelegate;
                    item.parent = PoolManager.Pools[poolName].transform;
                    PoolManager.Pools[poolName].Despawn(item);
                }
            }
        }
    }

    public static DateTime ConvertFromJavaTimestamp(long timestamp)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddMilliseconds(timestamp).ToLocalTime();
    }

    public static long ConvertToJavaTimestamp(DateTime date)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
    }

    public static int GetActiveChildCount(Transform parent)
    {
        var childCount = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            if(NGUITools.GetActive(parent.GetChild(i).gameObject))
            {
                childCount++;
            }
        }
        return childCount;
    }
}

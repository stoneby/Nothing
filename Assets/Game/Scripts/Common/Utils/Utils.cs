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
            Debug.LogError("path should not be null or empty.");
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

    public static T Decode<T>(string path) where T :  TBase, new()
    {
        var membuffer = new TMemoryBuffer(Resources.Load<TextAsset>(path).bytes);
        TProtocol protocol = (new TCompactProtocol(membuffer));
        var temp = new T();
        temp.Read(protocol);
        return temp;
    }
}

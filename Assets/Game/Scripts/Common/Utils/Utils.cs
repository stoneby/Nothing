using System;
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

    private static UIRoot root;

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

    #region Object Finding

    public static UIRoot Root
    {
        get { return root ?? (root = GameObject.FindGameObjectWithTag("Root").GetComponent<UIRoot>()); }
    }

    #endregion

    #region MyPoolManager Helper
    public static void AddChild(GameObject sender, GameObject childObject)
    {
        var t = childObject.transform;
        t.parent = sender.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        childObject.SetActive(true);
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
        var membuffer = (string.IsNullOrEmpty(ServiceManager.ServerData.DataUrl)) ? 
            new TMemoryBuffer(Resources.Load<TextAsset>(path).bytes) :
            new TMemoryBuffer(HttpResourceManager.Instance.GetTemplateData(path));
        TProtocol protocol = (new TCompactProtocol(membuffer));
        var temp = new T();
        temp.Read(protocol);
        return temp;
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

    public static string GetTimeUntilNow(DateTime dateTime)
    {
        var timeSpan = DateTime.Now.Subtract(dateTime);
        var oneHour = new TimeSpan(1, 0, 0);
        var oneDay = new TimeSpan(1, 0, 0, 0);
        var thirtyDays = new TimeSpan(30, 0, 0, 0);
        if (timeSpan <= oneHour)
        {
            return Mathf.CeilToInt((float)timeSpan.TotalMinutes) +
                   LanguageManager.Instance.GetTextValue("UIFriendEntry.MinAgo");
        }
        if (timeSpan <= oneDay)
        {
            return Mathf.CeilToInt((float)timeSpan.TotalHours) + LanguageManager.Instance.GetTextValue("UIFriendEntry.HourAgo");
        }
        if (timeSpan <= thirtyDays)
        {
            return Mathf.CeilToInt((float)timeSpan.TotalDays) + LanguageManager.Instance.GetTextValue("UIFriendEntry.DayAgo");
        }
        return 1 + LanguageManager.Instance.GetTextValue("UIFriendEntry.MonthAgo");
    }

    public static string GetTimeUntilNow(long timestamp)
    {
        var time = ConvertFromJavaTimestamp(timestamp);
        return GetTimeUntilNow(time);
    }

    public static bool IsSameDay(DateTime datetime1, DateTime datetime2)
    {
        return datetime1.Year == datetime2.Year
            && datetime1.Month == datetime2.Month
            && datetime1.Day == datetime2.Day;
    }

    public static string ConvertTimeSpanToString(TimeSpan timeRemain)
    {
        return string.Format("{0:D2}", (int)timeRemain.TotalHours) + ":" +
               string.Format("{0:D2}", timeRemain.Minutes) + ":" +
               string.Format("{0:D2}", timeRemain.Seconds);
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

    public static void AdjustDepth(UIWidget uiWidget, int depthAdjust, bool includeChild = true)
    {
        if(uiWidget == null)
        {
            return;
        }
        if (includeChild == false)
        {
            uiWidget.depth += depthAdjust;
            return;
        }
        var widgets = uiWidget.GetComponentsInChildren<UIWidget>(true);
        foreach(var widget in widgets)
        {
            widget.depth += depthAdjust;
        }
    }

    public static Position OneDimToTwo(int newIndex, int column)
    {
        return new Position { X = newIndex / column, Y = newIndex % column };
    }

    public static int TwoDimToOne(Position position, int column)
    {
        return position.X * column + position.Y;
    }
}

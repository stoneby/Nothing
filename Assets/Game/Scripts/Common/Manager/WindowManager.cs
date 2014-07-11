﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Windows mananger including the following types:
/// - Screen (fullscreen)
/// - Popup (simple pop up window)
/// - TabPanel (tab based pop up window)
/// - Face (toppest window)
/// </summary>
/// <remarks>
/// More please refers to WindowGroupType.
/// </remarks>
public class WindowManager : Singleton<WindowManager>
{
    #region Public Fields

    public AbstractPathLayerMapping Mapping;

    public WindowRootObjectManager WindowRootManager;

    public bool DestroyLastWindow;

    #endregion

    #region Private Fields

    /// <summary>
    /// Window map between window group type and list of windows.
    /// </summary>
    private readonly Dictionary<WindowGroupType, List<Window>> windowMap = new Dictionary<WindowGroupType, List<Window>>();

    /// <summary>
    /// Current window map between window group type and current window.
    /// </summary>
    private readonly Dictionary<WindowGroupType, Window> currentWindowMap = new Dictionary<WindowGroupType, Window>();

    /// <summary>
    /// History window list that keep track of window open records.
    /// </summary>
    private readonly List<Window> historyList = new List<Window>();

    #endregion

    #region Public Properties

    public Dictionary<WindowGroupType, Window> CurrentWindowMap
    {
        get { return currentWindowMap; }
    }

    #endregion

    #region Puiblic Methods

    /// <summary>
    /// Get window with specific type.
    /// </summary>
    /// <returns>The window with specific type</returns>
    public T GetWindow<T>() where T : Window
    {
        var window = GetWindow(typeof(T));
        return window as T;
    }

    /// <summary>
    /// Get window by name
    /// </summary>
    /// <param name="windowName">Window type name</param>
    /// <returns>The window with specific type name</returns>
    public Window GetWindow(string windowName)
    {
        return GetWindow(Type.GetType(windowName));
    }

    /// <summary>
    /// Get window by type
    /// </summary>
    /// <param name="type">Window type</param>
    /// <returns>The window with specific type</returns>
    public Window GetWindow(Type type)
    {
        var path = Mapping.TypePathMap[type];
        var windowGroupType = Mapping.PathLayerMap[path];

        if (!windowMap.ContainsKey(windowGroupType))
        {
            windowMap[windowGroupType] = new List<Window>();
        }

        var window = windowMap[windowGroupType].Find(win => win.Path == path);
        if (window == null)
        {
            window = CreateWindow(windowGroupType, path);
            windowMap[windowGroupType].Add(window);
            Logger.Log("Create window with type - " + type + ", groupType - " + windowGroupType + ", path - " + path);
        }
        else
        {
            Logger.Log("Find window with type - " + type + ", groupType - " + windowGroupType + ", path - " + path);
        }
        return window;
    }

    /// <summary>
    /// Show by generic type.
    /// </summary>
    /// <typeparam name="T">Generic window type</typeparam>
    /// <param name="show">Flag indicates if window to show or hide</param>
    /// <returns>Window to show</returns>
    public T Show<T>(bool show) where T : Window
    {
        var type = typeof(T);
        return Show(type, show) as T;
    }

    /// <summary>
    /// Show by window type name.
    /// </summary>
    /// <param name="windowName">Window name</param>
    /// <param name="show">Flag indicates if window to show or hide</param>
    /// <returns>Window to show</returns>
    public Window Show(string windowName, bool show)
    {
        var type = Type.GetType(windowName);
        return Show(type, show);
    }

    /// <summary>
    /// Show by window type.
    /// </summary>
    /// <param name="type">Window type</param>
    /// <param name="show">Flag indicates if window to show or hide</param>
    /// <returns>Window to show</returns>
    public Window Show(Type type, bool show)
    {
        var path = Mapping.TypePathMap[type];
        var windowGroupType = Mapping.PathLayerMap[path];
        var window = GetWindow(type);

        if (show)
        {
            var lastWindow = currentWindowMap.ContainsKey(windowGroupType) ? currentWindowMap[windowGroupType] : null;
            if (lastWindow != null)
            {
                if (lastWindow != window && lastWindow.Active)
                {
                    var groupType = lastWindow.WindowGroup;
                    Logger.Log("Last window hide with type - " + lastWindow.GetType().Name + ", groupType - " + groupType +
                              ", path - " + lastWindow.Path);

                    HideWindow(lastWindow);
                }
                else if (lastWindow == window && lastWindow.Active)
                {
                    Logger.Log("The window is currently showing already." + lastWindow.name);
                    return window;
                }
            }

            currentWindowMap[windowGroupType] = window;
            window.gameObject.SetActive(true);

            historyList.Add(window);

            Logger.LogWarning("===== Adding window to history ====" + window.name + ", from group: " + window.WindowGroup);
        }
        else
        {
            currentWindowMap[windowGroupType] = window;
            HideWindow(window);

            historyList.Remove(window);
        }
        return window;
    }

    private void HideWindow(Window lastWindow)
    {
        if (DestroyLastWindow)
        {
            DestroyWindow(lastWindow);
        }
        else
        {
            lastWindow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Destroy window
    /// </summary>
    /// <param name="lastWindow">Last window to destroy</param>
    private void DestroyWindow(Window lastWindow)
    {
        var groupType = lastWindow.WindowGroup;
        Logger.Log("Removing last window hold: " + windowMap[groupType][0]);

        var find = windowMap[groupType].Remove(lastWindow);
        if (!find)
        {
            Logger.LogError("Trying to remove window - " + lastWindow.name +
                           ", but we got nothing from window map of group - " + groupType);
        }
        historyList.Remove(lastWindow);

        Destroy(lastWindow.gameObject);
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Show windows by group type.
    /// </summary>
    /// <param name="groupType">Group type</param>
    /// <param name="show">Flag indicates if show or hide</param>
    public void Show(WindowGroupType groupType, bool show)
    {
        if ((int)groupType == Utils.Invalid || !Mapping.LayerPathMap.ContainsKey(groupType))
        {
            Logger.LogError("Could not contain group type - " + groupType +
                           ", please double check with WindowGroupType, that's all we have.");
            return;
        }

        windowMap[groupType].ForEach(win =>
        {
            if (win.gameObject.activeSelf != show)
            {
                win.gameObject.SetActive(show);
            }
        });
    }

    /// <summary>
    /// Show all windows.
    /// </summary>
    /// <param name="show">Flag indicates if show or hide</param>
    public void Show(bool show)
    {
        foreach (var windowGroupType in Mapping.LayerPathMap.Keys)
        {
            Show(windowGroupType, show);
        }
    }

    public void CloseLastWindowInHistory()
    {
        //Window lastWindow;
        //for (var i = historyList.Count - 1; i > 0; --i)
        //{
        //    lastWindow = historyList[i];
        //    if (lastWindow == null || !lastWindow.Active)
        //    {
        //        historyList.RemoveAt(i);
        //        Logger.LogWarning("===== Removing inactive window from history ====" + lastWindow.name + ", from group: " + lastWindow.WindowGroup);
        //    }
        //    else
        //    {
        //        break;
        //    }
        //}

        if (historyList.Count <= 0)
        {
            Logger.LogWarning("There is no more windows in history list, please check whether the close button is correct or not.");
            return;
        }
        var lastWindow = historyList[historyList.Count - 1];
        historyList.RemoveAt(historyList.Count - 1);
        Show(lastWindow.GetType(), false);

        // [Note:] The following lines are tricky.
        // In the full history window list, there should be some window is hidden by the same group window showing,
        // We need to show back the hidden window for the full history list.
        if (historyList.Count > 0)
        {
            lastWindow = historyList[historyList.Count - 1];
            if (lastWindow != null && !lastWindow.Active)
            {
                lastWindow.gameObject.SetActive(true);
            }
        }
        Logger.LogWarning("===== Close last window from history ====" + lastWindow.name + ", from group: " + lastWindow.WindowGroup);
    }

    /// <summary>
    /// Clear up window history.
    /// </summary>
    /// <remarks>Keep history only when in UI oepration, should clear out when back from battle.</remarks>
    public void ClearHistory()
    {
        Logger.LogWarning("Clear history window list.");
        historyList.Clear();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Create window by group type and prefab path
    /// </summary>
    /// <param name="groupType">Window group type</param>
    /// <param name="path">Prefab path</param>
    /// <returns>The window handle</returns>
    private Window CreateWindow(WindowGroupType groupType, string path)
    {
        var root = WindowRootManager.WindowObjectMap[groupType];
        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Logger.LogError("Could not find window from groupType - " + groupType + ", path - " + path);
            return null;
        }

        // This is importance, Awake / OnEnable should not be called until Show is get called.
        prefab.SetActive(false);
        var child = NGUITools.AddChild(root, prefab);
        var windowName = Utils.PrefabNameToWindow(Utils.GetNameFromPath(path));
        var component = child.GetComponent(windowName) ?? child.AddComponent(windowName);
        var window = component.GetComponent<Window>();
        window.Path = path;
        window.WindowGroup = groupType;
        return window;
    }

    /// <summary>
    /// Debug display
    /// </summary>
    private void Display()
    {
        foreach (var pair in windowMap)
        {
            Logger.Log("Window map including : group - " + pair.Key);
            foreach (var window in pair.Value)
            {
                Logger.Log("Including window - " + window);
            }
        }
    }

    #endregion

    #region Mono

    void Start()
    {
    }

    #endregion
}

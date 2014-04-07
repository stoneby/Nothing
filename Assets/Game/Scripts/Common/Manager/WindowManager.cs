using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Windows mananger including the following types:
/// - Screen (fullscreen)
/// - Popup (simple pop up window)
/// - TabPanel (tab based pop up window)
/// - Face (toppest window)
/// </summary>
public class WindowManager : Singleton<WindowManager>
{
    #region Public Fields

    public AbstractPathLayerMapping Mapping;

    public WindowRootObjectManager WindowRootManager;

    public bool DestroyLastWindow;

    #endregion

    #region Private Fields

    /// <summary>
    /// Window map between layer and list of windows.
    /// </summary>
    private readonly Dictionary<WindowGroupType, List<Window>> windowMap = new Dictionary<WindowGroupType, List<Window>>();

    /// <summary>
    /// Current window map between layer and current window.
    /// </summary>
    private readonly Dictionary<WindowGroupType, Window> currentWindowMap = new Dictionary<WindowGroupType, Window>();

    #endregion

    #region Public Properties

    public Dictionary<WindowGroupType, Window> CurrentWindowMap
    {
        get { return currentWindowMap; }
    }

    #endregion

    #region Puiblic Methods

    /// <summary>
    /// Show by window type.
    /// </summary>
    /// <param name="type">Window type</param>
    /// <param name="show">Flag indicates if window to show or hide</param>
    /// <returns>Window to show</returns>
    public Window Show(Type type, bool show)
    {
        var path = Mapping.TypePathMap[type];
        var layer = Mapping.PathLayerMap[path];

        if (!windowMap.ContainsKey(layer))
        {
            windowMap[layer] = new List<Window>();
        }

        var window = windowMap[layer].Find(win => win.Path.Equals(path));
        if (window == null)
        {
            window = GetWindow(layer, type, path);
            windowMap[layer].Add(window);
            Debug.Log("Create window with type - " + type + ", layer - " + layer + ", path - " + path);
        }
        else
        {
            Debug.Log("Find window with type - " + type + ", layer - " + layer + ", path - " + path);
        }

        if (window == null)
        {
            Debug.LogError("Could not find window from type - " + type + ", layer - " + layer + ", path - " +
                           path);
            return null;
        }

        if (show)
        {
            var lastWindow = currentWindowMap.ContainsKey(layer) ? currentWindowMap[layer] : null;
            if (lastWindow != null && lastWindow.Active)
            {
                var groupType = (WindowGroupType)lastWindow.gameObject.layer;
                Debug.Log("Last window hide with type - " + lastWindow.Type + ", layer - " + groupType + ", path - " + lastWindow.Path);

                if (DestroyLastWindow)
                {
                    Debug.Log("Removing last window hold: " + windowMap[groupType][0]);

                    windowMap[groupType].RemoveAt(0);
                    Destroy(lastWindow.gameObject);
                    Resources.UnloadUnusedAssets();
                }
                else
                {
                    lastWindow.gameObject.SetActive(false);
                }
                lastWindow.Close();
            }

            currentWindowMap[layer] = window;
            window.gameObject.SetActive(true);
            window.Open();
        }
        else
        {
            currentWindowMap[layer] = window;
            window.gameObject.SetActive(false);
            window.Close();
        }

        Display();

        return window;
    }

    /// <summary>
    /// Show windows by layer name.
    /// </summary>
    /// <param name="layerName">Layer name</param>
    /// <param name="show">Flag indicates if show or hide</param>
    public void Show(string layerName, bool show)
    {
        var layer = LayerMask.NameToLayer(layerName);
        Show((WindowGroupType)layer, show);
    }

    /// <summary>
    /// Show windows by layer value.
    /// </summary>
    /// <param name="layer">Layer name</param>
    /// <param name="show">Flag indicates if show or hide</param>
    public void Show(WindowGroupType layer, bool show)
    {
        if ((int)layer == Utils.Invalid || !Mapping.LayerPathMap.ContainsKey(layer))
        {
            Debug.LogError("Could not contain layer name - " + LayerMask.LayerToName((int)layer) +
                           ", please double check with layer manager of unity.");
            return;
        }

        windowMap[layer].ForEach(win =>
        {
            if (win.gameObject.activeSelf != show)
            {
                win.gameObject.SetActive(show);
                if (show)
                {
                    win.Open();
                }
                else
                {
                    win.Close();
                }
            }
        });
    }

    /// <summary>
    /// Show all windows.
    /// </summary>
    /// <param name="show">Flag indicates if show or hide</param>
    public void Show(bool show)
    {
        foreach (var layer in Mapping.LayerPathMap.Keys)
        {
            Show(layer, show);
        }
    }

    #endregion

    #region Private Methods

    private Window GetWindow(WindowGroupType layer, Type type, string path)
    {
        var root = WindowRootManager.WindowObjectMap[layer];
        var prefab = Resources.Load<GameObject>(path);
        var child = NGUITools.AddChild(root, prefab);

        //var window = child.GetComponent<Window>() ?? child.AddComponent<Window>();
        var windowName = Utils.PrefabNameToWindow(Utils.GetNameFromPath(path));
        var component = child.GetComponent(windowName) ?? child.AddComponent(windowName);
        var window = component as Window;
        window.Path = path;
        //window.Type = type;
        return window;
    }

    private void Display()
    {
        foreach (var pair in windowMap)
        {
            Debug.Log("Window map including : group - " + pair.Key);
            foreach (var window in pair.Value)
            {
                Debug.Log("Including window - " + window);
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

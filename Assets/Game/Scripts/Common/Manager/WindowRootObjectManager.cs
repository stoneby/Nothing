using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Window game object hiarachy manager, which game object layer under NguiCamera object root.
/// - Screen.
/// - Popup.
/// - TabPanel.
/// - Face.
/// </summary>
public class WindowRootObjectManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Path layer mapping.
    /// </summary>
    public AbstractPathLayerMapping Mapping;

    /// <summary>
    /// Parent game object of all windows.
    /// </summary>
    public GameObject Root;

    public float FadeInDuration;
    public float FadeOutDuration;

    public delegate void FadeComplete(Window window);

    #endregion

    #region Public Properties

    /// <summary>
    /// Map from window group type to window group game object root.
    /// </summary>
    public Dictionary<WindowGroupType, GameObject> WindowObjectMap = new Dictionary<WindowGroupType, GameObject>();

    /// <summary>
    /// Map from window group type to window group game object's tween alpha component.
    /// </summary>
    public Dictionary<WindowGroupType, TweenAlpha> WindowAlphaTweenMap = new Dictionary<WindowGroupType, TweenAlpha>();

    #endregion

    #region Public Methods

    public void FadeIn(WindowGroupType group, FadeComplete onFadeComplete = null)
    {
        StartCoroutine(DoFade(group, true, onFadeComplete));
    }

    public void FadeOut(WindowGroupType group, FadeComplete onFadeComplete = null)
    {
        StartCoroutine(DoFade(group, false, onFadeComplete));
    }

    #endregion

    #region Private Methods

    private IEnumerator DoFade(WindowGroupType group, bool fadeIn, FadeComplete onFadeComplete)
    {
        var tweenAlpha = WindowAlphaTweenMap[group];
        tweenAlpha.ResetToBeginning();
        if (fadeIn)
        {
            tweenAlpha.PlayForward();
        }
        else
        {
            tweenAlpha.PlayReverse();
        }

        var lastWindow = WindowManager.Instance.CurrentWindowMap[group];
        Logger.LogWarning("DoFade: " + group + ", window: " + lastWindow.name);

        yield return new WaitForSeconds(tweenAlpha.duration);

        if (onFadeComplete != null)
        {
            onFadeComplete(lastWindow);
        }
    }

    private void InitAlphaTween(Transform layerTrans, WindowGroupType groupType)
    {
        var tweenAlpha = layerTrans.GetComponent<TweenAlpha>() ??
                         layerTrans.gameObject.AddComponent<TweenAlpha>();
        tweenAlpha.from = 0f;
        tweenAlpha.to = 1f;
        tweenAlpha.duration = FadeInDuration;
        tweenAlpha.enabled = false;
        WindowAlphaTweenMap[groupType] = tweenAlpha;
    }

    #endregion

    #region Mono

    void Start()
    {
        var trans = Root.transform;

        foreach (var groupType in Mapping.LayerPathMap.Keys)
        {
            var windowGroup = groupType.ToString();
            var layerTrans = trans.Find(windowGroup);
            if (layerTrans == null)
            {
                var layerObject = new GameObject();
                layerObject.transform.parent = trans;
                layerObject.transform.localScale = Vector3.one;
                layerTrans = layerObject.transform;

                Logger.Log("Adding game object to root - " + trans.name);
            }

            layerTrans.name = windowGroup;

            Logger.Log("Game object is - " + layerTrans.name + ", with window group - " + windowGroup);

            WindowObjectMap[groupType] = layerTrans.gameObject;
            InitAlphaTween(layerTrans, groupType);
        }

        EventManager.Instance.Post(new WindowManagerReady());
    }

    #endregion
}

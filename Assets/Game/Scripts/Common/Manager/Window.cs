using UnityEngine;

/// <summary>
/// Window which will be attached to each window prefab object.
/// - OnOpen will be sent when window is opend.
/// - OnClose will be sent when window is closed.
/// </summary>
public abstract class Window : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Window enum type.
    /// </summary>
    public WindowType Type;

    /// <summary>
    /// Prefab path.
    /// </summary>
    public string Path;

    #endregion

    #region Public Properties

    /// <summary>
    /// Flag indicates if the window is active (show) or not.
    /// </summary>
    public bool Active { get { return gameObject.activeSelf; } }

    #endregion

    #region Const

    /// <summary>
    /// OnOpen method.
    /// </summary>
    private const string OnOpenMethod = "OnOpen";

    /// <summary>
    /// OnClose method.
    /// </summary>
    private const string OnCloseMethod = "OnClose";

    #endregion

    #region Message

    /// <summary>
    /// Send out OnOpen event.
    /// </summary>
    public void Open()
    {
        gameObject.SendMessage(OnOpenMethod, null, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Send out OnClose event.
    /// </summary>
    public void Close()
    {
        gameObject.SendMessage(OnCloseMethod, null, SendMessageOptions.DontRequireReceiver);
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    #endregion

    #region Object

    public override string ToString()
    {
        return string.Format("Window: name-{0}, type-{1}, path-{2}, active-{3}", name, Type, Path, Active);
    }

    #endregion

    #region Mono

    void Start()
    {
    }

    #endregion
}

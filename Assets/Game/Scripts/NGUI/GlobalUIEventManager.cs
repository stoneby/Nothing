
/// <summary>
/// Global NGUI event manager, which is going to listene falling through events.
/// </summary>
public class GlobalUIEventManager : Singleton<GlobalUIEventManager>
{
    #region Public Properties

    /// <summary>
    /// Event listener.
    /// </summary>
    public UIEventListener EventListener
    {
        get { return UIEventListener.Get(gameObject); }
    }

    #endregion

    #region Mono

    private void Start()
    {
        UICamera.fallThrough = gameObject;
    }

    #endregion
}

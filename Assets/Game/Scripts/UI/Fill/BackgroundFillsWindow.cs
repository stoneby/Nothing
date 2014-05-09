

public class BackgroundFillsWindow : Window
{
    #region Window

    public override void OnEnter()
    {
        Logger.Log("I am OnEnter with type - " + GetType().Name);
    }

    public override void OnExit()
    {
        Logger.Log("I am OnExit with type - " + GetType().Name);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {

    }

    #endregion
}

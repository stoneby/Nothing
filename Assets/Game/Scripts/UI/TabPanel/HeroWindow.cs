using UnityEngine;

public class HeroWindow : Window
{
    #region Public Fields

    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public GameObject Button4;

    #endregion

    #region Private Fields

    private UIEventListener button1UIEventListener;

    #endregion

    #region Window

    // Use this for initialization
    public override void OnEnter()
    {
        Logger.Log("I am OnEnter. " + GetType().Name);

        button1UIEventListener.onClick += OnButtonClick;
        button1UIEventListener.onDoubleClick += OnDoubleClick;
    }

    public override void OnExit()
    {
        Logger.Log("I am OnExit. " + GetType().Name);

        button1UIEventListener.onClick -= OnButtonClick;
        button1UIEventListener.onDoubleClick -= OnDoubleClick;
    }

    #endregion

    #region Callback

    private void OnButtonClick(GameObject game)
    {
        Logger.LogWarning("XXXXXXXXXXXXXXXXXXXX I got clicked. " + game.name);
    }

    private void OnDoubleClick(GameObject game)
    {
        Logger.LogWarning("XXXXXXXXXXXXXXXXXXXX I got double clicked. " + game.name);
    }

    #endregion

    #region Mono

    void Awake()
    {
        Logger.Log("Awake" + " framerate:  " + Time.frameCount);
        button1UIEventListener = UIEventListener.Get(Button1);
    }

    #endregion
}

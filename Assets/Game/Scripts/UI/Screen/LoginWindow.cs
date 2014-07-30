using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginWindow : Window
{
    

    #region Window

    public override void OnEnter()
    {
        //GlobalUIEventManager.Instance.EventListener.onClick += OnFallThroughClick;
        WindowManager.Instance.Show(typeof(LoginMainWindow), true);
    }

    public override void OnExit()
    {
        //GlobalUIEventManager.Instance.EventListener.onClick -= OnFallThroughClick;
    }

    #endregion

//    private void OnFallThroughClick(GameObject sender)
//    {
//        Logger.LogWarning("This is a fall through event.");
//    }

    #region Mono

    // Use this for initialization
    void Start()
    {
       
        
    }

    #endregion
}

using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginWindow : Window
{
    

    #region Window

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
       
        WindowManager.Instance.Show(typeof(LoginMainWindow), true);
    }

    #endregion
}

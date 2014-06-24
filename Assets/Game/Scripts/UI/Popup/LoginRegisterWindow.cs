using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginRegisterWindow : Window
{
    private GameObject btnReturn;

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
        btnReturn = transform.FindChild("Image Button - back").gameObject;
        btnReturn.AddComponent<CloseRegisterHandler>();
    }

    #endregion
}

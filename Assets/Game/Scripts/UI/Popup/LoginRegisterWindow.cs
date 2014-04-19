using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginRegisterWindow : Window
{
    private GameObject BtnRegister;
    private GameObject BtnReturn;

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
        BtnRegister = transform.FindChild("Image Button - register").gameObject;
        //BtnRegister.AddComponent<DoRegisterHandler>();

        BtnReturn = transform.FindChild("Image Button - back").gameObject;
        BtnReturn.AddComponent<CloseRegisterHandler>();
    }

    #endregion
}

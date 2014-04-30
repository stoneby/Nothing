using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class BattleFieldWindow : Window
{
    #region Public Fields

    public bool EditMode;

    public TeamSelectController TeamLeft;
    public TeamSelectController TeamRight;

    #endregion

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
    private void Start()
    {
    }

    private void OnGUI()
    {
        if (!EditMode)
        {
            if (GUI.Button(new Rect(100, 100, 200, 200), "Edit Mode On"))
            {
                EditMode = true;
                TeamLeft.EditMode = true;
                TeamRight.EditMode = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(100, 100, 200, 200), "Edit Mode Off"))
            {
                EditMode = false;
                TeamLeft.EditMode = false;
                TeamRight.EditMode = false;
            }
        }
    }


    #endregion
}

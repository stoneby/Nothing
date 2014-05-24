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
        TeamLeft.OnAttack += TeamRight.OnDragOverAnotherTeamHandler;
        TeamRight.OnAttack += TeamLeft.OnDragOverAnotherTeamHandler;
    }

    private void OnGUI()
    {
        var x = Screen.width / 20;
        var y = Screen.height / 20;
        var width = Screen.width / 8;
        var height = Screen.height / 10;
        if (!EditMode)
        {
            if (GUI.Button(new Rect(x, y, width, height), "Edit Mode On"))
            {
                EditMode = true;
                TeamLeft.EditMode = true;
                TeamRight.EditMode = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(x, y, width, height), "Edit Mode Off"))
            {
                EditMode = false;
                TeamLeft.EditMode = false;
                TeamRight.EditMode = false;
            }
        }
    }


    #endregion
}

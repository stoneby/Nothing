using UnityEngine;

public class DragBarNo1Controller : AbstractDragBarController
{
    #region Public Fields

    public UISprite DragBar;
    public Vector2 OffSet;

    #endregion

    #region Abstract Dragbar Controller
    public override void SetWidth(float width)
    {
        DragBar.width = (int)(width + OffSet.x);
    }

    #endregion
}

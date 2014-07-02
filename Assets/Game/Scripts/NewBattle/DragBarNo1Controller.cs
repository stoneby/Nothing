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
        DragBar.width = (int)(width + OffSet.x + OffSet.x);
    }

    public override void SetSprite(string spriteName)
    {
        DragBar.spriteName = spriteName;
    }

    public override void SetDepth(int depth)
    {
        DragBar.depth = depth;
    }

    public override int GetDepth()
    {
        return DragBar.depth;
    }

    #endregion
}

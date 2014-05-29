using UnityEngine;

public class DragBarNo2Controller : AbstractDragBarController
{
    #region Public Fields

    public UISprite CeneterSprite;
    public UISprite BarSprite;

    #endregion

    #region Abstract Dragbar Controller

    public override void SetWidth(float width)
    {
        BarSprite.width = (int)(width);
    }

    #endregion
}

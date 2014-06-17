using UnityEngine;

public class DragBarNo2Controller : AbstractDragBarController
{
    #region Public Fields

    public UISprite CenterSprite;
    public UISprite BarSprite;

    #endregion

    #region Abstract Dragbar Controller

    public override void SetWidth(float width)
    {
        BarSprite.width = (int)(width);
    }

    public override void SetSprite(string spriteName)
    {
        BarSprite.spriteName = spriteName;
        CenterSprite.spriteName = spriteName;
    }

    #endregion
}

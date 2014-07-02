
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

    public override void SetDepth(int depth)
    {
        BarSprite.depth = depth;
        CenterSprite.depth = depth;
    }

    public override int GetDepth()
    {
        return BarSprite.depth;
    }

    #endregion
}

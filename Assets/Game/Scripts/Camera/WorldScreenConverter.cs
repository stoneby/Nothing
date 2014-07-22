using UnityEngine;

/// <summary>
/// Convert screen pixel position to NGUI world position.
/// </summary>
/// <remarks>
/// Reply on NGUI stuff, Used for UI developer make exactly same pixel fit with artist design.
/// Remember that let's using the screen ratio 16:9 in game view rendering settings.
/// </remarks>
public class WorldScreenConverter : MonoBehaviour
{
    public static readonly Vector2 FullScreen = new Vector2(1280, 720);

    public enum Style
    {
        TopLeft,
        BottomLeft
    }

    public Vector3 ScreenPosition;

    public Style ZeroStyle;

#if UNITY_EDITOR
    [ContextMenu("Convert")]
    public void Convert()
    {
        var gameViewSize = UIPanel.GetMainGameViewSize();
        var root = NGUITools.FindInParents<UIRoot>(gameObject);
        var ratio = root.GetPixelSizeAdjustment((int)gameViewSize.y);
        Debug.LogWarning("Game view size: " + gameViewSize + ", ratio: " + ratio);

        var screenPosition = (ZeroStyle == Style.BottomLeft)
            ? ScreenPosition
            : (new Vector3(ScreenPosition.x, (int)FullScreen.y - ScreenPosition.y, ScreenPosition.z));
        var worldPoint = Camera.main.ScreenToWorldPoint(screenPosition / ratio * Camera.main.orthographicSize);
        transform.position = worldPoint;
    }
#endif
}

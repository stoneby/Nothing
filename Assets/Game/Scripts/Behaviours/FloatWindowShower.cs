using UnityEngine;

public class FloatWindowShower : MonoBehaviour
{
    /// <summary>
    /// The bg of the window, its pivot should be top left.
    /// </summary>
    public UIWidget Bg;

    public void Show()
    {
        var adjustment = new Vector3();
        var mousePos = Input.mousePosition;
        adjustment += IsHorizontalValid(mousePos) ? Vector3.zero : new Vector3(-Bg.width / Utils.PixelAjustFactor, 0);
        adjustment += IsVerticalValid(mousePos) ? Vector3.zero : new Vector3(0, Bg.height / Utils.PixelAjustFactor - mousePos.y);
        var showPos = mousePos + adjustment;
        var showWorldPos = UICamera.mainCamera.ScreenToWorldPoint(showPos);
        var bgWorldPos = Bg.transform.position;
        transform.position += (showWorldPos - bgWorldPos);
    }

    private bool IsHorizontalValid(Vector3 pos)
    {
        return (pos.x + Bg.width / Utils.PixelAjustFactor) < Screen.width;
    }

    private bool IsVerticalValid(Vector3 pos)
    {
        return pos.y * Utils.PixelAjustFactor > Bg.height;
    }
}

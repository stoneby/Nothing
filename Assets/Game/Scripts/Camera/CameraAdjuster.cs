using UnityEngine;

/// <summary>
/// Adjust NGUI camera automatically.
/// The texture is always 16/9, we have to adjust to any kinds of monitors.
/// </summary>
public class CameraAdjuster : MonoBehaviour
{
    #region Public Fields

    public Camera Camera;

    #endregion

    #region Consts

    private const float ScreenRatio = 16/9f;

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
        //Adjust();
    }

    private static void Adjust()
    {
        float r1 = Screen.height / 540.0f;
        float r2 = Screen.width / 960.0f;
        var cameraScale = (r1 < r2) ? r1 : r2;
        Camera.main.transform.localScale = new Vector3(cameraScale, cameraScale, 1);
    }

    #endregion
}

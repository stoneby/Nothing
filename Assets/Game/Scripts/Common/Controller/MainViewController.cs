using UnityEngine;

public class MainViewController : MonoBehaviour
{
    public GameObject[] Screens;
    public GameObject[] Windows;
    public GameObject[] Effects;

    public GameObject TextPrefab;

    public static float CameraScale = 1;

    // Use this for initialization
    void Start()
    {
		float r1 = Screen.height / 540.0f;
		float r2 = Screen.width / 960.0f;
		CameraScale = (r1 < r2) ? r1 : r2;
		Camera.main.transform.localScale = new Vector3(CameraScale, CameraScale, 1);

        var screenroot = GameObject.Find("Screen");
        ScreenManager.InitScreens(Screens, screenroot);
        ScreenManager.CurrentScreen = ScreenType.GameWorld;

        var windowroot = GameObject.Find("TabPanel");
        SubWindowManager.InitWindows(Windows, windowroot);

        PopTextManager.Init(GameObject.Find("EffectPanel"), TextPrefab);
    }
}

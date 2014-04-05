using UnityEngine;

public class ScreenManager
{
    private static GameObject[] screenFabs;
    private static GameObject[] screens;
    private static int screenCount;
    private static int[] initedFlags;

    public static int PreviousScreen;
    private static int currentScreen;

    private static GameObject rootGameObject;

    public static void InitScreens(GameObject[] thescreenprefabs, GameObject rootobj)
    {
        screenFabs = thescreenprefabs;
        screenCount = screenFabs.Length;
        initedFlags = new int[screenCount];
        screens = new GameObject[screenCount];
        PreviousScreen = -1;
        currentScreen = -1;
        rootGameObject = rootobj;
    }

    public static int CurrentScreen
    {
        get { return currentScreen; }
        set
        {
            if (currentScreen != value)
            {
                PreviousScreen = currentScreen;
                HideAll();
                currentScreen = value;
                if (initedFlags[currentScreen] == 0)
                {
                    screens[currentScreen] = NGUITools.AddChild(rootGameObject, screenFabs[currentScreen]);
                    initedFlags[currentScreen] = 1;
                }
                else
                {
                    screens[currentScreen].SetActive(true);
                }
            }
        }
    }

    public static void HideAll()
    {
        if (currentScreen >= 0) screens[currentScreen].SetActive(false);
    }
}

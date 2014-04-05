using UnityEngine;

public class SubWindowManager
{
    private static GameObject[] windowFabs;
    private static GameObject[] windows;
    private static int windowCount;
    private static int[] initedFlags;

    public static int PreviousWindow;
    private static int currentWindow;

    private static GameObject rootGameObject;

    public static void InitWindows(GameObject[] thescreenprefabs, GameObject rootobj)
    {
        windowFabs = thescreenprefabs;
        windowCount = windowFabs.Length;
        initedFlags = new int[windowCount];
        windows = new GameObject[windowCount];
        PreviousWindow = -1;
        currentWindow = -1;
        rootGameObject = rootobj;
    }

    public static int CurrentWindow
    {
        get { return currentWindow; }
        set
        {
            if (currentWindow != value)
            {
                PreviousWindow = currentWindow;
                HideAll();
                currentWindow = value;
                if (initedFlags[currentWindow] == 0)
                {
                    windows[currentWindow] = NGUITools.AddChild(rootGameObject, windowFabs[currentWindow]);
                    initedFlags[currentWindow] = 1;
                }
                else
                {
                    windows[currentWindow].SetActive(true);

                }

                var bc = windows[currentWindow].GetComponent<BattleEndControl>();
                if (bc != null)
                {
                    bc.Show();
                }
            }
        }
    }

    public static void HideAll()
    {
        if (currentWindow >= 0)
        {
            windows[currentWindow].SetActive(false);
            currentWindow = -1;
        }
    }
}

using UnityEngine;

public class WindowManagerTest : MonoBehaviour
{
    void OnGUI()
    {
        if (GUILayout.Button("Create UI"))
        {
            CreateUIWindows();
        }

        if (GUILayout.Button("Destroy UI"))
        {
            WindowManager.Instance.DestroyWindows(Window.MemoryManagementType.UI);
            Resources.UnloadUnusedAssets();
        }

        if (GUILayout.Button("Destroy Battle"))
        {
            WindowManager.Instance.DestroyWindows(Window.MemoryManagementType.Battle);
            Resources.UnloadUnusedAssets();
        }
    }
    // Use this for initialization
    void Start()
    {
        //CreateUIWindows();
    }

    private static void CreateUIWindows()
    {
        WindowManager.Instance.Show<LoginWindow>(true);
        WindowManager.Instance.Show<LoginAccountWindow>(true);
        WindowManager.Instance.Show<LoginMainWindow>(true);
    }
}

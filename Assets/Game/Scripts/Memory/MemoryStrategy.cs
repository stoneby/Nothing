using UnityEngine;

public class MemoryStrategy : Singleton<MemoryStrategy>
{
    public bool Enabled;

    public void HandleBattleBegin()
    {
        if (Enabled)
        {
            WindowManager.Instance.DestroyWindows(Window.MemoryManagementType.UI);
            Resources.UnloadUnusedAssets();
        }
    }

    public void HandleBattleEnd()
    {
        if (Enabled)
        {
            WindowManager.Instance.DestroyWindows(Window.MemoryManagementType.Battle);
            CharacterPoolManager.Instance.Cleanup();
            Resources.UnloadUnusedAssets();
        }
    }

    public void HandleUIBegin()
    {
        if (Enabled)
        {
            WindowManager.Instance.DestroyWindows(Window.MemoryManagementType.Login);
            Resources.UnloadUnusedAssets();
        }
    }
}

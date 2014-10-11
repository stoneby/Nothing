
public class BattleResultHelper
{
    /// <summary>
    /// Cleanup operation when battle result window show end.
    /// </summary>
    public static void Cleanup()
    {
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        if (battlemanager != null)
        {
            battlemanager.ResetAll();
        }
    }
}

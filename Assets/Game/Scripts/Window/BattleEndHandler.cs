using UnityEngine;

public class BattleEndHandler : MonoBehaviour
{
    void OnClick()
    {
        //SubWindowManager.HideAll();

        //var obj = GameObject.Find("Battle(Clone)");
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        battlemanager.DestroyBattle();

        //ScreenManager.CurrentScreen = ScreenType.GameWorld;

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        //WindowManager.Instance.Show(typeof(MainMenuWindow), true);
        WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
    }
}

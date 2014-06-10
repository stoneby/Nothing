using UnityEngine;

public class BattleEndHandler : MonoBehaviour
{
    void OnClick()
    {
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        battlemanager.DestroyBattle();

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
        WindowManager.Instance.Show(typeof(MissionTabWindow), true);
    }
}

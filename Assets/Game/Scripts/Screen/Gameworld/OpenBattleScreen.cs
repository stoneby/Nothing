using UnityEngine;

public class OpenBattleScreen : MonoBehaviour
{
    void OnClick()
    {
        //ScreenManager.CurrentScreen = ScreenType.Battle;
        
        var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;

        //GameObject obj = GameObject.Find("Battle(Clone)");
        Debug.Log(window);
        var battlemanager = window.GetComponent<InitBattleField>();
        var attracks = new int[12];
        for (var i = 0; i < attracks.Length; i++)
        {
            attracks[i] = (i % 2 == 0) ? 1 : 5;
        }
        var enemys = new int[2];
        enemys[0] = 1;
        enemys[1] = 2;
        battlemanager.StartBattle(attracks, enemys);
    }
}

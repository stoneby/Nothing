using UnityEngine;

public class LeaderPress : MonoBehaviour
{
    public GameObject Leader;

    void OnClick()
    {
        var lc = Leader.GetComponent<LeaderControl>();
        lc.Reset();

        GameObject obj = GameObject.Find("Battle(Clone)");
        var battlemanager = obj.GetComponent<InitBattleField>();

        battlemanager.InvokeLeaderSkill(lc.leaderIndex);
    }
}

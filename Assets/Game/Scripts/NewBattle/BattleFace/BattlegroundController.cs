﻿using UnityEngine;

/// <summary>
/// Battleground controller.
/// </summary>
/// <remarks>
/// Battle ground with any nunber of steps (saying total steps are N) to go with.
/// - begin step. (1)
/// - looping step. (N - 2)
/// - end step. (1)
/// </remarks>
public class BattlegroundController : MonoBehaviour
{
    /// <summary>
    /// Parent object that holding battlegrounds.
    /// </summary>
    public GameObject Parent;

    public int BattleID;

    /// <summary>
    /// Total step that battleground are going to set.
    /// </summary>
    public int TotalStep;

    /// <summary>
    /// Total battle number to set.
    /// </summary>
    /// <remarks>
    /// Just for testing purpose, used random choose one, will remove in future with designer define.
    /// </remarks>
    public int TotalBattleNum = 2;

    private AbstractBattlegroundLooper battlegroundLooper;
    private int currentStep;

    private const string BasePath = "Prefabs/NewBattle/Battleground";
    private const int MinimalTotalStep = 3;

    public void Initialize()
    {
        var path = string.Format("{0}/{1}", BasePath, BattleID);
        var battle = Resources.Load<GameObject>(path);
        if (battle == null)
        {
            Logger.LogError("Could not find path: " + path + " to generator battleground.");
            return;
        }

        if (TotalStep < MinimalTotalStep)
        {
            Logger.LogError("Total step should be greater or equals to " + MinimalTotalStep);
            return;
        }

        if (Parent == null)
        {
            Logger.LogError("Parent should not be null.");
            return;
        }

        var battlegroundObject = NGUITools.AddChild(Parent, battle);
        battlegroundLooper = battlegroundObject.GetComponent<AbstractBattlegroundLooper>();
    }

    public void Reset()
    {
        BattleID = -1;
        currentStep = 0;
    }

    public void Play()
    {
        if (battlegroundLooper == null)
        {
            Logger.LogError("Please call Initialize() first.");
            return;
        }

        if (currentStep < 0 || currentStep >= TotalStep)
        {
            Logger.LogError("current step is not invalid, current range is [0, " + TotalStep + ")");
            return;
        }

        if (currentStep == 0)
        {
            battlegroundLooper.PlayBegin();
        }
        else if(currentStep == TotalStep - 1)
        {
            battlegroundLooper.PlayEnd();
        }
        else
        {
            battlegroundLooper.PlayOnce();
        }

        ++currentStep;
    }

    public void Cleanup()
    {
        Destroy(battlegroundLooper.gameObject);
        Resources.UnloadUnusedAssets();
    }
}

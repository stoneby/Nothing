using UnityEngine;

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
    public int TotalBattleNum;

    private AbstractBattlegroundLooper battlegroundLooper;
    private int currentStep;

    private const string BasePath = "Prefabs/NewBattle/Battleground";

    private bool initialized;

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (BattleID <= 0 || BattleID > TotalBattleNum)
        {
            Logger.LogWarning("Battle ID should be in range [1, " + TotalBattleNum + "], but is: " + BattleID + ". Let's make it default to 1.");
            BattleID = 1;
        }

        var path = string.Format("{0}/{1}", BasePath, BattleID);
        var battle = Resources.Load<GameObject>(path);
        if (battle == null)
        {
            Logger.LogError("Could not find path: " + path + " to generator battleground.");
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
        if (!initialized)
        {
            return;
        }

        initialized = false;

        Destroy(battlegroundLooper.gameObject);
        Resources.UnloadUnusedAssets();
    }
}

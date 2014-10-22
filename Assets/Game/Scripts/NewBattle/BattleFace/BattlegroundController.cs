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

    public float Duration { get { return battlegroundLooper.Duration; } }

    private AbstractBattlegroundLooper battlegroundLooper;
    private int currentStep;

    private const string BasePath = "AssetBundles/Prefabs/NewBattle/Battleground";
    private const string AudioBattleNormalPathBase = "Sounds/level_normal_";
    private const string AudioBattleBossPath = "Sounds/level_boss";

    private AudioClip normalClip;
    private AudioClip bossClip;

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
        var battle = ResoucesManager.Instance.Load<GameObject>(path);
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

        InitializeSounds();

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
            audio.clip = normalClip;
            audio.Play();
            battlegroundLooper.PlayBegin();
        }
        else if(currentStep == TotalStep - 1)
        {
            audio.Stop();
            audio.PlayOneShot(bossClip);
            battlegroundLooper.PlayEnd();
        }
        else
        {
            if (!audio.isPlaying)
            {
                audio.clip = normalClip;
                audio.Play();
            }
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
        battlegroundLooper = null;

        CleanupSounds();

        // with memeory strategy enabled will call unload unused assets over there.
        if (!MemoryStrategy.Instance.Enabled)
        {
            Resources.UnloadUnusedAssets();
        }
    }

    private void InitializeSounds()
    {
        var audioIndex = GetAudioIndex();
        var normalClipSoundPath = string.Format("{0}{1}", AudioBattleNormalPathBase, audioIndex);
        normalClip = Resources.Load<AudioClip>(normalClipSoundPath);
        if (normalClip == null)
        {
            Logger.LogWarning("Level normal sounds does not load successfully on path: " + normalClipSoundPath);
        }

        bossClip = Resources.Load<AudioClip>(AudioBattleBossPath);
        if (bossClip == null)
        {
            Logger.LogWarning("Level normal sounds does not load successfully on path: " + AudioBattleBossPath);
        }
    }

    private void CleanupSounds()
    {
        audio.clip = null;
        normalClip = null;
        bossClip = null;
    }

    /// <summary>
    /// Get audio index by somehow strategy
    /// </summary>
    /// <returns>Audio index one based.</returns>
    /// <remarks>
    /// Temperary way fix with designer requirements.
    /// Later you could change this method to find correct audio clip index.
    /// </remarks>
    private int GetAudioIndex()
    {
        if (BattleID == 8)
        {
            return 1;
        }

        if (BattleID == 9)
        {
            return 2;
        }

        if (BattleID == 2)
        {
            return 3;
        }

        return Random.Range(1, 3);
    }
}

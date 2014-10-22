using com.kx.sglm.gs.battle.share.data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Character base game object.
/// </summary>
[Serializable]
public class Character : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Character state type.
    /// </summary>
    /// <remarks>
    /// Idle to Hurt should be normal state that applied to all characters.
    /// Special state when nine attack is ongoing, some characters have.
    /// SP state is just for state when SP is ongoing, no animations for this
    /// state for now, just a tag for a special case like audio playing.
    /// </remarks>
    public enum State
    {
        Idle = 0,
        Run,
        Attack,
        Hurt,
        Special,    // which is nine attack
        Super,
    }

    /// <summary>
    /// One dimenstion index base on character arrangement.
    /// </summary>
    public int Index;

    /// <summary>
    /// Identifier pointing to specific character.
    /// </summary>
    /// <remarks>Refers to CharacterPoolManager.</remarks>
    public int IDIndex;

    /// <summary>
    /// Two dimension position base on character arrangement.
    /// </summary>
    public Position Location;

    /// <summary>
    /// Character type.
    /// </summary>
    public int Type;

    /// <summary>
    /// Color index which locates underground of each character.
    /// </summary>
    /// <remarks>Used for cross connect to the same kinds of characters</remarks>
    public int ColorIndex;

    /// <summary>
    /// Job index which indicates the job of the character. 
    /// </summary>
    public int JobIndex;

    /// <summary>
    /// Flag indicates if current character is leader.
    /// </summary>
    public bool IsLeader;

    /// <summary>
    /// Animator if any.
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// Animation if any.
    /// </summary>
    public Animation Animation;

    /// <summary>
    /// Depth basement used to manage characters draw calls.
    /// </summary>
    public int DepthBase;

    /// <summary>
    /// Flag indicates if this character could be selected.
    /// </summary>
    public bool CanSelected
    {
        get { return canSelected && ((BuffController == null) || ((BuffController != null) && BuffController.CanSelected)); }
        set { canSelected = value; }
    }

    /// <summary>
    /// Buff data.
    /// </summary>
    public CharacterBuffController BuffController;

    /// <summary>
    /// Buff bar controller.
    /// </summary>
    public BuffBarController BuffBarController;

    /// <summary>
    /// Face object like CharacterController and MonsterController.
    /// </summary>
    public GameObject FaceObject;

    /// <summary>
    /// Animated game object without face stuffes.
    /// </summary>
    public GameObject AnimatedObject;

    /// <summary>
    /// Logic data.
    /// </summary>
    [HideInInspector]
    public FighterInfo Data;

    /// <summary>
    /// Total color count which is 5 for now
    /// </summary>
    /// <remarks>The index is started from 1 to 5, not 0 based in atlas.</remarks>
    public const int TotalColorCount = 5;

    #endregion

    #region Private Fields

    private const int NeighborDistance = 1;
    private List<string> animationList;
    private bool canSelected = true;

    private const string AttackSoundBase = "Sounds/attack_job";
    private static List<AudioClip> attackAudio;

    private const string AttackSuperBase = "Sounds/attack_super";
    private static AudioClip spAudio;

    #endregion

    #region Public Methods

    /// <summary>
    /// Check if the other character is the neighbor of current character.
    /// </summary>
    /// <param name="otherCharacter">The other character</param>
    /// <returns>Flag indicates if the other character is the neighbor of current character</returns>
    public virtual bool IsNeighborhood(Character otherCharacter)
    {
        var sameColor = ColorIndex == otherCharacter.ColorIndex;
        var xDistance = Math.Abs(Location.X - otherCharacter.Location.X);
        var yDistance = Math.Abs(Location.Y - otherCharacter.Location.Y);
        return otherCharacter.CanSelected && sameColor && Math.Max(xDistance, yDistance) == NeighborDistance;
    }

    /// <summary>
    /// Play character state.
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="loop">Flag indicates if state is in loop mode</param>
    public void PlayState(State state, bool loop)
    {
        PlaySounds(state);

        // WARNING, current only sp and special (nine attack) might not be in character animation list. Refers to State as details.
        if (!animationList.Contains(state.ToString()))
        {
            state = State.Attack;
        }

        Animation[animationList[(int)state]].wrapMode = (loop) ? WrapMode.Loop : WrapMode.Once;
        Animation.Play(animationList[(int)state]);
    }

    private void PlaySounds(State state)
    {
        if (state == State.Super)
        {
            if (spAudio != null)
            {
                audio.PlayOneShot(spAudio);
            }
        }
        else if (state == State.Attack)
        {
            var audioIndex = JobIndex - 1;
            // WARNING. audio index should be in range of 0 to attack audio count.
            if (audioIndex >= 0 && audioIndex < attackAudio.Count)
            {
                audio.PlayOneShot(attackAudio[audioIndex]);
            }
        }
    }

    /// <summary>
    /// Stop character state.
    /// </summary>
    /// <param name="state">Current state</param>
    public void StopState(State state)
    {
        Animation.Stop(animationList[(int)state]);
    }

    public void StopState()
    {
        Animation.Stop();
    }

    /// <summary>
    /// Set animation speed by state.
    /// </summary>
    /// <param name="state">Character state.</param>
    /// <param name="speed">Character speed.</param>
    public void SetSpeed(State state, float speed)
    {
        if (animationList.Contains(state.ToString()))
        {
            Animation[animationList[(int) state]].speed = speed;
        }
    }

    public void ShowBuff()
    {
        BuffController.ShowBuff();
        BuffController.ShowBuffCD(BuffBarController);
    }

    public void ResetBuff()
    {
        BuffBarController.gameObject.SetActive(false);
        BuffController.ResetBuff();
    }

    public void ShowDebuff()
    {
        BuffController.ShowDebuff();
    }

    /// <summary>
    /// Setup character depth based on depth base.
    /// </summary>
    /// <param name="increase">True on set depth base.</param>
    public void SetupDepthBase(bool increase)
    {
        var depth = (increase) ? DepthBase : (-DepthBase);
        var spriteList = AnimatedObject.transform.GetComponentsInChildren<UISprite>().ToList();
        spriteList.ForEach(sprite => sprite.depth += depth);
    }

    public static void LoadAudio()
    {
        if (attackAudio == null)
        {
            var jobCount = Enum.GetNames(typeof(HeroJob)).Count();

            attackAudio = new List<AudioClip>();
            for (var i = 0; i < jobCount; i++)
            {
                var clip = Resources.Load(AttackSoundBase + (i + 1)) as AudioClip;
                if (clip == null)
                {
                    Logger.LogWarning("Audio clip loads fail on path: " + AttackSoundBase + (i + 1));
                    continue;
                }
                attackAudio.Add(clip);
            }
        }

        if (spAudio == null)
        {
            spAudio = Resources.Load<AudioClip>(AttackSuperBase);
            if (spAudio == null)
            {
                Logger.LogWarning("Loading sp audio fail on path" + AttackSuperBase);
            }
        }
    }

    public static void CleanUpAudio()
    {
        attackAudio = null;
        spAudio = null;
    }

    /// <overrides/>
    public override string ToString()
    {
        return string.Format("Character with name - {0}, index - {1}, position - {2}, color - {3}", name, Index, Location, ColorIndex);
    }

    #endregion

    #region Private Methods

    private T GetAnimationStuff<T>() where T : Component
    {
        var animations = transform.GetComponentsInChildren<T>();
        return animations.Any() ? animations.First() : null;
    }

    #endregion

    #region Mono

    protected void Awake()
    {
        Animation = GetAnimationStuff<Animation>();
        Animator = GetAnimationStuff<Animator>();

        if (Animation != null)
        {
            animationList = new List<string>(Animation.Cast<AnimationState>().Select(item => item.name));
            if (animationList.Count != Enum.GetNames(typeof(State)).Count())
            {
                Logger.LogWarning("Animation list count: " + animationList.Count + ", should be equals to State count define from script: " + Enum.GetNames(typeof(State)).Count());
            }
        }

        if (BuffController == null)
        {
            BuffController = GetComponent<CharacterBuffController>() ??
                             gameObject.AddComponent<CharacterBuffController>();
        }

        if (transform.childCount != 1)
        {
            Logger.LogWarning("Child should only count for aniamted game object when in awake.");
            return;
        }
        AnimatedObject = transform.GetChild(0).gameObject;

        if (!gameObject.GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
        }
    }

    #endregion
}

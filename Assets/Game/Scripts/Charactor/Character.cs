using System;
using System.Collections.Generic;
using System.Linq;
using com.kx.sglm.gs.battle.share.data;
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
    public enum State
    {
        Idle = 0,
        Run,
        Attack,
        Hurt
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
    /// Color index which locates undergrand of each character.
    /// </summary>
    /// <remarks>Used for cross connect to the same kinds of characters</remarks>
    public int ColorIndex;

    /// <summary>
    /// Animator if any.
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// Animation if any.
    /// </summary>
    public Animation Animation;

    /// <summary>
    /// Flag indicates if this character could be selected.
    /// </summary>
    public bool CanSelected
    {
        get { return (BuffController == null) || ((BuffController != null) && BuffController.CanSelected); }
    }

    /// <summary>
    /// Logic data.
    /// </summary>
    [HideInInspector]
    public FighterInfo Data;

    /// <summary>
    /// Buff data.
    /// </summary>
    public CharacterBuffController BuffController;

    #endregion

    #region Private Fields

    private const int NeighborDistance = 1;

    private List<string> animationList; 

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
        Animation[animationList[(int)state]].wrapMode = (loop) ? WrapMode.Loop : WrapMode.Once;
        Animation.Play(animationList[(int)state]);
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

    public void ShowBuff()
    {
        BuffController.ShowBuff();
    }

    public void ResetBuff()
    {
        BuffController.ResetBuff();
    }

    public void ShowBuffCD(BuffBarController buffController)
    {
        BuffController.ShowBuffCD(buffController);
    }

    public void ShowDebuff()
    {
        BuffController.ShowDebuff();
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
        }

        if (BuffController == null)
        {
            BuffController = GetComponent<CharacterBuffController>() ??
                             gameObject.AddComponent<CharacterBuffController>();
        }
    }

    #endregion
}

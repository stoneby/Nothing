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

    public enum State
    {
        Idle = 0,
        Run,
        Attack,
        Hurt
    }

    //private CharacterStateMachine stateMachine;

    public Animator Animator;

    public Animation Animation;

    public int Index;

    public Position Location;

    public int ColorIndex;

    #endregion

    #region Private Fields

    private const int NeighborDistance = 1;

    private List<string> animationList; 

    #endregion

    #region Public Methods

    public virtual bool IsNeighborhood(Character draggedCharacter)
    {
        var sameColor = ColorIndex == draggedCharacter.ColorIndex;
        var xDistance = Math.Abs(Location.X - draggedCharacter.Location.X);
        var yDistance = Math.Abs(Location.Y - draggedCharacter.Location.Y);
        return sameColor && Math.Max(xDistance, yDistance) == NeighborDistance;
    }

    public override string ToString()
    {
        return string.Format("Character with name - {0}, index - {1}, position - {2}", name, Index, Location);
    }

    private T GetAnimationStuff<T>() where T : Component
    {
        var animations = transform.GetComponentsInChildren<T>();
        return animations.Any() ? animations.First() : null;
    }

    public void PlayState(State state, bool loop)
    {
        Animation[animationList[(int) state]].wrapMode = (loop) ? WrapMode.Loop : WrapMode.Once;
        Animation.Play(animationList[(int)state]);
    }

    #endregion

    #region Mono

    protected virtual void Start()
    {
        Animation = GetAnimationStuff<Animation>();
        Animator = GetAnimationStuff<Animator>();

        if (Animation != null)
        {
            animationList = new List<string>(Animation.Cast<AnimationState>().Select(item => item.name));
        }
    }

    #endregion
}

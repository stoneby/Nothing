using System;
using UnityEngine;

/// <summary>
/// Character base game object.
/// </summary>
[Serializable]
public class Character : MonoBehaviour
{
    #region Public Fields

    //private CharacterStateMachine stateMachine;

    public Animator Animator;

    public int Index;

    public Position Location;

    public int ColorIndex;

    #endregion

    #region Private Fields

    private const int NeighborDistance = 1;

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

    #endregion

    #region Mono

    protected virtual void Start()
    {
        if (Animator == null)
        {
            Logger.LogWarning("Animator is null.");
        }
    }

    #endregion
}

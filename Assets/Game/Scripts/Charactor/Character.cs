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
    public enum State
    {
        Idle = 0,
        Run,
        Attack,
        Hurt
    }

    //private CharacterStateMachine stateMachine;

    /// <summary>
    /// One dimenstion index base on character arrangement.
    /// </summary>
    public int Index;

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

    #endregion

    #region Private Fields

    private const int NeighborDistance = 1;

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
        return sameColor && Math.Max(xDistance, yDistance) == NeighborDistance;
    }

    /// <overrides/>
    public override string ToString()
    {
        return string.Format("Character with name - {0}, index - {1}, position - {2}", name, Index, Location);
    }

    #endregion
}

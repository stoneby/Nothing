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

    public int Index;

    public Position Location;

    #endregion

    #region Private Fields

    private const int NeighborDistance = 1;

    #endregion

    #region Public Methods

    public virtual bool IsNeighborhood(Character draggedCharacter)
    {
        var xDistance = Math.Abs(Location.X - draggedCharacter.Location.X);
        var yDistance = Math.Abs(Location.Y - draggedCharacter.Location.Y);
        return Math.Max(xDistance, yDistance) == NeighborDistance;
    }

    public override string ToString()
    {
        return string.Format("Character with name - {0}, index - {1}, position - {2}", name, Index, Location);
    }

    #endregion

    #region Mono

    protected virtual void Start()
    {
    }

    #endregion
}

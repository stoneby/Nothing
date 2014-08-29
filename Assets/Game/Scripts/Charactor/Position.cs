using System;
using UnityEngine;

[Serializable]
public struct Position
{
    #region public Fields

    [SerializeField]
    public int X;

    [SerializeField]
    public int Y;

    #endregion

    #region Public Methods

    public override string ToString()
    {
        return string.Format("Position: ({0}, {1})", X, Y);
    }

    public static bool operator ==(Position lhs, Position rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    public static bool operator !=(Position lhs, Position rhs)
    {
        return !(lhs == rhs);
    }

    #endregion
}

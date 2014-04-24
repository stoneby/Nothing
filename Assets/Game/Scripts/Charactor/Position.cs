using System;
using UnityEngine;

[Serializable]
public class Position
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

    #endregion
}

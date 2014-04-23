using UnityEngine;

/// <summary>
/// Character base game object.
/// </summary>
public class Character : MonoBehaviour
{
    #region Public Fields

    //private CharacterStateMachine stateMachine;

    #endregion

    #region Public Properties

    public int Index { get; set; }

    #endregion

    #region Mono

    protected virtual void Start()
    {
    }

    #endregion
}

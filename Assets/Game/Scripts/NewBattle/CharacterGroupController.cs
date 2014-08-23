using System.Collections.Generic;
using UnityEngine;

public class CharacterGroupController : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Team Formation controller.
    /// </summary>
    public TeamFormationController FormationController;

    /// <summary>
    /// Character list.
    /// </summary>
    public List<Character> CharacterList;

    /// <summary>
    /// Character generator.
    /// </summary>
    public CharacterGenerator Generator;

    /// <summary>
    /// Total number of characters.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Position list of all characters.
    /// </summary>
    public List<Vector3> PositionList
    {
        get { return FormationController.LatestPositionList; }
    }

    #endregion

    #region Private Fields

    private bool initialized;

    #endregion

    #region Public Methods

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        if (CharacterList.Count == 0)
        {
            Logger.LogWarning("Dynamic binding mode, take character from pool of number: " + Total, gameObject);

            Generator.Generate();
        }

        Total = CharacterList.Count;

        // get formation with position list count with total.
        if (PositionList.Count != Total)
        {
            Logger.LogError("Total: " + Total + " should be equal to position list count: " + PositionList.Count +
                            " of index: " + (Total - 1) + " from formation list.", gameObject);
        }
    }

    public void Cleanup()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;

        Generator.Cleanup();

        CharacterList.Clear();
    }

    #endregion
}

using System.Collections.Generic;
using UnityEngine;

public class CharacterGroupController : MonoBehaviour
{
    #region Public Fields


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
    public int Total
    {
        get { return Generator.Total; } 
        set { Generator.Total = value; }
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
            CharacterList.AddRange(Generator.CharacterList);
        }

        Total = CharacterList.Count;
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

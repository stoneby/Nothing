using UnityEngine;

/// <summary>
/// The item of menu bar.
/// </summary>
public class BarItem : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The label to show the name of the bar item.
    /// </summary>
    public UILabel NameLabel;

    /// <summary>
    /// The label to show the description of the bar item.
    /// </summary>
    public UILabel DescLabel;

    #endregion

    #region Private Fields

    private string BarItemName
    {
        set
        {
            if(NameLabel != null)
            {
                NameLabel.text = value;
            }
        }
    }

    private string DescName
    {
        set
        {
            if (DescLabel != null)
            {
                DescLabel.text = value;
            }
        }
    }

    private LanguageManager loc;

    #endregion


    #region Public Methods

    /// <summary>
    /// Initialize the current bar item.
    /// </summary>
    /// <param name="itemNameKey">The name of the bar item.</param>
    public virtual void Init(string itemNameKey)
    {
        if(loc == null)
        {
            loc = LanguageManager.Instance;
        }
        var itemName = loc.GetTextValue(itemNameKey);
        BarItemName = itemName;
        DescName = itemName;
    }

    /// <summary>
    /// Clean up the current bar item.
    /// </summary>
    public virtual void CleanUp()
    {
        BarItemName = "";
    }

    #endregion
}

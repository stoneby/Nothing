using UnityEngine;

/// <summary>
/// Blood bar controller.
/// </summary>
public class BloodBarController : MonoBehaviour
{
	#region Public Fields

    public GameObject BloodBar;
    public GameObject BloodLabel;

    /// <summary>
    /// Current value.
    /// </summary>
    public float CurrentValue;

    /// <summary>
    /// Max value.
    /// </summary>
    public float MaxValue;

	#endregion
	
    #region Private Fields

    private UISlider slider;
    private UILabel label;

    #endregion

	#region Public Methods

	/// <summary>
	/// Set blood bar data.
	/// </summary>
	/// <param name="current">Current value</param>
	/// <param name="max">Max value</param>
	public void Set(float current, float max)
	{
		CurrentValue = current;
		MaxValue = max;
	}

	/// <summary>
	/// Show or hide blood bar control.
	/// </summary>
	/// <param name="flag">If set to <c>true</c> flag.</param>
	public void Show(bool flag)
    {
		// set only if flag is opposite to bloodbar's active status.
        if (BloodBar.activeSelf != flag)
        {
			BloodBar.SetActive(flag);
			BloodLabel.SetActive(flag);
        }
    }

	/// <summary>
	/// Update blood bar value.
	/// </summary>
    /// <remarks>
    /// Please make sure current and max value is setup correctly
    /// </remarks>
    public void UpdateUI()
    {
        slider.value = CurrentValue / MaxValue;
        label.text = CurrentValue + "/" + MaxValue;
    }

	#endregion

    #region Mono

    private void Awake()
    {
        slider = BloodBar.GetComponent<UISlider>();
        label = BloodLabel.GetComponent<UILabel>();
    }

    #endregion
}

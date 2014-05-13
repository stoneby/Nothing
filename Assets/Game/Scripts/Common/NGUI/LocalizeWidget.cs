using UnityEngine;


[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/UI/LocalizeWidget")]
public class LocalizeWidget : MonoBehaviour
{
    public string Key;

    private string mLanguage;
    private LanguageManager loc;

    private void Awake()
    {
        if (string.IsNullOrEmpty(Key))
        {
            Logger.LogError("Key should not be empty, please fill it in.");
        }
    }

    // Localize the widget on start.
    private void Start()
    {
        // Reference the language manager
        loc = LanguageManager.Instance;

        // Hook up a delegate to run the localize script whenever a language was changed
        loc.OnChangeLanguage += Localize;

        // Initial localize run
        Localize();
    }

    // Force-localize the widget.
    private void Localize(LanguageManager thisLanguage = null)
    {
        var w = GetComponent<UIWidget>();
        var lbl = w as UILabel;
        var sp = w as UISprite;

        var val = loc.GetTextValue(Key);
        if (string.IsNullOrEmpty(val))
        {
            val = "Missing String";
        }

        if (lbl != null)
        {
            // If this is a label used by input, we should localize its default value instead
            var input = NGUITools.FindInParents<UIInput>(lbl.gameObject);
            if (input != null && input.label == lbl)
            {
                input.defaultText = val;
            }
            else
            {
                lbl.text = val;
            }
        }
        else if (sp != null)
        {
            sp.spriteName = val;
            sp.MakePixelPerfect();
        }

        // Set this widget's current language
        mLanguage = loc.language;
    }
}

using UnityEngine;

public static class Alert
{
    public static void Show(AssertionWindow.Type type, string title, string msg, 
        AssertionWindow.VoidDelegate confimfunc = null, AssertionWindow.VoidDelegate cancelfunc = null)
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.AssertType = type;
        assertWindow.Title = title;
        assertWindow.Message = msg;
        WindowManager.Instance.Show(typeof(AssertionWindow), true);
        if (confimfunc != null) assertWindow.OkButtonClicked += confimfunc;
        if (cancelfunc != null) assertWindow.CancelButtonClicked += cancelfunc;
    }
}

using UnityEngine;

public class CloseWindow : MonoBehaviour
{
    void OnClick()
    {
        Debug.LogWarning("On Click.");

        WindowManager.Instance.CloseLastWindowInHistory();
    }
}

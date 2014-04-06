using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    #region Coroutine

    private IEnumerator GameStart()
    {
        yield return null;

        Debug.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show(WindowType.MainMenu, true);
        WindowManager.Instance.Show(WindowType.BackgroundFills, true);
    }

    #endregion

    #region Mono

    void Start()
    {
        StartCoroutine(GameStart());
    }

    #endregion
}

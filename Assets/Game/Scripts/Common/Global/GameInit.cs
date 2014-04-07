using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    #region Coroutine

    private IEnumerator GameStart()
    {
        yield return null;

        Debug.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show(typeof(MainMenuWindow), true);
        WindowManager.Instance.Show(typeof(BackgroundFillsWindow), true);
    }

    #endregion

    #region Mono

    void Start()
    {
        StartCoroutine(GameStart());
    }

    #endregion
}

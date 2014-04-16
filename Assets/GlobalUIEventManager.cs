using UnityEngine;

public class GlobalUIEventManager : MonoBehaviour
{
    #region Public Methods

    public void RegisterCallback(GameObject sender, EventDelegate eventDelegate)
    {
        
    }

    #endregion

    #region EventHandler

    void OnClick()
    {
        Debug.Log("********* i am on click, " + gameObject.name);
        if (UICamera.lastHit.transform != null)
        {
            Debug.Log("--------- last hit, " + UICamera.lastHit.transform.name);
        }
    }

    #endregion

    #region Mono

    private void Start()
    {
        UICamera.genericEventHandler = gameObject;
    }

    #endregion
}

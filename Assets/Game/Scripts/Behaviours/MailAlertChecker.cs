using System.Collections;
using KXSGCodec;
using UnityEngine;

public class MailAlertChecker : MonoBehaviour
{
    public Transform AlertIcon;

    public void OnEnter()
    {
        SendUnReadStateMsg();
        NGUITools.SetActive(AlertIcon.gameObject, MailModelLocator.Instance.UnReadCount > 0);
        StartCoroutine("MonitorMailAlert");
        MailHandler.OnMailUnReadUpdated += OnMailUnReadUpdated;
    }

    private void OnMailUnReadUpdated()
    {
        if (MailModelLocator.Instance.UnReadCount > 0)
        {
            NGUITools.SetActive(AlertIcon.gameObject, true);
        }
    }

    private static void SendUnReadStateMsg()
    {
        if (MailModelLocator.Instance.UnReadCount == 0)
        {
            var csmailUnRead = new CSMailUnreadStateMsg();
            NetManager.SendMessage(csmailUnRead, false);
        }
    }

    private IEnumerator MonitorMailAlert()
    {
        var interval = MailModelLocator.Instance.MailUpdateInterval;
        while (true)
        {
            yield return new WaitForSeconds(interval);
            SendUnReadStateMsg();
        }
    }

    public void OnExit()
    {
        StopCoroutine("MonitorMailAlert");
        MailHandler.OnMailUnReadUpdated -= OnMailUnReadUpdated;
    }
}

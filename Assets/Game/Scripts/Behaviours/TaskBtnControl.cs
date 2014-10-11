using KXSGCodec;
using UnityEngine;
using System.Collections;

public class TaskBtnControl : MonoBehaviour 
{
    private GameObject AlertIcon;

    private UIEventListener BtnUIEventListener;
    // Use this for initialization
    void Start()
    {
        AlertIcon = transform.FindChild("Sprite alert").gameObject;
        AlertIcon.SetActive(PlayerModelLocator.Instance.HasFinishedQuest);
        BtnUIEventListener = UIEventListener.Get(gameObject);
        BtnUIEventListener.onClick += ClickHandler;

        EventManager.Instance.AddListener<QuestFinishEvent>(OnFinishHandler);
    }

    private void OnFinishHandler(QuestFinishEvent e)
    {
        AlertIcon.SetActive(true);
    }

    private void ClickHandler(GameObject obj)
    {
        AlertIcon.SetActive(false);
        var msg = new CSQuest();
        NetManager.SendMessage(msg);

        //WindowManager.Instance.Show<SignWindow>(true);
    }
}

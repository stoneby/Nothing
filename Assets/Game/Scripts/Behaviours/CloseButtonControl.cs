using UnityEngine;

public class CloseButtonControl : MonoBehaviour 
{
    private UIEventListener closeBtnLis;
    private StretchItem closeBtnLine;

    public delegate void CloseWindow();
    public CloseWindow OnCloseWindow;

    private void Awake()
    {
        closeBtnLis = UIEventListener.Get(transform.Find("Button-Close").gameObject);
        closeBtnLine = transform.Find("Button-CloseLine").GetComponent<StretchItem>();
    }

    private void OnEnable()
    {
        closeBtnLis.onClick = OnClose;
        closeBtnLine.DragFinish = OnClose;
    }

    private void OnDisable()
    {
        closeBtnLis.onClick = null;
        closeBtnLine.DragFinish = null;
    }

    private void OnClose(GameObject go)
    {
        if(OnCloseWindow != null)
        {
            OnCloseWindow();
        }
    }

}

using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class AccountItemControl : MonoBehaviour {

    //private GameObject BackBtn;
    private GameObject DeleteBtn;
    private GameObject AccountLabel;

    public AccountVO Data;

    private UIEventListener LabelUIEventListener;
    private UIEventListener BtnCloseUIEventListener;

    // Use this for initialization
    void Start()
    {
        //BackBtn = transform.FindChild("Image Button").gameObject;
        DeleteBtn = transform.FindChild("Button").gameObject;
        AccountLabel = transform.FindChild("Label").gameObject;

        LabelUIEventListener = UIEventListener.Get(AccountLabel);
        BtnCloseUIEventListener = UIEventListener.Get(DeleteBtn);

        LabelUIEventListener.onClick += OnLabelClick;
        BtnCloseUIEventListener.onClick += OnDeleteButtonClick;

        if (Data != null)
        {
            Reset();
        }
    }

    public void SetData(AccountVO account)
    {
        Data = account;

        Reset();
    }

    private void Reset()
    {
        if (AccountLabel != null)
        {
            var lb = AccountLabel.GetComponent<UILabel>();
            lb.text = Data.Account;


        }
    }

    public void Destory()
    {
        if (LabelUIEventListener != null) LabelUIEventListener.onClick -= OnLabelClick;
        if (BtnCloseUIEventListener != null) BtnCloseUIEventListener.onClick -= OnDeleteButtonClick;
    }

    private void OnDeleteButtonClick(GameObject game)
    {
        var e = new SelectAccountEvent();
        e.type = SelectAccountEvent.TYPE_DELETE;
        e.account = Data;
        EventManager.Instance.Post(e);
    }

    private void OnLabelClick(GameObject game)
    {
        var e = new SelectAccountEvent();
        e.type = SelectAccountEvent.TYPE_CHANGE;
        e.account = Data;
        EventManager.Instance.Post(e);
    }
}

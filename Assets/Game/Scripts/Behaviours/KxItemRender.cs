using UnityEngine;
using System.Collections;

public abstract class KxItemRender : MonoBehaviour
{
    public delegate void OnSelectedCallback(GameObject obj);

    public OnSelectedCallback OnSelected;

    private UIEventListener BtnClickUIEventListener;

    public int ItemIndex = -1;

    private bool HaveNotInit = true;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitItem()
    {
        if (HaveNotInit)
        {
            HaveNotInit = false;
            BtnClickUIEventListener = UIEventListener.Get(gameObject);
            BtnClickUIEventListener.onClick += OnItemClick;
        }
    }

    public abstract void SetData<T>(T data);

    private void OnItemClick(GameObject game)
    {
        if (OnSelected != null)
        {
            OnSelected(game);
        }
        


    }
}

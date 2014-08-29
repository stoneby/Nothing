using UnityEngine;
using System.Collections;

public abstract class KxItemRender : MonoBehaviour
{
    public delegate void OnSelectedCallback(GameObject obj);

    public delegate void OnHoverCallback(GameObject go, bool state);

    public OnSelectedCallback OnSelected;
    public OnHoverCallback OnHovered;
    public OnHoverCallback OnPress;

    private UIEventListener BtnClickUIEventListener;

    public int ItemIndex = -1;

    private bool HaveNotInit = true;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitItem(bool addhover = false)
    {
        if (HaveNotInit)
        {
            HaveNotInit = false;
            BtnClickUIEventListener = UIEventListener.Get(gameObject);
            BtnClickUIEventListener.onClick += OnItemClick;
            BtnClickUIEventListener.onPress += OnItemPress;
            if (addhover)
            {
                BtnClickUIEventListener.onHover += OnItemHover;
            }
        }
    }

    public abstract void SetData<T>(T data);

    protected virtual void OnItemClick(GameObject game)
    {
        if (OnSelected != null)
        {
            OnSelected(game);
        }
    }


    private void OnItemHover(GameObject game, bool state)
    {
        if (OnHovered != null)
        {
            OnHovered(game, state);
        }
    }

    private void OnItemPress(GameObject game, bool state)
    {
        if (OnPress != null)
        {
            OnPress(game, state);
        }
    }
}

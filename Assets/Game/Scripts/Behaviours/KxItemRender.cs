using UnityEngine;
using System.Collections;

public abstract class KxItemRender : MonoBehaviour
{
    public delegate void OnSelectedCallback(GameObject obj);
    public delegate void OnResizeCallback(GameObject obj);

    public delegate void OnHoverCallback(GameObject go, bool state);

    public OnSelectedCallback OnSelected;
    public OnHoverCallback OnHovered;
    public OnHoverCallback OnPress;

    public OnResizeCallback OnResize;

    private UIEventListener BtnClickUIEventListener;

    public int ItemIndex = -1;

    private float itemHeight = 0;
    private float itemWidth = 0;

    private bool HaveNotInit = true;

    int integer;
    public float ItemHeight
    {
        get { return itemHeight; }
        set
        {
            if (itemHeight > 0 && itemHeight != value)
            {
                itemHeight = value;
                if (OnResize != null)
                {
                    OnResize(gameObject);
                }
            }
            else
            {
                itemHeight = value;
            }
        }
    }

    public float ItemWidth
    {
        get { return itemWidth; }
        set
        {
            if (itemWidth > 0 && itemWidth != value)
            {
                if (OnResize != null)
                {
                    OnResize(gameObject);
                }
            }
            itemWidth = value;
        }
    }  
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

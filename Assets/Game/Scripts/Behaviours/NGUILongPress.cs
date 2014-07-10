using UnityEngine;

public class NGUILongPress : MonoBehaviour 
{
    public UIEventListener.VoidDelegate OnLongPress;
    public UIEventListener.VoidDelegate OnNormalPress;
    public float LongClickDuration = 2f;
    private bool dragged;
    private float lastPress = -1f;

    void OnPress(bool pressed)
    {
        if (pressed)
        {
            lastPress = Time.realtimeSinceStartup;
            dragged = false;
        }
        else
        {
            //If the press time is over long click duration and the object is not be dragged, trigger long press.
            if (Time.realtimeSinceStartup - lastPress > LongClickDuration && !dragged)
            {
                if(OnLongPress != null)
                {
                    OnLongPress(gameObject);
                }
            }
        }
    }

    private void OnClick()
    {
        if (Time.realtimeSinceStartup - lastPress < LongClickDuration)
        {
            if (OnNormalPress != null)
            {
                OnNormalPress(gameObject);
            }
        }
    }

    private void OnDragStart()
    {
        dragged = true;
    }
}

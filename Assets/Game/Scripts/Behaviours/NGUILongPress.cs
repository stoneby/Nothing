using UnityEngine;

public class NGUILongPress : MonoBehaviour 
{
    public enum TriggerType
    {
        Release,
        Press
    }

    public TriggerType LongTriggerType;
    public UIEventListener.VoidDelegate OnLongPress;
    public UIEventListener.VoidDelegate OnNormalPress;
    public float LongClickDuration = 2f;
    private bool dragged;
    private float lastPress = -1f;
    private bool isInPress;

    void OnPress(bool pressed)
    {
        if (pressed)
        {
            lastPress = Time.realtimeSinceStartup;
            isInPress = true;
            dragged = false;
            if (LongTriggerType == TriggerType.Press)
            {
                Invoke("TriggerLongPress", LongClickDuration);
            }
        }
        else
        {
            isInPress = false;
            //If the press time is over long click duration and the object is not be dragged, trigger long press.
            if (Time.realtimeSinceStartup - lastPress > LongClickDuration && !dragged)
            {
                if(LongTriggerType == TriggerType.Release && OnLongPress != null)
                {
                    OnLongPress(gameObject);
                }
            }
        }
    }

    private void TriggerLongPress()
    {
        if (!dragged && isInPress)
        {
            OnLongPress(gameObject);
        }
    }

    private void OnClick()
    {
        isInPress = false;
        if (Time.realtimeSinceStartup - lastPress < LongClickDuration)
        {
            CancelInvoke("TriggerLongPress");
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

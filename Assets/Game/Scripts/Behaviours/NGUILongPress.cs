using UnityEngine;

public class NGUILongPress : MonoBehaviour 
{
    public enum TriggerType
    {
        Release,
        Press
    }

    public bool NPressNotTriggerWhenLPress = true;

    public TriggerType LongTriggerType;
    public UIEventListener.VoidDelegate OnLongPress;
    public UIEventListener.VoidDelegate OnNormalPress;
    /// <summary>
    /// Only used for press type.
    /// </summary>
    public UIEventListener.VoidDelegate OnLongPressFinish;
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
            if (Time.realtimeSinceStartup - lastPress > LongClickDuration)
            {
                if(LongTriggerType == TriggerType.Release && !dragged && OnLongPress != null)
                {
                    OnLongPress(gameObject);
                }
                if (LongTriggerType == TriggerType.Press && OnLongPressFinish != null)
                {
                    OnLongPressFinish(gameObject);
                }
            }
        }
    }

    private void TriggerLongPress()
    {
        if (!dragged && isInPress && OnLongPress != null)
        {
            OnLongPress(gameObject);
        }
    }

    private void OnClick()
    {
        isInPress = false;
        if (NPressNotTriggerWhenLPress)
        {
            if (Time.realtimeSinceStartup - lastPress < LongClickDuration)
            {
                CancelInvoke("TriggerLongPress");
                if (OnNormalPress != null)
                {
                    OnNormalPress(gameObject);
                }
            }
        }
        else
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

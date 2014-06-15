﻿using UnityEngine;

public class NGUILongPress : MonoBehaviour 
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate OnLongPress;
    public VoidDelegate OnNormalPress;
    public float LongClickDuration = 2f;

    float lastPress = -1f;

    void OnPress(bool pressed)
    {
        if (pressed)
        {
            lastPress = Time.realtimeSinceStartup;
        }
        else
        {
            if (Time.realtimeSinceStartup - lastPress > LongClickDuration)
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
}
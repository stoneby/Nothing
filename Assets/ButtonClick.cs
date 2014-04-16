using UnityEngine;
using System.Collections;

public class ButtonClick : MonoBehaviour {

    void OnClick()
    {
        Debug.Log("I am clicked." + gameObject.name);
    }
}

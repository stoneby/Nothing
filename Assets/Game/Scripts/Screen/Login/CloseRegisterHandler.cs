using UnityEngine;
using System.Collections;

public class CloseRegisterHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnClick()
    {
        WindowManager.Instance.Show(typeof(LoginAccountWindow), true);
    }
}

using UnityEngine;
using System.Collections;

public abstract class KxItemRender : MonoBehaviour
{
    public int ItemIndex = -1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void SetData<T>(T data);
}

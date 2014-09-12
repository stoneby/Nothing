using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITweenTest : MonoBehaviour
{
    public List<Transform> PathList;
    public float Duration;

    private IEnumerator Move()
    {
        for (var i = 1; i < PathList.Count; ++i)
        {
            var lastTrans = PathList[i - 1];
            var currentTrans = PathList[i];
            gameObject.transform.position = lastTrans.position;
            //iTween.MoveTo(gameObject, currentTrans.position, Duration - 0.1f);
            iTween.MoveTo(gameObject, iTween.Hash("position", currentTrans.position, "easetype", iTween.EaseType.spring, "duration", Duration));
            yield return new WaitForSeconds(Duration);
        }    
    }

    void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            StartCoroutine(Move());
        }
    }
}

using UnityEngine;
using System.Collections;

public class AdvertisePlayer : MonoBehaviour
{
    public Transform Back;
    public Transform Front;

    private Vector3 offset;
    private Vector3 cachedLocal;
    private Vector3 cachedBack;
    private Vector3 cachedFront;

    private void Awake()
    {
        offset = Front.transform.localPosition - Back.transform.localPosition;
        cachedLocal = transform.localPosition;
        cachedBack = Back.localPosition;
        cachedFront = Front.localPosition;
    }

    public void Reset()
    {
        transform.localPosition = cachedLocal;
        Back.localPosition = cachedBack;
        Front.localPosition = cachedFront;
    }

    public void Play()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", transform.localPosition + offset,
                                  "islocal", true,
                                  "oncomplete", "OnComplete"));
    }
    
    public void Stop()
    {
        StopAllCoroutines();
    }

    public void OnComplete()
    {
        StartCoroutine(PrepareForNext());
    }

    private IEnumerator PrepareForNext()
    {
        Front.transform.localPosition -= 2 * offset;
        var t = Front;
        Front = Back;
        Back = t;
        yield return new WaitForSeconds(2);
        Play();
    }
}

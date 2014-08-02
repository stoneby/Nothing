using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AdvertisePlayer : MonoBehaviour
{
    public List<Texture> Textures;
    public int StartPos;
    public float MoveTime = 1.0f;
    public float IntervalTime = 2.0f;

    public UITexture Back;
    public UITexture Front;

    private Vector3 offset;
    private Vector3 cachedLocal;
    private Vector3 cachedBack;
    private Vector3 cachedFront;
    private int backIndex;

    private int GetIndex(int pos)
    {
        return pos % Textures.Count;
    }

    private void Awake()
    {
        offset = Front.transform.localPosition - Back.transform.localPosition;
        cachedLocal = transform.localPosition;
        cachedBack = Back.transform.localPosition;
        cachedFront = Front.transform.localPosition;

        backIndex = GetIndex(StartPos + 1);
        Front.mainTexture = Textures[GetIndex(StartPos)];
        Back.mainTexture = Textures[backIndex];
    }

    public void Reset()
    {
        transform.localPosition = cachedLocal;
        Back.transform.localPosition = cachedBack;
        Front.transform.localPosition = cachedFront;
    }

    public void Play()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", transform.localPosition + offset,
                                  "islocal", true,
                                  "time", MoveTime,
                                  "oncomplete", "OnComplete"));
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    public void Init(List<Texture> textures, int startIndex = 0)
    {
        Textures = textures;
        StartPos = startIndex;
    }

    private void OnComplete()
    {
        StartCoroutine(PrepareForNext());
    }

    private IEnumerator PrepareForNext()
    {
        Front.transform.localPosition -= 2 * offset;
        var t = Front;
        Front = Back;
        Back = t;
        backIndex = GetIndex(backIndex + 1);
        Back.mainTexture = Textures[backIndex];
        yield return new WaitForSeconds(IntervalTime);
        Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCardEffectController : MonoBehaviour
{
    public List<GameObject> Particles; 
    public float Effect2StartDelay = 2f;
    public float CardMoveInterval = 1f;
    public float ExploreDelay = 0.15f;
    public List<Vector3> Positions; 
    public delegate void CardMoveComplete();
    public delegate void EffectComplete();
    public CardMoveComplete OnCardMoveComplete;
    public EffectComplete OnEffectComplete;
    private readonly List<EffectController> effectControllerList = new List<EffectController>();

    public void Init()
    {
        foreach (var system in Particles)
        {
            var effectController = GetEffectControl(system);
            effectControllerList.Add(effectController);
            NGUITools.SetActive(effectController.gameObject, false);
        }  
    }

    private EffectController GetEffectControl(GameObject system)
    {
        var child = NGUITools.AddChild(gameObject, system);
        var setRenderQueue = child.GetComponent<SetRenderQueue>() ??
                             child.AddComponent<SetRenderQueue>();
        setRenderQueue.RenderQueue = RenderQueue.Overlay;

        return child.GetComponent<EffectController>() ?? child.AddComponent<EffectController>();
    }

    public void Play()
    {
        Init();
        StopCoroutine("DoPlay");
        StartCoroutine("DoPlay");
    }

    public void CleanMainEffect()
    {
        if (effectControllerList.Count > 0)
        {
            Destroy(effectControllerList[0].gameObject);
            effectControllerList.RemoveAt(0);
        }
    }

    private void PlayEffect(EffectController effectController)
    {
        NGUITools.SetActive(effectController.gameObject, true);
        effectController.Play(true);
    }

    private IEnumerator DoPlay()
    {
        PlayEffect(effectControllerList[0]);
        PlayEffect(effectControllerList[1]);
        yield return new WaitForSeconds(Effect2StartDelay);
        PlayEffect(effectControllerList[2]);
        if (Positions.Count * CardMoveInterval >= effectControllerList[2].Duration)
        {
            NGUITools.SetActive(effectControllerList[3].gameObject, true);
            foreach (var pos in Positions)
            {
                PlayCardMove(effectControllerList[0].transform.localPosition, pos);
                yield return new WaitForSeconds(CardMoveInterval);
                effectControllerList[4].transform.position = effectControllerList[3].transform.position;
                PlayEffect(effectControllerList[4]);
                yield return new WaitForSeconds(ExploreDelay);
                if(OnCardMoveComplete != null)
                {
                    OnCardMoveComplete();
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(effectControllerList[2].Duration * 0.21f);
        }
        //Wait to complete.
        yield return new WaitForSeconds(0.1f);
        if(OnEffectComplete != null)
        {
            OnEffectComplete();
        }
        CleanUp();
    }

    private void CleanUp()
    {
        var count = effectControllerList.Count - 1;
        for (var i = count; i >= 1; i--)
        {
            NGUITools.Destroy(effectControllerList[i].gameObject);
            effectControllerList.RemoveAt(i);
        }
    }

    public void PlayCardMove(Vector3 positionFrom, Vector3 positionTo)
    {
        effectControllerList[3].transform.localRotation = Quaternion.identity;
        effectControllerList[3].transform.localPosition = positionFrom;
        iTween.MoveTo(effectControllerList[3].gameObject,
                      iTween.Hash("position", positionTo, "time", CardMoveInterval, "isLocal", true));
        
    }
}

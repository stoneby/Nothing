using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffController : Singleton<BuffController>
{
    /// <summary>
    /// Buff type
    /// </summary>
    /// <remarks>1 based according to server logic.</remarks>
    public enum BuffType
    {
        Poison = 1,
        Palsy,
        Sleep,
        Burn,
        Freeze,
        Petrify,
        Seal,
        Delay,
        IncreaseData,
        Relieve
    }

    public List<GameObject> BuffOnceList;
    public List<GameObject> BuffLoopList;

    private Dictionary<GameObject, Dictionary<BuffType, GameObject>> characterBuffMap;

    public float ShowIntroduce(BuffType type, GameObject character)
    {
        return Show(BuffOnceList, type, character, false);
    }

    public void ShowLoop(BuffType type, GameObject character)
    {
        Show(BuffLoopList, type, character, true);
    }

    public void Show(BuffType type, GameObject character)
    {
        StartCoroutine(DoShow(type, character));
    }

    public void Stop(BuffType type, GameObject character)
    {
        var buffMap = characterBuffMap.ContainsKey(character) ? characterBuffMap[character] : new Dictionary<BuffType, GameObject>();
        if (buffMap.ContainsKey(type))
        {
            var buffController = buffMap[type].GetComponent<EffectController>() ?? buffMap[type].AddComponent<EffectController>();
            buffController.Stop();
        }
    }

    private IEnumerator DoShow(BuffType type, GameObject character)
    {
        var duration = ShowIntroduce(type, character);
        yield return new WaitForSeconds(duration);
        ShowLoop(type, character);
    }

    private float Show(IList<GameObject> buffList, BuffType type, GameObject character, bool loop)
    {
        var buffMap = characterBuffMap.ContainsKey(character) ? characterBuffMap[character] : new Dictionary<BuffType, GameObject>();
        var buff = buffMap.ContainsKey(type) ? buffMap[type] : Instantiate(buffList[(int) type - 1]) as GameObject;
        // make render queue higher than default in NGUI.
        var renderQueue = buff.GetComponent<SetRenderQueue>() ?? buff.AddComponent<SetRenderQueue>();
        renderQueue.renderQueue = RenderQueue.Overlay;
        // make buff child on character.
        buff.transform.parent = character.transform;
        buff.transform.position = character.transform.position;
        buff.transform.localScale = Vector3.one;
        // play buff through buff controller.
        var buffController = buff.GetComponent<EffectController>() ?? buff.AddComponent<EffectController>();
        buffController.Play(loop);

        return buffController.Duration;
    }

    private void Awake()
    {
        if (BuffOnceList == null || BuffLoopList == null)
        {
            Logger.LogError("BuffList should not be null.");
            return;
        }

        var buffTypeSize = Enum.GetNames(typeof(BuffType)).Count();
        var buffOnceListSize = BuffOnceList.Count;
        var buffLoopListSize = BuffLoopList.Count;
        if (buffTypeSize != buffOnceListSize || buffTypeSize != buffLoopListSize)
        {
            Logger.LogError("Buff type size from BuffType: " + buffTypeSize + " should be the same as BuffOnceList and BuffLoopList count: " + buffOnceListSize);
            return;
        }

        characterBuffMap = new Dictionary<GameObject, Dictionary<BuffType, GameObject>>();
    }
}

using System.Collections;
using System.Collections.Generic;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data.record;
using UnityEngine;

/// <summary>
/// Next character color manager.
/// </summary>
/// <remarks>Display as colored circles on the left part of screen</remarks>
public class NextFootManager : MonoBehaviour
{
    /// <summary>
    /// Next color prefab.
    /// </summary>
    public GameObject FootPrefab;

    private GameObject leftContainer;
    private PointRecord[] leftFootIndexes;

    public int Size;
    public GameObject FootContainer;

    private List<GameObject> footList;
    private bool initialized;

    void Awake()
    {
        Intialize();
    }

    /// <summary>
    /// Intialize everything well.
    /// </summary>
    public void Intialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (Size <= 0)
        {
            Logger.LogError("Size is zero, there is nothing foot prefab to spawn with.");
            return;
        }

        if (FootContainer == null)
        {
            Logger.LogError("FootContainer should not be null, please take a look.");
            return;
        }

        if (footList == null)
        {
            footList = new List<GameObject>();
        }

        for (var i = 0; i < Size; ++i)
        {
            var footObject = NGUITools.AddChild(FootContainer, FootPrefab);
            footList.Add(footObject);
        }
    }

    /// <summary>
    /// Reset everything to inital status.
    /// </summary>
    public void Reset()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;
        footList.Clear();
    }

    public int GetNext()
    {
        return BattleModelLocator.Instance.GetNext().Color;

        if (leftFootIndexes == null)
        {
            return BattleModelLocator.Instance.GetNextFromNextList(0).Color;
        }

        var theindex = (leftFootIndexes[0] == null) ? BattleModelLocator.Instance.GetNextFromNextList(1).Color : leftFootIndexes[0].Color;

        leftFootIndexes[0] = (leftFootIndexes[1] == null) ? BattleModelLocator.Instance.GetNextFromNextList(2) : leftFootIndexes[1];
        leftFootIndexes[1] = (leftFootIndexes[2] == null) ? BattleModelLocator.Instance.GetNextFromNextList(3) : leftFootIndexes[2];
        leftFootIndexes[2] = BattleModelLocator.Instance.GetNextFromNextList(4);

        var obj = footList[1] as GameObject;
        var tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(265 - 65, -32, 0);
        tp.to = new Vector3(265, -32, 0);
        tp.duration = .1f;
        tp.Play(true);

        obj = footList[2] as GameObject;
        tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(265 - 130, -32, 0);
        tp.to = new Vector3(265 - 65, -32, 0);
        tp.duration = .1f;
        tp.Play(true);

        obj = footList[0] as GameObject;
        footList.RemoveAt(0);
        footList.Add(obj);
        var sp = obj.GetComponent<UISprite>();
        sp.spriteName = "pck_" + leftFootIndexes[2].Color.ToString();
        tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(-100, -32, 0);
        tp.to = new Vector3(265 - 65 * 2, -32, 0);
        tp.duration = .2f;
        tp.Play(true);

        obj = NGUITools.AddChild(leftContainer, FootPrefab);
        sp = obj.GetComponent<UISprite>();
        sp.spriteName = "pck_" + theindex.ToString();
        tp = obj.GetComponent<TweenPosition>();
        float v = 100;//Random.Range (0, 150);
        tp.from = new Vector3(265, -32, 0);
        tp.to = new Vector3(365 + v, -32, 0);
        tp.duration = 0.8f;
        tp.Play(true);

        var ta = obj.AddComponent<TweenAlpha>();
        ta.delay = 0.2f;
        ta.from = 1;
        ta.to = 0;
        ta.duration = 0.3f;
        ta.PlayForward();

        var ts = obj.AddComponent<TweenScale>();
        ts.from = new Vector3(1, 1, 1);
        v = 2.0f;//Random.Range (2.0f, 4.0f);
        ts.to = new Vector3(v, v, 1);
        ts.duration = 0.4f;
        ts.PlayForward();
        Destroy(obj, 0.8f);

        return theindex;
    }
}

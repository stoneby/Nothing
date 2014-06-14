using System.Collections;
using System.Collections.Generic;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data.record;
using UnityEngine;

public class NextFootManager : MonoBehaviour
{
    public GameObject NextPrefab;

    private GameObject leftContainer;
    private List<GameObject> leftObjs;
    private ArrayList rightObjs;
    private PointRecord[] leftFootIndexes;
    //private bool haveInit;

    // Use this for initialization
    void Awake()
    {
        leftContainer = GameObject.Find("Anchor-topleft");
        GameObject.Find("Anchor-topright");

        leftFootIndexes = new PointRecord[3];
        leftObjs = new List<GameObject>();
        for (var i = 0; i < leftFootIndexes.Length; i++)
        {
            leftFootIndexes[i] = null;
            var obj = NGUITools.AddChild(leftContainer, NextPrefab);
            leftObjs.Add(obj);
        }
    }

    private void InitTest()
    {
        for (int i = 0; i < leftFootIndexes.Length; i++)
        {
            leftFootIndexes[i] = null;//BattleModelLocator.Instance.GetNextFromNextList();

            var obj = leftObjs[i] as GameObject;
            obj.SetActive(true);
            var sp = obj.GetComponent<UISprite>();
            //sp.spriteName = "pck_" + leftFootIndexes[i].ToString();
            var tp = obj.GetComponent<TweenPosition>();
            tp.from = new Vector3(-100, -32, 0);
            tp.to = new Vector3(265 - 65 * i, -32, 0);
            tp.duration = .5f;
            tp.Play(true);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < leftFootIndexes.Length; i++)
        {
            leftFootIndexes[i] = null;
        }
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

        var obj = leftObjs[1] as GameObject;
        var tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(265 - 65, -32, 0);
        tp.to = new Vector3(265, -32, 0);
        tp.duration = .1f;
        tp.Play(true);

        obj = leftObjs[2] as GameObject;
        tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(265 - 130, -32, 0);
        tp.to = new Vector3(265 - 65, -32, 0);
        tp.duration = .1f;
        tp.Play(true);

        obj = leftObjs[0] as GameObject;
        leftObjs.RemoveAt(0);
        leftObjs.Add(obj);
        var sp = obj.GetComponent<UISprite>();
        sp.spriteName = "pck_" + leftFootIndexes[2].Color.ToString();
        tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(-100, -32, 0);
        tp.to = new Vector3(265 - 65 * 2, -32, 0);
        tp.duration = .2f;
        tp.Play(true);

        obj = NGUITools.AddChild(leftContainer, NextPrefab);
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

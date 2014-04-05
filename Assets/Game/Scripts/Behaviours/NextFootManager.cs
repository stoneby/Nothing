using System.Collections;
using UnityEngine;

public class NextFootManager : MonoBehaviour
{
    public GameObject NextPrefab;

    private GameObject leftContainer;
    private ArrayList leftObjs;
    private ArrayList rightObjs;
    private int[] leftFootIndexes;
    private bool haveInit;

    // Use this for initialization
    void Start()
    {
        leftContainer = GameObject.Find("Anchor-topleft");
        GameObject.Find("Anchor-topright");

        leftFootIndexes = new int[3];
        leftObjs = new ArrayList();
        for (var i = 0; i < leftFootIndexes.Length; i++)
        {
            var obj = NGUITools.AddChild(leftContainer, NextPrefab);
            leftObjs.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!haveInit)
        {
            StartCoroutine(Init());
            haveInit = true;
        }
    }

    IEnumerator Init()
    {
        for (int i = 0; i < leftFootIndexes.Length; i++)
        {
            yield return new WaitForSeconds(.5f);
            leftFootIndexes[i] = Random.Range(0, 4);

            var obj = leftObjs[i] as GameObject;
            obj.SetActive(true);
            var sp = obj.GetComponent<UISprite>();
            sp.spriteName = "flag_" + leftFootIndexes[i].ToString();
            var tp = obj.GetComponent<TweenPosition>();
            tp.from = new Vector3(-100, -32, 0);
            tp.to = new Vector3(265 - 65 * i, -32, 0);
            tp.duration = .5f;
            tp.Play(true);
        }
    }

    public int GetNext()
    {
        if (!haveInit || leftFootIndexes == null)
        {
            return Random.Range(0, 4);
        }
        var theindex = leftFootIndexes[0];

        leftFootIndexes[0] = leftFootIndexes[1];
        leftFootIndexes[1] = leftFootIndexes[2];
        leftFootIndexes[2] = Random.Range(0, 5);

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
        sp.spriteName = "flag_" + leftFootIndexes[2].ToString();
        tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(-100, -32, 0);
        tp.to = new Vector3(265 - 65 * 2, -32, 0);
        tp.duration = .2f;
        tp.Play(true);

        obj = NGUITools.AddChild(leftContainer, NextPrefab);
        sp = obj.GetComponent<UISprite>();
        sp.spriteName = "flag_" + theindex.ToString();
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

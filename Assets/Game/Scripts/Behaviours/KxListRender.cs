using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class KxListRender : MonoBehaviour
{
    private GameObject ItemPrefab;
    private IList DataPravider;

    private GameObject Item0;
    private GameObject Item1;
    private GameObject Item2;

    private List<GameObject> Items; 

    public int CurrentIndex = 0;
    private bool HaveNotInit = true;

    private int TheWidth;
    private int TheHeight;
    private float baseX1;
    private float baseX2;
    private float baseY1;
    private float baseY2;
    private float moveFlag;

    public delegate void OnSelectedCallback(GameObject obj);

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    private OnSelectedCallback OnSelected;

    public void Init(IList datas, string prefabname, int currindex, int thewidth = 0, int theheight = 0, OnSelectedCallback selectedcallback = null)
    {
        CurrentIndex = currindex;
        DataPravider = datas;
        OnSelected = selectedcallback;
        if (HaveNotInit)
        {
            ItemPrefab = Resources.Load(prefabname) as GameObject;
            Item0 = NGUITools.AddChild(gameObject, ItemPrefab);
            Item1 = NGUITools.AddChild(gameObject, ItemPrefab);
            Item2 = NGUITools.AddChild(gameObject, ItemPrefab);
            Items = new List<GameObject>();
            Items.Add(Item0);
            Items.Add(Item1);
            Items.Add(Item2);
            OldXList = new List<float>();
            OldYList = new List<float>();
            StartXList = new List<float>();
            OldXList.Add(0);
            OldXList.Add(0);
            OldXList.Add(0);
            OldYList.Add(0);
            OldYList.Add(0);
            OldYList.Add(0);
            StartXList.Add(0);
            StartXList.Add(0);
            StartXList.Add(0);
            TheWidth = thewidth;
            TheHeight = theheight;
            baseX1 = gameObject.transform.localPosition.x + (Screen.width - TheWidth) / 2;
            baseX2 = baseX1 + TheWidth;
            baseY1 = gameObject.transform.localPosition.y + (Screen.height - TheHeight) / 2;
            baseY2 = baseY1 + TheHeight;
            moveFlag = TheWidth/3;
            HaveNotInit = false;
        }

        ShowItem(0);
    }

    private void SetData(GameObject item, int theoffsetindex)
    {
        var theindex = CurrentIndex + theoffsetindex;
        item.transform.localPosition = new Vector3(TheWidth * theoffsetindex, 0, 0);
        if (theindex < 0 || theindex >= DataPravider.Count)
        {
            item.SetActive(false);
        }
        else
        {
            item.SetActive(true);
            var itemrender = item.GetComponent<KxItemRender>();
            if (itemrender.ItemIndex != theindex)
            {
                itemrender.ItemIndex = theindex;
                itemrender.SetData(DataPravider[theindex]);
            }
        }
    }

    private void ShowItem(int offset)
    {
        CurrentIndex += offset;
        int selectindex;
        if (CurrentIndex == 0)
        {
            SetData(Items[0], 0);
            SetData(Items[1], 1);
            SetData(Items[2], 2);
            selectindex = 0;
        }
        else if (CurrentIndex == DataPravider.Count - 1)
        {
            SetData(Items[0], -2);
            SetData(Items[1], -1);
            SetData(Items[2], 0);
            selectindex = 2;
        }
        else
        {
            SetData(Items[0], -1);
            SetData(Items[1], 0);
            SetData(Items[2], 1);
            selectindex = 1;
        }
        if (OnSelected != null)
        {
            OnSelected(Items[selectindex]);
        }
    }

    public void MoveLeft()
    {
        if (CurrentIndex == 0) return;
        for (int i = 0; i < 3; i++)
        {
            StartXList[i] = Items[i].transform.localPosition.x;
        }
        StartCoroutine(MoveToPos(TheWidth));
    }

    public void MoveRight()
    {
        if (CurrentIndex == DataPravider.Count - 1) return;
        for (int i = 0; i < 3; i++)
        {
            StartXList[i] = Items[i].transform.localPosition.x;
        }
        StartCoroutine(MoveToPos(-TheWidth));
    }

    // Use this for initialization
	void Start () 
    {
       
	}

    private bool isDraging = false;
    private List<float> OldXList;
    private List<float> OldYList;
    private List<float> StartXList;
    private float oldX;
    private float startX;
	// Update is called once per frame
	void Update () 
    {
        var mx = Input.mousePosition.x;
        var my = Input.mousePosition.y;

        if (Input.GetMouseButtonDown(0))
        {
            //Logger.Log("Mouse Value (" + mx + ", " + my + ")");
            if (mx > baseX1 && mx < baseX2 && my > baseY1 && my < baseY2)
            {
                isDraging = true;
                startX = oldX = mx;
                for (int i = 0; i < 3; i++)
                {
                    StartXList[i] = OldXList[i] = Items[i].transform.localPosition.x;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDraging)
        {
            isDraging = false;
            float move = mx - startX;
            if (move > 0 && move > moveFlag && CurrentIndex != 0)
            {
                StartCoroutine(MoveToPos(TheWidth));
                
            }
            else if (move < 0 && move < -moveFlag && CurrentIndex != DataPravider.Count - 1)
            {
                StartCoroutine(MoveToPos(-TheWidth));
            }
            else
            {
                StartCoroutine(MoveToPos(0));
            }
        }

        if (isDraging)
        {
            for (int i = 0; i < 3; i++)
            {
                OldXList[i] += mx - oldX;
                Items[i].transform.localPosition = new Vector3(OldXList[i], 0, 0);
            }
            oldX = mx;
        }
	}

    IEnumerator MoveToPos(float xx)
    {
        for (int i = 0; i < 3; i ++)
        {
            PlayTweenPosition(Items[i], 0.2f, Items[i].transform.localPosition, new Vector3(StartXList[i] + xx, 0, 0)); 
        }

        yield return new WaitForSeconds(0.4f);

        if (xx > 0)
        {
            if (CurrentIndex != 1)
            {
                var item = Items[2];
                Items[2] = Items[1];
                Items[1] = Items[0];
                Items[0] = item;
                
            }
            ShowItem(-1);
        }
        else if (xx < 0)
        {
            if (CurrentIndex != DataPravider.Count - 2)
            {
                var item = Items[0];
                Items[0] = Items[1];
                Items[1] = Items[2];
                Items[2] = item;
                
            }
            ShowItem(1);
        }
    }

    void PlayTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenPosition ts = obj.AddComponent<TweenPosition>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }
}

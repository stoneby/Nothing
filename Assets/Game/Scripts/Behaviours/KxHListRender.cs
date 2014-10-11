using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KxHListRender : MonoBehaviour {

    private GameObject itemBox;
    private GameObject itemPrefab;
    private GameObject selectBox;
    private IList dataPravider;

    private List<GameObject> Items;

    private int StartIndex; //当前显示在第一个位置的指针，其前面的会一次接到队尾，其为显示列表的队头
    private int StartDataIndex;//StartIndex对应第一个数据的index
    private int EndDataIndex;//最后一个数据的index
    //    private int newIndex;

    private bool haveNotInit = true;

    private int theWidth;
    private int theHeight;
    private int itemWidth;

    private float baseX1;
    private float baseX2;

    private float baseY1;
    private float baseY2;

    private float centerX;
    private float centerY;

    public delegate void OnSelectedCallback(GameObject obj);

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    private OnSelectedCallback OnSelected;

    private bool isMoving;

    private int maxItemCount;//最大使用Item的数量

    private int minX;
    private int maxX;
    private float minItemX;
    private float maxItemX;

    public void Init(IList datas, string prefabname,
        int thewidth, int theheight, int itemwidth, int itemheight, OnSelectedCallback selectedcallback = null)
    {
        dataPravider = datas;
        if (datas == null || datas.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        //PopTextManager.PopTip("Stage count " + dataPravider.Count);
        OnSelected = selectedcallback;
        if (haveNotInit)
        {
            itemBox = transform.FindChild("Container").gameObject;
            selectBox = transform.FindChild("Container/Sprite select").gameObject;
            var selectbox = selectBox.GetComponent<UISprite>();
            selectbox.width = 114;
            selectbox.height = 114;
            
            itemBox.GetComponent<UIWidget>();
            itemPrefab = Resources.Load(prefabname) as GameObject;

            theWidth = thewidth;
            theHeight = theheight;
            itemWidth = itemwidth;

            maxItemCount = theWidth * 3 / itemWidth;

            Items = new List<GameObject>();

            baseX1 = gameObject.transform.localPosition.x - theWidth / 2;
            baseX2 = baseX1 + theWidth;
            centerX = Screen.width / 2;
            centerY = Screen.height / 2;
            baseY1 = gameObject.transform.localPosition.y - theHeight / 2;
            baseY2 = baseY1 + theHeight;
            baseX1 *= CameraAdjuster.CameraScale;
            baseX2 *= CameraAdjuster.CameraScale;
            baseY1 *= CameraAdjuster.CameraScale;
            baseY2 *= CameraAdjuster.CameraScale;

            minX = - theWidth / 2 + itemWidth / 2;
            maxX = theWidth/2 - itemWidth/2;
            minItemX = baseX1 - theWidth;
            maxItemX = baseX2 + theWidth;
            haveNotInit = false;
        }

        while (Items.Count < maxItemCount && Items.Count < dataPravider.Count)
        {
            Items.Add(NGUITools.AddChild(itemBox, itemPrefab));
            var theitem = Items[Items.Count - 1].GetComponent<KxItemRender>();
            theitem.OnSelected += OnSelecteHandler;
            theitem.InitItem();
        }

        //SystemInfo.deviceModel

        for (int j = 0; j < Items.Count; j++)
        {
            Items[j].SetActive(false);
        }

        int i;
        for (i = 0; i < Items.Count && i < dataPravider.Count; i++)
        {
            Items[i].SetActive(true);
            var theitem = Items[i].GetComponent<KxItemRender>();
            theitem.ItemIndex = i;
            EndDataIndex = i;
            theitem.SetData(dataPravider[i]);
            Items[i].transform.localPosition = new Vector3(minX + itemWidth * i, 0, 0);
        }
        StartIndex = 0;
        StartDataIndex = 0;
        //PopTextManager.PopTip("data count " + dataPravider.Count);
        if (Items.Count > 0)
        {
            OnSelecteHandler(Items[0]);
        }
    }

    private void OnSelecteHandler(GameObject obj)
    {
        isDraging = false;
        if (OnSelected != null)
        {
            OnSelected(obj);
            selectBox.SetActive(true);
            selectBox.transform.localPosition = obj.transform.localPosition;
        }
    }

    private bool isDraging;
    private float oldX;
    private float startX;
    private float startTime;

    void Update()
    {
        var mx = Input.mousePosition.x - centerX;
        var my = Input.mousePosition.y - centerY;

        if (Input.GetMouseButtonDown(0))
        {
            if (!isMoving && mx > baseX1 && mx < baseX2 && my > baseY1 && my < baseY2)
            {
                isDraging = true;
                oldX = mx;
                startX = itemBox.transform.localPosition.x;
                startTime = Time.time;
                //PopTextManager.PopTip(startTime.ToString());
            }
        }

        if (Input.GetMouseButtonUp(0) && isDraging)
        {
            isDraging = false;
            var endtime = Time.time;
            //PopTextManager.PopTip(endtime.ToString());
            var boxx = itemBox.transform.localPosition.x;
            var v = 0.25f * (boxx - startX) / (endtime - startTime);
            boxx += v;
            var maxx = boxx + dataPravider.Count * itemWidth - theWidth / 2 - itemWidth / 2;
            if (boxx > 0)
            {
                StartCoroutine(MoveToPos(0));
            }
            else if (maxx < maxX)
            {
                var tox = boxx + maxX - maxx;// * itemWidth - theWidth / 2 - 3 * itemWidth / 2;
                if (tox > 0) tox = 0;
                StartCoroutine(MoveToPos(tox));
            }
            else
            {
                StartCoroutine(MoveToPos(boxx));
            }
        }

        if (isDraging)
        {
            var v = itemBox.transform.localPosition.x + mx - oldX;
            itemBox.transform.localPosition = new Vector3(v, 0, 0);
            oldX = mx;
        }
    }

    private float GetItemPosByIndex(int theindex)
    {
        return minX + itemWidth * theindex;
    }

    private float GetItemScreenPosByIndex(int theindex)
    {
        return MoveToX + GetItemPosByIndex(theindex);
    }

    private void ResetList()
    {
        //var k = 0;
        if (dataPravider.Count <= Items.Count) return;

        if (GetItemScreenPosByIndex(StartDataIndex) > maxItemX)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var nextdataindex = EndDataIndex + 1;
                var itemobj = Items[StartIndex];
                if (nextdataindex < dataPravider.Count && GetItemScreenPosByIndex(StartDataIndex) > maxItemX)
                {
                    //PopTextManager.PopTip("向后 " + nextdataindex);
                    var theitem = itemobj.GetComponent<KxItemRender>();
                    theitem.ItemIndex = nextdataindex;
                    EndDataIndex = nextdataindex;
                    theitem.SetData(dataPravider[nextdataindex]);
                    itemobj.transform.localPosition = new Vector3(GetItemPosByIndex(nextdataindex), 0, 0);
                    StartIndex++;
                    if (StartIndex >= Items.Count) StartIndex = 0;
                    StartDataIndex++;

                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var predataindex = StartDataIndex - 1;
                var preposindex = StartIndex - 1;
                if (preposindex < 0) preposindex = Items.Count - 1;
                var itemobj = Items[preposindex];
                if (predataindex >= 0 && GetItemScreenPosByIndex(EndDataIndex) < minItemX)
                {
                    //PopTextManager.PopTip("向前 " + predataindex);
                    var theitem = itemobj.GetComponent<KxItemRender>();
                    theitem.ItemIndex = predataindex;
                    EndDataIndex--;
                    theitem.SetData(dataPravider[predataindex]);
                    itemobj.transform.localPosition = new Vector3(GetItemPosByIndex(predataindex), 0, 0);
                    StartIndex = preposindex;
                    StartDataIndex = predataindex;
                }
                else
                {
                    break;
                }
            }
        }
    }

    private float MoveToX;
    IEnumerator MoveToPos(float xx)
    {
        MoveToX = xx;
        ResetList();
        if (isMoving) yield break;
        isMoving = true;
        PlayTweenPosition(itemBox, 0.5f, itemBox.transform.localPosition, new Vector3(xx, 0, 0));

        yield return new WaitForSeconds(0.7f);
        //ResetList();
        isMoving = false;
    }

    private static void PlayTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        var ts = obj.AddComponent<TweenPosition>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }
}

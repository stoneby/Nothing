using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KxVListRender : MonoBehaviour
{
    private GameObject itemBox;
    private GameObject itemPrefab;
    private IList dataPravider;

    private List<GameObject> Items;

    private int StartIndex; //当前显示在第一个位置的指针，其前面的会一次接到队尾，其为显示列表的队头
    private int StartDataIndex;//StartIndex对应第一个数据的index
    private int EndDataIndex;//最后一个数据的index
//    private int newIndex;

    private bool haveNotInit = true;

    private int theWidth;
    private int theHeight;
    private int itemHeight;

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

    private int maxY;
    private float minItemY;
    private float maxItemY;

    public void Init(IList datas, string prefabname,
        int thewidth, int theheight, int itemwidth, int itemheight, OnSelectedCallback selectedcallback = null)
    {
        dataPravider = datas;
        OnSelected = selectedcallback;
        if (haveNotInit)
        {
            itemBox = transform.FindChild("Container").gameObject;
            itemBox.GetComponent<UIWidget>();
            itemPrefab = Resources.Load(prefabname) as GameObject;

            theWidth = thewidth;
            theHeight = theheight;
            itemHeight = itemheight;

            maxItemCount = theHeight * 3 / itemHeight;

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

            maxY = theHeight / 2 - itemHeight / 2;
            minItemY = baseY1 - theHeight;
            maxItemY = baseY2 + theHeight;
            haveNotInit = false;
        }

        while (Items.Count < maxItemCount && Items.Count < dataPravider.Count)
        {
            Items.Add(NGUITools.AddChild(itemBox, itemPrefab));
            var theitem = Items[Items.Count - 1].GetComponent<KxItemRender>();
            theitem.OnSelected += OnSelecteHandler;
            theitem.InitItem();
        }

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
            Items[i].transform.localPosition = new Vector3(0, maxY - itemHeight * i, 0);
        }
        StartIndex = 0;
        StartDataIndex = 0;
        //PopTextManager.PopTip("data count " + dataPravider.Count);
    }

    private void OnSelecteHandler(GameObject obj)
    {
        isDraging = false;
        if (OnSelected != null)
        {
            OnSelected(obj);
        }
    }

    private bool isDraging;
    private float oldY;
    private float startY;
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
                oldY = my;
                startY = itemBox.transform.localPosition.y;
                startTime = Time.time;
                //PopTextManager.PopTip(startTime.ToString());
            }
        }

        if (Input.GetMouseButtonUp(0) && isDraging)
        {
            isDraging = false;
            var endtime = Time.time;
            //PopTextManager.PopTip(endtime.ToString());
            var boxy = itemBox.transform.localPosition.y;
            var v = 0.25f * (boxy - startY)/(endtime - startTime);
            boxy += v;
            var maxy = boxy - dataPravider.Count * itemHeight + theHeight / 2;
            if (boxy < 0)
            {
                StartCoroutine(MoveToPos(0));
            }
            else if (maxy > -theHeight / 2)
            {
                var toy = dataPravider.Count * itemHeight - theHeight / 2 - 3 * itemHeight / 2;
                if (toy < 0) toy = 0;
                StartCoroutine(MoveToPos(toy));
            }
            else
            {
                StartCoroutine(MoveToPos(boxy));
            }
            
        }

        if (isDraging)
        {
            var v = itemBox.transform.localPosition.y + my - oldY;
            itemBox.transform.localPosition = new Vector3(0, v, 0);
            oldY = my;
        }
    }

    private float GetItemPosByIndex(int theindex)
    {
        return maxY - itemHeight*theindex;
    }

    private float GetItemScreenPosByIndex(int theindex)
    {
        return MoveToY + GetItemPosByIndex(theindex);
    }

    private void ResetList()
    {
        //var k = 0;
        if (dataPravider.Count <= Items.Count)return;

        if (GetItemScreenPosByIndex(StartDataIndex) > maxItemY)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var nextdataindex = EndDataIndex + 1;
                var itemobj = Items[StartIndex];
                if (nextdataindex < dataPravider.Count && GetItemScreenPosByIndex(StartDataIndex) > maxItemY)
                {
                    //PopTextManager.PopTip("向后 " + nextdataindex);
                    var theitem = itemobj.GetComponent<KxItemRender>();
                    theitem.ItemIndex = nextdataindex;
                    EndDataIndex = nextdataindex;
                    theitem.SetData(dataPravider[nextdataindex]);
                    itemobj.transform.localPosition = new Vector3(0, GetItemPosByIndex(nextdataindex), 0);
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
                if (predataindex >= 0 && GetItemScreenPosByIndex(EndDataIndex) < minItemY)
                {
                    //PopTextManager.PopTip("向前 " + predataindex);
                    var theitem = itemobj.GetComponent<KxItemRender>();
                    theitem.ItemIndex = predataindex;
                    EndDataIndex--;
                    theitem.SetData(dataPravider[predataindex]);
                    itemobj.transform.localPosition = new Vector3(0, GetItemPosByIndex(predataindex), 0);
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

    private float MoveToY;
    IEnumerator MoveToPos(float yy)
    {
        MoveToY = yy;
        ResetList();
        if (isMoving) yield break;
        isMoving = true;
        PlayTweenPosition(itemBox, 0.5f, itemBox.transform.localPosition, new Vector3(0, yy, 0));

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

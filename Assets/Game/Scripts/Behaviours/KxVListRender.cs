using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KxVListRender : MonoBehaviour
{
    private GameObject itemBox;
    private GameObject itemPrefab;
    private IList dataPravider;

    private List<GameObject> Items;

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
    private int maxIndex;
    private int maxY;

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
            theitem.SetData(dataPravider[i]);
            Items[i].transform.localPosition = new Vector3(0, maxY - itemHeight * i, 0);
        }
        maxIndex = i - 1;
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
            }
        }

        if (Input.GetMouseButtonUp(0) && isDraging)
        {
            isDraging = false;
            var boxy = itemBox.transform.localPosition.y;
            var maxy = boxy - maxIndex * itemHeight + theHeight / 2;
            if (boxy < 0)
            {
                StartCoroutine(MoveToPos(0));
            }
            else if (maxy > -theHeight / 2)
            {
                var toy = maxIndex * itemHeight - theHeight / 2 - itemHeight / 2;
                if (toy < 0) toy = 0;
                StartCoroutine(MoveToPos(toy));
            }
        }

        if (isDraging)
        {
            var v = itemBox.transform.localPosition.y + my - oldY;
            itemBox.transform.localPosition = new Vector3(0, v, 0);
            oldY = my;
        }
    }

    IEnumerator MoveToPos(float yy)
    {
        if (isMoving) yield break;
        isMoving = true;
        PlayTweenPosition(itemBox, 0.5f, itemBox.transform.localPosition, new Vector3(0, yy, 0));

        yield return new WaitForSeconds(0.7f);

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

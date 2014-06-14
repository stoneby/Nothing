using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class KxVListRender : MonoBehaviour
{
    private GameObject ItemBox;
    private UIWidget ItemBoxWidget;
    private GameObject ItemPrefab;
    private IList DataPravider;

    private List<GameObject> Items; 

    private bool HaveNotInit = true;
    
    private int TheWidth;
    private int TheHeight;
    private int ItemWidth;
    private int ItemHeight;

    private float baseX1;
    private float baseX2;

    private float baseY1;
    private float baseY2;
//    private float moveFlag;

    private float CenterX;
    private float CenterY;

    public delegate void OnSelectedCallback(GameObject obj);

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    private OnSelectedCallback OnSelected;

    private bool IsMoving = false;

    private int MaxItemCount;//最大使用Item的数量
    private int MaxIndex;
    private int MaxY;

    public void Init(IList datas, string prefabname, 
        int thewidth, int theheight, int itemwidth, int itemheight, OnSelectedCallback selectedcallback = null)
    {
        DataPravider = datas;
        OnSelected = selectedcallback;
        if (HaveNotInit)
        {
            ItemBox = transform.FindChild("Container").gameObject;
            ItemBoxWidget = ItemBox.GetComponent<UIWidget>();
            ItemPrefab = Resources.Load(prefabname) as GameObject;

            TheWidth = thewidth;
            TheHeight = theheight;
            ItemWidth = itemwidth;
            ItemHeight = itemheight;

            MaxItemCount = (int)(TheHeight * 3 / ItemHeight);

            Items = new List<GameObject>();

            baseX1 = gameObject.transform.localPosition.x - TheWidth / 2;
            baseX2 = baseX1 + TheWidth;
            CenterX = Screen.width/2;
            CenterY = Screen.height/2;
            baseY1 = gameObject.transform.localPosition.y - TheHeight / 2;
            baseY2 = baseY1 + TheHeight;
            baseX1 *= CameraAdjuster.CameraScale;
            baseX2 *= CameraAdjuster.CameraScale;
            baseY1 *= CameraAdjuster.CameraScale;
            baseY2 *= CameraAdjuster.CameraScale;

            MaxY = TheHeight / 2 - ItemHeight / 2;
            HaveNotInit = false;
        }

        while (Items.Count < MaxItemCount && Items.Count < DataPravider.Count)
        {
            Items.Add(NGUITools.AddChild(ItemBox, ItemPrefab));
            var theitem = Items[Items.Count - 1].GetComponent<KxItemRender>();
            theitem.OnSelected += OnSelecteHandler;
            theitem.InitItem();
        }

        for (int j = 0; j < Items.Count; j++)
        {
            Items[j].SetActive(false);
        }

        int i;
        for (i = 0; i < Items.Count && i < DataPravider.Count; i++)
        {
            Items[i].SetActive(true);
            var theitem = Items[i].GetComponent<KxItemRender>();
            theitem.ItemIndex = i;
            theitem.SetData(DataPravider[i]);
            Items[i].transform.localPosition = new Vector3(0, MaxY - ItemHeight * i, 0);
        }
        MaxIndex = i - 1;
        //ShowItem(0);
    }

    private void OnSelecteHandler(GameObject obj)
    {
        isDraging = false;
        if (OnSelected != null)
        {
            OnSelected(obj);
        }
    }

    private bool isDraging = false;
    private float oldY;
    private float startY;
	// Update is called once per frame
	void Update ()
	{
	    var mx = Input.mousePosition.x - CenterX;// * CameraAdjuster.CameraScale;
	    var my = Input.mousePosition.y - CenterY;// * CameraAdjuster.CameraScale;
//        xx = xx / CameraAdjuster.CameraScale;
//        yy = yy / CameraAdjuster.CameraScale;

        if (Input.GetMouseButtonDown(0))
        {
            //Logger.Log("Mouse Value (" + mx + ", " + my + ")");
            if (!IsMoving && mx > baseX1 && mx < baseX2 && my > baseY1 && my < baseY2)
            {
                isDraging = true;
                oldY = my;
                startY = ItemBox.transform.localPosition.y;
            }
        }

        if (Input.GetMouseButtonUp(0) && isDraging)
        {
            isDraging = false;
            float move = my - startY;
            var boxy = ItemBox.transform.localPosition.y;
            var maxy = boxy - MaxIndex * ItemHeight + TheHeight / 2;
            if (boxy < 0)
            {
                StartCoroutine(MoveToPos(0));
            }
            else if (maxy > -TheHeight/2)
            {
				var toy = MaxIndex * ItemHeight - TheHeight/2 - ItemHeight / 2;
				if (toy < 0)toy = 0;
				StartCoroutine(MoveToPos(toy));
            }
            else
            {
                
            }
        }

        if (isDraging)
        {
            var v = ItemBox.transform.localPosition.y + my - oldY;
            ItemBox.transform.localPosition = new Vector3(0, v, 0);
            oldY = my;
        }
	}

    IEnumerator MoveToPos(float yy)
    {
        if (IsMoving) yield break;
        IsMoving = true;
        PlayTweenPosition(ItemBox, 0.5f, ItemBox.transform.localPosition, new Vector3(0, yy, 0)); 

        yield return new WaitForSeconds(0.7f);

        IsMoving = false;
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

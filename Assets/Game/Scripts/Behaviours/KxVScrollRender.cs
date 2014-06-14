using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class KxVScrollRender : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
            CenterX = Screen.width / 2;
            CenterY = Screen.height / 2;
            baseY1 = gameObject.transform.localPosition.y - TheHeight / 2;
            baseY2 = baseY1 + TheHeight;
            baseX1 *= CameraAdjuster.CameraScale;
            baseX2 *= CameraAdjuster.CameraScale;
            baseY1 *= CameraAdjuster.CameraScale;
            baseY2 *= CameraAdjuster.CameraScale;

            MaxY = TheHeight / 2 - ItemHeight / 2;
            HaveNotInit = false;
        }

        while (Items.Count < DataPravider.Count)
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
        for (i = 0; i < DataPravider.Count; i++)
        {
            Items[i].SetActive(true);
            var theitem = Items[i].GetComponent<KxItemRender>();
            theitem.ItemIndex = i;
            theitem.SetData(DataPravider[i]);
            Items[i].transform.localPosition = new Vector3(0, 0 - ItemHeight * i, 0);
        }
        MaxIndex = i - 1;
        //ShowItem(0);
    }

    private void OnSelecteHandler(GameObject obj)
    {
        if (OnSelected != null)
        {
            OnSelected(obj);
        }
    }
}

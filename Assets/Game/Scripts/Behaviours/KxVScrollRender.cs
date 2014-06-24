using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KxVScrollRender : MonoBehaviour
{
    private GameObject itemBox;
    private GameObject itemPrefab;
    private IList dataPravider;

    private List<GameObject> items;

    private bool haveNotInit = true;

    private int itemHeight;

    public delegate void OnSelectedCallback(GameObject obj);

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    private OnSelectedCallback onSelected;

    private bool isMoving;

    public void Init(IList datas, string prefabname,
        int thewidth, int theheight, int itemwidth, int itemheight, OnSelectedCallback selectedcallback = null)
    {
        dataPravider = datas;
        onSelected = selectedcallback;
        if (haveNotInit)
        {
            itemBox = transform.FindChild("Container").gameObject;
            itemBox.GetComponent<UIWidget>();
            itemPrefab = Resources.Load(prefabname) as GameObject;

            itemHeight = itemheight;

            items = new List<GameObject>();

            haveNotInit = false;
        }

        while (items.Count < dataPravider.Count)
        {
            items.Add(NGUITools.AddChild(itemBox, itemPrefab));
            var theitem = items[items.Count - 1].GetComponent<KxItemRender>();
            theitem.OnSelected += OnSelecteHandler;
            theitem.InitItem();
        }

        for (int j = 0; j < items.Count; j++)
        {
            items[j].SetActive(false);
        }

        int i;
        for (i = 0; i < dataPravider.Count; i++)
        {
            items[i].SetActive(true);
            var theitem = items[i].GetComponent<KxItemRender>();
            theitem.ItemIndex = i;
            theitem.SetData(dataPravider[i]);
            items[i].transform.localPosition = new Vector3(0, 0 - itemHeight * i, 0);
        }
    }

    private void OnSelecteHandler(GameObject obj)
    {
        if (onSelected != null)
        {
            onSelected(obj);
        }
    }
}

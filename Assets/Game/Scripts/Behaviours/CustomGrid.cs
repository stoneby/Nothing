using UnityEngine;
using System.Collections;

public class CustomGrid : UIWidgetContainer
{
    #region Public Field

    public delegate void UpdateData(GameObject sender, int index);

    public UpdateData OnUpdate;

    public GameObject Item;

    public int CellHeight = 60;

    public int CellWidth = 700;

    public int CurIndex
    {
        get
        {
            return curIndex;
        }
    }

    public int MaxPerLine
    {
        get { return m_maxLine; }
    }

    #endregion

    #region Private Fields

    private float m_height;

    private int m_maxLine;

    private WrapItemBase[] m_cellList;

    private CustomScrollView mDrag;

    private float lastY = -1;

    private IList m_listData;

    private Vector3 defaultVec;

    private int curIndex;

    #endregion

    #region Mono

    void Awake()
    {
        defaultVec = new Vector3(0, CellHeight, 0); 

        mDrag = NGUITools.FindInParents<CustomScrollView>(gameObject);

        m_height = mDrag.GetComponent<UIPanel>().height;

        m_maxLine = Mathf.CeilToInt(m_height / CellHeight) + 1;

        m_cellList = new WrapItemBase[m_maxLine];

        CreateItem();
        lastY = mDrag.transform.localPosition.y;
    }

    void Update()
    {
        if (m_listData == null || m_listData.Count == 0)
        {
            return;
        }
        if (!Mathf.Approximately(mDrag.transform.localPosition.y, lastY))
        {
            Validate();

            lastY = mDrag.transform.localPosition.y;
        }
    }

    #endregion

    #region Private Methods

    private void UpdateBounds(int count)
    {
        var vMin = new Vector3
                       {
                           x = -transform.localPosition.x,
                           y = transform.localPosition.y - count * CellHeight,
                           z = transform.localPosition.z
                       };
        var b = new Bounds(vMin, Vector3.one);
        var pos = transform.localPosition + 0.5f * CellHeight * Vector3.up;
        b.Encapsulate(pos);

        mDrag.bounds = b;
        mDrag.UpdateScrollbars(true);
        mDrag.RestrictWithinBounds(true);
    }

    #endregion

    public void Init(IList list)
    {
        m_listData = list;
        if (m_listData == null || m_listData.Count == 0)
        {
            return;
        }
        Validate();
        UpdateBounds(m_listData.Count);
    }

    private void Validate()
    {
        Vector3 position = mDrag.panel.transform.localPosition;

        float _ver = Mathf.Max(position.y, 0);

        curIndex = Mathf.FloorToInt(_ver / CellHeight);
        int endIndex = Mathf.Min(m_listData.Count, curIndex + m_maxLine);

        int index = 0;
        for (int i = curIndex; i < curIndex + m_maxLine; i++)
        {
            var cell = m_cellList[index];

            if (i < endIndex)
            {
                cell.gameObject.SetActive(true);
                cell.transform.localPosition = new Vector3(0, i * -CellHeight, 0);
                cell.SetData(m_listData[i], i);
                if(OnUpdate != null)
                {
                    OnUpdate(cell.gameObject, i);
                }
            }
            else
            {
                cell.transform.localPosition = defaultVec;
                cell.gameObject.SetActive(false);
            }

            index++;
        }
    }

    private void CreateItem()
    {
        for (int i = 0; i < m_maxLine; i++)
        {
            var child = NGUITools.AddChild(gameObject, Item);
            child.name = "Item" + i;
            child.SetActive(true);
            var item = child.GetComponent<WrapItemBase>();
            m_cellList[i] = item;
            child.SetActive(false);
        }
    }

    public void Reset()
    {
        mDrag.ResetPosition();
        foreach (var cell in m_cellList)
        {
            cell.transform.localPosition = Vector3.zero;
        }
        Validate();
    }
}
using UnityEngine;

public class ScrollBackWhenNotFull : MonoBehaviour
{
    #region Public Fields

    public delegate void SrollUpdate();
    public SrollUpdate OnSrollUpdate;
    public UIScrollView ScrollView;
    public float SrollUpdateDetla = 10;

    #endregion

    #region Private Fields

    private UIPanel panelCached;
    private UIGrid gridCached;
    private bool restrictWithinPanelCached;

    #endregion

    #region Private Methods

    private void Awake()
    {
        panelCached = ScrollView.GetComponentInChildren<UIPanel>();
        gridCached = ScrollView.GetComponentInChildren<UIGrid>();
        ScrollView.onDragFinished += OnDragFinished;
        restrictWithinPanelCached = ScrollView.restrictWithinPanel;
    }

    private void OnDragFinished()
    {
        if(panelCached.finalClipRegion.w > gridCached.cellHeight * gridCached.GetChildList().size)
        {
            if(ScrollView.movement == UIScrollView.Movement.Vertical)
            {
                if(panelCached.clipOffset.y > 0.01)
                {
                    ScrollView.restrictWithinPanel = false;
                    var pos = panelCached.transform.localPosition + new Vector3(0, panelCached.clipOffset.y, 0);
                    SpringPanel.Begin(panelCached.gameObject, pos, 13f);
                }
                else
                {
                    ScrollView.restrictWithinPanel = restrictWithinPanelCached;
                }
            }
            else if(ScrollView.movement == UIScrollView.Movement.Horizontal)
            {
                if(panelCached.clipOffset.x < 0.01)
                {
                    ScrollView.restrictWithinPanel = false;
                    var pos = panelCached.transform.localPosition + new Vector3(panelCached.clipOffset.x, 0, 0);
                    var sp = SpringPanel.Begin(panelCached.gameObject, pos, 13f);
                    sp.onFinished = onFinished;
                }
                else
                {
                    ScrollView.restrictWithinPanel = restrictWithinPanelCached;
                }
            }
        }
        else
        {
            ScrollView.restrictWithinPanel = restrictWithinPanelCached;
        }
        if(ScrollView.movement == UIScrollView.Movement.Vertical)
        {
            if(panelCached.clipOffset.y > SrollUpdateDetla)
            {
                if(OnSrollUpdate != null)
                {
                    OnSrollUpdate();
                }
            }
        }
        else if(ScrollView.movement == UIScrollView.Movement.Horizontal)
        {
            if(panelCached.clipOffset.x < -SrollUpdateDetla)
            {
                if(OnSrollUpdate != null)
                {
                    OnSrollUpdate();
                }
            }
        }
    }

    private void onFinished()
    {
        ScrollView.restrictWithinPanel = restrictWithinPanelCached;
    }

    #endregion
}

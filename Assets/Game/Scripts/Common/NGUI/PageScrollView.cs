using UnityEngine;

/// <summary>
/// Attach this script to the container that has the objects to allow paging
/// </summary>

public class PageScrollView : MonoBehaviour
{
    public float springStrength = 8.0f;

    private UIScrollView scrollView;
    private int elementsPerPage;
    private int currentScrolledElements;
    private Vector3 startingScrollPosition;
    private UIGrid grid;

    // Use this for initialization
    void Start()
    {
        if (scrollView == null)
        {
            scrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
            if (scrollView == null)
            {
                Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " object in order to work", this);
                enabled = false;
                return;
            }

            grid = GetComponent<UIGrid>();
            //This supports horizontal and vertical movement only. 
            elementsPerPage = scrollView.movement == UIScrollView.Movement.Horizontal
                                  ? (int) (scrollView.panel.clipRange.z / grid.cellWidth)
                                  : (int) (scrollView.panel.clipRange.w / grid.cellHeight);
            currentScrolledElements = 0;
            startingScrollPosition = scrollView.panel.cachedTransform.localPosition;
        }
    }

    /// <summary>
    /// Scrolls until target position matches target panelAnchorPosition (may be the center of the panel, one of its sides, etc)
    /// </summary>	
    void MoveBy(Vector3 target)
    {
        if (scrollView != null && scrollView.panel != null)
        {
            // Spring the panel to this calculated position
            SpringPanel.Begin(scrollView.panel.cachedGameObject, startingScrollPosition - target, springStrength);
        }
    }

    public void NextPage()
    {
        if (scrollView != null && scrollView.panel != null)
        {
            currentScrolledElements += elementsPerPage;
            if (currentScrolledElements > (transform.childCount - elementsPerPage))
            {
                currentScrolledElements = (transform.childCount - elementsPerPage);
            }
            var nextScroll = scrollView.movement == UIScrollView.Movement.Horizontal
                                   ? grid.cellWidth * currentScrolledElements
                                   : -grid.cellHeight * currentScrolledElements;
            var target = scrollView.movement == UIScrollView.Movement.Horizontal
                                 ? new Vector3(nextScroll, 0.0f, 0.0f)
                                 : new Vector3(0, nextScroll, 0);
            MoveBy(target);
        }
    }

    public void PreviousPage()
    {
        if (scrollView != null && scrollView.panel != null)
        {
            currentScrolledElements -= elementsPerPage;
            if (currentScrolledElements < 0)
            {
                currentScrolledElements = 0;
            }
            var nextScroll = scrollView.movement == UIScrollView.Movement.Horizontal
                       ? grid.cellWidth * currentScrolledElements
                       : -grid.cellHeight * currentScrolledElements;
            var target = scrollView.movement == UIScrollView.Movement.Horizontal
                                 ? new Vector3(nextScroll, 0.0f, 0.0f)
                                 : new Vector3(0, nextScroll, 0);
            MoveBy(target);
        }
    }
}
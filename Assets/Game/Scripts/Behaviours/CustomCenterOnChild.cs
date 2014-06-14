//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ever wanted to be able to auto-center on an object within a draggable panel?
/// Attach this script to the container that has the objects to center on as its children.
/// </summary>
public class CustomCenterOnChild : MonoBehaviour
{
    /// <summary>
    /// Callback to be triggered when the centering operation completes.
    /// </summary>

    public SpringPanel.OnFinished onFinished;

    UIScrollView mScrollView;
    private UIGrid uiGrid;

    void OnEnable() { Recenter(); }
    void OnDragFinished() { if (enabled) Recenter(); }

    /// <summary>
    /// Recenter the draggable list on the center-most child.
    /// </summary>

    public void Recenter()
    {
        var trans = transform;
        if (trans.childCount == 0) return;

        if (uiGrid == null)
        {
            uiGrid = GetComponent<UIGrid>();
        }

        if (mScrollView == null)
        {
            mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);

            if (mScrollView == null)
            {
                Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " on a parent object in order to work", this);
                enabled = false;
                return;
            }
            mScrollView.onDragFinished = OnDragFinished;

            if (mScrollView.horizontalScrollBar != null)
                mScrollView.horizontalScrollBar.onDragFinished = OnDragFinished;

            if (mScrollView.verticalScrollBar != null)
                mScrollView.verticalScrollBar.onDragFinished = OnDragFinished;
        }
        if (mScrollView.panel == null) return;
        // Calculate the panel's center in world coordinates
        var corners = mScrollView.panel.worldCorners;
        var panelCenter = (corners[2] + corners[0]) * 0.5f;

        // Offset this value by the momentum
        var pickingPoint = panelCenter - mScrollView.currentMomentum * (mScrollView.momentumAmount * 0.1f);
        mScrollView.currentMomentum = Vector3.zero;

        var visibleItems = 0;
        if (mScrollView.movement == UIScrollView.Movement.Horizontal)
        {
            visibleItems = Mathf.FloorToInt(mScrollView.panel.width / uiGrid.cellWidth);
        }
        if (mScrollView.movement == UIScrollView.Movement.Vertical)
        {
            visibleItems = Mathf.FloorToInt(mScrollView.panel.height / uiGrid.cellHeight);
        }
        var maxPerLine = uiGrid.maxPerLine != 0
                  ? uiGrid.maxPerLine
                  : (uiGrid.arrangement == UIGrid.Arrangement.Horizontal
                         ? Mathf.FloorToInt(mScrollView.panel.width / uiGrid.cellWidth)
                         : Mathf.FloorToInt(mScrollView.panel.height / uiGrid.cellHeight));
        var closestPos = visibleItems % 2 == 0
                             ? FindClosestForEven(pickingPoint, maxPerLine, visibleItems / 2)
                             : FindClosestForOdd(pickingPoint, maxPerLine, visibleItems / 2);
        CenterOnPos(closestPos);
    }

    private void CenterOnPos(Vector3 pos)
    {
        var offset = -mScrollView.panel.cachedTransform.InverseTransformPoint(pos);
        if (!mScrollView.canMoveHorizontally) offset.x = mScrollView.panel.cachedTransform.localPosition.x;
        if (!mScrollView.canMoveVertically) offset.y = mScrollView.panel.cachedTransform.localPosition.y;
        SpringPanel.Begin(mScrollView.panel.cachedGameObject, offset, 6f);
    }

    private Vector3 FindClosestForEven(Vector3 centerPos, int maxPerLine, int half)
    {
        var trans = transform;
        var row = 0;
        var min = float.MaxValue;
        var list = new List<Vector3>();
        var totalRow = Mathf.CeilToInt((float)trans.childCount / maxPerLine) - 2;
        var offset = GetOffset(true, maxPerLine);

        for (int i = 0; i <= totalRow; i++)
        {
            var pos = trans.GetChild(i * maxPerLine).localPosition + offset;
            pos = trans.TransformPoint(pos);
            list.Add(pos);
        }
        // Determine the closest child
        for (int i = 0, imax = list.Count; i < imax; ++i)
        {
            var sqrDist = Vector3.SqrMagnitude(list[i] - centerPos);
            if (sqrDist < min)
            {
                min = sqrDist;
                row = i;
            }
        }
        row = Mathf.Clamp(row, half - 1, totalRow - half + 1);
        return list[row];
    }

    private Vector3 FindClosestForOdd(Vector3 centerPos, int maxPerLine, int half)
    {
        var row = 0;
        var min = float.MaxValue;
        var list = new List<Vector3>();
        var trans = transform;
        var totalRow = Mathf.CeilToInt((float) trans.childCount / maxPerLine) - 1;
        var offset = GetOffset(false, maxPerLine);
  
        for (var i = 0; i <= totalRow; i++)
        {
            var pos = trans.GetChild(i * maxPerLine).localPosition + offset;
            pos = trans.TransformPoint(pos);
            list.Add(pos);
        }
        // Determine the closest child
        for (int i = 0, imax = list.Count; i < imax; ++i)
        {
            var sqrDist = Vector3.SqrMagnitude(list[i] - centerPos);
            if (sqrDist < min)
            {
                min = sqrDist;
                row = i;
            }
        }
        row = Mathf.Clamp(row, half, totalRow - half);
        return list[row];
    }

    private Vector3 GetOffset(bool isEven,int maxPerLine)
    {
        Vector3 offset;
        if (isEven)
        {
            offset = maxPerLine % 2 == 0
                    ? new Vector3(uiGrid.cellWidth * (maxPerLine - 1) / 2, -uiGrid.cellHeight / 2, 0)
                    : (uiGrid.arrangement == UIGrid.Arrangement.Horizontal
                           ? new Vector3(uiGrid.cellWidth * (maxPerLine - 1) / 2, -uiGrid.cellHeight / 2, 0)
                           : new Vector3(uiGrid.cellWidth / 2, uiGrid.cellHeight * (maxPerLine - 1) / 2, 0));
        }
        else
        {
            offset = uiGrid.arrangement == UIGrid.Arrangement.Horizontal
                         ? new Vector3(uiGrid.cellWidth * (maxPerLine - 1) / 2, 0, 0)
                         : new Vector3(0, -uiGrid.cellHeight * (maxPerLine - 1) / 2, 0);
        }
        return offset;
    }
}

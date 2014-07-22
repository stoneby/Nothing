using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UICustomDragAndDropItem : UIDragDropItem
{
    /// <summary>
    /// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
    /// </summary>

    private List<Vector3> dropPostions;

    private Vector3 cachedPos;

    protected override void OnDragDropStart()
    {
        base.OnDragDropStart();
        cachedPos = transform.position;
    }

    protected override void OnDragDropRelease(GameObject surface)
    {
        if (surface != null)
        {
            var dds = surface.GetComponent<MyDragDropSurface>();
            if (dds != null)
            {
                Utils.MoveToParent(dds.ReparentTarget, transform);
                var localPos = dds.ReparentTarget.InverseTransformPoint(UICamera.lastHit.point);
                bool canDrop;
                int targetIndex;
                transform.localPosition = dds.GetClosest(localPos, out canDrop, out targetIndex);
                if (targetIndex != MyDragDropSurface.InvalidDropIndex)
                {
                    mTouchID = int.MinValue;
                    dds.StartDrop(targetIndex, transform);
                    // Re-enable the collider
                    if (mButton != null) mButton.isEnabled = true;
                    else if (mCollider != null) mCollider.enabled = true;
                    cloneOnDrag = false;
                }
                else
                {
                    NGUITools.Destroy(gameObject);
                }
                return;
            }
            var commonSurface = surface.GetComponent<UIHeroCommonSurface>();
            if (commonSurface != null)
            {
                NGUITools.Destroy(gameObject);
                return;
            }
            transform.position = cachedPos;
            mTouchID = int.MinValue;
            // Re-enable the collider
            if (mButton != null) mButton.isEnabled = true;
            else if (mCollider != null) mCollider.enabled = true;
            //base.OnDragDropRelease(surface);
        }
    }
}

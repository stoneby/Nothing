using UnityEngine;

public class StretchItem : MonoBehaviour 
{
    public enum Direction
    {
        Horizontal,
        Vertical,
    }

    public Direction Movement;

    protected UIRoot mRoot;
    private int cachedHeight;
    private int cachedWidth;
    private Vector3 cachedPos;
    private UISprite sprite;

    public delegate void DragFinished(GameObject sender);

    public DragFinished DragFinish;

    private void Awake()
    {
        mRoot = NGUITools.FindInParents<UIRoot>(transform);
        sprite = GetComponent<UISprite>();
        cachedHeight = sprite.height;
        cachedWidth = sprite.width;
        cachedPos = transform.localPosition;
    }

    /// <summary>
    /// Perform the dragging.
    /// </summary>

    void OnDrag(Vector2 delta)
    {
        OnDragDropMove((Vector3)delta * mRoot.pixelSizeAdjustment);
    }

    /// <summary>
    /// Notification sent when the drag event has ended.
    /// </summary>

    void OnDragEnd()
    {
        if (Movement == Direction.Vertical)
        {
            if (sprite.height > cachedHeight)
            {
                transform.localPosition = cachedPos;
                sprite.height = cachedHeight;
                if (DragFinish != null)
                {
                    DragFinish(gameObject);
                }
            }
        }
        else
        {
            if (sprite.width > cachedHeight)
            {
                transform.localPosition = cachedPos;
                sprite.width = cachedWidth;
                if (DragFinish != null)
                {
                    DragFinish(gameObject);
                }
            }
        }

    }

    private void OnDragDropMove(Vector3 vector3)
    {
        if (Movement == Direction.Vertical)
        {
            if (vector3.y < 0)
            {
                sprite.height -= (int)vector3.y;
                transform.localPosition = transform.localPosition + 0.5f * vector3.y * Vector3.up;
            }
        }
        else
        {
            if(vector3.x > 0)
            {
                sprite.width += (int) vector3.x;
                transform.localPosition = transform.localPosition + 0.5f * vector3.x * Vector3.left;
            }
        }

    }
}

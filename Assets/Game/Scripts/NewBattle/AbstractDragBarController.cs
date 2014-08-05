using UnityEngine;

public abstract class AbstractDragBarController : MonoBehaviour
{
    protected float Factor;

    /// <summary>
    /// Set drag bar width.
    /// </summary>
    public abstract void SetWidth(float width);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public void SetWidth(Vector3 source, Vector3 target)
    {
        var width = Mathf.Abs(Vector3.Distance(source, target));
        width *= Factor;
        SetWidth(Mathf.Abs(width));
    }

    /// <summary>
    /// Set dragbar sprite.
    /// </summary>
    /// <param name="spriteName">Sprite name</param>
    public abstract void SetSprite(string spriteName);

    /// <summary>
    /// Set rotation by quaternion.
    /// </summary>
    /// <param name="quater">Quaternion</param>
    public void SetRotate(Quaternion quater)
    {
        transform.localRotation = quater;
    }

    /// <summary>
    /// Set rotation in between source and target.
    /// </summary>
    /// <param name="source">Source position</param>
    /// <param name="target">Target position</param>
    public void SetRotate(Vector3 source, Vector3 target)
    {
        var quater = Utils.GetRotation(source, target);
        SetRotate(quater);
    }

    /// <summary>
    /// Set dragbar depth.
    /// </summary>
    /// <param name="depth">Depth</param>
    /// <remarks>Used for overlapped dragbar's controlling.</remarks>
    public abstract void SetDepth(int depth);

    /// <summary>
    /// Get dragbar depth.
    /// </summary>
    /// <returns>The dragbar depth.</returns>
    public abstract int GetDepth();

    protected virtual void Start()
    {
        Factor = UIRoot.GetPixelSizeAdjustment(gameObject);
        Factor *= UICamera.currentCamera.orthographicSize;
    }
}

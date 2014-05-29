using UnityEngine;

public abstract class AbstractDragBarController : MonoBehaviour
{
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
        SetWidth(Mathf.Abs(width));
    }

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
}

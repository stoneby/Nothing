using UnityEngine;

/// <summary>
/// Generator bounds to current game object according to all its children.
/// </summary>
public class BoundsGenerator : MonoBehaviour
{
    public bool IncludeInactiveChild;

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (transform.childCount == 0)
        {
            Debug.LogWarning("There is no children at all, we will not do anything.");
            return;
        }

        var bounds = transform.GetChild(0).collider.bounds;
        for (var i = 1; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);
            if (!IncludeInactiveChild && !child.gameObject.activeSelf)
            {
                continue;
            }
            bounds.Encapsulate(child.collider.bounds);
        }
        var collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        // convert world space to local space.
        // Note: bounds using world space, but box collider using local space.
        var localMin = transform.InverseTransformPoint(bounds.min);
        var localMax = transform.InverseTransformPoint(bounds.max);
        collider.center = (localMin + localMax) / 2;
        collider.size = localMax - localMin;
    }

    void Start()
    {
        Generate();
    }
}

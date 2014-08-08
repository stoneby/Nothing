using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class UnityNGUIColliderConverter : MonoBehaviour
{
    public GameObject Sprite;

    private const float ConvertScale = 100f;

    [ContextMenu("Convert")]
    public void Convert()
    {
        if (Sprite == null)
        {
            Debug.LogError("Sprite should not be null.");
            return;
        }

        ConvertPolygon();
    }

    private void ConvertPolygon()
    {
        var colliderLeft = Sprite.GetComponent<PolygonCollider2D>();
        if (colliderLeft != null)
        {
            Print(colliderLeft.points.ToList());

            var points = colliderLeft.points.Select(point => point * ConvertScale).ToList();

            var colliderRight = GetComponent<PolygonCollider2D>();
            if(colliderRight == null)
            {
                colliderRight = gameObject.AddComponent<PolygonCollider2D>();
            }
            colliderRight.points = points.ToArray();

            Print(colliderRight.points.ToList());
        }
    }

    private void Print(List<Vector2> list)
    {
        list.ForEach(item => Debug.LogWarning("Point: " + item));
    }
}

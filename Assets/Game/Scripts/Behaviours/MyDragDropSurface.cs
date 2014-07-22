using System.Collections.Generic;
using UnityEngine;

public class MyDragDropSurface : MonoBehaviour
{
    public Transform ReparentTarget;
    private readonly List<Vector3> postions = new List<Vector3>();
    private readonly Dictionary<int, Transform> dropped = new Dictionary<int, Transform>(); 
    public int Row;
    public int Column;
    public float CellWidth;
    public float CellHeight;
    public const int InvalidDropIndex = -1;

    private void Awake()
    {
        for (var i = 0; i < Row; i++)
        {
            for (var j = 0; j < Column; j++)
            {
                postions.Add(new Vector3(j * CellWidth, -i * CellHeight, 0));
            }
        }
    }

    public Vector3 GetClosest(Vector3 localPos, out bool canDrop, out int dropIndex)
    {
        float min = float.MaxValue;
        var closest = new Vector3();
        var index = 0;
        dropIndex = InvalidDropIndex;
        // Determine the closest child
        for (int i = 0, imax = postions.Count; i < imax; ++i)
        {
            Vector3 t = postions[i];
            float sqrDist = Vector3.SqrMagnitude(localPos - t);

            if (sqrDist < min)
            {
                min = sqrDist;
                closest = t;
                index = i;
            }
        }
        canDrop = !dropped.ContainsKey(index);
        if(canDrop)
        {
            dropIndex = index;
        }
        return closest;
    }

    public void StartDrop(int dropIndex, Transform tran)
    {
        dropped.Add(dropIndex, tran);
    }
}

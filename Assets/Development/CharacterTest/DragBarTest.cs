using UnityEngine;
using System.Collections;

public class DragBarTest : MonoBehaviour
{
    public Transform Source;
    public Vector2 Offset;

    private void OnDrag(Vector2 delta)
    {
        //Input.mousePosition;
        Debug.Log("OnDrag" );

        var targetPosition = UICamera.currentTouch.pos;

        var sourcePosition = UICamera.mainCamera.WorldToScreenPoint(transform.position);
        transform.localRotation = Utils.GetRotation(new Vector2(sourcePosition.x, sourcePosition.y), targetPosition);

        var bar = Source.GetComponent<UIWidget>();
        bar.width = (int)Vector2.Distance(targetPosition, sourcePosition) + (int)Offset.x;
        Debug.Log("Width: " + bar.width);

        //var factor = (Screen.height / 720.0);
        //bar.width = (int)(bar.width * factor);

        //Debug.Log("Screen height : " + Screen.height + " factor: " + factor);
        Debug.Log("Width after: " + bar.width);
    }
}

using UnityEngine;

public class DragBarControllerTest : MonoBehaviour
{
    public DragBarNo1Controller Controller1;
    public DragBarNo2Controller Controller2;

    private string widthStr = string.Empty;
    private string rotateStr = string.Empty;

#if UNITY_EDITOR
    private void OnGUI()
    {
        var controller = Controller1 ?? (AbstractDragBarController) Controller2;

        widthStr = GUILayout.TextField(widthStr, 40);

        if (GUILayout.Button("Set width"))
        {
            var width = float.Parse(widthStr); 
            controller.SetWidth(width);
        }

        rotateStr = GUILayout.TextField(rotateStr, 40);
        if (GUILayout.Button("Set Rotate"))
        {
            var rotate = float.Parse(rotateStr);
            controller.SetRotate(Quaternion.Euler(0, 0, rotate));
        }
    }
#endif

}

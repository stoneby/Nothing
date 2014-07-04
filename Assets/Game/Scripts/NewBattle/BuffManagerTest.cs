using UnityEngine;

public class BuffManagerTest : MonoBehaviour
{
    public BuffManager Controller;
    public GameObject Character;

    private string indexStr = string.Empty;

#if UNITY_EDITOR
    private void OnGUI()
    {
        indexStr = (GUILayout.TextField(indexStr, 40));

        if (GUILayout.Button("Show Introduce"))
        {
            var index = (BuffManager.BuffType)int.Parse(indexStr); 
            Controller.ShowIntroduce(index, Character);
        }

        if (GUILayout.Button("Show Loop"))
        {
            var index = (BuffManager.BuffType)int.Parse(indexStr);
            Controller.ShowLoop(index, Character);
        }

        if (GUILayout.Button("Show All"))
        {
            var index = (BuffManager.BuffType)int.Parse(indexStr);
            Controller.Show(index, Character);
        }

        if (GUILayout.Button("Stop"))
        {
            var index = (BuffManager.BuffType)int.Parse(indexStr);
            Controller.Stop(index, Character);
        }
    }
#endif
}

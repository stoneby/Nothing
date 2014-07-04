using UnityEngine;

public class BuffDisplayerTest : MonoBehaviour
{
    public BuffDisplayer Displayer;
    public GameObject Parent;

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Show Intro"))
        {
            Displayer.ShowIntro(Parent);
        }

        if (GUILayout.Button("Show Loop"))
        {
            Displayer.ShowLoop(Parent);
        }

        if (GUILayout.Button("Show All"))
        {
            Displayer.Show(Parent);
        }

        if (GUILayout.Button("Stop"))
        {
            Displayer.Stop(Parent);
        }
    }
#endif
}

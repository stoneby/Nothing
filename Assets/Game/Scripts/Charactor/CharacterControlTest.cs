using UnityEngine;

public class CharacterControlTest : MonoBehaviour
{
    public Character CharacterControl;

    private string animationIndexStr = string.Empty;

#if UNITY_EDITOR
    private void OnGUI()
    {
        animationIndexStr = GUILayout.TextField(animationIndexStr, 40);
        if (GUILayout.Button("Play Animation"))
        {
            var animationIndex = (Character.State)int.Parse(animationIndexStr);
            CharacterControl.PlayState(animationIndex, true);
        }

        if (GUILayout.Button("Stop Animation"))
        {
            var animationIndex = (Character.State)int.Parse(animationIndexStr);
            CharacterControl.StopState(animationIndex);
        }
    }
#endif
}

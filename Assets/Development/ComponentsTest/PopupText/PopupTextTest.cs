using UnityEngine;

public class PopupTextTest : MonoBehaviour
{
    public GameObject Parent;

    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            PopupTextController.Instance.Show(Parent, 100);
        }
    }}

using UnityEngine;

/// <summary>
/// Game object rotator.
/// </summary>
public class Rotator : MonoBehaviour
{
    #region Public Fields

    public Camera MainCamera;
    public bool TurnOn;

    #endregion

    #region Mono

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        var targetPosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.rotation = Utils.GetRotation(gameObject.transform.position, targetPosition);
    }

    #endregion
}

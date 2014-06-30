using System.Collections;
using UnityEngine;

/// <summary>
/// 2D camera like effect
/// </summary>
/// <remarks>We operate game object scale, position to simulate 3d effect like perspective effect.</remarks>
public class CameraLikeEffect : MonoBehaviour
{
    #region Public Fields

    #region Look At

    public Transform LookAt;
    public float LookAtScale;
    public float LookAtTime;

    #endregion

    #region Shake

    public Vector3 ShakeAmount;
    public float ShakeTime;

    #endregion

    #endregion

    #region Private Fields

    private Vector3 originalPosition;
    private float originalScale;

    #endregion

    #region Public Methods

    /// <summary>
    /// Look into effect.
    /// </summary>
    public void LookInto()
    {
        var increment = -LookAt.position - gameObject.transform.position;
        iTween.MoveBy(gameObject, increment * LookAtScale, LookAtTime);
        iTween.ScaleTo(gameObject, new Vector3(LookAtScale, LookAtScale, 0) , LookAtTime);
    }

    /// <summary>
    /// Look out effect.
    /// </summary>
    public void LookOut()
    {
        var increment = originalPosition - gameObject.transform.position;
        iTween.MoveBy(gameObject, increment, LookAtTime);
        iTween.ScaleTo(gameObject, new Vector3(originalScale, originalScale, 0), LookAtTime);
    }

    /// <summary>
    /// Look in and out.
    /// </summary>
    public void LookAround()
    {
        StartCoroutine(DoLookAround());
    }

    /// <summary>
    /// Shake effect.
    /// </summary>
    public void Shake()
    {
        iTween.ShakeScale(gameObject, ShakeAmount, ShakeTime);
    }

    #endregion

    #region Private Methods

    private IEnumerator DoLookAround()
    {
        LookInto();
        yield return new WaitForSeconds(LookAtTime);
        LookOut();
    }

    #endregion

    #region Mono

    private void Awake()
    {
        originalPosition = gameObject.transform.position;
        // let's assume scaling in same size on both x and y.
        originalScale = gameObject.transform.localScale.x;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Look Into"))
        {
            LookInto();
        }

        if (GUILayout.Button("Look Out"))
        {
            LookOut();
        }

        if (GUILayout.Button("Look Around"))
        {
            LookAround();
        }

        if (GUILayout.Button("Shake"))
        {
            Shake();
        }
    }
#endif

    #endregion
}

using UnityEngine;

public class ClickEffectHandler : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The camera used to get the world position of mouse position on the screen.
    /// </summary>
    public Camera MainCamera;

    /// <summary>
    /// The click particle system.
    /// </summary>
    public GameObject ClickEffectParticle;


    public bool ContinousMode;
    public float TimeInterval;
    #endregion

    #region Private Feilds

    /// <summary>
    /// Used to determine the distance of the particle system and the camera.
    /// </summary>
    private const float FrontOfCamera = 1f;
    private float beginTime;

    #endregion

    #region Private Methods

    /// <summary>
    /// Show the click effect.
    /// </summary>
    private void ShowClick()
    {
        var clickEffect = Instantiate(ClickEffectParticle) as GameObject;
        clickEffect.transform.parent = transform;
        var ps = clickEffect.GetComponent<ParticleSystem>();
        ps.Play();

        var mouseWorldPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        clickEffect.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, MainCamera.transform.position.z + FrontOfCamera);
        Destroy(clickEffect, ps.duration);
    }

    /// <summary>
    /// Called when the user has pressed the mouse button while over the collider.
    /// </summary>
    private void OnMouseDown()
    {
        ShowClick();
    }

    private void OnMouseDrag()
    {
        if (ContinousMode)
        {
            var currentTime = Time.realtimeSinceStartup;
            if (currentTime - beginTime >= TimeInterval)
            {
                ShowClick();
                beginTime = currentTime;
            }
        }
    }

    private void Start()
    {
        beginTime = Time.realtimeSinceStartup;
    }
	
    #endregion
}

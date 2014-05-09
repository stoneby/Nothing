using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoadingWaitWindow : Window
{
    #region Public Fields

    public ParticleSystem LoadingParticle;
    
    #endregion

    #region Window

    public override void OnEnter()
    {
        LoadingParticle.Play();
    }

    public override void OnExit()
    {
        LoadingParticle.Stop();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion
}

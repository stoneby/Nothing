using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class CommunitingWindow : Window
{
    public ParticleSystem CommunitingParticle;
    public float TimeOut = 3f;

    #region Window

    public override void OnEnter()
    {
        CommunitingParticle.Play();
        Invoke("ExcuteTimeOut", TimeOut);
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Private Methods

    private void ExcuteTimeOut()
    {
        WindowManager.Instance.Show<CommunitingWindow>(false);
    }

    #endregion
}

using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
    public int RenderQueue = 4000;

    Material mMat;

    void Start()
    {
        var pss = GetComponentsInChildren<ParticleSystem>();
        foreach(var ps in pss)
        {
            Renderer ren = ps.gameObject.renderer ?? ps.renderer;

            if (ren != null)
            {
                mMat = new Material(ren.sharedMaterial) {renderQueue = RenderQueue};
                ren.material = mMat;
            }
        }  
    }

    void OnDestroy() { if (mMat != null) Destroy(mMat); }
}
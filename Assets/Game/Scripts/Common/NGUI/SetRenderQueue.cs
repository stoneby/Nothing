using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
    public int renderQueue = 3000;

    Material mMat;

    void Start()
    {
        var pss = GetComponentsInChildren<ParticleSystem>();
        foreach(var ps in pss)
        {
            Renderer ren = ps.gameObject.renderer ?? ps.renderer;

            if (ren != null)
            {
                mMat = new Material(ren.sharedMaterial) {renderQueue = renderQueue};
                ren.material = mMat;
            }
        }  
    }

    void OnDestroy() { if (mMat != null) Destroy(mMat); }
}
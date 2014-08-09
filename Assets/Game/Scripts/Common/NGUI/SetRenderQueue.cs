using UnityEngine;

/// <summary>
/// Set render queue of current game obejct.
/// </summary>
/// <remarks>
/// Useful in unmanagement parts of NGUI, like particle system.
/// This may cause material leak. Please look into it.
/// </remarks>
public class SetRenderQueue : MonoBehaviour
{
    public int RenderQueue = 4000;

    private Material material;

    void Start()
    {
        var pss = GetComponentsInChildren<ParticleSystem>();
        foreach(var ps in pss)
        {
            Renderer ren = ps.gameObject.renderer ?? ps.renderer;

            ReplaceMaterial(ren);
        }

        var mrs = GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in mrs)
        {
            ReplaceMaterial(mr);
        }
    }

    private void ReplaceMaterial(Renderer ren)
    {
        if (ren != null)
        {
            material = new Material(ren.sharedMaterial) {renderQueue = RenderQueue};
            ren.material = material;
        }
    }

    void OnDestroy() { if (material != null) Destroy(material); }
}
using System.Collections.Generic;
using System.Linq;
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

    private List<ParticleSystem> particleSystemList;
    private List<MeshRenderer> meshRendererList;
    private Material material;

    public void SetQueue()
    {
        foreach (var particleSystem in particleSystemList)
        {
            var ren = particleSystem.gameObject.renderer ?? particleSystem.renderer;
            ReplaceMaterial(ren);
        }

        foreach (var meshRenderer in meshRendererList)
        {
            ReplaceMaterial(meshRenderer);
        }
    }

    private void ReplaceMaterial(Renderer ren)
    {
        if (ren != null)
        {
            material = new Material(ren.sharedMaterial) { renderQueue = RenderQueue };
            ren.material = material;
        }
    }

    void Awake()
    {
        particleSystemList = new List<ParticleSystem>();
        particleSystemList.AddRange(GetComponentsInChildren<ParticleSystem>());
        meshRendererList = new List<MeshRenderer>();
        meshRendererList.AddRange(GetComponentsInChildren<MeshRenderer>());

        SetQueue();
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
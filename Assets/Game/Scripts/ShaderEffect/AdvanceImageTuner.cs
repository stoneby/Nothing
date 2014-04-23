using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class AdvanceImageTuner : MonoBehaviour
{
    #region Public Fields

    public Color Color = Color.white;
    [Range(0f, 1f)]
    public float Highlight = 0f;
    [Range(0f, 1f)]
    public float Saturation = 1f;

    #endregion

    #region Private Fields

    private const float Invalid = -1f;
    private const float Tolerance = 0.1f;

    private Mesh mBuffMesh;
    private float lastH = Invalid;
    private float lastS = Invalid;
    private Color lastC = new Color(Invalid, Invalid, Invalid, Invalid);

    private readonly int[] triangles =
    {
        0, 1, 2, 0, 2, 3
    };

    private readonly Vector2[] uv1 =
    {
        new Vector2(0f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f)
    };

    #endregion

    #region Private Methods

    private void Init()
    {
        //var mesh = mBuffMesh ?? new Mesh();
        var mesh = mBuffMesh != null ? mBuffMesh : new Mesh();
        mesh.Clear();

        var vertices = new[]
        {
            new Vector3(-0.5f, -0.5f, 0f), 
            new Vector3(-0.5f, 0.5f, 0f), 
            new Vector3(0.5f, 0.5f, 0f), 
            new Vector3(0.5f, -0.5f, 0f)
        };

        var uv2 = new[]
        {
            new Vector2(Saturation, Highlight),
            new Vector2(Saturation, Highlight),
            new Vector2(Saturation, Highlight),
            new Vector2(Saturation, Highlight) 
        };

        var colors = new Color[4];
        colors[0] = Color;
        colors[1] = Color;
        colors[2] = Color;
        colors[3] = Color;

        mesh.vertices = vertices;
        mesh.uv = uv1;
        mesh.uv2 = uv2;
        mesh.colors = colors;
        mesh.triangles = triangles;

        var mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        mBuffMesh = mesh;
    }

    #endregion

    #region Mono

    void Start()
    {
        Init();
    }

    void Update()
    {
        // Pass through the same one.
        if (Math.Abs(lastH - Highlight) < Tolerance && Math.Abs(lastS - Saturation) < Tolerance && lastC == Color)
        {
            return;
        }

        Init();
        lastH = Highlight;
        lastS = Saturation;
        lastC = Color;
    }

    #endregion
}

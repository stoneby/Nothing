using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SSSSS : MonoBehaviour {

	public Color color = Color.white;
	[Range(0f, 1f)]
	public float highlight = 0f;
	[Range(0f, 1f)]
	public float saturation = 1f;

	private Mesh mBuffMesh;

	void Start () {
		init ();
	}
	private void init() {
		Mesh mesh = mBuffMesh != null ? mBuffMesh : new Mesh();
		mesh.Clear();
		Vector3[] vertices = new Vector3[4];
		vertices[0] = new Vector3(-0.5f, -0.5f, 0f);
		vertices[1] = new Vector3(-0.5f, 0.5f, 0f);
		vertices[2] = new Vector3(0.5f, 0.5f, 0f);
		vertices[3] = new Vector3(0.5f, -0.5f, 0f);
		Vector2[] uv1 = new Vector2[4];
		uv1[0] = new Vector2(0f, 0f);
		uv1[1] = new Vector2(0f, 1f);
		uv1[2] = new Vector2(1f, 1f);
		uv1[3] = new Vector2(1f, 0f);
		Color[] colors = new Color[4];
		colors[0] = color;
		colors[1] = color;
		colors[2] = color;
		colors[3] = color;
		Vector2[] uv2 = new Vector2[4];
		uv2[0] = new Vector2(saturation, highlight);
		uv2[1] = new Vector2(saturation, highlight);
		uv2[2] = new Vector2(saturation, highlight);
		uv2[3] = new Vector2(saturation, highlight);
		int[] triangles = new int[]{0, 1, 2, 0, 2, 3};
		mesh.vertices = vertices;
		mesh.uv = uv1;
		mesh.uv2 = uv2;
		mesh.colors = colors;
		mesh.triangles = triangles;
		
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
		mBuffMesh = mesh;
	}

	private float lastH = -1f;
	private float lastS = -1f;
	private Color lastC = new Color(-1f, -1f, -1f, -1f);
	void Update() {
		if (lastH != highlight || lastS != saturation || lastC != color) {
			init();
			lastH = highlight;
			lastS = saturation;
			lastC = color;
		}
	}
}

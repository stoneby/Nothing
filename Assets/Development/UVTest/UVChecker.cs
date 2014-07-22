using UnityEngine;
using System.Collections;

public class UVChecker : MonoBehaviour
{
    // using an image that is an 8x8 grid
    // each image is 0.125 in width and 0.125 in height of the full image

    // UVs in this example are given as a Rect
    // uvs : start position X, start position y, width, height
    // start positions are staggered to show different images
    // width and height are set to 0.125 (1/8th square of the full image)

    public Rect uvsFront = new Rect(0.0f, 1.0f, 0.125f, 0.125f);
    public Rect uvsBack = new Rect(0.125f, 0.875f, 0.125f, 0.125f);
    public Rect uvsLeft = new Rect(0.25f, 0.75f, 0.125f, 0.125f);
    public Rect uvsRight = new Rect(0.375f, 0.625f, 0.125f, 0.125f);
    public Rect uvsTop = new Rect(0.5f, 0.5f, 0.125f, 0.125f);
    public Rect uvsBottom = new Rect(0.625f, 0.375f, 0.125f, 0.125f);

    private Mesh theMesh;
    private Vector2[] theUVs;
    private float xOffset = 0.0f;

    private void Start()
    {
        theMesh = transform.GetComponent<MeshFilter>().mesh;
        theUVs = new Vector2[theMesh.uv.Length];
        theUVs = theMesh.uv;

        SetUVs();
    }

    private void Update()
    {
        // change the UV settings in the Inspector, then click the left mouse button to view
        if (Input.GetMouseButtonUp(0))
        {
            SetUVs();
        }
    }

    // 2 --- 3
    // |     |
    // |     |
    // 0 --- 1

    private void SetUVs()
    {
        // - set UV coordinates -

        // FRONT    2    3    0    1
        theUVs[2] = new Vector2(uvsFront.x, uvsFront.y);
        theUVs[3] = new Vector2(uvsFront.x + uvsFront.width, uvsFront.y);
        theUVs[0] = new Vector2(uvsFront.x, uvsFront.y - uvsFront.height);
        theUVs[1] = new Vector2(uvsFront.x + uvsFront.width, uvsFront.y - uvsFront.height);

        // BACK    6    7   10   11
        theUVs[6] = new Vector2(uvsBack.x, uvsBack.y);
        theUVs[7] = new Vector2(uvsBack.x + uvsBack.width, uvsBack.y);
        theUVs[10] = new Vector2(uvsBack.x, uvsBack.y - uvsBack.height);
        theUVs[11] = new Vector2(uvsBack.x + uvsBack.width, uvsBack.y - uvsBack.height);

        // LEFT   19   17   16   18
        theUVs[19] = new Vector2(uvsLeft.x, uvsLeft.y);
        theUVs[17] = new Vector2(uvsLeft.x + uvsLeft.width, uvsLeft.y);
        theUVs[16] = new Vector2(uvsLeft.x, uvsLeft.y - uvsLeft.height);
        theUVs[18] = new Vector2(uvsLeft.x + uvsLeft.width, uvsLeft.y - uvsLeft.height);

        // RIGHT   23   21   20   22
        theUVs[23] = new Vector2(uvsRight.x, uvsRight.y);
        theUVs[21] = new Vector2(uvsRight.x + uvsRight.width, uvsRight.y);
        theUVs[20] = new Vector2(uvsRight.x, uvsRight.y - uvsRight.height);
        theUVs[22] = new Vector2(uvsRight.x + uvsRight.width, uvsRight.y - uvsRight.height);

        // TOP    4    5    8    9
        theUVs[4] = new Vector2(uvsTop.x, uvsTop.y);
        theUVs[5] = new Vector2(uvsTop.x + uvsTop.width, uvsTop.y);
        theUVs[8] = new Vector2(uvsTop.x, uvsTop.y - uvsTop.height);
        theUVs[9] = new Vector2(uvsTop.x + uvsTop.width, uvsTop.y - uvsTop.height);

        // BOTTOM   15   13   12   14
        theUVs[15] = new Vector2(uvsBottom.x, uvsBottom.y);
        theUVs[13] = new Vector2(uvsBottom.x + uvsBottom.width, uvsBottom.y);
        theUVs[12] = new Vector2(uvsBottom.x, uvsBottom.y - uvsBottom.height);
        theUVs[14] = new Vector2(uvsBottom.x + uvsBottom.width, uvsBottom.y - uvsBottom.height);

        // - Assign the mesh its new UVs -
        theMesh.uv = theUVs;
    }
}

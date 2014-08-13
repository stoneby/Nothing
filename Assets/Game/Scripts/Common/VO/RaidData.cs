using UnityEngine;
using System.Collections;

public class RaidData
{
    private static int[][] chapters;

    public static int[][] GetChapters()
    {
        if (chapters == null)
        {
            chapters = new int[6][];
            chapters[0] = new[] { 64, -22, 370, 225 };
            chapters[1] = new[] { 140, 201, 335, 206 };
            chapters[2] = new[] { 223, -190, 349, 260 };
            chapters[3] = new[] { -214, 82, 336, 263 };
            chapters[4] = new[] { -118, -196, 417, 251 };
            chapters[5] = new[] { -452, -126, 255, 316 };
        }

        Mesh m = new Mesh();
        
        
        return chapters;
    }
}

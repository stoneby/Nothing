using UnityEditor;
using UnityEngine;
using System.Collections;

public class GetMapDataWindow : EditorWindow
{
    private string myString = "New Sprite";
    private string getStr = "";

    private void GetDatas()
    {
        var obj = GameObject.Find(myString);
        var ctwo = obj.GetComponent<PolygonCollider2D>();
        if (ctwo != null)
        {
            getStr = "";
            for (int i = 0; i < ctwo.points.Length; i++)
            {
                //ctwo.points[i].x *= 100;
                //ctwo.points[i].y *= 100;
                int xx = Mathf.FloorToInt(ctwo.points[i].x * 100);
                int yy = Mathf.FloorToInt(ctwo.points[i].y * 100);
                getStr += xx  + "," + yy;
                if (i < ctwo.points.Length - 1)
                {
                    getStr += ",";
                }
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label("输入GameObject名称,并点击按钮获取顶点数据", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Map对象名称", myString);
        
        if (GUILayout.Button("获取数据"))
        {
            GetDatas();
        }

        getStr = EditorGUILayout.TextArea(getStr);
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BattleBGControl : MonoBehaviour
{
    private string bgName;
    private UIAtlas bgAtlas;
    private List<UISprite> sprites; 
//    private UISprite sprite0;
//    private UISprite sprite1;
//    private UISprite sprite2;
//    private UISprite sprite3;

    private int currentIndex;

	// Use this for initialization
	void Start () {
	
	}

    public void SetData(string name)
    {
        if (bgName == name) return;
        Destroy(bgAtlas);
        ClearSprites();

        bgName = name;
        bgAtlas = Resources.Load("Textures/Battle/" + bgName + "/battle" + bgName, typeof(UIAtlas)) as UIAtlas;
        if (sprites == null)sprites = new List<UISprite>();
        var sp1 = NGUITools.AddSprite(gameObject, bgAtlas, "bg3");
        sprites.Add(sp1);
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg0"));
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg1"));
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg2"));

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].MakePixelPerfect();
            sprites[i].transform.localPosition = new Vector3(-960 + 640 * i, 0, 0);
        }
    }

    private void ClearSprites()
    {
        if (sprites == null) return;
        while (sprites.Count > 0)
        {
            Destroy(sprites[0]);
            sprites.RemoveAt(0);
        }
    }

    public void MoveToNext()
    {
        var sp0 = sprites[0];

        for (int i = 1; i < sprites.Count; i++)
        {
            var tp = sprites[i].transform.gameObject.AddComponent<TweenPosition>();
            tp.from = new Vector3(sprites[i].transform.localPosition.x, sprites[i].transform.localPosition.y, sprites[i].transform.localPosition.z);
            tp.to = new Vector3(sprites[i].transform.localPosition.x - 640, sprites[i].transform.localPosition.y, sprites[i].transform.localPosition.y);
            tp.duration = GameConfig.RunRoNextMonstersTime;
            tp.PlayForward();
            Destroy(tp, GameConfig.RunRoNextMonstersTime);
        }
        sprites.RemoveAt(0);
        sp0.transform.localPosition = new Vector3(960, 0, 0);
        sprites.Add(sp0);
    }
}

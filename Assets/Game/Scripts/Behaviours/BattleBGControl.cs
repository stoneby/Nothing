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
        //Destroy(bgAtlas);
        ClearSprites();

        bgName = name;
        bgAtlas = Resources.Load("Textures/Battle/" + bgName + "/battle" + bgName, typeof(UIAtlas)) as UIAtlas;
        if (sprites == null)sprites = new List<UISprite>();
//      var sp1 = NGUITools.AddSprite(gameObject, bgAtlas, "bg3");
//      sp1.depth = 1;
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg0"));
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg1"));
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg1"));
        sprites.Add(NGUITools.AddSprite(gameObject, bgAtlas, "bg2"));

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].depth = 1;
            sprites[i].MakePixelPerfect();
        }

        sprites[0].transform.localPosition = new Vector3(-320, 0, 0);
        
        sprites[1].transform.localPosition = new Vector3(640, 0, 0);
        sprites[2].transform.localPosition = new Vector3(1280, 0, 0);
        sprites[1].width = sprites[2].width = 1280;
        sprites[1].height = sprites[2].height = 720;
        sprites[3].transform.localPosition = new Vector3(960, 0, 0);
        sprites[0].width = sprites[3].width = 640;
        sprites[0].height = sprites[3].height = 720;
        currentIndex = 0;
    }

    private void ClearSprites()
    {
        if (sprites == null) return;
        while (sprites.Count > 0)
        {
            Destroy(sprites[0].gameObject);
            sprites.RemoveAt(0);
        }
    }

    private void MovePic(UISprite obj, float from)
    {
        var tp = obj.transform.gameObject.AddComponent<TweenPosition>();
        tp.from = new Vector3(from, obj.transform.localPosition.y, obj.transform.localPosition.z);
        tp.to = new Vector3(from - 640, obj.transform.localPosition.y, obj.transform.localPosition.y);
        tp.duration = GameConfig.RunRoNextMonstersTime;
        tp.PlayForward();
        Destroy(tp, GameConfig.RunRoNextMonstersTime);
    }

    public void MoveToNext(bool isend = false)
    {

        if (currentIndex == 0)
        {
            for (int i = 0; i <= 1; i++)
            {
                MovePic(sprites[i], sprites[i].transform.localPosition.x);
            }
        }
        else if (isend && currentIndex % 2 == 1)
        {
            if ((currentIndex - 1)%4 == 0)
            {
                MovePic(sprites[1], 0);
            }
            else
            {
                MovePic(sprites[2], 0);
            }
            MovePic(sprites[3], 960);
        }
        else
        {
            if ((currentIndex - 1)%4 == 0)
            {
                MovePic(sprites[1], 0);
                MovePic(sprites[2], 1280);
            }
            else if ((currentIndex - 2)%4 == 0)
            {
                MovePic(sprites[1], -640);
                MovePic(sprites[2], 640);
            }
            else if ((currentIndex - 3) % 4 == 0)
            {
                MovePic(sprites[1], 1280);
                MovePic(sprites[2], 0);
            }
            else
            {
                MovePic(sprites[1], 640);
                MovePic(sprites[2], -640);
            }
        }

//        var sp0 = sprites[0];
//
//        for (int i = 1; i < sprites.Count; i++)
//        {
//            var tp = sprites[i].transform.gameObject.AddComponent<TweenPosition>();
//            tp.from = new Vector3(sprites[i].transform.localPosition.x, sprites[i].transform.localPosition.y, sprites[i].transform.localPosition.z);
//            tp.to = new Vector3(sprites[i].transform.localPosition.x - 640, sprites[i].transform.localPosition.y, sprites[i].transform.localPosition.y);
//            tp.duration = GameConfig.RunRoNextMonstersTime;
//            tp.PlayForward();
//            Destroy(tp, GameConfig.RunRoNextMonstersTime);
//        }
//        sprites.RemoveAt(0);
//        sp0.transform.localPosition = new Vector3(960, 0, 0);
//        sprites.Add(sp0);
        currentIndex++;
    }
}

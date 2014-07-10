//----------------------------------------------
//	ngui/NGUIEditorTools.
//	ngui/UISprite.cs/SetAtlasSprite need to be public.
//		it is protected by default.
// 
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ntl : ScriptableObject {
	static List<Transform> listSelection = new List<Transform>();
	static List<Transform> listBone = new List<Transform>();
	static List<Transform> ListSprite = new List<Transform>();

	static public AnimationClip idle;
	static public AnimationClip attack;
	static public AnimationClip hurt;
	static public AnimationClip run;
	static public AnimationClip fig;

	[MenuItem("ntl/Rename_Tag_Link")]
	static void Rename () {
		listSelection.Clear ();
		listBone.Clear ();
		ListSprite.Clear ();
		GetTransform(Selection.activeGameObject , listSelection);

		foreach (Transform t in listSelection) {
			string tname = t.name.Substring(0,1);
			if (tname == "D") {
				t.tag = "Bones";
				listBone.Add (t);
			}else{
				t.tag = "Sprites";
				ListSprite.Add (t);
				t.localScale = new Vector3(-1f,1f,1f);
			}
		}
//		Debug.Log (ListSprite.Count);
		for(int i = 0 ; i < ListSprite.Count; i++){
			UISprite ut = ListSprite[i].GetComponent<UISprite>();
			ut.SetAtlasSprite(ut.atlas.spriteList[i]);
			ListSprite[i].name = ut.spriteName;
		}

		foreach (Transform t in listBone) {
			string bname = t.name.Substring(2,t.name.Length-2);
			foreach (Transform s in ListSprite) {
				if (s.name == bname) {
					s.parent = t;
					s.transform.localPosition = Vector3.zero;
					UISprite ut = s.GetComponent<UISprite>();
					ut.depth = (int)t.localPosition.z + 1000;
				}
			}
		}

		SnapSprite ();
	}

	[MenuItem("ntl/Cut_animation")]
	static void Cut_animation () {
		GameObject sl = Selection.activeGameObject;
		sl.animation.AddClip (idle,"testIdle",1,20);
		Debug.Log (sl.animation.GetClipCount());
	}

	static void SnapSprite(){
		foreach (Transform go in ListSprite)
		{
			UIWidget pw = go.gameObject.GetComponent<UIWidget>();
			
			if (pw != null)
			{
				NGUIEditorTools.RegisterUndo("Snap Dimensions", pw);
				NGUIEditorTools.RegisterUndo("Snap Dimensions", pw.transform);
				pw.MakePixelPerfect();
			}
		}
	}


	static void GetTransform(GameObject parent , List<Transform> l) {   
		foreach (Transform t in parent.transform) {
			l.Add(t);
			GetTransform(t.gameObject , l);
		}   
	} 
}
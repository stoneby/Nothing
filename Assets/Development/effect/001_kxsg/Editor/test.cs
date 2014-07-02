using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ntl : ScriptableObject {
	static List<Transform> listSprite = new List<Transform>();
	static List<Transform> listBone = new List<Transform>();

	[MenuItem("ntl/Rename")]
	static void Rename () {
		listSprite.Clear ();
		foreach (GameObject g in Selection.gameObjects) {
			if(g.name == "Sprite") {
				UISprite t = g.GetComponent<UISprite>();
				g.name = t.spriteName;
			}
			listSprite.Add(g.transform);
		}
		Debug.Log (listSprite.Count);
	}
	
	[MenuItem("ntl/Link")]
	static void Link () {
		listBone.Clear();
		GetTransform(Selection.activeGameObject , listBone);
		Debug.Log (listBone.Count);
	}
	
	static void GetTransform(GameObject parent , List<Transform> l) {   
		foreach (Transform t in parent.transform) {
			l.Add(t);
			GetTransform(t.gameObject , l);
		}   
	} 
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ntl : ScriptableObject {
	static List<Transform> listSelection = new List<Transform>();
	static List<Transform> listBone = new List<Transform>();
	static List<Transform> ListSprite = new List<Transform>();

	[MenuItem("ntl/Rename_Tag_Link")]
	static void Rename () {
		listSelection.Clear ();
		GetTransform(Selection.activeGameObject , listSelection);

		foreach (Transform t in listSelection) {
			string tname = t.name.Substring(0,1);
			if (tname == "D") {
				t.tag = "Bones";
				listBone.Add (t);
			}else{
				t.tag = "Sprites";
				UISprite ut = t.GetComponent<UISprite>();
				t.name = ut.spriteName;
				ListSprite.Add (t);
			}
		}

		foreach (Transform t in listBone) {
			string bname = t.name.Substring(2,t.name.Length-2);
			foreach (Transform s in ListSprite) {
				if (s.name == bname) {
					s.parent = t;
					s.transform.localPosition = Vector3.zero;
				}
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
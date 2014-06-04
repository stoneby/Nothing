using UnityEngine;
using UnityEditor;

public class ntl_SpirtScale : ScriptableObject {
	[MenuItem("ntl_SpirtScale/Scale")]
	static void Scale (){
		string n = "";
		foreach (GameObject g in Selection.gameObjects){
			n += "/"+g.name;
		}

		if (n != ""){
			Debug.Log(n);	
		}
	}
}
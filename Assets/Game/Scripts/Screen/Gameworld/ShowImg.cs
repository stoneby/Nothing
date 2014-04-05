using UnityEngine;
using System.Collections;

public class ShowImg : MonoBehaviour {
	public GameObject showTexture;

	int currentIndex = 0;
	// Use this for initialization
	void Start () {
		showTexture.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		
	}

	void OnClick()
	{

		StartCoroutine (DoSlidePicture());
	}

	IEnumerator DoSlidePicture()
	{
		if (currentIndex != 0)
		{
			playTweenPosition(showTexture, 0.3f, new Vector3(0,0,0), new Vector3(-960,0,0));
			yield return new WaitForSeconds(0.3f);
		}
		currentIndex = (currentIndex + 1) % 3;
		if (currentIndex != 0)
		{
			UITexture tt = showTexture.GetComponent<UITexture>();
			tt.mainTexture = (Texture2D)Resources.Load("textures/design/eff_" + currentIndex.ToString(), typeof(Texture2D));
			showTexture.SetActive(true);
			showTexture.transform.localPosition = new Vector3 (960, 0, 0);
			playTweenPosition(showTexture, 0.3f, new Vector3(960,0,0), new Vector3(0,0,0));
		}
		else
		{
			showTexture.SetActive(false);
		}
	}

	void playTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
	{
		TweenPosition ts = obj.AddComponent<TweenPosition>();
		ts.from = from;
		ts.to = to;
		ts.duration = playtime;
		ts.PlayForward ();
		Destroy (ts, playtime);
	}
}

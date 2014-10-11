using UnityEngine;
using System.Collections;

public class SignCourencyInfo : MonoBehaviour 
{
    private UIEventListener BgUIEventListener;

    private GameObject TitleLabel;
    private GameObject CountLabel;
    private GameObject IconItem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnEnter(int rewardid)
    {
        if (TitleLabel == null)
        {
            TitleLabel = transform.FindChild("Label name").gameObject;
            CountLabel = transform.FindChild("Label count").gameObject;
            IconItem = transform.FindChild("SignItem").gameObject;

            BgUIEventListener = UIEventListener.Get(transform.FindChild("Container click").gameObject);
            BgUIEventListener.onClick += ClickBgHandler;

        }

        var item = IconItem.GetComponent<SignItemControl>();
        item.SetData(rewardid, false, false);
        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = item.Name;
        lb = CountLabel.GetComponent<UILabel>();
        lb.text = "数量：" + item.Count;
    }

    private void ClickBgHandler(GameObject obj)
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public class CustomDragScrollView : MonoBehaviour
{
    #region Public Fields

    public CustomScrollView ScrollView;

    #endregion

    #region Private Fields

    [HideInInspector]
    [SerializeField]
    CustomScrollView draggablePanel;

	Transform mTrans;
    CustomScrollView mScroll;
	bool mAutoFind;

    #endregion

    #region Mono

    void OnEnable ()
	{
		mTrans = transform;

        if (ScrollView == null && draggablePanel != null)
		{
			ScrollView = draggablePanel;
			draggablePanel = null;
		}
		FindScrollView();
	}

	void FindScrollView ()
	{

        var sv = NGUITools.FindInParents<CustomScrollView>(mTrans);

		if (ScrollView == null)
		{
			ScrollView = sv;
			mAutoFind = true;
		}
		else if (ScrollView == sv)
		{
			mAutoFind = true;
		}
		mScroll = ScrollView;
	}


	void Start () { FindScrollView(); }

    #endregion

    #region Call Backs

    void OnPress (bool pressed)
	{
		if (mAutoFind && mScroll != ScrollView)
		{
			mScroll = ScrollView;
			mAutoFind = false;
		}

		if (ScrollView && enabled && NGUITools.GetActive(gameObject))
		{
			ScrollView.Press(pressed);
			
			if (!pressed && mAutoFind)
			{
                ScrollView = NGUITools.FindInParents<CustomScrollView>(mTrans);
				mScroll = ScrollView;
			}
		}
	}

	void OnDrag (Vector2 delta)
	{
		if (ScrollView && NGUITools.GetActive(this))
			ScrollView.Drag();
	}

	void OnScroll (float delta)
	{
		if (ScrollView && NGUITools.GetActive(this))
			ScrollView.Scroll(delta);
    }

    #endregion
}

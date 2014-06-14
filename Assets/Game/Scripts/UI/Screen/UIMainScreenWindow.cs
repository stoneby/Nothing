using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIMainScreenWindow : Window
{
    private UIEventListener addMoneyLis;
    private UIEventListener addMp;
    private UIEventListener startGameLis;
    private UIEventListener filpLeftLis;

	private UILabel gold;
	private UILabel diamond;
    private UILabel sprit;
    private UILabel nameLabel;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
		refreshData ();
    }

	public void refreshData()
	{
	    if (diamond == null) return;
		diamond.text = PlayerModelLocator.Instance.Diamond.ToString ();
		gold.text = PlayerModelLocator.Instance.Gold.ToString ();
		sprit.text = PlayerModelLocator.Instance.Sprit.ToString ();
	    nameLabel.text = PlayerModelLocator.Instance.Name;
	}

    public override void OnExit()
    {
        UnstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        addMoneyLis = UIEventListener.Get(Utils.FindChild(transform, "Button-AddMoney").gameObject);
        addMp = UIEventListener.Get(Utils.FindChild(transform, "Button-AddMp").gameObject);
        startGameLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Start").gameObject);
        filpLeftLis = UIEventListener.Get(Utils.FindChild(transform, "Button-FlipL").gameObject);

		diamond = transform.FindChild("Fortune/Diamond/Coins-Value").GetComponent<UILabel>();
		gold = transform.FindChild("Fortune/Coins/Coins-Value").GetComponent<UILabel>();
		sprit = transform.FindChild("Fortune/Souls/Coins-Value").GetComponent<UILabel>();
        nameLabel = transform.FindChild("Info/Name - Label").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        addMoneyLis.onClick += OnAddMoneyClicked;
        addMp.onClick += OnAddMpClicked;
        startGameLis.onClick += OnStartGameClicked;
        filpLeftLis.onClick += OnFlipLeftClicked;
    }

    private void UnstallHandlers()
    {
        addMoneyLis.onClick -= OnAddMoneyClicked;
        addMp.onClick -= OnAddMpClicked;
        startGameLis.onClick -= OnStartGameClicked;
        filpLeftLis.onClick -= OnFlipLeftClicked;
    }

    private void OnAddMoneyClicked(GameObject go)
    {
        
    }

    private void OnAddMpClicked(GameObject go)
    {
        
    }

    private void OnStartGameClicked(GameObject go)
    {
        if (MissionModelLocator.Instance.RaidLoadingAll == null)
        {
            var csmsg = new CSRaidLoadingAll();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show(typeof(MissionTabWindow), true);
        }
    }

    private void OnFlipLeftClicked(GameObject go)
    {
        
    }

    #endregion
}

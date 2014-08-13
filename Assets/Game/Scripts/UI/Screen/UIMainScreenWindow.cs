using System.Collections.Generic;
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
    private UILabel soul;
    private UILabel nameLabel;
    private UILabel coins;

    private UILabel level;
    private UISlider lvlSlider;
    private UISlider energySlider;

    private const int LeaderCount = 3;

    private readonly List<Transform> leaders = new List<Transform>();
    private readonly List<int> indexs = new List<int>();
    private readonly List<Transform> characterToolkits = new List<Transform>();

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
		RefreshData ();
        SpawnAndPlay();
    }

    public override void OnExit()
    {
        UnstallHandlers();
        Despawn();
    }

    #endregion

    #region Private Methods

    private void Despawn()
    {
        foreach (var ch in characterToolkits)
        {
            NGUITools.SetActive(ch.gameObject, true);
        }
        var count = leaders.Count;
        for(var i = count - 1; i >= 0 ; i--)
        {
            var character = leaders[i].GetChild(0).GetComponent<Character>();
            character.StopState(Character.State.Idle);
            CharacterPoolManager.Instance.CharacterPoolList[indexs[i]].Return(character.gameObject);
        }
        indexs.Clear();
        characterToolkits.Clear();
    } 
    
    private void SpawnAndPlay()
    {
        for (int i = 0; i < LeaderCount; i++)
        {
            var characterPoolManager = CharacterPoolManager.Instance;
            var character = characterPoolManager.CharacterPoolList[i].Take().GetComponent<Character>();
            character.PlayState(Character.State.Idle, true);
            var characterToolkit = character.transform.Find("CharacterToolkit");
            NGUITools.SetActive(characterToolkit.gameObject, false);
            characterToolkits.Add(characterToolkit);
            Utils.AddChild(leaders[i].gameObject, character.gameObject);
            indexs.Add(i);
        }
    }

    // Use this for initialization
    private void Awake()
    {
        startGameLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Start").gameObject);
        nameLabel = transform.FindChild("Name").GetComponent<UILabel>();
        level = transform.FindChild("Level/LvlValue").GetComponent<UILabel>();
        lvlSlider = transform.FindChild("Level/LvlBar").GetComponent<UISlider>();
        var fortune = transform.FindChild("Fortune");
        energySlider = fortune.FindChild("Energy/EnergyBar").GetComponent<UISlider>();
        coins = fortune.FindChild("Coins/CoinsValue").GetComponent<UILabel>();
        soul = fortune.FindChild("Soul/SoulValue").GetComponent<UILabel>();
        CommonHandler.PlayerPropertyChanged += OnPlayerPropertyChanged;
        var leadersTran = transform.Find("Leaders");
        leaders.Add(leadersTran.Find("MainLeader"));
        leaders.Add(leadersTran.Find("SecondLeader"));
        leaders.Add(leadersTran.Find("ThirdLeader"));
    }

    /// <summary>
    /// Used to do some clean work before destorying.
    /// </summary>
    private void OnDestory()
    {
        CommonHandler.PlayerPropertyChanged -= OnPlayerPropertyChanged;
    }

    private void InstallHandlers()
    {
        startGameLis.onClick = OnStartGameClicked;
    }

    private void UnstallHandlers()
    {
        startGameLis.onClick = null;
    }

    private void RefreshData()
    {
        coins.text = PlayerModelLocator.Instance.Gold.ToString();
        soul.text = PlayerModelLocator.Instance.Sprit.ToString();
        nameLabel.text = PlayerModelLocator.Instance.Name;
        var curLvl = PlayerModelLocator.Instance.Level;
        level.text = curLvl.ToString();
        var levelTemps = LevelModelLocator.Instance.LevelUpTemplates.LevelUpTmpls;
        if (!levelTemps.ContainsKey(curLvl))
        {
            Logger.LogError(string.Format("The current player level is {0}, it is not in the level up template.",curLvl));
        }
        var levelTemp = levelTemps[curLvl];
        lvlSlider.value = (float)PlayerModelLocator.Instance.Exp / levelTemp.MaxExp;
        energySlider.value = (float)PlayerModelLocator.Instance.Energy / levelTemp.MaxEnergy;
        
    }

    private void OnPlayerPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        RefreshData();
    }

    private void OnStartGameClicked(GameObject go)
    {
        MissionModelLocator.Instance.ShowRaidWindow();  
    }

    #endregion
}

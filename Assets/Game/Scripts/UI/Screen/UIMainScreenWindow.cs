using System.Collections.Generic;
using KXSGCodec;
using Property;
using Template.Auto.Hero;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIMainScreenWindow : Window
{
    public float Scale = 1.8f;

    private UIEventListener startGameLis;
	private UILabel diamond;
    private UILabel soul;
    private UILabel nameLabel;
    private UILabel coins;
    private UILabel level;
    private UILabel atk;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private UILabel energy;

    private GameObject BtnRecharge;

    private readonly List<int> iconIds = new List<int>(); 

    private int Atk
    {
        set { atk.text = value.ToString(); }
    } 
    
    private int Hp
    {
        set { hp.text = value.ToString(); }
    } 
    
    private int Recover
    {
        set { recover.text = value.ToString(); }
    } 
    
    private int Mp
    {
        set { mp.text = value.ToString(); }
    } 
    
    private int Lvl
    {
        set
        {
            var levelTemps = LevelModelLocator.Instance.LevelUpTemplates.LevelUpTmpls;
            if(!levelTemps.ContainsKey(value))
            {
                Logger.LogError(string.Format("The current player level is {0}, it is not in the level up template.",
                                              value));
            }
            var levelTemp = levelTemps[value];
            lvlSlider.value = (float) PlayerModelLocator.Instance.Exp / levelTemp.MaxExp;
            var energyValue = PlayerModelLocator.Instance.Energy;
            var maxEnergy = levelTemp.MaxEnergy;
            energySlider.value = (float) energyValue / maxEnergy;
            energy.text = string.Format("{0}/{1}", energyValue, maxEnergy);
            level.text = value.ToString();
        }
    }

    private UISlider lvlSlider;
    private UISlider energySlider;
    private const int LeaderCount = 3;
    private readonly List<Transform> leaders = new List<Transform>();
    private readonly List<Transform> characterToolkits = new List<Transform>();

    #region Window

    public override void OnEnter()
    {
        //if (ServiceManager.IsTest)
        //{
            WindowManager.Instance.Show<MainMenuBarWindow>(true);
        //}
        InstallHandlers();
		RefreshData ();
        SpawnAndPlay();
    }

    public override void OnExit()
    {
        //if (ServiceManager.IsTest)
        //{
            WindowManager.Instance.Show<MainMenuBarWindow>(false);
        //}
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
            character.transform.localScale = Vector3.one;
            CharacterPoolManager.Instance.CharacterPoolList[iconIds[i]].Return(character.gameObject);
        }
        iconIds.Clear();
        characterToolkits.Clear();
    } 
    
    private void SpawnAndPlay()
    {
        for (int i = 0; i < LeaderCount; i++)
        {
            var characterPoolManager = CharacterPoolManager.Instance;
            var character = characterPoolManager.CharacterPoolList[iconIds[i]].Take().GetComponent<Character>();
            character.PlayState(Character.State.Idle, true);
            var characterToolkit = character.transform.Find("CharacterToolkit");
            NGUITools.SetActive(characterToolkit.gameObject, false);
            characterToolkits.Add(characterToolkit);
            Utils.AddChild(leaders[i].gameObject, character.gameObject);
            character.transform.localScale = Scale * Vector3.one;
        }
    }

    // Use this for initialization
    private void Awake()
    {
        startGameLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Start").gameObject);
        nameLabel = transform.Find("Name").GetComponent<UILabel>();
        level = transform.Find("Level/LvlValue").GetComponent<UILabel>();
        var property = transform.Find("Property");
        atk = property.Find("Atk/AtkValue").GetComponent<UILabel>();
        hp = property.Find("Hp/HpValue").GetComponent<UILabel>();
        recover = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        mp = property.Find("Mp/MpValue").GetComponent<UILabel>();
        
        lvlSlider = transform.Find("Level/LvlBar").GetComponent<UISlider>();
        var fortune = transform.Find("Fortune");
        diamond = fortune.Find("Diamond/DiamondValue").GetComponent<UILabel>();
        energySlider = fortune.Find("Energy/EnergyBar").GetComponent<UISlider>();
        energy = fortune.Find("Energy/EnergyValue").GetComponent<UILabel>();
        coins = fortune.Find("Coins/CoinsValue").GetComponent<UILabel>();
        soul = fortune.Find("Soul/SoulValue").GetComponent<UILabel>();
        var leadersTran = transform.Find("Leaders");
        leaders.Add(leadersTran.Find("MainLeader"));
        leaders.Add(leadersTran.Find("SecondLeader"));
        leaders.Add(leadersTran.Find("ThirdLeader"));
        CommonHandler.PlayerPropertyChanged += OnPlayerPropertyChanged;

        BtnRecharge = transform.FindChild("Buttons/Button-Play").gameObject;
        if (ServiceManager.AppData != null) BtnRecharge.SetActive(ServiceManager.AppData.RechargeType != "0");
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
        diamond.text = PlayerModelLocator.Instance.Diamond.ToString();
        coins.text = PlayerModelLocator.Instance.Gold.ToString();
        soul.text = PlayerModelLocator.Instance.Sprit.ToString();
        nameLabel.text = PlayerModelLocator.Instance.Name;
        Lvl = PlayerModelLocator.Instance.Level;
        Atk = PlayerModelLocator.Instance.TeamProp[RoleProperties.ROLE_ATK];
        Hp = PlayerModelLocator.Instance.TeamProp[RoleProperties.ROLE_HP];
        Recover = PlayerModelLocator.Instance.TeamProp[RoleProperties.ROLE_RECOVER];
        Mp = PlayerModelLocator.Instance.TeamProp[RoleProperties.ROLE_MP];
        iconIds.Clear();
        var tempIds = PlayerModelLocator.Instance.TeamList;
        foreach(var tempId in tempIds)
        {
            HeroTemplate template;
            HeroModelLocator.Instance.HeroTemplates.HeroTmpls.TryGetValue(tempId, out template);
            iconIds.Add(template.Icon - 1);
        }
    }

    private void OnPlayerPropertyChanged(SCPropertyChangedNumber scpropertychanged = null)
    {
        RefreshData();
    }

    private void OnStartGameClicked(GameObject go)
    {
        MissionModelLocator.Instance.ShowRaidWindow();  
    }

    public static void GoToMainScreen()
    {
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
        WindowManager.Instance.Show<MainMenuBarWindow>(true);
    }

    #endregion
}

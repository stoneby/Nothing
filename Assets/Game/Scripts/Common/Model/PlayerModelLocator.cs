using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public sealed class PlayerModelLocator
{
    public short HeroId;
    public long RoleId;
    public string Name;
    public short Level;
    public int HeadIconId;
    public int Exp;
    public long Diamond;
    public long Gold;
    public int Sprit;
    public int Energy
    {
        get { return EnergyIncreaseControl.Instance.Energy; }
    }

    public int HeroMax;
    public int ItemMax;
    public int ExtendHeroTimes;
    public int ExtendItemTimes;
    public int Famous;
    public int SuperChip;
    public Dictionary<int, int> TeamProp;
    public List<int> TeamList; 

    private static volatile PlayerModelLocator instance;
    private static readonly object SyncRoot = new Object();

    private PlayerModelLocator()
    {
    }

    public static PlayerModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new PlayerModelLocator();
                }
            }
            return instance;
        }
    }

    public static void UpdateTeamInfos(List<long> uuids)
    {
        var mainingUuids = uuids.Where(uuid => uuid != HeroConstant.NoneInitHeroUuid).ToList();
        var heros = mainingUuids.Select(uuid => HeroModelLocator.Instance.FindHero(uuid)).ToList();
        Instance.TeamList = heros.Select(hero => hero.TemplateId).ToList();
        foreach (var key in HeroConstant.PropKeys)
        {
            Instance.TeamProp[key] = heros.Sum(hero => hero.Prop[key]);
        }
    }
}
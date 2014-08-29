/// <summary>
/// The constant varibles related to heros.
/// </summary>
public class HeroConstant 
{
    /// <summary>
    /// The prefix of job sprite name.
    /// </summary>
    public const string HeroJobPrefix = "job_";

    /// <summary>
    /// The pool name of hero and item prefab.
    /// </summary>
    public const string HeroPoolName = "Heros";

    /// <summary>
    /// The index of first equip in the hero info's equip list.
    /// </summary>
    public const int HeroFirstEquip = 0;

    /// <summary>
    /// The index of second equip in the hero info's equip list.
    /// </summary>
    public const int HeroSecondEquip = 1;

    /// <summary>
    /// The max count of hero  each per team.
    /// </summary>
    public const int MaxHerosPerTeam = 9;

    /// <summary>
    /// The uuid of none init hero.
    /// </summary>
    public const int NoneInitHeroUuid = -1;

    /// <summary>
    /// The two visible rows. 
    /// </summary>
    public const int TwoRowsVisible = 2;

    /// <summary>
    /// The three visible rows.
    /// </summary>
    public const int ThreeRowsVisble = 3;

    /// <summary>
    /// The index position of main leader in team.
    /// </summary>
    public const int LeaderPosInTeam = 0;

    /// <summary>
    /// The index position of second leader in team.
    /// </summary>
    public const int SecondLeaderPosInTeam = 1;

    /// <summary>
    /// The index position of third leader in team.
    /// </summary>
    public const int ThirdLeaderPosInTeam = 2;

    /// <summary>
    /// The prefix of the hero icon id.
    /// </summary>
    public const string IconIdPrefix = "head_";

    public const int ResourceReadyCount = 39;

    public enum LeaderState
    {
        NotInTeam,
        MainLeader,
        ViceLeader,
        Member,
        MemberInOtherTeam
    }

    /// <summary>
    /// Just for demo.
    /// </summary>
    public enum HeroDetailEnterType
    {
        InValid,
        BuildingTeam,
        BreakLimit,
        LvlUp,
        SellHero,
    }

    public static HeroDetailEnterType EnterType = HeroDetailEnterType.InValid;

    public const string ExtendContentKey = "UIHeroCommon.ExtendContent";
    public const string ExtendLimitKey = "UIHeroCommon.ExtendLimit";
    public const string TeamPrefixKey = "UIBuildingTeam.team";

    public static void SetHeadByTemplate(UISprite sprite, int templateID)
    {
        var index = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[templateID].Icon - 1;
        SetHeadByIndex(sprite, index);
    }

    public static void SetHeadByIndex(UISprite sprite, int index)
    {
        sprite.spriteName = IconIdPrefix + ((index > ResourceReadyCount) ? 0 : index);
    }
}

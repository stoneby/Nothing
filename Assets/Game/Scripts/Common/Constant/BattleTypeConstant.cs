
public class BattleTypeConstant
{
    //多连时的加成
    public static float[] MoreHitTimes = { 1.0f, 1.2f, 1.5f, 1.9f, 2.4f, 3.0f, 3.7f, 4.5f, 9.0f };

    //血条显示的位置关系
    public const int PosHPMin = -303;
    public const int PosHPMax = 128;
    public const int PosHPY = 121;
    public const int PosMPMin = -323;
    public const int PosMPMax = 110;
    public const int PosMPY = 101;
}

/// <summary>
/// Foot color type.
/// </summary>
public enum FootColorType
{
    Red = 1,
    Yellow,
    Green,
    Blue,
    Pink
};

/// <summary>
/// Character job type.
/// </summary>
public enum HeroJob
{
    Jian = 1,
    Tu,
    Gong,
    Chui,
    Ji,
};

/// <summary>
/// Character type.
/// </summary>
public enum CharacterType
{
    Hero = 0,
    Friend,
    Guest
}
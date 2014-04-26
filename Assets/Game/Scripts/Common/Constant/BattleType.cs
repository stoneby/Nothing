
public class BattleType
{
    public static float[] MoreHitTimes = new float[9] { 1.0f, 1.2f, 1.5f, 1.9f, 2.4f, 3.0f, 3.7f, 4.5f, 9.0f };

    public const int FootRed = 1;
    public const int FootYellow = 2;
    public const int FootGreen = 3;
    public const int FootBlue = 4;
    public const int FootPink = 5;
    public const int FootMin = 1;
    public const int FootMax = 6;

    public static readonly string[] SelectLines = new string[6] { "", "drag_1", "drag_2", "drag_0", "drag_3", "drag_4"};

    public const int JobSword = 1;      //剑
    public const int JobKinght = 2;     //骑
    public const int JobArcher = 3;     //弓弩
    public const int JobShielder = 4;   //盾
    public const int JobMaster = 5;     //
    public const int JobHeart = 6;      //回复

    public const int PosHPMin = -303;
    public const int PosHPMax = 128;
    public const int PosHPY = 121;
    public const int PosMPMin = -323;
    public const int PosMPMax = 110;
    public const int PosMPY = 101;

}

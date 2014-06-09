using UnityEngine;
using System.Collections;

public class GameConfig
{
    public static string Version;
    public static int VersionValue;
    public static string FName;
    public static bool ShowLog;
    public static string Language;
    public static string ServicePath;
    public static string ServerIpAddress;
    public static string LocalServicePath;
    public static string CookieAddress;
    public static string BattleConfig;

    //battle中的节奏
    public static float HeroRunReturnTime = 0.5f;        //武将出手后跑回的时间长度
    public static float ShortTime = 0.5f;               //最小的时间片段
    public static float PlayAttrackTime = 0.4f;          //回血时攻击动画所用的时间
    public static float RunStepNeedTime = 0.5f;          //跑位所需的单位时间（可用于计算速度）
    public static float NextRunWaitTime = 0.5f;         //两个人之间的间隔时间
    public static float PlayRecoverEffectTime = 0.6f;    //播放回血特效的时间
    public static float RunRoNextMonstersTime = 0.8f;    //所有武将跑到下一波怪的时间
    public static float RunToAttrackPosTime = 0.3f;      //武将跑到攻击位所用的时间
    public static float NextAttrackWaitTime = 0.3f;      //武将连续攻击之间的时间间隔
    public static float TotalHeroAttrackTime = 0.7f;     //武将一次出手的总得大约时间
    public static float MoveCameraTime = 0.25f;          //拉镜头所用的时间
    public static float Attrack9PlayEffectTime = 0.9f;   //第9击武将特效播放时间
    public static float Attrack9HeroInTime = 0.3f;       //第9击人物大图飞入时间
    public static float Attrack9TextInTime = 0.2f;       //第9击文字进入时间
    public static float Attrack9TextShowTime = 1.0f;     //第9击文字显示的时间
    public static float Attrack9TextFadeTime = 1.0f;     //第9击文字消失的时间
    public static float Attrack9HeroFadeTime = 0.2f;     //第9击武将大图消失的时间
    public static float Attrack9SwardEffectTime = 0.5f;  //第9击刀光效果播放时间
    public static float Attrack9SwardWaitTime = 0.1f;    //第9击刀光效果时间间隔
    public static float Attrack9TotalSwardTime = 1.0f;   //第9击刀光使用总时间
    public static float PlayMonsterEffectTime = 0.7f;    //怪的特效时间
    public static float MonsterAttrackStepTime = 0.2f;   //怪普通攻击间隔时间
    public static float HeroBeenAttrackTime = 0.5f;      //武将受击时间

}

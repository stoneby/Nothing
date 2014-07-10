using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildManager
{
    public static void BuildAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        //PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, );
        PlayerSettings.productName = "开心三国0.1";
        PlayerSettings.bundleVersion = "0.0.1";
        PlayerSettings.bundleIdentifier = "cn.kx.kxsg";

        FileUtil.DeleteFileOrDirectory("release/AndroidBuild");
        Directory.CreateDirectory("release/AndroidBuild");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var d = DateTime.Now;
        var str = "";
        str += d.Year + GetValueName(d.Month) + GetValueName(d.Day) + "-" + GetValueName(d.Hour) + GetValueName(d.Minute);
        var res = BuildPipeline.BuildPlayer(scenes, "release/AndroidBuild/kxsg-"+str+".apk", BuildTarget.Android, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }

    private static string GetValueName(int k)
    {
        if (k < 10)
        {
            return "0" + k;
        }
        else
        {
            return "" + k;
        }
    }

    public static void BuildExe()
    {
        Debug.Log("转换Target");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
        PlayerSettings.productName = "开心三国0.1";
        PlayerSettings.bundleVersion = "0.0.1";
        PlayerSettings.bundleIdentifier = "cn.kx.kxsg";

        FileUtil.DeleteFileOrDirectory("release/ExeBuild");
        FileUtil.DeleteFileOrDirectory("release/ExeRelease");
        Directory.CreateDirectory("release/ExeBuild");
        Directory.CreateDirectory("release/ExeRelease");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        Debug.Log("开始打包exe");
        var res = BuildPipeline.BuildPlayer(scenes, "release/ExeBuild/kxsg.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        Debug.Log("打包完成");
    }

    public static void BuildIos()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
        PlayerSettings.productName = "开心三国0.1";
        PlayerSettings.bundleVersion = "0.0.1";
        PlayerSettings.bundleIdentifier = "cn.kx.wg.kxsginhouse";

        FileUtil.DeleteFileOrDirectory("release/IosBuild");
        Directory.CreateDirectory("release/IosBuild");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var res = BuildPipeline.BuildPlayer(scenes, "release/IosBuild", BuildTarget.iPhone, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }
}

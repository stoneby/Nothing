using System;
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

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var res = BuildPipeline.BuildPlayer(scenes, "release/AndroidBuild/testbuild.apk", BuildTarget.Android, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }

    public static void BuildExe()
    {
        Debug.Log("转换Target");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
        PlayerSettings.productName = "开心三国0.1";
        PlayerSettings.bundleVersion = "0.0.1";
        PlayerSettings.bundleIdentifier = "cn.kx.kxsg";

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        Debug.Log("开始打包exe");
        var res = BuildPipeline.BuildPlayer(scenes, "release/ExeBuild/testbuild.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
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
        PlayerSettings.bundleIdentifier = "cn.kx.kxsg";

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var res = BuildPipeline.BuildPlayer(scenes, "release/IosBuild", BuildTarget.iPhone, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }
}

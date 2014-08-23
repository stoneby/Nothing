using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

public class BuildManager
{
    public static void BuildAndroid()
    {
        Debug.Log("转换Target");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        //PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, );

        ReadGameConfigurationXml();

        PlayerSettings.productName = GameConfig.GameName;
        PlayerSettings.bundleVersion = GameConfig.Version;
        PlayerSettings.bundleIdentifier = GameConfig.BundleID;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        PlayerSettings.Android.keystoreName = "release/key/user.keystore";
        PlayerSettings.Android.keystorePass = "111111";
        PlayerSettings.Android.keyaliasName = "sglm";
        PlayerSettings.Android.keyaliasPass = "111111";

        FileUtil.DeleteFileOrDirectory("release/AndroidBuild");
        Directory.CreateDirectory("release/AndroidBuild");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var d = DateTime.Now;
        var str = "";
        str += d.Year + GetValueName(d.Month) + GetValueName(d.Day) + "-" + GetValueName(d.Hour) + GetValueName(d.Minute);
        Debug.Log("开始打包Android");
        var res = BuildPipeline.BuildPlayer(scenes, "release/AndroidBuild/kxsg-"+str+".apk", BuildTarget.Android, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
        Debug.Log("打包完成");
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
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        ReadGameConfigurationXml();

        PlayerSettings.productName = GameConfig.GameName;
        PlayerSettings.bundleVersion = GameConfig.Version;
        PlayerSettings.bundleIdentifier = GameConfig.BundleID;

        FileUtil.DeleteFileOrDirectory("release/ExeBuild");
        FileUtil.DeleteFileOrDirectory("release/ExeRelease");
        Directory.CreateDirectory("release/ExeBuild");
        Directory.CreateDirectory("release/ExeRelease");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        Debug.Log("开始打包exe");
        var res = BuildPipeline.BuildPlayer(scenes, "release/ExeBuild/kxsg.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        Debug.Log("打包完成");
    }

    private static void SwapAsset(string src, string target)
    {
        if (System.IO.File.Exists(target))
        {
            FileUtil.ReplaceFile(src, target);
        }
        else
        {
            FileUtil.CopyFileOrDirectory(src, target);
        }
    }

    public static void BuildIos()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);

        ReadGameConfigurationXml();

        PlayerSettings.productName = GameConfig.GameName;
        PlayerSettings.bundleVersion = GameConfig.Version;
        PlayerSettings.bundleIdentifier = GameConfig.BundleID;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.iOS.exitOnSuspend = true;
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        var iconFile = "Assets/icons/sglm/icon.png";
        int[] sizeList = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.iPhone);
        Texture2D[] iconList = new Texture2D[sizeList.Length];
        for (int i = 0; i < sizeList.Length; i++)
        {
            int iconSize = sizeList[i];
            iconList[i] = AssetDatabase.LoadMainAssetAtPath(iconFile) as Texture2D;
            iconList[i].Resize(iconSize, iconSize, TextureFormat.ARGB32, false);
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iPhone, iconList);
        
//        var splashStr = "Assets/loading/device/splash.png";
//        var resourcePath = "Assets/loading/sglm/splash.png";
//        SwapAsset(resourcePath, splashStr);
//        AssetDatabase.Refresh();
        
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;

        FileUtil.DeleteFileOrDirectory("release/IosBuild");
        Directory.CreateDirectory("release/IosBuild");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var res = BuildPipeline.BuildPlayer(scenes, "release/IosBuild", BuildTarget.iPhone, BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }

    private static void ReadGameConfigurationXml()
    {
        var GameConfigurationText = Resources.Load("Config/GameConfiguration") as TextAsset;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(GameConfigurationText.text);
        var nodeList = xmlDoc.SelectNodes("config");

        if (nodeList == null)
        {
            return;
        }

        foreach (XmlNode node in nodeList)
        {
            if (node["GameVersion"] != null)
            {
                GameConfig.Version = node["GameVersion"].InnerText;
                GameConfig.VersionValue = ServiceManager.GetVersionValue(GameConfig.Version);
            }

            if (node["BundleID"] != null)
            {
                GameConfig.BundleID = node["BundleID"].InnerText;
            }

            if (node["GameName"] != null)
            {
                GameConfig.GameName = node["GameName"].InnerText;
            }
        }
    }
}

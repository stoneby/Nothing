using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildManager
{
    private const string BaseBundlePath = "AssetBundles/";
    private const BuildAssetBundleOptions BuildAssetOptions = BuildAssetBundleOptions.CompleteAssets |
                                                              BuildAssetBundleOptions.CollectDependencies |
                                                              BuildAssetBundleOptions.DeterministicAssetBundle;
    public static void BuildAndroid()
    {
        Debug.Log("====================================Switch To Android Target=======================");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        ReadGameConfigurationXml();

        if (!GameConfig.IsFullAssetBundles)
        {
            FileUtil.DeleteFileOrDirectory("Assets/Game/Resources/AssetBundles");
        }

        PlayerSettings.productName = GameConfig.GameName;
        PlayerSettings.bundleVersion = GameConfig.Version;
        PlayerSettings.bundleIdentifier = GameConfig.BundleID;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        PlayerSettings.Android.keystoreName = "release/key/user.keystore";
        PlayerSettings.Android.keystorePass = "111111";
        PlayerSettings.Android.keyaliasName = "sglm";
        PlayerSettings.Android.keyaliasPass = "111111";
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;

        var iconFile = "Assets/icons/" + GameConfig.GameIcon + ".png";
        
        //Debug.Log("============================================================" + iconFile);
        int[] sizeList = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);//{144,96,72,48,36};
        Texture2D[] iconList = new Texture2D[sizeList.Length];
        //Debug.Log("============================================================Create icon count " + sizeList.Length);
        for (int i = 0; i < sizeList.Length; i++)
        {

            int iconSize = sizeList[i];
            //Debug.Log("============================================================Create icon size " + iconSize);
            iconList[i] = AssetDatabase.LoadMainAssetAtPath(iconFile) as Texture2D;
            //iconList[i].Resize(iconSize, iconSize, TextureFormat.ARGB32, false);
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, iconList);

        //FileUtil.DeleteFileOrDirectory("Assets/Game/Arts/Atlases/Login/logo.png");
        //AssetDatabase.CopyAsset("Assets/logo/" + GameConfig.NameLogo + ".png", "Assets/Game/Arts/Atlases/Login/logo.png");

        FileUtil.DeleteFileOrDirectory("release/AndroidBuild");
        Directory.CreateDirectory("release/AndroidBuild");
        //FileUtil.MoveFileOrDirectory("release/AndroidBuild/*.apk", "release/AndroidBuild/old");
        //FileUtil.DeleteFileOrDirectory("release/AndroidBuild/*.apk");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
        var d = DateTime.Now;
        var str = "";
        str += d.Year + GetValueName(d.Month) + GetValueName(d.Day) + "-" + GetValueName(d.Hour) +
               GetValueName(d.Minute) + "-" + PlayerSettings.bundleIdentifier;
        Debug.Log("开始打包Android");
        str = "sglm-" + str + ".apk";
        string res;
        if (GameConfig.Build == "true" || GameConfig.Build == "development")
        {
            res = BuildPipeline.BuildPlayer(scenes, "release/AndroidBuild/" + str, BuildTarget.Android, BuildOptions.ConnectWithProfiler | BuildOptions.Development);
        }
        else
        {
            res = BuildPipeline.BuildPlayer(scenes, "release/AndroidBuild/" + str, BuildTarget.Android, BuildOptions.None);
        } 

        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
        else
        {
            //FileUtil.DeleteFileOrDirectory("Assets/Plugins/Android");
        }

        //FileUtil.CopyFileOrDirectory("release/AndroidBuild/" + str, "release/AndroidBuild/old/");

        Debug.Log("********************************************************************************");
        Debug.Log("*************************  Build Android Successful  ***************************");
        str = "  " + str + "  ";
        int k = (80 - str.Length)/2;
        for (int i = 0; i < k; i++)
        {
            str = "*" + str + "*";
        }
        Debug.Log(str);
        Debug.Log("*************************************************************************");
    }

    private static string GetValueName(int k)
    {
        if (k < 10)
        {
            return "0" + k;
        }
        return "" + k;
    }

    public static void BuildExe()
    {
        Debug.Log("转换Target");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        ReadGameConfigurationXml();

        if (!GameConfig.IsFullAssetBundles)
        {
            FileUtil.DeleteFileOrDirectory("Assets/Game/Resources/AssetBundles");
        }

        Debug.Log("------------------------------------" + GameConfig.GameName);
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
        if (File.Exists(target))
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

        if (!GameConfig.IsFullAssetBundles)
        {
            FileUtil.DeleteFileOrDirectory("Assets/Game/Resources/AssetBundles");
        }

        Debug.Log("------------------------------------" + GameConfig.GameName);
        PlayerSettings.productName = GameConfig.GameName;
        PlayerSettings.bundleVersion = GameConfig.Version;
        PlayerSettings.bundleIdentifier = GameConfig.BundleID;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.iOS.exitOnSuspend = true;
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.exitOnSuspend = false;
        
        //PlayerSettings.iOS.targetResolution
        
        var iconFile = "Assets/icons/"+GameConfig.GameIcon+".png";
        //var iconFile = "Assets/icons/sglm.png";
        int[] sizeList = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.iPhone);
        Texture2D[] iconList = new Texture2D[sizeList.Length];
        for (int i = 0; i < sizeList.Length; i++)
        {
            int iconSize = sizeList[i];
            Debug.Log("============================================================Create icon size " + iconSize);
            iconList[i] = AssetDatabase.LoadMainAssetAtPath(iconFile) as Texture2D;
            if (iconList[i] == null)
            {
                Debug.Log("Texture is null");
                continue;
            }
            //iconList[i].Resize(iconSize, iconSize, TextureFormat.ARGB32, false);
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iPhone, iconList);
        
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
        var paths = new string[1];
        paths[0] = EditorApplication.currentScene;
        var result = FindAllDependTextures(paths);
        foreach(var path in result)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureFormat = TextureImporterFormat.PVRTC_RGBA4;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            textureImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(path);
        }

        FileUtil.DeleteFileOrDirectory("Assets/Plugins/Android");
        FileUtil.DeleteFileOrDirectory("Assets/Plugins/AndroidI18N");
        FileUtil.DeleteFileOrDirectory("release/IosBuild");
        Directory.CreateDirectory("release/IosBuild");

        string[] scenes = { "Assets/game/scenes/BattleScene.unity" };
//        var res = BuildPipeline.BuildPlayer(scenes, "release/IosBuild", BuildTarget.iPhone, BuildOptions.None);
        string res;
        if (GameConfig.Build == "true" || GameConfig.Build == "development")
        {
            res = BuildPipeline.BuildPlayer(scenes, "release/IosBuild", BuildTarget.iPhone, BuildOptions.ConnectWithProfiler | BuildOptions.Development);
        }
        else
        {
            res = BuildPipeline.BuildPlayer(scenes, "release/IosBuild", BuildTarget.iPhone, BuildOptions.None);
        } 

        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
//        else
//        {
//            FileUtil.CopyFileOrDirectory("release/IosLocal", "release/IosBuild");
//        }
    }

    private static void ReadGameConfigurationXml()
    {
        var gameConfigurationText = Resources.Load("Config/GameConfiguration") as TextAsset;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(gameConfigurationText.text);
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

            if (node["IconPath"] != null)
            {
                GameConfig.GameIcon = node["IconPath"].InnerText;
            }

            if (node["WithAssetBundles"] != null)
            {
                GameConfig.IsFullAssetBundles = node["WithAssetBundles"].InnerText == "full";
            }

            if (node["Build"] != null)
            {
                GameConfig.Build = node["Build"].InnerText;
            }

            if (node["NameLogo"] != null)
            {
                GameConfig.NameLogo = node["NameLogo"].InnerText;
            }
        }
    }

    [MenuItem("Tool/AutoBuildBundle/BuildExe")]
    public static void BuildExeBundles()
    {
        BuildBundles(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tool/AutoBuildBundle/BuildAndroid")]
    public static void BuildAndroidBundles()
    {
        BuildBundles(BuildTarget.Android);
    }

    [MenuItem("Tool/AutoBuildBundle/BuildIos")]
    public static void BuildIosBundles()
    {
        BuildBundles(BuildTarget.iPhone);
    }   
    
    [MenuItem("Tool/TestDependency")]
    public static void TestDependency()
    {
        var paths = new string[1];
        paths[0] = EditorApplication.currentScene;
        var result = FindAllDependTextures(paths);
        Debug.Log(result.Count);
    }

    public static void BuildBundles(BuildTarget target)
    {
        FileUtil.DeleteFileOrDirectory("release/Bundles");
        var platformPath = CheckPlatformPath(target);
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        List<AssetBundleConfig> configurations;
        ReadBundleBuildConfigXml(out configurations);
        foreach (var configuration in configurations)
        {
            var path = BaseBundlePath + Utils.ConvertToResoucesPath(configuration.Folder);

            var objects = Resources.LoadAll(path);
            switch (configuration.Type)
            {
                case "Texture":
                    {
                        objects = objects.Where(obj => obj is Texture2D).ToArray();
                        break;
                    }
                case "Prefab":
                    {
                        objects = objects.Where(obj => obj is GameObject).ToArray();
                        break;
                    } 
                case "TextAsset":
                    {
                        objects = objects.Where(obj => obj is TextAsset).ToArray();
                        break;
                    }
            }

            if (configuration.Separately == "true")
            {
                foreach (var o in objects)
                {
                    path = AssetDatabase.GetAssetPath(o);
                    var bundleName = Utils.ConvertToAssetBundleName(path);
                    bundleName = bundleName.Substring(0, bundleName.LastIndexOf('.'));
                    bundleName += ".assetbundle";
                    bundleName = GetResourcesLoadPath(bundleName);
                    path = platformPath + bundleName;
                    BuildPipeline.BuildAssetBundle(o, null, path, BuildAssetOptions, target);
                }
            }
            else
            {
                var bundleName = Utils.ConvertToAssetBundleName(path);
                bundleName += ".assetbundle";
                bundleName = GetResourcesLoadPath(bundleName);
                path = platformPath + bundleName;
                BuildPipeline.BuildAssetBundle(null, objects, path, BuildAssetOptions, target);
            }
        }
        GenerateVersionNum.Execute(target);
    }

    private static string GetResourcesLoadPath(string path)
    {
        if(path.Contains(".Resources."))
        {
            var index = path.IndexOf(".Resources.");
            path = path.Substring(index + ".Resources.".Length);
        }
        return path;
    }

    private static string CheckPlatformPath(BuildTarget target)
    {
        var platformPath = BundleCreaterWindow.GetPlatformPath(target);
        if (!Directory.Exists(platformPath))
        {
            Directory.CreateDirectory(platformPath);
        }
        return platformPath;
    }

    private static void ReadBundleBuildConfigXml(out List<AssetBundleConfig> configurations)
    {
        var configurationText = Resources.Load("Config/BundleConfig") as TextAsset;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(configurationText.text);
        var xmlRoot = xmlDoc.DocumentElement;
        configurations = (from element in xmlRoot.ChildNodes.OfType<XmlElement>()
                          let folder = element.GetAttribute("FolderName")
                          let type = element.GetAttribute("Type")
                          let separately = element.GetAttribute("Separately")
                          select new AssetBundleConfig {Folder = folder, Type = type, Separately = separately}).ToList();
    }

    public class AssetBundleConfig
    {
        public string Folder { get; set; }
        public string Type { get; set; }
        public string Separately { get; set; }
    }

    public static List<string> FindAllDependTextures(string[] scenes)
    {
        var allDepends = AssetDatabase.GetDependencies(scenes);
        var allDependTextures = allDepends.Where(item => item.EndsWith(".png") || item.EndsWith("jpg")).ToList();
        var allDependPrefabs = allDepends.Where(item => item.EndsWith(".prefab")).ToList();
        var allPaths = AssetDatabase.GetAllAssetPaths();
        var resoursPrefabs = allPaths.Where(path => path.EndsWith(".prefab")).ToList();
        var resourcesTextures = allPaths.Where(path => path.EndsWith(".png") || path.EndsWith("jpg")).ToList();
        allDependPrefabs.AddRange(resoursPrefabs);
        allDependPrefabs = allDependPrefabs.Distinct().ToList();
        allDependTextures.AddRange(resourcesTextures);
        var texturesOfPrefabs =
            AssetDatabase.GetDependencies(allDependPrefabs.ToArray()).Where(item => item.EndsWith(".png") || item.EndsWith("jpg"));
        allDependTextures.AddRange(texturesOfPrefabs);
        allDependTextures = allDependTextures.Distinct().ToList();
        return allDependTextures.ToList();
    }
}

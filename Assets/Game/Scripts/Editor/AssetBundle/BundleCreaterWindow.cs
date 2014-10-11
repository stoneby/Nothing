using UnityEditor;
using UnityEngine;

public class BundleCreaterWindow : EditorWindow
{
    public static BundleCreaterWindow window;
    public static BuildTarget buildTarget = BuildTarget.StandaloneWindows;
    private bool collectDependencies = true;
    private bool completeAssets = true;
    private bool disableWriteTypeTree;
    private bool deterministicAssetBundle = true;
    private bool uncompressedAssetBundle;
    private bool buildSeparately = true;

    void OnGUI()
    {
        GUILayout.Label("Export Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);
        collectDependencies = EditorGUILayout.Toggle("CollectDependencies", collectDependencies);
        completeAssets = EditorGUILayout.Toggle("CompleteAssets", completeAssets);
        disableWriteTypeTree = EditorGUILayout.Toggle("DisableWriteTypeTree", disableWriteTypeTree);
        deterministicAssetBundle = EditorGUILayout.Toggle("DeterministicAssetBundle", deterministicAssetBundle);
        uncompressedAssetBundle = EditorGUILayout.Toggle("UncompressedAssetBundle", uncompressedAssetBundle);
        var buildAssetOptions = GetAssetBundleOptions();
        GUILayout.Space(10);
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildTarget);
        buildSeparately = EditorGUILayout.Toggle("Build Separately", buildSeparately);
        if (GUILayout.Button("Build Asset Bundles"))
        {
            CreateAssetBundle.Execute(buildTarget, buildSeparately, buildAssetOptions);
            CreateMD5List.Execute(buildTarget);
            CampareMD5ToGenerateVersionNum.Execute(buildTarget);
        }
    }

    private BuildAssetBundleOptions GetAssetBundleOptions()
    {
        BuildAssetBundleOptions buildAssetOptions = 0;
        if(collectDependencies)
        {
            if(buildAssetOptions == 0)
            {
                buildAssetOptions = BuildAssetBundleOptions.CollectDependencies;
            }
            else
            {
                buildAssetOptions |= BuildAssetBundleOptions.CollectDependencies;
            }
        }

        if(completeAssets)
        {
            if(buildAssetOptions == 0)
            {
                buildAssetOptions = BuildAssetBundleOptions.CompleteAssets;
            }
            else
            {
                buildAssetOptions |= BuildAssetBundleOptions.CompleteAssets;
            }
        }
        if(disableWriteTypeTree)
        {
            if(buildAssetOptions == 0)
            {
                buildAssetOptions = BuildAssetBundleOptions.DisableWriteTypeTree;
            }
            else
            {
                buildAssetOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
            }
        }
        if(deterministicAssetBundle)
        {
            if(buildAssetOptions == 0)
            {
                buildAssetOptions = BuildAssetBundleOptions.DeterministicAssetBundle;
            }
            else
            {
                buildAssetOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            }
        }
        if(uncompressedAssetBundle)
        {
            if(buildAssetOptions == 0)
            {
                buildAssetOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
            }
            else
            {
                buildAssetOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }
        }
        return buildAssetOptions;
    }

    public static string GetPlatformPath(BuildTarget target)
    {
        string savePath;
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                savePath = "release/Bundles/Windows32/";
                break;
            case BuildTarget.StandaloneWindows64:
                savePath = "release/Bundles/Windows64/";
                break;
            case BuildTarget.iPhone:
                savePath = "release/Bundles/IOS/";
                break;
            case BuildTarget.StandaloneOSXUniversal:
                savePath = "release/Bundles/Mac/";
                break;
            case BuildTarget.Android:
                savePath = "release/Bundles/Android/";
                break;
            case BuildTarget.WebPlayer:
                savePath = "release/Bundles/WebPlayer/";
                break;
            default:
                savePath = "release/Bundles/AssetBundle/";
                break;
        }
        return savePath;
    }

    public static string GetPlatformName(BuildTarget target)
    {
        string platform = "Windows32";
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                platform = "Windows32";
                break;
            case BuildTarget.StandaloneWindows64:
                platform = "Windows64";
                break;
            case BuildTarget.iPhone:
                platform = "IOS";
                break;
            case BuildTarget.StandaloneOSXUniversal:
                platform = "Mac";
                break;
            case BuildTarget.Android:
                platform = "Android";
                break;
            case BuildTarget.WebPlayer:
                platform = "WebPlayer";
                break;
        }
        return platform;
    }
}
using UnityEditor;
using UnityEngine;
using System.Collections;

public class BundleHelper : MonoBehaviour {

    public static string GetPlatformPath(BuildTarget target)
    {
        string savePath;
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                savePath = "Bundles/Windows32/";
                break;
            case BuildTarget.StandaloneWindows64:
                savePath = "Bundles/Windows64/";
                break;
            case BuildTarget.iPhone:
                savePath = "Bundles/IOS/";
                break;
            case BuildTarget.StandaloneOSXUniversal:
                savePath = "Bundles/Mac/";
                break;
            case BuildTarget.Android:
                savePath = "Bundles/Android/";
                break;
            case BuildTarget.WebPlayer:
                savePath = "Bundles/WebPlayer/";
                break;
            default:
                savePath = "Bundles/AssetBundle/";
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

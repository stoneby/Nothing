public class AssetBundlePathProvider
{
    public static string BaseVersionPath = "BundleVersions";
    public static string VersionFileName = "VersionNum";
    public static string GetPlatform()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IPHONE
         return "IOS";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        return "Windows";
#else
        string.Empty;
#endif
    }

    public static string GetBundleBaseUrl()
    {
        var platform = GetPlatform();
        return string.Format("{0}/{1}/{2}/{3}/{4}/", ServiceManager.ResourceUrl, ResourcePath.BundlePath,
                             GameConfig.Version, platform, GameConfig.Language);
    }

    public static string GetVerionPathInResource()
    {
        var platform = GetPlatform();
        return string.Format("{0}/{1}/{2}", BaseVersionPath, platform, VersionFileName);
    }
}

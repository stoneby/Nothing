public class AssetBundlePathProvider
{
    public static string BasePath = "http://27.131.223.229/client_res/tech_external";
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
        return string.Format("{0}/{1}/{2}/{3}/{4}/", BasePath, ResourcePath.BundlePath,
                             GameConfig.Version, platform, GameConfig.Language);
    }
}

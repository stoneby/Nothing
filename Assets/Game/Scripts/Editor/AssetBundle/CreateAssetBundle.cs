using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class CreateAssetBundle
{
    public static void Execute(BuildTarget target, bool buildSeparately, BuildAssetBundleOptions buildAssetOptions)
    {
        var platformPath = BundleCreaterWindow.GetPlatformPath(target);
        if (!Directory.Exists(platformPath))
        {
            Directory.CreateDirectory(platformPath);
        }
        var selectObjs =
            Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets).Where(item => item != Selection.activeObject).ToArray();
        if (!buildSeparately)
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var bundleName = Utils.ConvertToAssetBundleName(path);
            if(bundleName.Contains(".Resources."))
            {
                var index = bundleName.IndexOf(".Resources.");
                bundleName = bundleName.Substring(index + ".Resources.".Length);
            }
            path = platformPath + bundleName;
            path += ".assetbundle";
            BuildPipeline.BuildAssetBundle(null, selectObjs, path, buildAssetOptions);
        }
        else
        {
            // 当前选中的资源列表
            foreach (var o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
            {
                var path = AssetDatabase.GetAssetPath(o);

                // 过滤掉meta文件和文件夹
                if (path.Contains(".meta") || path.Contains(".") == false)
                    continue;
                var bundleName = Utils.ConvertToAssetBundleName(path);
                if (bundleName.Contains(".Resources."))
                {
                    var index = bundleName.IndexOf(".Resources.");
                    bundleName = bundleName.Substring(index + ".Resources.".Length);
                }
                bundleName = bundleName.Substring(0, bundleName.LastIndexOf('.'));
                bundleName += ".assetbundle";
                path = platformPath + bundleName;
                BuildPipeline.BuildAssetBundle(o, null, path, buildAssetOptions, target);
            }
        }
        AssetDatabase.Refresh();
    }
}
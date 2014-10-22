using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using VersionInfo = BundleUtils.VersionInfo;

public class GenerateVersionNum
{
    public static Dictionary<string, string> GetMd5Values(string platformPath)
    {
        var dicFileMd5 = new Dictionary<string, string>();
        var md5Generator = new MD5CryptoServiceProvider();

        //var dir = Path.Combine(Application.dataPath, "AssetBundle/" + platform);
        foreach (var filePath in Directory.GetFiles(platformPath))
        {
            if (filePath.Contains(".meta") || filePath.Contains("VersionMD5") || filePath.Contains(".xml"))
            {
                continue;
            }
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var hash = md5Generator.ComputeHash(file);
            var strMd5 = System.BitConverter.ToString(hash);
            file.Close();

            string key = filePath.Substring(platformPath.Length, filePath.Length - platformPath.Length);

            if (dicFileMd5.ContainsKey(key) == false)
                dicFileMd5.Add(key, strMd5);
            else
                Debug.LogWarning("<Two File has the same name> name = " + filePath);
        }
        return dicFileMd5;
    }

    public static void Execute(BuildTarget target)
    {
        var platformPath = BundleCreaterWindow.GetPlatformPath(target);
        Execute(platformPath);
        AssetDatabase.Refresh();
    }

    public static void Execute(string platformPath)
    {
        // 读取版本号记录文件VersinNum.xml
        var oldVersionNum = platformPath + "VersionNum.xml";
        var dicVersionNumInfo = ReadVersionNumFile(oldVersionNum);

        var dicNewMD5Info = GetMd5Values(platformPath);

        // 对比新旧MD5信息，并更新版本号，即对比dicNewMD5Info&&dicOldMD5Info来更新dicVersionNumInfo
        foreach (var newPair in dicNewMD5Info)
        {
            // 旧版本中有
            if (dicVersionNumInfo.Any(item => item.Value.Md5 == newPair.Key))
            {
                // MD5一样，则不变
                // MD5不一样，则+1
                if (newPair.Value != dicVersionNumInfo[newPair.Key].Md5)
                {
                    var varsionInfo = dicVersionNumInfo[newPair.Key];
                    varsionInfo.Version = varsionInfo.Version + 1;
                }
            }
            else // 旧版本中没有，则添加新纪录，并=1
            {
                var varsionInfo = new VersionInfo(newPair.Value, 1);
                dicVersionNumInfo.Add(newPair.Key, varsionInfo);
            }
        }
        // 存储最新的VersionNum.xml
        SaveVersionNumFile(dicVersionNumInfo, oldVersionNum);
    }

    static void SaveVersionNumFile(Dictionary<string, VersionInfo> data, string savePath)
    {
        var xmlDoc = new XmlDocument();
        var xmlRoot = xmlDoc.CreateElement("VersionNum");
        xmlDoc.AppendChild(xmlRoot);

        foreach (var pair in data)
        {
            var xmlElem = xmlDoc.CreateElement("File");
            xmlRoot.AppendChild(xmlElem);
            xmlElem.SetAttribute("FileName", pair.Key);
            xmlElem.SetAttribute("Md5", pair.Value.Md5);
            xmlElem.SetAttribute("Num", XmlConvert.ToString(pair.Value.Version));
        }
        xmlDoc.Save(savePath);
    }

    static Dictionary<string, VersionInfo> ReadVersionNumFile(string fileName)
    {
        var dicVersionNum = new Dictionary<string, VersionInfo>();
        // 如果文件不存在，则直接返回
        if (File.Exists(fileName) == false)
        {
            return dicVersionNum;
        }
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(fileName);
        var xmlRoot = xmlDoc.DocumentElement;
        return BundleUtils.ReadVersionNumFile(xmlRoot);
    }
}
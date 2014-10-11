using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Security.Cryptography;

public class CreateMD5List
{
    public static void Execute(BuildTarget target)
    {
        string platform = BundleCreaterWindow.GetPlatformPath(target);
        Execute(platform);
        AssetDatabase.Refresh();
    }

    public static void Execute(string platformPath)
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
            var strMD5 = System.BitConverter.ToString(hash);
            file.Close();

            string key = filePath.Substring(platformPath.Length, filePath.Length - platformPath.Length);

            if (dicFileMd5.ContainsKey(key) == false)
                dicFileMd5.Add(key, strMD5);
            else
                Debug.LogWarning("<Two File has the same name> name = " + filePath);
        }
        string savePath = platformPath + "/VersionNum";
        if (Directory.Exists(savePath) == false)
        {
            Directory.CreateDirectory(savePath);
        }
        // 删除前一版的old数据
        if (File.Exists(savePath + "/VersionMD5-old.xml"))
        {
            File.Delete(savePath + "/VersionMD5-old.xml");
        }
        // 如果之前的版本存在，则将其名字改为VersionMD5-old.xml
        if (File.Exists(savePath + "/VersionMD5.xml"))
        {
            File.Move(savePath + "/VersionMD5.xml", savePath + "/VersionMD5-old.xml");
        }
        var xmlDoc = new XmlDocument();
        var xmlRoot = xmlDoc.CreateElement("Files");
        xmlDoc.AppendChild(xmlRoot);
        foreach (var pair in dicFileMd5)
        {
            var xmlElem = xmlDoc.CreateElement("File");
            xmlRoot.AppendChild(xmlElem);
            xmlElem.SetAttribute("FileName", pair.Key);
            xmlElem.SetAttribute("MD5", pair.Value);
        }
        // 读取旧版本的MD5
        var dicOldMD5 = ReadMD5File(savePath + "/VersionMD5-old.xml");
        // VersionMD5-old中有，而VersionMD5中没有的信息，手动添加到VersionMD5
        foreach (var pair in dicOldMD5)
        {
            if (dicFileMd5.ContainsKey(pair.Key) == false)
                dicFileMd5.Add(pair.Key, pair.Value);
        }
        xmlDoc.Save(savePath + "/VersionMD5.xml");
    }

    static Dictionary<string, string> ReadMD5File(string fileName)
    {
        var dicMd5 = new Dictionary<string, string>();
        // 如果文件不存在，则直接返回
        if (File.Exists(fileName) == false)
        {
            return dicMd5;
        }
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(fileName);
        var xmlRoot = xmlDoc.DocumentElement;
        foreach (XmlNode node in xmlRoot.ChildNodes)
        {
            if ((node is XmlElement) == false)
            {
                continue;
            }
            var file = (node as XmlElement).GetAttribute("FileName");
            var md5 = (node as XmlElement).GetAttribute("MD5");
            if (dicMd5.ContainsKey(file) == false)
            {
                dicMd5.Add(file, md5);
            }
        }
        return dicMd5;
    }

}
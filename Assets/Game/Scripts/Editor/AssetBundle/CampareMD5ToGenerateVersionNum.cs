using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class CampareMD5ToGenerateVersionNum
{
    public static void Execute(BuildTarget target)
    {
        var platformPath = BundleCreaterWindow.GetPlatformPath(target);
        Execute(platformPath);
        AssetDatabase.Refresh();
    }

    // 对比对应版本目录下的VersionMD5和VersionMD5-old，得到最新的版本号文件VersionNum.xml
    public static void Execute(string platformPath)
    {
        // 读取新旧MD5列表
        var newVersionMD5 = platformPath + "VersionNum/VersionMD5.xml";
        var oldVersionMD5 = platformPath + "VersionNum/VersionMD5-old.xml";

        var dicNewMD5Info = ReadMD5File(newVersionMD5);
        var dicOldMD5Info = ReadMD5File(oldVersionMD5);

        // 读取版本号记录文件VersinNum.xml
        var oldVersionNum = platformPath + "VersionNum/VersionNum.xml";
        var dicVersionNumInfo = ReadVersionNumFile(oldVersionNum);

        // 对比新旧MD5信息，并更新版本号，即对比dicNewMD5Info&&dicOldMD5Info来更新dicVersionNumInfo
        foreach (var newPair in dicNewMD5Info)
        {
            // 旧版本中有
            if (dicOldMD5Info.ContainsKey(newPair.Key))
            {
                // MD5一样，则不变
                // MD5不一样，则+1
                // 容错：如果新旧MD5都有，但是还没有版本号记录的，则直接添加新纪录，并且将版本号设为1
                if (dicVersionNumInfo.ContainsKey(newPair.Key) == false)
                {
                    dicVersionNumInfo.Add(newPair.Key, 1);
                }
                else if (newPair.Value != dicOldMD5Info[newPair.Key])
                {
                    var num = dicVersionNumInfo[newPair.Key];
                    dicVersionNumInfo[newPair.Key] = num + 1;
                }
            }
            else // 旧版本中没有，则添加新纪录，并=1
            {
                dicVersionNumInfo.Add(newPair.Key, 1);
            }
        }
        // 不可能出现旧版本中有，而新版本中没有的情况，原因见生成MD5List的处理逻辑

        // 存储最新的VersionNum.xml
        SaveVersionNumFile(dicVersionNumInfo, oldVersionNum);
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

    static Dictionary<string, int> ReadVersionNumFile(string fileName)
    {
        var dicVersionNum = new Dictionary<string, int>();
        // 如果文件不存在，则直接返回
        if (File.Exists(fileName) == false)
        {
            return dicVersionNum;
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
            var num = XmlConvert.ToInt32((node as XmlElement).GetAttribute("Num"));
            if (dicVersionNum.ContainsKey(file) == false)
            {
                dicVersionNum.Add(file, num);
            }
        }
        return dicVersionNum;
    }

    static void SaveVersionNumFile(Dictionary<string, int> data, string savePath)
    {
        var xmlDoc = new XmlDocument();
        var xmlRoot = xmlDoc.CreateElement("VersionNum");
        xmlDoc.AppendChild(xmlRoot);

        foreach (var pair in data)
        {
            var xmlElem = xmlDoc.CreateElement("File");
            xmlRoot.AppendChild(xmlElem);
            xmlElem.SetAttribute("FileName", pair.Key);
            xmlElem.SetAttribute("Num", XmlConvert.ToString(pair.Value));
        }

        xmlDoc.Save(savePath);
    }

}
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;

public class BundleUtils 
{
    public struct VersionInfo
    {
        public string Md5 { get; set; }
        public int Version { get; set; }

        public VersionInfo(string md5, int version)
            : this()
        {
            Md5 = md5;
            Version = version;
        }
    }

    public static Dictionary<string, VersionInfo> ReadVersionNumFile(XmlElement xmlRoot)
    {
        var dicVersionNum = new Dictionary<string, VersionInfo>();
        if (xmlRoot != null)
        {
            foreach (XmlNode node in xmlRoot.ChildNodes)
            {
                if ((node is XmlElement) == false)
                {
                    continue;
                }
                var element = (node as XmlElement);
                var file = element.GetAttribute("FileName");
                var md5 = element.GetAttribute("Md5");
                var num = XmlConvert.ToInt32(element.GetAttribute("Num"));
                if (dicVersionNum.ContainsKey(file) == false)
                {
                    var versionInfo = new VersionInfo(md5, num);
                    dicVersionNum.Add(file, versionInfo);
                }
            }
        }
        return dicVersionNum;
    }

    public static void SaveVersionNumFile(Dictionary<string, VersionInfo> data, string savePath)
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

    public static string ComputeMd5(byte[] bytes)
    {
        var md5Generator = new MD5CryptoServiceProvider();
        var hash = md5Generator.ComputeHash(bytes);
        return System.BitConverter.ToString(hash);
    }
}

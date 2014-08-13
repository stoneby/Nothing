using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using KXSGCodec;
using UnityEngine;
using System.Collections;

#if !SILVERLIGHT
[Serializable]
#endif
public class FriendVO
{
    private const int FieldCount = 1;
    private const string DataClassName = "Data:";

    public FriendInfo Data;

    [XmlIgnore]
    public bool IsFriend;

    public void WriteClass(StreamWriter writer, string className)
    {
        writer.Write(className);
        Data.WriteClass(writer, DataClassName);
    }

    public void ReadClass(string value)
    {
        string[] splitStrings=new string[]{DataClassName};
        string[] outStrings = value.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings,FieldCount);
        Data=new FriendInfo();
        Data.ReadClass(outStrings[0]);
    }
}

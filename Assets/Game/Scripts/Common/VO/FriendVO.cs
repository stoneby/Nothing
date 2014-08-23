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
    //Nums count and ClassName.
    private const int FieldCount = 1;
    private const string DataClassName = "Data:";

    public FriendInfo Data;

    [XmlIgnore]
    public bool IsFriend;

    /// <summary>
    /// Write this whole class to stream.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="className"></param>
    public void WriteClass(StreamWriter writer, string className)
    {
        writer.Write(className);
        Data.WriteClass(writer, DataClassName);
    }

    /// <summary>
    /// Read this whole class from string.
    /// </summary>
    /// <param name="value"></param>
    public void ReadClass(string value)
    {
        string[] splitStrings=new string[]{DataClassName};
        string[] outStrings = value.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings,FieldCount);
        Data=new FriendInfo();
        Data.ReadClass(outStrings[0]);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class PersistenceFileIOHandler
{
    public static void WriteBasic(StreamWriter writer, object basic)
    {
        writer.Write(basic + "\t");
    }

    public static void WriteList<T>(StreamWriter writer, string listName, List<T> list)
    {
        if (listName != null)
        {
            writer.Write(listName);
        }

        for (int i = 0; i < list.Count; i++)
        {
            WriteBasic(writer, list[i]);
        }
    }

    public static void WriteDic<T1, T2>(StreamWriter writer, string dicName, Dictionary<T1, T2> dic)
    {
        if (dicName != null)
        {
            writer.Write(dicName);
        }

        if (dic != null)
        {
            foreach (var pair in dic)
            {
                WriteBasic(writer, pair.Key);
                WriteBasic(writer, pair.Value);
            }
        }
    }

    public static List<T> ReadList<T>(string[] listStrings, int startIndex, int endIndex)
    {
        if (startIndex >= listStrings.Length)
        {
            Logger.LogError("ReadList: endindex > startindex, readlist aborted! Please check the data.");
            throw new Exception("ReadList: endindex > startindex");
        }

        var returnList = new List<T>();
        for (int i = startIndex; i <= endIndex; i++)
        {
            returnList.Add(Convert<T>(listStrings[i]));
        }
        return returnList;
    }

    public static Dictionary<T1, T2> ReadDic<T1, T2>(string[] listStrings, int startIndex, int endIndex)
    {
        if (startIndex >= listStrings.Length)
        {
            Logger.LogError("ReadDic: endindex > startindex, readdic aborted! Please check the data.");
            throw new Exception("ReadDic: endindex > startindex");
        }

        if ((endIndex - startIndex + 1) % 2 != 0)
        {
            Logger.LogError("Not couple strings num, readdic aborted! Please check the data.");
            throw new Exception("ReadDic: Not couple strings num");
        }

        var returnDic = new Dictionary<T1, T2>();
        for (int i = startIndex; i <= endIndex; )
        {
            returnDic.Add(Convert<T1>(listStrings[i]), Convert<T2>(listStrings[i + 1]));
            i += 2;
        }
        return returnDic;
    }

    private static T Convert<T>(string input)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
        {
            //Cast ConvertFromString(string text) : object to (T)
            return (T)converter.ConvertFromString(input);
        }
        return default(T);
    }

    public static int CheckCount(string[] value, int count)
    {
        if (value.Length != count)
        {
            Logger.LogError("NotCorrect strings num! Num=" + value.Length);
            throw new Exception("NotCorrect strings num! Num=" + value.Length + "not equal to" + count);
        } 
        return value.Length;
    }

    public static int CheckCount(string[] value, int count1, int count2)
    {
        if (value.Length != count1 && value.Length != count2)
        {
            Logger.LogError("NotCorrect strings num! Num=" + value.Length);
            throw new Exception("NotCorrect strings num! Num=" + value.Length + "not equal to" + count1 + "or" + count2);
        }
        return value.Length;
    }
}

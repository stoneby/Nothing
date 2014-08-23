using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

/// <summary>
/// Handle the read and write of stored persistence file.
/// </summary>
public class PersistenceFileIOHandler
{
    /// <summary>
    /// Write basic variable with '\t' after it.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="basic"></param>
    public static void WriteBasic(StreamWriter writer, object basic)
    {
        writer.Write(basic + "\t");
    }

    /// <summary>
    /// Write generic list with '\t' after each item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer"></param>
    /// <param name="listName"></param>
    /// <param name="list"></param>
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

    /// <summary>
    /// Write generic dictionary with '\t' after each item.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="writer"></param>
    /// <param name="dicName"></param>
    /// <param name="dic"></param>
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

    /// <summary>
    /// Read generic list from string array with start and end index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listStrings"></param>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Read generic dictionary from string array with start and end index.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="listStrings"></param>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Transform string to generics, which is useful in .NET3.5.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Check string array's count.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static int CheckCount(string[] value, int count)
    {
        if (value.Length != count)
        {
            Logger.LogError("NotCorrect strings num! Num=" + value.Length);
            throw new Exception("NotCorrect strings num! Num=" + value.Length + "not equal to" + count);
        }
        return value.Length;
    }

    /// <summary>
    /// Override CheckCount function for checking mutiple count.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="count1"></param>
    /// <param name="count2"></param>
    /// <returns></returns>
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

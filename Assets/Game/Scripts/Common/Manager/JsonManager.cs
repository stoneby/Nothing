using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;

public class JsonManager
{

//    public static string Encode<T>(T t)
//    {
//        DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
//        MemoryStream ms = new MemoryStream();
//        ds.WriteObject(ms, t);
//
//        string strReturn = Encoding.UTF8.GetString(ms.ToArray());
//        ms.Close();
//        return strReturn;
//    }
//
//    public T Decode<T>(string strJson) where T : class
//    {
//        DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
//        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));
//
//        return ds.ReadObject(ms) as T;
//    }
}

﻿using UnityEngine;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Encryption Handler for Battle Persistence.
/// </summary>
public class EncryptionManagement
{
    /// <summary>
    /// Key, IV and temp variable.
    /// </summary>
    private readonly static byte[] Key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    private readonly static byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    private static RijndaelManaged RMCrypto = new RijndaelManaged();

    /// <summary>
    /// Return stream to write file in encrypted way.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static CryptoStream Encrypt(string path)
    {
        FileStream encryptWriter = new FileStream(path, FileMode.Append,FileAccess.Write,FileShare.Delete);
        var returnStream = new CryptoStream(encryptWriter, RMCrypto.CreateEncryptor(Key, IV), CryptoStreamMode.Write);
        Debug.Log("Return encrypt stream");
        return returnStream;
    }

    /// <summary>
    /// Return stream to read encrypted file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static CryptoStream Decrypt(string path)
    {
        FileStream encryptReader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Delete);
        var returnStream = new CryptoStream(encryptReader, RMCrypto.CreateDecryptor(Key, IV), CryptoStreamMode.Read);
        Debug.Log("Return encrypt stream");
        return returnStream;
    }
}
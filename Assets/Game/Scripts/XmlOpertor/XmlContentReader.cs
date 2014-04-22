using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Xml content reader, simply cover reading path for both android and ios.
/// Android path like "jar://file//.../...apk!/streamingdata/..", have to use WWW class.
/// IOS path is normal like pc / mac path, use directly fetch instead.
/// </summary>
public class XmlContentReader : MonoBehaviour
{
    /// <summary>
    /// Xml read complete event.
    /// </summary>
    public event EventHandler<EventArgs> XmlReadComplete;

    /// <summary>
    /// Current xml content cache.
    /// </summary>
    /// <remarks>
    /// Please make sure using this field after XmlReadComplete event has fired.
    /// </remarks>
    public string Content { get; private set; }

    public void ReadXml(string path)
    {
        StartCoroutine(DoReadXml(path));
    }

    IEnumerator DoReadXml(string path)
    {
        if (path.Contains("://"))
        {
            var www = new WWW(path);
            yield return www;
            Content = www.text;
        }
        else
        {
            Content = System.IO.File.ReadAllText(path);
        }

        if (XmlReadComplete != null)
        {
            XmlReadComplete(this, new EventArgs());
        }
    }

    /// <summary>
    /// Get XmlContentReader instance.
    /// </summary>
    /// <param name="go">Game object which need this instance</param>
    /// <returns>The XmlContentReader</returns>
    public static XmlContentReader Get(GameObject go)
    {
        return go.GetComponent<XmlContentReader>() ?? go.AddComponent<XmlContentReader>();
    }
}

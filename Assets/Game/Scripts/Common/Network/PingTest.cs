﻿using System;
using System.Net;
using UnityEngine;
using System.Collections;

/// <summary>
/// Ping test utilities.
/// </summary>
public class PingTest : Singleton<PingTest>
{
    #region Public Fields

    public string IpAddress;

    public bool HasConnection { get; set; }

    #endregion

    #region Private Fields

    private const float MaxTime = 2.0f;

    #endregion

    #region Public Methods

    public IEnumerator TestConnection()
    {
        return TestConnection(IpAddress);
    }

    /// <summary>
    /// Test whether player has connect to network.
    /// </summary>
    /// <param name="ipAddress">IP address to ping through</param>
    /// <returns>IEnumerator</returns>
    public IEnumerator TestConnection(string ipAddress)
    {
        var timeTaken = 0.0F;
        var testPing = new Ping(ipAddress);
        while (!testPing.isDone)
        {
            timeTaken += Time.deltaTime;
            if (timeTaken > MaxTime)
            {
                // if time has exceeded the max time, break out and return false
                HasConnection = false;
                break;
            }
            yield return null;
        }
        if (timeTaken <= MaxTime)
        {
            HasConnection = true;
        }
    }

    public bool IsWebResourceAvailable(string webResourceAddress)
    {
        try
        {
            var req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(webResourceAddress));
            req.Method = "HEAD";
            req.Timeout = 5000;
            var res = (HttpWebResponse)req.GetResponse();
            return (res.StatusCode == HttpStatusCode.OK);
        }
        catch (WebException wex)
        {
            //System.Diagnostics.Trace.Write(wex.Message);
            return false;
        }
    }
    
    public void CheckConnection()
    {
        StartCoroutine("DoCheckConnection");
    }

    private IEnumerator DoCheckConnection()
    {
        yield return null;
        if (!Instance.IsWebResourceAvailable(GameConfig.OfficialSiteAddress))
        {
            // Show assert window.
            Alert.Show(AssertionWindow.Type.Ok, "系统提示", "网络连接失败，请您接入网络后再试", OnAssertButtonClicked);
        }
        else if (!Instance.IsWebResourceAvailable(GameConfig.ServicePath))
        {
            // Show assert window.
            Alert.Show(AssertionWindow.Type.Ok, "系统提示", "无法连接至服务器，请检查您的网络连接再试", OnAssertButtonClicked);
        }
    }

    private void OnAssertButtonClicked(GameObject sender)
    {
        StartCoroutine(DoCheckConnection());
    }

    #endregion
}

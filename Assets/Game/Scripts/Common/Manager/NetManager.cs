using System;
using System.IO;
using System.Net;
using System.Threading;
using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using Thrift.Protocol;
using UnityEngine;
using System.Collections;

public class NetManager
{

    //private const string URL = "http://172.16.7.131:8080/sglm-webserver/request";//李媛
    //private const string URL = "http://172.16.7.132:8080/sglm-webserver/request";//方勇
    //private const string URL = "http://27.131.223.229:8080/sglm-webserver/request";
    private const string METHOD = "POST";
    private static ClientSocket SocketClient = new ClientSocket("127.0.0.1", "8080");

    private static Queue CSMsgQueue = new Queue();
    private static Queue SCMsgQueue = new Queue();
    private static Thread MsgThread = new Thread(new ThreadStart(DoSend));

    private static string SessionId = "";

    public static void SendMessage(TBase msg)
    {
         lock (CSMsgQueue.SyncRoot)
         {
             CSMsgQueue.Enqueue(msg);
         }
           
         if (MsgThread.ThreadState == ThreadState.Unstarted)
         {
             MsgThread.Start();
         }       
    }

    public static ThriftSCMessage GetMessage()
    {
        if (SCMsgQueue == null || SCMsgQueue.Count <= 0)
        {
            return null;
        }

        ThriftSCMessage msg = null;
        lock (SCMsgQueue.SyncRoot)
        {
            try
            {
                msg = SCMsgQueue.Dequeue() as ThriftSCMessage;
            }
            catch (Exception e)
            {

                Logger.ThreadLog("An error occurred GetMessage: " + e.Message);
            }         
        }

        return msg;
    }

    private static void DoSend()
    {
        while (true)
        {
            if (CSMsgQueue.Count <= 0)
            {
                Thread.Sleep(1000);
                continue;
            }

            HttpWebRequest hwrequest = null;
            Stream postStream = null;
            HttpWebResponse hwresponse = null;
            Stream responseStream = null;
            try
            {
                TBase csMsg = null;
                lock (CSMsgQueue.SyncRoot)
                {
                    csMsg = CSMsgQueue.Dequeue() as TBase;
                }
                if (csMsg == null)
                {
                    continue;
                }
                Logger.ThreadLog("Do send" + csMsg.GetType());

                hwrequest = (HttpWebRequest) WebRequest.Create(ServiceManager.ServerData.Url);
                hwrequest.Accept = "*/*";
                hwrequest.AllowAutoRedirect = true;
                hwrequest.UserAgent = "http_requester/0.1";
                hwrequest.Timeout = 60000;
                hwrequest.Method = METHOD;
                hwrequest.Headers.Add("ISESSION", SessionId);
                hwrequest.ContentType = "multipart/form-data";

                var msg = new ThriftCSMessage(csMsg);
                byte[] postByteArray = msg.Encode();
                if (postByteArray == null || postByteArray.Length <= 0)
                {
                    // ClientLog.Instance.LogError("send data is null or length is 0, msg type = " + csMsg.GetType().ToString());
                    continue;
                }
                hwrequest.ContentLength = postByteArray.Length;

                postStream = hwrequest.GetRequestStream();
                postStream.Write(postByteArray, 0, postByteArray.Length);
                postStream.Close();
                postStream = null;


                // deal with receive sc msg
                hwresponse = (HttpWebResponse) hwrequest.GetResponse();
                Logger.ThreadLog("response status:" + hwresponse.StatusCode);
                if (hwresponse.StatusCode == HttpStatusCode.OK)
                {

                    int respLen = (int) hwresponse.ContentLength;
                    Logger.ThreadLog("respLen:" + respLen);
                    if (respLen <= 0)
                    {
                        hwresponse.Close();
                        hwresponse = null;
                        continue;
                    }

                    byte[] recDatas = new byte[respLen];
                    responseStream = hwresponse.GetResponseStream();
                    responseStream.Read(recDatas, 0, respLen);
                    responseStream.Close();
                    responseStream = null;
                    SocketClient.DecodeMsg(recDatas, respLen, SCMsgQueue);                   

                    SessionId = hwresponse.GetResponseHeader("ISESSION");
                }

                hwresponse.Close();
                hwresponse = null;
            }
            catch (Exception e)
            {
                Logger.ThreadLog("An error occurred: " + e.Message);
                ThriftSCMessage globalmessage = new ClientSCMessage((short) MessageType.SC_SYSTEM_INFO_MSG, e.Message);
                lock (SCMsgQueue.SyncRoot)
                {
                    SCMsgQueue.Enqueue(globalmessage);
                }
            }
            finally
            {
                if (postStream != null)
                {
                    try
                    {
                        postStream.Close();
                    }
                    catch (Exception e)
                    {

                        Logger.ThreadLog("An error occurred close postStream: " + e.Message);
                    }
                   
                }
                if (responseStream != null)
                {
                    try
                    {
                        responseStream.Close();
                    }
                    catch (Exception e)
                    {

                        Logger.ThreadLog("An error occurred close responseStream: " + e.Message);
                    }

                }
                if (  hwresponse != null)
                {
                    
                    try
                    {
                        hwresponse.Close();
                    }
                    catch (Exception e)
                    {

                        Logger.ThreadLog("An error occurred close hwresponse: " + e.Message);
                    }
                }
            }

        }
    }
}

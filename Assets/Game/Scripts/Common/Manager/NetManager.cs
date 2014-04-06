using System;
using System.IO;
using System.Net;
using System.Threading;
using KXSGCodec;
using Thrift.Protocol;
using UnityEngine;
using System.Collections;

public class NetManager
{
    private const string URL = "http://172.16.7.132:8080/sglm-webserver/request";
    private const string METHOD = "POST";
    private static ClientSocket SocketClient = new ClientSocket("127.0.0.1", "8080");

    private static Queue CSMsgQueue;
    private static Queue SCMsgQueue;
    private static Thread MsgThread;

    public static void SendMessage(TBase msg)
    {
        if (CSMsgQueue == null)
        {
            CSMsgQueue = new Queue();
            SCMsgQueue = new Queue();
        }
        CSMsgQueue.Enqueue(msg);
        if (MsgThread == null)
        {
            var entry = new ThreadStart(DoSend);
            MsgThread = new Thread(entry);
        }

        //Debug.Log(MsgThread.ThreadState);
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

        var msg = SCMsgQueue.Dequeue() as ThriftSCMessage;
        Debug.Log(msg);
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

            string responseData = "";
            try
            {
                var hwrequest = (HttpWebRequest)WebRequest.Create(URL);
                hwrequest.Accept = "*/*";
                hwrequest.AllowAutoRedirect = true;
                hwrequest.UserAgent = "http_requester/0.1";
                hwrequest.Timeout = 60000;
                hwrequest.Method = METHOD;
                hwrequest.ContentType = "multipart/form-data";
                Debug.Log("Do send" + CSMsgQueue.Count.ToString());
                var csMsg = CSMsgQueue.Dequeue() as TBase;
                //Debug.Log("Do send" + CSMsgQueue.Count.ToString());
                var msg = new ThriftCSMessage(csMsg);
                byte[] postByteArray = msg.Encode();
                if (postByteArray == null || postByteArray.Length <= 0)
                {
                    // ClientLog.Instance.LogError("send data is null or length is 0, msg type = " + csMsg.GetType().ToString());
                    continue;
                }

                hwrequest.ContentLength = postByteArray.Length;

                Stream postStream = hwrequest.GetRequestStream();
                postStream.Write(postByteArray, 0, postByteArray.Length);
                postStream.Close();

                var hwresponse = (HttpWebResponse)hwrequest.GetResponse();
                if (hwresponse.StatusCode == HttpStatusCode.OK)
                {
                    int respLen = (int)hwresponse.ContentLength;
                    if (respLen <= 0)
                    {
                        hwresponse.Close();
                        continue;
                    }

                    byte[] recDatas = new byte[respLen];
                    Stream responseStream = hwresponse.GetResponseStream();
                    responseStream.Read(recDatas, 0, respLen);

                    ThriftSCMessage scMsg = SocketClient.DecodeMsg(recDatas, respLen);
                    SCMsgQueue.Enqueue(scMsg);
                    Debug.Log(scMsg);
                    Debug.Log(SCMsgQueue.Count);
                }
                hwresponse.Close();
            }
            catch (Exception e)
            {
                responseData = "An error occurred: " + e.Message;
            }

        }
    }  
}

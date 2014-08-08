using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using Thrift.Protocol;

public class NetManager
{
    private const string Method = "POST";
    private static readonly ClientSocket SocketClient = new ClientSocket("127.0.0.1", "8080");

    private static readonly Queue CSMsgQueue = new Queue();
    private static readonly Queue SCMsgQueue = new Queue();
    private static readonly Thread MsgThread = new Thread(DoSend);

    private static string sessionId = "";
    private const int PkgReadLenPerTime = 1024;

    public delegate void MessageSended();
    public static MessageSended OnMessageSended;

    public static void SendMessage(TBase msg, bool playEffect = true)
    {
        lock (CSMsgQueue.SyncRoot)
        {
            CSMsgQueue.Enqueue(msg);
            if (OnMessageSended != null)
            {
                OnMessageSended();
            }
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
                Logger.Log("An error occurred GetMessage: " + e.Message);
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

            Stream postStream = null;
            HttpWebResponse hwresponse = null;
            Stream responseStream = null;
            try
            {
                TBase csMsg;
                lock (CSMsgQueue.SyncRoot)
                {
                    csMsg = CSMsgQueue.Dequeue() as TBase;
                }
                if (csMsg == null)
                {
                    continue;
                }

                Logger.Log("Do send" + csMsg.GetType());

                var hwrequest = (HttpWebRequest)WebRequest.Create(ServiceManager.ServerData.Url);
                hwrequest.Accept = "*/*";
                hwrequest.AllowAutoRedirect = true;
                hwrequest.UserAgent = "http_requester/0.1";
                hwrequest.Timeout = 60000;
                hwrequest.Method = Method;
                hwrequest.Headers.Add("ISESSION", sessionId);
                hwrequest.ContentType = "multipart/form-data";

                var msg = new ThriftCSMessage(csMsg);
                var postByteArray = msg.Encode();
                if (postByteArray == null || postByteArray.Length <= 0)
                {
                    // Logger.LogError("send data is null or length is 0, msg type = " + csMsg.GetType().ToString());
                    continue;
                }

                hwrequest.ContentLength = postByteArray.Length;

                postStream = hwrequest.GetRequestStream();
                postStream.Write(postByteArray, 0, postByteArray.Length);
                postStream.Close();
                postStream = null;

                // deal with receive sc msg
                hwresponse = (HttpWebResponse)hwrequest.GetResponse();
                Logger.Log("response status:" + hwresponse.StatusCode);
                if (hwresponse.StatusCode == HttpStatusCode.OK)
                {

                    var respLen = (int)hwresponse.ContentLength;
                    Logger.Log("respLen:" + respLen);
                    if (respLen <= 0)
                    {
                        hwresponse.Close();
                        hwresponse = null;
                        continue;
                    }

                    responseStream = hwresponse.GetResponseStream();
                    ByteBuffer recBuffer = new ByteBuffer(respLen);
                    var recDataPerTime = new byte[PkgReadLenPerTime];
                    long totalBytesRead = 0;
                    int bytesRead;

                    while ((bytesRead = responseStream.Read(recDataPerTime, 0, recDataPerTime.Length)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        recBuffer.Put(recDataPerTime, bytesRead);
                    }
                    if (totalBytesRead < respLen)
                    {
                        Logger.Log("An error occurred: msg read len is not:" + respLen);
                        continue;
                    }
                    recBuffer.Flip();

                    var recDatas = new byte[respLen];
                    recBuffer.Get(recDatas);
                    responseStream.Close();
                    responseStream = null;

                    SocketClient.DecodeMsg(recDatas, respLen, SCMsgQueue);

                    sessionId = hwresponse.GetResponseHeader("ISESSION");
                }

                hwresponse.Close();
                hwresponse = null;
            }
            catch (Exception e)
            {
                Logger.Log("An error occurred: " + e.Message);
                ThriftSCMessage globalmessage = new ClientSCMessage((short)MessageType.SC_SYSTEM_INFO_MSG, e.Message);
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
                        Logger.Log("An error occurred close postStream: " + e.Message);
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

                        Logger.Log("An error occurred close responseStream: " + e.Message);
                    }

                }
                if (hwresponse != null)
                {
                    try
                    {
                        hwresponse.Close();
                    }
                    catch (Exception e)
                    {
                        Logger.Log("An error occurred close hwresponse: " + e.Message);
                    }
                }
            }
        }
    }
}

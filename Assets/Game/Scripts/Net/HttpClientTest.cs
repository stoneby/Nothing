using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using KXSGCodec;

namespace WindowsApplication1
{
    class HttpClientTest
    {
        private const string url = "http://192.168.11.10:8080/sglm-webserver/request";
        private static ClientSocket socketClient = new ClientSocket("127.0.0.1", "8080");
        static void Main(string[] args)
        {
            //ClientLog.Instance.LogInfo("msg type:" + MessageType.CS_QUICK_LOGIN.GetHashCode());
            //ClientLog.Instance.LogInfo("msg type:" + MessageType.SC_ERROR_INFO.GetHashCode());
            try
            {
                //HttpClient httpClient = new HttpClient(url);
                Console.WriteLine(WRequest(url, "post"));

                Console.WriteLine(WRequest1(url, "post"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine();

        }

        public static string WRequest(string URL, string method)
        {
            string responseData = "";
            try
            {
                HttpWebRequest hwrequest = (HttpWebRequest)WebRequest.Create(URL);
                hwrequest.Accept = "*/*";
                hwrequest.AllowAutoRedirect = true;
                hwrequest.UserAgent = "http_requester/0.1";
                hwrequest.Timeout = 60000;
                hwrequest.Method = method;
                if (hwrequest.Method == "POST")
                {
                    hwrequest.ContentType = "multipart/form-data";

                    CSQuickLoginMsg csMsg = new CSQuickLoginMsg();
                    csMsg.DeviceId = "123456789";
                    csMsg.DeviceType = 2;
                   /* CSPasswdLoginMsg csMsg = new CSPasswdLoginMsg();
                    csMsg.DeviceId = "1";
                    csMsg.DeviceType = 2;
                    csMsg.Passwd = "123456";
                    csMsg.AccountName = "test";
                    */

                    ThriftCSMessage msg = new ThriftCSMessage(csMsg);
                    byte[] postByteArray = msg.Encode();
                    if (postByteArray == null || postByteArray.Length <= 0)
                    {
                       // ClientLog.Instance.LogError("send data is null or length is 0, msg type = " + csMsg.GetType().ToString());
                        return null;
                    }

                    hwrequest.ContentLength = postByteArray.Length;
                    System.IO.Stream postStream = hwrequest.GetRequestStream();
                    postStream.Write(postByteArray, 0, postByteArray.Length);
                    postStream.Close();
                }
                System.Net.HttpWebResponse hwresponse =
                  (System.Net.HttpWebResponse)hwrequest.GetResponse();
                if (hwresponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int _respLen = (int)hwresponse.ContentLength;
                    if (_respLen <=0) {
                        return responseData;
                    }

                    byte[] _recDatas = new byte[_respLen];
                    System.IO.Stream responseStream = hwresponse.GetResponseStream();
                    responseStream.Read(_recDatas, 0, _respLen);

                   ThriftSCMessage _scMsg = socketClient.DecodeMsg(_recDatas, _respLen);
                   Console.WriteLine("rec sc msg:" + _scMsg);
                   // System.IO.StreamReader myStreamReader = new System.IO.StreamReader(responseStream);
 
                   // responseData = myStreamReader.ReadToEnd();
                }
                hwresponse.Close();
            }
            catch (Exception e)
            {
                responseData = "An error occurred: " + e.Message;
            }

            //Console.WriteLine(responseData);
            return responseData;
        }

        public static string WRequest1(string URL, string method)
        {
            string responseData = "";
            try
            {
                HttpWebRequest hwrequest = (HttpWebRequest)WebRequest.Create(URL);
                hwrequest.Accept = "*/*";
                hwrequest.AllowAutoRedirect = true;
                hwrequest.UserAgent = "http_requester/0.1";
                hwrequest.Timeout = 60000;
                hwrequest.Method = method;
                if (hwrequest.Method == "POST")
                {
                    hwrequest.ContentType = "multipart/form-data";

                     CSPasswdLoginMsg csMsg = new CSPasswdLoginMsg();
                     csMsg.DeviceId = "1";
                     csMsg.DeviceType = 2;
                     csMsg.Passwd = "123456";
                     csMsg.AccountName = "test";
                   

                    ThriftCSMessage msg = new ThriftCSMessage(csMsg);
                    byte[] postByteArray = msg.Encode();
                    if (postByteArray == null || postByteArray.Length <= 0)
                    {
                        //ClientLog.Instance.LogError("send data is null or length is 0, msg type = " + csMsg.GetType().ToString());
                        return null;
                    }

                    hwrequest.ContentLength = postByteArray.Length;
                    System.IO.Stream postStream = hwrequest.GetRequestStream();
                    postStream.Write(postByteArray, 0, postByteArray.Length);
                    postStream.Close();
                }
                System.Net.HttpWebResponse hwresponse =
                  (System.Net.HttpWebResponse)hwrequest.GetResponse();
                if (hwresponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int _respLen = (int)hwresponse.ContentLength;
                    if (_respLen <= 0)
                    {
                        return responseData;
                    }

                    byte[] _recDatas = new byte[_respLen];
                    System.IO.Stream responseStream = hwresponse.GetResponseStream();
                    responseStream.Read(_recDatas, 0, _respLen);

                    ThriftSCMessage _scMsg = socketClient.DecodeMsg(_recDatas, _respLen);
                    Console.WriteLine("rec sc msg:" + _scMsg);
                    // System.IO.StreamReader myStreamReader = new System.IO.StreamReader(responseStream);

                    // responseData = myStreamReader.ReadToEnd();
                }
                hwresponse.Close();
            }
            catch (Exception e)
            {
                responseData = "An error occurred: " + e.Message;
            }

            //Console.WriteLine(responseData);
            return responseData;
        }
    }
}

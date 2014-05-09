using System;
using Thrift.Protocol;

namespace KXSGCodec
{
    public enum MessageId
    {
        None,
        DisconnectServer
    }

    /// <summary>
    /// client network process
    /// </summary>
    public class ClientNetwork
    {
        private ClientSocket socketClient;
        private float sendPingTime = -1;
        private bool canPingCheck = false;
        private DateTime lastRecvMsgTime; // for log
        public bool Loaded = false;
        private float autoReconnectTime = 0f;
        private float checkDisconnectTime = 0f;
        private string ipAddress = "192.168.11.10";
        private string serverPort = "8080";
        private float tickNetTime = 0f;
        private MessageId errorMsgId = MessageId.None;

        public string IpAddress
        {
            get { return ipAddress; }
            set { if (!string.IsNullOrEmpty(value)) ipAddress = value; }
        }

        public string ServerPort
        {
            get { return serverPort; }
            set { if (!string.IsNullOrEmpty(value)) serverPort = value; }
        }

        public bool CanShowDisconnect { get; set; }

        /// <summary>
        /// try connect to server
        /// </summary>
        public void ConnectToServer()
        {
            string serverIp = IpAddress;

            if (null == socketClient)
            {
                socketClient = new ClientSocket(serverIp, ServerPort);
            }
            else
            {
                socketClient.Close();
                socketClient = new ClientSocket(serverIp, ServerPort);
            }

            if (socketClient.IsConnected())
            {
                return;
            }
            socketClient.ResetServerAddressStatus();
            socketClient.TryConnect();
        }

        public void CloseConnect()
        {
            if (null == socketClient)
            {
                return;
            }

            socketClient.Close();
        }

        public bool CanTryConnect()
        {
            return (null != socketClient) && socketClient.CanTryConnect();
        }

        public bool IsConnected()
        {
            return (null != socketClient) && socketClient.IsConnected();
        }

        public bool IsConnectStateTimeout()
        {
            if (null == socketClient)
                return false;

            return socketClient.ClientConnectState == EClientConnectState.ConnectStateTimeOut;
        }

        public void StartSendMsg(TBase msgContent)
        {
            if (null == socketClient || !socketClient.IsConnected())
                return;

            Logger.LogError("send msg at time " + DateTime.Now + " : " + msgContent);
            socketClient.SendMessage(msgContent);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using KXSGCodec;
using KXSGLog;
using Thrift.Protocol;

namespace KXSGCodec
{
    public enum MESSAGE_ID
    {
        NONE,
        DISCONNECT_SERVER
    }

    /// <summary>
    /// client network process
    /// </summary>
    public class ClientNetwork
    {
        private KXSGCodec.ClientSocket m_SocketClient;
        private float m_fRecvPingTime = -1;
        private float m_fSendPingTime = -1;
        private bool m_CanPingCheck = false;
        private DateTime m_LastRecvMsgTime; // for log
        public bool m_bLoad = false;
        private bool m_bCanShowDisconnect = false;
        private float m_fAutoReconnectTime = 0f;
        private float m_fCheckDisconnectTime = 0f;
        private string m_strIPAddr = "192.168.11.10";
        private string m_strServerPort = "8080";
        private float m_fTickNetTime = 0f;
        private MESSAGE_ID m_eErrorMsgId = MESSAGE_ID.NONE;

        private const float RECONNECT_TIME = 30f; // Auto reconnect time
        private const float PING_DISCONNECT_TIME = 60f;
        private const float SEND_PING_TIME = 5f;	// ping server timedelta

        public string IPAddr
        {
            get { return m_strIPAddr; }
            set { if (value != null && value.Length > 0) m_strIPAddr = value; }
        }

        public string ServerPort
        {
            get { return m_strServerPort; }
            set { if (value != null && value.Length > 0) m_strServerPort = value; }
        }

        public bool CanShowDisconnect
        {
            get { return m_bCanShowDisconnect; }
            set { m_bCanShowDisconnect = value; }
        }

        void Update()
        {
            if (null == m_SocketClient)
            {
                return;
            }

            if (m_SocketClient.ClientConnectState == EClientConnectState.CONNECT_STATE_DO_TRY_CONNECT)
            {
                m_SocketClient.TryConnect();
            }

            m_SocketClient.HandleReceiveMsgs();
            int iMsgCount = m_SocketClient.GetHandleMsgCount();

            KXSGCodec.ThriftSCMessage Msg;
            for (int iLoop = 0; iLoop < iMsgCount; ++iLoop)
            {
                Msg = m_SocketClient.PopHandleMsg();
                if (null == Msg)
                    break;

                ProcessMsg(Msg);
            }
        }

        /// <summary>
        /// try connect to server
        /// </summary>
        public void ConnectToServer()
        {
            string serverIp = IPAddr;

            if (null == m_SocketClient)
            {
                m_SocketClient = new KXSGCodec.ClientSocket(serverIp, ServerPort);
            }
            else
            {
                m_SocketClient.Close();
                m_SocketClient = new KXSGCodec.ClientSocket(serverIp, ServerPort);
            }

            if (!m_SocketClient.IsConnected())
            {
                m_SocketClient.ResetServerAddressStatus();
                m_SocketClient.TryConnect();
            }
        }

        public void CloseConnect()
        {
            if (null == m_SocketClient)
                return;

            m_SocketClient.Close();
        }

        public bool CanTryConnect()
        {
            if (null == m_SocketClient)
                return false;

            return m_SocketClient.CanTryConnect();
        }

        public bool IsConnected()
        {
            if (null == m_SocketClient)
                return false;

            return m_SocketClient.IsConnected();
        }

        public bool IsConnectStateTimeout()
        {
            if (null == m_SocketClient)
                return false;

            return m_SocketClient.ClientConnectState == EClientConnectState.CONNECT_STATE_TIME_OUT;
        }

        public void StartSendMsg(TBase msgContent)
        {
            if (null == m_SocketClient || !m_SocketClient.IsConnected())
                return;

            ClientLog.Instance.LogError("send msg at time " + DateTime.Now.ToString() + " : " + msgContent.ToString());
            m_SocketClient.SendMessage(msgContent);
        }

        /// <summary>
        /// process receive msg
        /// </summary>
        /// <param name="msg"></param>
        void ProcessMsg(KXSGCodec.ThriftSCMessage msg)
        {
            TBase msgContent = msg.getContent();
            if (msgContent == null)
            {
                return;
            }

            // Reset ping time 
            m_fRecvPingTime = PING_DISCONNECT_TIME;

            ClientLog.Instance.LogError("Recv msg : " + msgContent.ToString());
            switch (msg.GetMsgType())
            {
//                case (short)KXSGCodec.MessageType.SC_USER_INFO:
//                    {
//
//                    }
//                    break;
            }

        }

        public void Recv_SystemInfo(short tipid, string[] args)
        {
            bool bSpecial = false;
            string strInfo = "";
            switch (tipid)
            {
                // change to real value
                case 1:
                    break;
            }
        }

        void Recv_ServerPing(TBase Msg)
        {

        }
    }
}
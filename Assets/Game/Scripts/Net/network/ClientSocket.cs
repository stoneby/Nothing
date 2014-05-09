using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Thrift.Protocol;

namespace KXSGCodec
{
    /// <summary>
    /// a async socket implementation, include msg send and receive
    /// </summary>
    public class ClientSocket
    {
        private const int MaxMsgPerLoop = 16;
        private const int DefaultReceiveSize = 64 * 1024;
        private const int DefaultSendSize = 32 * 1024;

        private EClientConnectState connectState = EClientConnectState.ConnectStateNone;

        // server address info
        private IpAddressWrapper[] ipAddressArry;

        private readonly Socket socketClient;

        // receive msg list
        private readonly IList<ThriftSCMessage> recMsgs;
        // receive msg copy, process from this list
        private readonly IList<ThriftSCMessage> msgCopy;

        private readonly ByteBuffer recMsgBuf;

        private readonly SCMessageRecognizor msgRecognizer;

#if USE_THREAD_RECEIVE_MSG
		Thread	mRecvThread =	null;
		bool	mThreadWork	=	false;
#endif

        private const bool SecurityPolicy = false;

        static private int showErrorIndex = 0;
        static private int errorCode = 0;
        static private SocketError socketError;

        public ClientSocket(String serverIp, String serverPorts)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = false,
                ReceiveBufferSize = DefaultReceiveSize,
                SendBufferSize = DefaultSendSize,
                ReceiveTimeout = 30000,
                SendTimeout = 30000
            };

            // Set to no blocking
            recMsgs = new List<ThriftSCMessage>();
            msgCopy = new List<ThriftSCMessage>();
            msgRecognizer = new SCMessageRecognizor();
            InitIpAddressArry(serverIp, serverPorts);

            recMsgBuf = new ByteBuffer(BaseMessage.MaxMsgLen);
        }

        /// <summary>
        /// close socket connection
        /// </summary>
        public void Close()
        {
            if (socketClient == null)
            {
                return;
            }

#if USE_THREAD_RECEIVE_MSG
			mThreadWork	=	false;
#endif
            socketClient.Close();
        }


        /// <summary>
        /// Tries the connect.
        /// </summary>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public void TryConnect()
        {
            try
            {
                IPEndPoint ep;

                if (SecurityPolicy)
                {
                    while (true)
                    {
                        ep = this.GetServerAddress();
                        if (null == ep)
                        {
                            connectState = EClientConnectState.ConnectStateTimeOut;
                            Logger.LogError("Connect timeout : no valid server ip or port");
                            return;
                        }
                        Logger.LogError("WebPlayer : Security Prefetch Socket Policy Failed : IP = " + ep.Address.ToString() + " Port = " + ep.Port.ToString() + ". To next port...");
                    }
                }
                else
                {
                    ep = GetServerAddress();
                    if (null == ep)
                    {
                        connectState = EClientConnectState.ConnectStateTimeOut;
                        Logger.LogError("Connect timeout : no valid server ip or port");

                        return;
                    }
                }

                connectState = EClientConnectState.ConnectStateTryConnect;
                socketClient.BeginConnect(ep, ConnectCallback, socketClient);
                Logger.LogError("Connect server : " + ep.Address + ":" + ep.Port);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        public void DoRetryConnect()
        {
            connectState = EClientConnectState.ConnectStateDoTryConnect;
        }

        /// <summary>
        /// get server address which has not choiced
        /// </summary>
        /// <returns></returns>
        private IPEndPoint GetServerAddress()
        {
            for (var i = 0; i < ipAddressArry.Length; i++)
            {
                var wrapper = ipAddressArry[i];
                if (wrapper.IsTried)
                {
                    continue;
                }
                wrapper.IsTried = true;
                // FIXME why
                ipAddressArry[i] = wrapper;
                Logger.Log("Try to connect : " + wrapper.IpPoint);
                return wrapper.IpPoint;
            }

            return null;
        }

        public void ResetServerAddressStatus()
        {
            connectState = EClientConnectState.ConnectStateNone;
            for (var i = 0; i < ipAddressArry.Length; i++)
            {
                var wrapper = ipAddressArry[i];
                wrapper.IsTried = false;
                ipAddressArry[i] = wrapper;
            }
        }

        private void InitIpAddressArry(String serverIp, String ports)
        {
            var ipAddress = IPAddress.Parse(serverIp);
            var tempArray = ports.Split(',');
            var portSize = tempArray.Length;
            ipAddressArry = new IpAddressWrapper[portSize];
            for (var i = 0; i < portSize; i++)
            {
                var port = Convert.ToInt32(tempArray[i].Trim());
                var addressWrapper = new IpAddressWrapper
                {
                    IpPoint = new IPEndPoint(ipAddress, port), IsTried = false
                };
                ipAddressArry[i] = addressWrapper;
            }
        }

        /// <summary>
        /// socket connect call back, if connect fail, try again. if connect successful, prepare to receive msg from server
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var socket = (Socket)ar.AsyncState;
                if (!socket.Connected)
                {
                    Logger.LogError(socket.LocalEndPoint + " connect failed!, try connect again");
                    DoRetryConnect();
                }
                else
                {
                    connectState = EClientConnectState.ConnectStateConnected;
                    socket.EndConnect(ar);
                    Logger.LogError(socket.LocalEndPoint + " connect successful!");
                    StartRecevieMsg();
                }
            }
            catch (Exception e)
            {
                connectState = EClientConnectState.ConnectStateTimeOut;
                Logger.LogError(e.ToString());
            }
        }

        /// <summary>
        /// send msg to server
        /// </summary>
        /// <param name="msgContent"></param>
        public void SendMessage(TBase msgContent)
        {
            var msg = new ThriftCSMessage(msgContent);
            var bytes = msg.Encode();
            if (bytes == null || bytes.Length <= 0)
            {
                Logger.LogError("send data is null or length is 0, msg type = " + msgContent.GetType().ToString());
                return;
            }

            // TODO Encrypt Message Data
            StartSendMsg(bytes);
        }

        public bool IsConnected()
        {
            return (null != socketClient && socketClient.Connected);
        }

        public bool CanTryConnect()
        {
            return (!IsConnected() && connectState < EClientConnectState.ConnectStateCanReconnect);
        }


        /// <summary>
        /// send encoded msg data to server
        /// </summary>
        /// <param name="bytes"></param>
        public void StartSendMsg(byte[] bytes)
        {
            try
            {
                if (!IsConnected())
                {
                    Logger.LogError("server is not connected!");
                }
                else
                {
                    socketClient.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendMsgCallback, socketClient);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// start async receive msg
        /// </summary>
        private void StartRecevieMsg()
        {
#if USE_THREAD_RECEIVE_MSG
			if( null == mRecvThread )
				mRecvThread = new Thread( RecvThreadDoWork );
			//
			if( null != mRecvThread )
			{
				mThreadWork = true;
				mRecvThread.Start();
			}
#else
            var receiveHelper = new MsgReceiveHelper
            {
                Socket = socketClient,
                Buffer = new byte[DefaultReceiveSize]
            };

            socketClient.BeginReceive(receiveHelper.Buffer, 0, receiveHelper.Buffer.Length, SocketFlags.None, ReceiveMsgCallback, receiveHelper);
#endif
        }


#if USE_THREAD_RECEIVE_MSG
		void	RecvThreadDoWork()
		{
			byte[]	_buffer = new byte[DEFAULT_RECEIVE_SIZE];
			while( mThreadWork )
			{
				try
				{
					int _recSize = this.socketClient.Receive( _buffer, DEFAULT_RECEIVE_SIZE, SocketFlags.None );
					if( _recSize > 0 )
					{
	                	this.DecodeMsg( _buffer, _recSize);
					}
					// < 0, the remote socket is close...
					else
					{
						SetShowNetworkError( 1, 0, SocketError.SocketError );					
						Logger.LogError("Socket EndReceive failed, the size is 0. The remote socket is closed. Disconnect...");
						this.Close();
						break;
					}
				}
				catch( SocketException se )
				{
					if(	se.SocketErrorCode == SocketError.WouldBlock ||
			        	se.SocketErrorCode == SocketError.IOPending ||
			          	se.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
			      	{
			        	// socket buffer is probably empty, wait and try again
			        	Thread.Sleep(50);
			      	}
					else
					{
						SetShowNetworkError( 2, se.ErrorCode, se.SocketErrorCode );							
						Logger.LogError("receive msg failed : " + se.ToString());
						Logger.LogError("Socket EndReceive Exception, ErrorCode = " + se.ErrorCode.ToString() + ", SocketErrorCode = " + se.SocketErrorCode.ToString());
						Logger.LogError("Socket fatal exception, disconnect...");
						this.Close();
						break;
					}
				}
				
				Thread.Sleep( 1 );
			}
			
			//
			mRecvThread.Join();
		}
#endif

        /// <summary>
        /// receive msg call back, decode msg and start new receive msg process
        /// </summary>
        /// <param name="receiveRes"></param>
        private void ReceiveMsgCallback(IAsyncResult receiveRes)
        {
            if (!IsConnected())
            {
                Logger.LogError("ReceiveMsgCallback : the socket is not connected!!!");
                return;
            }

            MsgReceiveHelper receiveHelper = null;
            try
            {
                var recSize = socketClient.EndReceive(receiveRes);
                if (recSize > 0)
                {
                    receiveHelper = (MsgReceiveHelper)receiveRes.AsyncState;
                    DecodeMsg(receiveHelper.Buffer, recSize, null);
                }
                else
                {
                    Logger.LogError("Socket EndReceive failed, the size is 0. The remote socket is closed. Disconnect...");
                    Close();
                }
            }
            catch (SocketException se)
            {
                Logger.LogError("receive msg failed : " + se);
                Logger.LogError("Socket EndReceive Exception, ErrorCode = " + se.ErrorCode + ", SocketErrorCode = " + se.SocketErrorCode);

                // Disconnect, WSAEWOULDBLOCK
                if (!se.SocketErrorCode.Equals(SocketError.WouldBlock))
                {
                    Logger.LogError("Socket fatal exception, disconnect...");
                    Close();
                }
            }
            catch (Exception e)
            {
                Logger.LogError("receive msg failed : " + e);
            }
            finally
            {
                if (receiveHelper != null)
                {
                    receiveHelper.Socket.BeginReceive(receiveHelper.Buffer, 0, receiveHelper.Buffer.Length, SocketFlags.None, ReceiveMsgCallback, receiveHelper);
                }
                else
                {
                    StartRecevieMsg();
                }
            }
        }

        private static void SendMsgCallback(IAsyncResult sendRes)
        {
            try
            {
                var socket = (Socket)sendRes.AsyncState;
                var nSentByte = socket.EndSend(sendRes);

                if (nSentByte <= 0)
                {
                    Logger.LogError("send msg failed!");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("send msg failed : " + e.ToString());
                var se = e as SocketException;
                if (null != se)
                {
                    Logger.LogError("Socket EndSend Exception, ErrorCode = " + se.ErrorCode + ", SocketErrorCode = " + se.SocketErrorCode);
                }
            }
        }

        /// <summary>
        /// decode bytes msg data to msg object
        /// </summary>
        /// <param name="receiveBytes"></param>
        /// <param name="size"></param>
        public void DecodeMsg(byte[] receiveBytes, int size, Queue msgQueue)
        {
            if (size <= 0)
            {
                return;
            }

            recMsgBuf.Put(receiveBytes, size);
            recMsgBuf.Flip();

            Logger.Log("DecodeMsg Buf size : " + recMsgBuf.Remaining());

            while (recMsgBuf.Remaining() >= BaseMessage.MinMsgLen)
            {
                var curPosition = recMsgBuf.Position();
                var arry = new byte[BaseMessage.MsgSizeLen];
                recMsgBuf.Get(arry);
                Array.Reverse(arry);
                var msgLen = BitConverter.ToInt16(arry, 0);
                if (msgLen <= 0)
                {
                    continue;
                }
                recMsgBuf.SetPosition(curPosition);

                // msg not receive complete
                if (msgLen > recMsgBuf.Remaining())
                {
                    break;
                }

                var msgData = new byte[msgLen];
                recMsgBuf.Get(msgData);

                try
                {
                    // TODO key msg exception handle
                    var msg = msgRecognizer.RecognizeMsg(msgData);
                    if (msg == null)
                    {
                        Logger.LogError("rec msg fail: " + msgData);
                    }
                    else
                    {

                        msg.Decode(msgData);
                        // FIXME fangyong 短连接使用消息queue， 不必要再存到list了， 做长连接时需要再修改
                        /**
                        lock (this.recMsgs)
                        {
                            this.recMsgs.Add(_msg);
                        }
                        */
                        if (msgQueue != null)
                        {
                            lock (msgQueue.SyncRoot)
                            {
                                msgQueue.Enqueue(msg);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }

                // data finish
                if (!recMsgBuf.HasRemaining())
                {
                    recMsgBuf.Clear();
                }
            }

            if (recMsgBuf.Position() != 0)
            {
                recMsgBuf.Compact();
            }
            else
            {
                recMsgBuf.SetPosition(recMsgBuf.Length());
            }
        }

        public void HandleReceiveMsgs()
        {
            lock (recMsgs)
            {
                var iMsgCount = Math.Min(recMsgs.Count, MaxMsgPerLoop);
                for (var iLoop = 0; iLoop < iMsgCount; ++iLoop)
                {
                    msgCopy.Add(recMsgs[0]);
                    recMsgs.RemoveAt(0);
                }
            }
        }

        public int GetHandleMsgCount()
        {
            return msgCopy.Count;
        }

        public ThriftSCMessage PopHandleMsg()
        {
            if (msgCopy.Count <= 0)
            {
                return null;
            }
            var msg = msgCopy[0];
            msgCopy.RemoveAt(0);

            return msg;
        }

        public EClientConnectState ClientConnectState
        {
            get { return connectState; }
            set
            {
                connectState = value;
            }
        }

    }


    public enum EClientConnectState
    {
        ConnectStateNone,
        ConnectStateTimeOut,
        ConnectStateCanReconnect,
        ConnectStateTryConnect,
        ConnectStateConnected,
        ConnectStateDoTryConnect
    }

    /// <summary>
    /// server address wrapper
    /// </summary>
    struct IpAddressWrapper
    {
        public IPEndPoint IpPoint;
        // has try connect tag
        public bool IsTried;
    };

    /// <summary>
    /// temp save object when msg received
    /// </summary>
    class MsgReceiveHelper
    {
        public Socket Socket;
        public byte[] Buffer;
    }
}

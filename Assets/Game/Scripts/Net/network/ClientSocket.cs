//#define	USE_THREAD_RECEIVE_MSG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using KXSGLog;
using Thrift.Protocol;

namespace KXSGCodec
{
    /// <summary>
    /// a async socket implementation, include msg send and receive
    /// </summary>
    public class ClientSocket
    {
        private const int MAX_MSG_PER_LOOP = 16;
        private const int DEFAULT_RECEIVE_SIZE = 64 * 1024;
		private const int DEFAULT_SEND_SIZE = 32 * 1024;

        private EClientConnectState ConnectState = EClientConnectState.CONNECT_STATE_NONE;

        // server address info
        private IPaddressWrapper[] ipAddressArry;

        private Socket socketClient;
        
        // receive msg list
        private IList<ThriftSCMessage> recMsgs;
        // receive msg copy, process from this list
        private IList<ThriftSCMessage> msgCopy;

        private ByteBuffer recMsgBuf;

        private SCMessageRecognizor msgRecognizer;

#if USE_THREAD_RECEIVE_MSG
		Thread	mRecvThread =	null;
		bool	mThreadWork	=	false;
#endif
		
		private bool	m_bSecurityPolicy	=	false;

		static	private	object		_ErrorLock	=	new object();
		static	private	int			_ShowErrorIndex = 0;
		static	private	int			_ErrorCode	=	0;
		static	private	SocketError	_SocketError;

        public ClientSocket(String serverIp, String serverPorts)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Set to no blocking
            socketClient.Blocking = false;
			socketClient.ReceiveBufferSize	=	DEFAULT_RECEIVE_SIZE;
			socketClient.SendBufferSize		=	DEFAULT_SEND_SIZE;
			socketClient.ReceiveTimeout		=	30000;
			socketClient.SendTimeout		=	30000;
            recMsgs = new List<ThriftSCMessage>();
            msgCopy = new List<ThriftSCMessage>();
            msgRecognizer = new SCMessageRecognizor();
            this.InitIpAddressArry(serverIp, serverPorts);

            recMsgBuf = new ByteBuffer(BaseMessage.MAX_MSG_LEN);
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
                IPEndPoint _ep = null;

                if (m_bSecurityPolicy)
                {
                    while (true)
                    {
                        _ep = this.GetServerAddress();
                        if (null == _ep)
                        {
                            ConnectState = EClientConnectState.CONNECT_STATE_TIME_OUT;
                            ClientLog.Instance.LogError("Connect timeout : no valid server ip or port");
                            return;
                        }
                        
                        //if (Security.PrefetchSocketPolicy(_ep.Address.ToString(), _ep.Port, 5000))
                        //    break;
						
                        ClientLog.Instance.LogError("WebPlayer : Security Prefetch Socket Policy Failed : IP = " + _ep.Address.ToString() + " Port = " + _ep.Port.ToString() + ". To next port...");
                    }
                }
                else
                {
                    _ep = this.GetServerAddress();
                    if (null == _ep)
                    {
                        ConnectState = EClientConnectState.CONNECT_STATE_TIME_OUT;
                        ClientLog.Instance.LogError("Connect timeout : no valid server ip or port");
						
                        return;
                    }
                }
				
                
                ConnectState = EClientConnectState.CONNECT_STATE_TRY_CONNECT;
                socketClient.BeginConnect(_ep, new AsyncCallback(ConnectCallback), socketClient);
				ClientLog.Instance.LogError("Connect server : " + _ep.Address.ToString() + ":" + _ep.Port.ToString() );
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError(ex.ToString());
            }
        }
		
		public	void	DoRetryConnect()
		{
			ConnectState = EClientConnectState.CONNECT_STATE_DO_TRY_CONNECT;
		}
   
        /// <summary>
        /// get server address which has not choiced
        /// </summary>
        /// <returns></returns>
        private IPEndPoint GetServerAddress()
        {
            for (int i = 0; i < ipAddressArry.Length; i++)
            {
                IPaddressWrapper _wrapper = ipAddressArry[i];       
                if (_wrapper.isTried)
                {
                    continue;
                }
                _wrapper.isTried = true;
                // FIXME why
                ipAddressArry[i] = _wrapper;
                ClientLog.Instance.LogInfo("Try to connect : " + _wrapper.ipPoint);
                return _wrapper.ipPoint;
            }

            return null;
        }

        public void ResetServerAddressStatus()
        {
            ConnectState = EClientConnectState.CONNECT_STATE_NONE;
            for (int i = 0; i < ipAddressArry.Length; i++)
            {
                IPaddressWrapper _wrapper = ipAddressArry[i];
                _wrapper.isTried = false;
                ipAddressArry[i] = _wrapper;

            }
        }

        private void InitIpAddressArry(String serverIp, String ports)
        {
            IPAddress _ipAddress = IPAddress.Parse(serverIp);
            string[] _tempArray = ports.Split(',');
            int _portSize = _tempArray.Length;
            ipAddressArry = new IPaddressWrapper[_portSize];
            for (int i = 0; i < _portSize; i++)
            {
                int _port = Convert.ToInt32(_tempArray[i].Trim());
                IPaddressWrapper _addressWrapper = new IPaddressWrapper();
                _addressWrapper.ipPoint = new IPEndPoint(_ipAddress, _port);
                _addressWrapper.isTried = false;
                ipAddressArry[i] = _addressWrapper;
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
                Socket _socket = (Socket)ar.AsyncState;
                if (!_socket.Connected)
                {
                    ClientLog.Instance.LogError(_socket.LocalEndPoint + " connect failed!, try connect again");
                    this.DoRetryConnect();
                }
                else
                {
                    ConnectState = EClientConnectState.CONNECT_STATE_CONNECTED;
                    _socket.EndConnect(ar);
                    ClientLog.Instance.LogError(_socket.LocalEndPoint + " connect successful!");
                    this.StartRecevieMsg();
                }
            }
            catch (Exception e)
            {

                ConnectState = EClientConnectState.CONNECT_STATE_TIME_OUT;
                // do something
                ClientLog.Instance.LogError(e.ToString());
            }

        }

        /// <summary>
        /// send msg to server
        /// </summary>
        /// <param name="msgContent"></param>
        public void SendMessage(TBase msgContent)
        {
            ThriftCSMessage msg = new ThriftCSMessage(msgContent);
            byte[] _bytes = msg.Encode();
            if (_bytes == null || _bytes.Length <= 0)
            {
                ClientLog.Instance.LogError("send data is null or length is 0, msg type = " + msgContent.GetType().ToString());
				return;
            }

            // TODO Encrypt Message Data

            this.StartSendMsg(_bytes);
        }

        public bool IsConnected()
        {
            return (null != this.socketClient && this.socketClient.Connected);
        }

        public bool CanTryConnect()
        {
            return (!IsConnected() && ConnectState < EClientConnectState.CONNECT_STATE_CAN_RECONNECT);
        }


        /// <summary>
        /// send encoded msg data to server
        /// </summary>
        /// <param name="bytes"></param>
        public void StartSendMsg(byte[] bytes)
        {
            try
            {
                if (!this.IsConnected())
                {
                    ClientLog.Instance.LogError("server is not connected!");
                }
                else
                {
                    socketClient.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(SendMsgCallback), socketClient);
                }
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError(ex.ToString());
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
			MsgReceiveHelper _receiveHelper = new MsgReceiveHelper();
            _receiveHelper.socket = this.socketClient;
            _receiveHelper.buffer = new byte[DEFAULT_RECEIVE_SIZE];
			
            this.socketClient.BeginReceive(_receiveHelper.buffer, 0, _receiveHelper.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMsgCallback), _receiveHelper);
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
						ClientLog.Instance.LogError("Socket EndReceive failed, the size is 0. The remote socket is closed. Disconnect...");
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
						ClientLog.Instance.LogError("receive msg failed : " + se.ToString());
						ClientLog.Instance.LogError("Socket EndReceive Exception, ErrorCode = " + se.ErrorCode.ToString() + ", SocketErrorCode = " + se.SocketErrorCode.ToString());
						ClientLog.Instance.LogError("Socket fatal exception, disconnect...");
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
			if( !IsConnected() )
			{
				ClientLog.Instance.LogError("ReceiveMsgCallback : the socket is not connected!!!");
				return;
			}
			
            MsgReceiveHelper _receiveHelper = null;
            try
            {
                int _recSize = this.socketClient.EndReceive(receiveRes);
				if( _recSize > 0 )
				{
                	_receiveHelper = (MsgReceiveHelper)receiveRes.AsyncState;
                	this.DecodeMsg(_receiveHelper.buffer, _recSize, null);
				}
				// < 0, the remote socket is close...
				else
				{			
					ClientLog.Instance.LogError("Socket EndReceive failed, the size is 0. The remote socket is closed. Disconnect...");
					this.Close();
					return;
				}
            }
			//
			catch( SocketException se )
			{			
				ClientLog.Instance.LogError("receive msg failed : " + se.ToString());
				ClientLog.Instance.LogError("Socket EndReceive Exception, ErrorCode = " + se.ErrorCode.ToString() + ", SocketErrorCode = " + se.SocketErrorCode.ToString());
				
				// Disconnect, WSAEWOULDBLOCK
				if( !se.SocketErrorCode.Equals( SocketError.WouldBlock ) )
				{					
					ClientLog.Instance.LogError("Socket fatal exception, disconnect...");
					this.Close();
					return;
				}
			}
            catch (Exception e)
            {			
                ClientLog.Instance.LogError("receive msg failed : " + e.ToString());
            }
			//
            finally
            {
                if (_receiveHelper != null)
                {
                    _receiveHelper.socket.BeginReceive(_receiveHelper.buffer, 0, _receiveHelper.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMsgCallback), _receiveHelper);
                }
                else
                {
                    this.StartRecevieMsg();
                }
            }
        }

        private void SendMsgCallback(IAsyncResult sendRes)
        {
            try
            {
                Socket _socket = (Socket)sendRes.AsyncState;
                int nSentByte = _socket.EndSend(sendRes);

                if (nSentByte <= 0)
                {			
                    ClientLog.Instance.LogError("send msg failed!");
                }
            }
            catch (Exception e)
            {
                ClientLog.Instance.LogError("send msg failed : " + e.ToString());
				//
				SocketException	se = e as SocketException;
				if( null != se )
				{				
					ClientLog.Instance.LogError("Socket EndSend Exception, ErrorCode = " + se.ErrorCode.ToString() + ", SocketErrorCode = " + se.SocketErrorCode.ToString());
				}
            }
        }

        private byte[] encrytData(byte[] sendData)
        {
            return sendData;
        }

        private byte[] decrytData(byte[] receiveData)
        {
            return receiveData;
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
			
			//
			ClientLog.Instance.LogError("DecodeMsg Buf size : " + recMsgBuf.Remaining() );

            ThriftSCMessage _msg = null;
            while (recMsgBuf.Remaining() >= BaseMessage.MIN_MSG_LEN)
            {
                long _curPosition = recMsgBuf.Position();
                byte[] _arry = new byte[BaseMessage.MSG_SIZE_LEN];
                recMsgBuf.Get(_arry);
                Array.Reverse(_arry);
                short _msgLen = BitConverter.ToInt16(_arry, 0);
                if (_msgLen <= 0)
                {
                    continue;
                }
                recMsgBuf.SetPosition(_curPosition);
				
                // msg not receive complete
                if (_msgLen > recMsgBuf.Remaining())
                {
                    break;
                }

                byte[] _msgData = new byte[_msgLen];
                recMsgBuf.Get(_msgData);

                try
                {
                    // TODO key msg exception handle
                    _msg = this.msgRecognizer.RecognizeMsg(_msgData);
                    if (_msg == null)
                    {
                        ClientLog.Instance.LogError("rec msg fail: " + _msgData);
                    }
                    else
                    {

                        _msg.Decode(_msgData);
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
                                msgQueue.Enqueue(_msg);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ClientLog.Instance.LogError(e.ToString());
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
            lock (this.recMsgs)
            {
                int iMsgCount = Math.Min(this.recMsgs.Count, MAX_MSG_PER_LOOP);
                for (int iLoop = 0; iLoop < iMsgCount; ++iLoop)
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
            if (msgCopy.Count > 0)
            {
                ThriftSCMessage Msg = msgCopy[0];
                msgCopy.RemoveAt(0);

                return Msg;
            }

            return null;
        }  
		
		public EClientConnectState ClientConnectState
		{
			get{ return ConnectState; }
			set
			{
				ConnectState = value;
			}
		}

	}


    public enum EClientConnectState
    {
        CONNECT_STATE_NONE,
        CONNECT_STATE_TIME_OUT,

        CONNECT_STATE_CAN_RECONNECT,

        CONNECT_STATE_TRY_CONNECT,
        CONNECT_STATE_CONNECTED,
        CONNECT_STATE_DO_TRY_CONNECT
    }

    /// <summary>
    /// server address wrapper
    /// </summary>
    struct IPaddressWrapper
    {
        public IPEndPoint ipPoint;
        // has try connect tag
        public bool isTried;
    };
    
    /// <summary>
    /// temp save object when msg received
    /// </summary>
    class MsgReceiveHelper
    {
        public Socket socket;
        public byte[] buffer;
    }

}

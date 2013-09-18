using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Text;

/// <summary>
/// Network library.
/// </summary>
namespace prj301.NetworkLib
{
	/// <summary>
	/// Connection manager
	/// </summary>
	public class ConnectionMgr
	{
		public ConnectionMgr ()
		{
			this.m_connState = ConnState.EDisconnected;

			this.m_serverAddr = null;
			this.m_serverPort = -1;
			this.m_tcpClient = new TcpClient ();

			this.m_sendMsgList = new Queue (10);
			this.m_recvMsgList = new Queue (20);
			this.m_waitAckMsgList = new ArrayList (20);

		}

		~ConnectionMgr ()
		{
		}
		#region Instance
		private static readonly ConnectionMgr _instance = new ConnectionMgr ();

		/// <summary>
		/// Get the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static ConnectionMgr getInstance ()
		{
			return _instance;
		}
		#endregion
		#region Attributes
		private ConnState m_connState;

		/// <summary>
		/// Gets the state of the connection.
		/// </summary>
		/// <value>The state of the connection.</value>
		public ConnState ConnectionState {     
			get {	
				return m_connState;   
			}
		}

		private IPAddress m_serverAddr;

		/// <summary>
		/// Gets the server address.
		/// </summary>
		/// <value>The server address.</value>
		public IPAddress ServerAddr { 
			get {
				return m_serverAddr;
			}
		}

		private int m_serverPort;

		/// <summary>
		/// Gets the server port.
		/// </summary>
		/// <value>The server port.</value>
		public int ServerPort { 
			get {
				return m_serverPort;
			}
		}

		private TcpClient m_tcpClient;

		public TcpClient Socket { 
			get {
				return m_tcpClient;
			}
			set {
				m_tcpClient = value;
			}
		}
		#endregion
		#region Variables
		private Queue m_sendMsgList;
		private Queue m_recvMsgList;
		private ArrayList m_waitAckMsgList;
		#endregion
		#region Methods
		/// <summary>
		/// Begin connect to server.
		/// </summary>
		/// <returns><c>true</c>, if connect was begun, <c>false</c> otherwise.</returns>
		/// <param name="serverAddr">Server address.</param>
		/// <param name="serverPort">Server port.</param>
		public bool BeginConnect (IPAddress serverAddr, int serverPort)
		{
			if (this.m_connState == ConnState.EConnected
				|| this.m_connState == ConnState.EConnecting)
				return false;

			this.m_serverAddr = serverAddr;
			this.m_serverPort = serverPort;

			IAsyncResult ar = m_tcpClient.BeginConnect (m_serverAddr, m_serverPort, null, null);
			System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
			//System.Threading.Thread.Sleep (TimeSpan.FromSeconds (2));
			try {
				if (!ar.AsyncWaitHandle.WaitOne (TimeSpan.FromSeconds (5), false)) {
					if (!this.m_tcpClient.Connected) {
						this.m_tcpClient.Close ();
						this.m_connState = ConnState.EConnectTimeOut;
					}
				} else {
					//this.m_tcpClient.Close ();
					this.m_connState = ConnState.EConnected;
				}
			} finally {
				wh.Close ();
			}

			return true;
		}

		/// <summary>
		/// Closes the connect.
		/// </summary>
		public void CloseConnect ()
		{
			if (this.m_connState == ConnState.EConnected) {
				this.m_tcpClient.Close ();
				this.m_connState = ConnState.EDisconnected;
			}

		}

		/// <summary>
		/// Update.
		/// </summary>
		/// <param name="dt">Dt.</param>
		public void Update (int dt)
		{
			// read
			if (m_tcpClient.GetStream ().DataAvailable) {
				RealRead ();
			}

			// write
			if (m_sendMsgList.Count > 0 && m_tcpClient.GetStream ().CanWrite) {
				RealSend ((Message)m_sendMsgList.Dequeue ());
			}
		}

		/// <summary>
		/// Add network message entity to send queue.
		/// </summary>
		/// <param name="msg">Message.</param>
		public void Send (Message msg)
		{
			m_sendMsgList.Enqueue (msg);
		}

		private void RealSend (Message msg)
		{
			if (msg == null)
				return;

			NetworkStream ns = m_tcpClient.GetStream ();//WARNING: different stream????
			if (!ns.CanWrite)
				return;
	
			m_waitAckMsgList.Add (msg);
			ns.BeginWrite (msg.GetOutBytes (), 0, (int)msg.GetOutSize (), new AsyncCallback (this.WriteCallBack), ns);

			return;
		}

		private void RealRead ()
		{
			MemoryStream ms = new MemoryStream (1024);
			int BufferSize = 1024;
			byte[] buffer = new byte[BufferSize];

			NetworkStream ns = m_tcpClient.GetStream ();
			IAsyncResult ar = ns.BeginRead(buffer, 0, BufferSize, new AsyncCallback(ReadCallBack), ns);
		}
		#endregion
		#region Callback
		private void WriteCallBack (IAsyncResult ar)
		{
			NetworkStream ns = (NetworkStream)ar.AsyncState;
			ns.EndWrite (ar);
			//ns.Close ();
		}

		private void ReadCallBack (IAsyncResult ar)
		{
			NetworkStream ns = (NetworkStream)ar.AsyncState;
			byte[] read = new byte[1024];
			String data = "";
			int recv;

			recv = ns.EndRead(ar);
			data = String.Concat(data, Encoding.ASCII.GetString(read, 0, recv));

			//接收到的消息长度可能大于缓冲区总大小，反复循环直到读完为止
			while (ns.DataAvailable)
			{
				ns.BeginRead(read, 0, read.Length, new AsyncCallback(ReadCallBack), ns);
			}
			//打印
			Console.WriteLine("您收到的信息是: " + data);
		}
		#endregion
	}
}


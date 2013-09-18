using System;
using System.IO;
using System.Text;

namespace prj301.NetworkLib
{
	public class Message
	{
		private static long s_msgID = -1;
		public static readonly object s_msgIDAddLock = new object ();

		public Message ()
		{
			lock (s_msgIDAddLock) {
				s_msgID += 1;
				m_msgID = s_msgID;
			}

			m_outStream = new MemoryStream (4);
			//byte[] msgID = System.BitConverter.GetBytes (System.Net.IPAddress.HostToNetworkOrder (m_msgID));
			//m_outStream.Write (msgID, 0, msgID.Length);

			m_inStream = new MemoryStream (4);
		}

		~Message ()
		{
		}

		private MemoryStream m_outStream;
		private MemoryStream m_inStream;
		private long m_msgID;

		public long MsgID { 
			get {
				return m_msgID;
			} 
		}

		public byte[] GetOutBytes ()
		{
			return m_outStream.ToArray ();
		}

		public long GetOutSize ()
		{
			return m_outStream.Length;
		}

		public byte[] GetInBytes ()
		{
			return m_inStream.ToArray ();
		}

		public long GetInSize ()
		{
			return m_inStream.Length;
		}


		public void WriteString (string str)
		{

			byte[] ary = Encoding.Default.GetBytes (str);
			byte[] len = System.BitConverter.GetBytes (System.Net.IPAddress.HostToNetworkOrder ((UInt16)ary.Length));
			byte[] realLen = new byte[2];
			Array.Copy (len, 2, realLen, 0, 2);

			//realLen [0] = len [2];
			//realLen [1] = len [3];

			m_outStream.Write (realLen, 0, realLen.Length);
			m_outStream.Write (ary, 0, ary.Length);
		}
	}
}


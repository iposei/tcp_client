using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using prj301.NetworkLib;

/// <summary>
/// Main class.
/// </summary>
namespace csharp_test
{
	/// <summary>
	/// Main class.
	/// </summary>
	class MainClass
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			ConnectionMgr connMgr = ConnectionMgr.getInstance ();

			IPAddress ip = IPAddress.Parse ("127.0.0.1");
			int port = 8810;
			connMgr.BeginConnect (ip, port);

			Message msg = new Message ();
			msg.WriteString ("hello world!!!!");
			connMgr.Send (msg);

			Message msg1 = new Message ();
			msg1.WriteString ("显示的是以下查询字词的结果");
			connMgr.Send (msg1);



			/*
			int k = 1024;
			Byte[] buffer = new byte[k];
			buffer [0] = 0;
			MemoryStream ms = new MemoryStream (buffer);

			ms.WriteByte (buffer [0]);
			ms.WriteByte (buffer [1]);

			UInt32 v = 12;

			ms.WriteByte((byte)((v >> 24) & 0xFF));
			ms.WriteByte((byte)((v >> 16) & 0xFF));
			             ms.WriteByte((byte)((v >> 8) & 0xFF));
			             ms.WriteByte((byte)((v ) & 0xFF));



			TcpClient client = new TcpClient ("127.0.0.1", 8819);
			//client.be

			Byte[] data = System.Text.Encoding.ASCII.GetBytes("hello world"); 
			Byte[] header = new Byte[2]{0, 11};

			Byte[] body = new Byte[13];
			header.CopyTo (body, 0);
			data.CopyTo (body, 2);

			NetworkStream stream = client.GetStream();
			stream.Write(body, 0, body.Length);
*/
			 while (true) 
			{
				System.Threading.Thread.Sleep (TimeSpan.FromSeconds (1));
				connMgr.Update (0);
			}

			//client.Close ();
		}
	}
}

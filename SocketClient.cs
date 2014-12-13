using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace iBot
{
	/// <summary>
	/// Description of SocketClient.
	/// </summary>
	public class SocketClient
	{
		private string IP;
		private int port;
		private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		public SocketClient(string IP, int port = 1204)
		{
			this.IP = IP;
			this.port = port;
			try
			{
				sock.Connect(this.IP, this.port);
			} catch(SocketException)
			{
				Console.WriteLine("A socket exception has occured - iBot will now exit [5 seconds]");
				Thread.Sleep(5000);
			}
		}
		
		public void Send(string data)
		{
			try {
				sock.Send(this.EncodeString(data + "\0"));
				Console.WriteLine("SENT: " + data);
			} catch(SocketException)
			{
				Console.WriteLine("A socket exception has occured - iBot will now exit [5 seconds]");
				Thread.Sleep(5000);
			}
		}
		
		public string GetRecv()
		{
			try
			{
			byte[] buff = new byte[2048];
			sock.Receive(buff);
			string ret = DecodeBytes(buff);
			if(ret.Replace("\0", "") != string.Empty) { Console.WriteLine("RECV: " + ret.Replace("\0", "")); }
			return ret.Replace("\0", "");
			} catch(ObjectDisposedException) {
				return null;
			} catch(SocketException) {
				Console.WriteLine("A socket exception has occured - iBot will now exit [5 seconds]");
				Thread.Sleep(5000);
				Environment.Exit(0);
				return null; // Wouldn't even reach this part
			}
		}
		
		private string DecodeBytes(byte[] data)
		{
			return Encoding.UTF8.GetString(data);
		}	

		private byte[] EncodeString(string data)
		{
			return Encoding.UTF8.GetBytes(data);
		}
		
		public void close()
		{
			this.sock.Close();
		}
	}
}

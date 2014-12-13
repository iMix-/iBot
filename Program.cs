using System;
using System.Net.Sockets;
using System.Text;
namespace iBot
{
	class Program
	{
		private static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		public static void Main(string[] args)
		{
			Console.Title = "iBot - iMix's iXat C# bot";
			Console.Write("IP: ");
			string IP = Console.ReadLine();
			Console.Write("Port: ");
			int port = Int32.Parse(Console.ReadLine());
			Console.Write("Chat ID: ");
			string chatID = Console.ReadLine();
			Console.Write("auser3 [Y/N]: ");
			string auser3 = Console.ReadLine();
			bool auser = false;
			string uid = "0";
			string k1 = "0";
			string j1;
			string j2;
			Console.Write("Bot protection name #1 [j1 is default]: ");
			j1 = Console.ReadLine();
			Console.Write("Bot protection name #2 [j2 is default]: ");
			j2 = Console.ReadLine();
			
			if(auser3.Equals("Y".ToLower()))
			{
				auser = true;
			}
			else
			{
				Console.Write("User ID: ");
				uid = Console.ReadLine();
				Console.Write("k1: ");
				k1 = Console.ReadLine();
			}
			Console.Write("What's YOUR ID [Owner-of-bot]: ");
			string ownID = Console.ReadLine();
			BotClient bc = new BotClient(new SocketClient(IP, port), chatID, uid, IP, port, auser, j1, j2, k1, ownID);
			Console.ReadLine();
		}
		
	
		private static string decodeBytes(byte[] data)
		{
			return Encoding.UTF8.GetString(data);
		}
		private static byte[] encodeBytes(string data)
		{
			return Encoding.UTF8.GetBytes(data);
		}
	}
}
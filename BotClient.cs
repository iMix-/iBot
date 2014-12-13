using System;
using System.Text;
using System.Net;
using System.IO;

namespace iBot
{
	/// <summary>
	/// Description of BotClient.
	/// </summary>
	public class BotClient
	{
		public SocketClient client;
		private string chatID, id, name, avatar, website;
		public string IP;
		public int port;
		private string j1;
		private string ownID;
		private string j2;
		private bool auser3;
		private string k1 = "0";
		CommandManager cm;
		string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\iBot";
		public BotClient(SocketClient client, string chatID, string id, string IP, int port, bool auser3, string j1, string j2, string k1, string ownID)
		{
			this.auser3 = auser3;
			string WMSG_E = "";
			if(!Directory.Exists(dir))
			   {
			   		this.name = "iMix.cs";
			   		this.avatar = "101";
			   		this.ownID = ownID;
			   		this.website = "http://elixir.rocks";
			   		WMSG_E = "Hey [USR]! I am [BN] at your will. Credits for this bot goes to iMix, if you need any help, contact iMix on Skype: mexi1212 or Rile5.";
			   		Directory.CreateDirectory(dir);
			   		FileStream fs = new FileStream(dir + @"\settings.conf", FileMode.OpenOrCreate, FileAccess.Write);
			   		fs.Write(Encoding.ASCII.GetBytes("name=iMix.cs\navatar=101\nhomepage=http://elixir.rocks\nwmsg=Hey [USR]! I am [BN] at your will. Credits for this bot goes to iMix, if you need any help, contact iMix on Skype: mexi1212 or Rile5."), 0, Encoding.ASCII.GetBytes("name=iMix.cs\navatar=101\nhomepage=http://elixir.rocks\nwmsg=Hey [USR]! I am [BN] at your will. Credits for this bot goes to iMix, if you need any help, contact iMix on Skype: mexi1212 or Rile5.").Length);
			   		fs.Close();
			} else {
				string[] lines = File.ReadAllLines(dir + @"\settings.conf");
				this.name = lines[0].Split('=')[1];
				this.avatar = lines[1].Split('=')[1];
				this.website = lines[2].Split('=')[1];
				WMSG_E = lines[3].Split('=')[1];
			}

			this.client = client;
			this.chatID = chatID;
			this.IP = IP;
			this.port = port;
			this.k1 = k1;
			this.j1 = j1;
			this.j2 = j2;
			this.id = id;
			authUser();
			cm = new CommandManager(client, Int32.Parse(id), this, ownID);
			cm.welcomeMsg = WMSG_E;
		}
		
	
		public void SetWebsite(string website)
		{
			FileStream fs = new FileStream(dir + @"\settings.conf", FileMode.OpenOrCreate, FileAccess.Write);
			byte[] bff = Encoding.ASCII.GetBytes("name=" + this.name + "\navatar=" + this.avatar + "\nhomepage=" + website + "\nwmsg=" + this.cm.welcomeMsg);
			fs.Write(bff, 0, bff.Length);
			fs.Close(); 
			this.website = website;
		}	
		public void SetAvatar(string avatar)
		{
			FileStream fs = new FileStream(dir + @"\settings.conf", FileMode.OpenOrCreate, FileAccess.Write);
			byte[] bff = Encoding.ASCII.GetBytes("name=" + this.name + "\navatar=" + avatar + "\nhomepage=" + this.website + "\nwmsg=" + this.cm.welcomeMsg);
			fs.Write(bff, 0, bff.Length);
			fs.Close();
			this.avatar = avatar;
		}
		public void SetWelcomeMessage(string newMessage)
		{
			FileStream fs = new FileStream(dir + @"\settings.conf", FileMode.OpenOrCreate, FileAccess.Write);
			Console.WriteLine(newMessage);
			fs.SetLength(0);
			fs.Flush();
			byte[] bff = Encoding.ASCII.GetBytes("name=" + this.name + "\navatar=" + this.avatar + "\nhomepage=" + this.website + "\nwmsg=" + newMessage);
			fs.Write(bff, 0, bff.Length);
			fs.Close();
			this.cm.welcomeMsg = newMessage;
		}
		public void ResetConfig()
		{
			string[] paths = Directory.GetFiles(dir);
			foreach(string path in paths)
			{
				File.Delete(path);
			}
			paths = null;
			paths = Directory.GetDirectories(dir);
			foreach(string path in paths)
			{
				Directory.Delete(path);
			}
			Directory.Delete(dir);
			this.refresh();
		}
		public void SetName(string newName)
		{
			FileStream fs = new FileStream(dir + @"\settings.conf", FileMode.OpenOrCreate, FileAccess.Write);
			byte[] bff = Encoding.ASCII.GetBytes("name=" + newName + "\navatar=" + this.avatar + "\nhomepage=" + this.website + "\nwmsg=" + this.cm.welcomeMsg);
			fs.Write(bff, 0, bff.Length);
			fs.Close();
			this.name = newName;
		}
		
		public string getName()
		{
			return this.name;
		}
		
		// ID: 1041 k1: 670944
		
		public BotClient refresh()
		{
			BotClient bc = new BotClient(new SocketClient(this.IP, this.port), this.chatID, this.id, this.IP, this.port, this.auser3, this.j1, this.j2, this.k1, this.ownID);
			return bc;
		}
		
		public bool authUser()
		{
			if(!auser3)
			{
			client.Send("<y r=\"" + this.chatID + "\" v=\"0\" u=\"" + id + "\" />");
			} else
			{
				client.Send("<y r=\"" + this.chatID + "\" v=\"0\" u=\"1\" />");
			}
			string returned = client.GetRecv();
			returned = returned.Replace(@"\", "");
			string yi = readAttrValue(returned, 1);
			string yc = readAttrValue(returned, 2);
			string ys = readAttrValue(returned, 3);
			Console.WriteLine("READ YI: " + yi);
			Console.WriteLine("READ YC: " + yc);
			Console.WriteLine("READ YS: " + ys);
			
			string ym1 = ((2 << (Int32.Parse(yi) % 30)) % Int32.Parse(yc) + Int32.Parse(yi)).ToString();
			string ym2 = Math.Pow(2, Int32.Parse(ys) % 32).ToString();
			
			
			if(!auser3)
			{
				client.Send("<j2 cb=\"0\" Y=\"2\" ym1=\"" + ym1 + "\" ym2=\"" + ym2 + "\" ym3=\"" + ym2 + "\" q=\"1\" y=\"" + yi + "\" k=\"" + this.k1 + "\" p=\"0\" c=\"" + this.chatID + "\" u=\"" + this.id + "\" d0=\"0\" n=\"" + this.name + "\" a=\"" + this.avatar + "\" h=\"" + this.website + "\" v=\"0\" />");
			} else {
				string[] aus = readAuser3();
				client.Send("<j2 cb=\"0\" Y=\"2\" ym1=\"" + ym1 + "\" ym2=\"" + ym2 + "\" ym3=\"" + ym2 + "\" q=\"1\" y=\"" + yi + "\" k=\"" + aus[1] + "\" p=\"0\" c=\"" + this.chatID + "\" u=\"" + aus[0] + "\" d0=\"0\" n=\"" + this.name + "\" a=\"" + this.avatar + "\" h=\"" + this.website + "\" v=\"0\" />");
			}
			//<c u="'+UserID+'" t="/r" />
			return true;
		}
		// -622686
		private string[] readAuser3()
		{
			WebClient wc = new WebClient();
			wc.Headers.Add(HttpRequestHeader.Referer, "http://fluffycoder.net/");
			// Should use proxies to read data from auser3
			string auser = wc.DownloadString("http://" + this.IP + "/web_gear/chat/auser3.php");
			string[] elements = auser.Split('&');
			string id = elements[1].Split('=')[1];
			string k1 = elements[2].Split('=')[1];
			string k2 = elements[3].Split('=')[1];
			return new string[] { id, k1, k2, auser };
		}
		
		private string readAttrValue(string xml, int indReq)
		{
			string[] rd = xml.Split('=');
			string[] val = rd[indReq].Split('"');
			return val[1];
		}
	}
}

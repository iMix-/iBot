using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace iBot
{
	/// <summary>
	/// Description of CommandManager.
	/// </summary>
	public class CommandManager
	{
		private SocketClient sock;
		string recv;
		int uID;
		private int minrank = 5;
		private BotClient bc;
		private Thread runningThread;
		private string ownID;
		public string welcomeMsg;
		List<string> allowed = new List<string>();
	
		public CommandManager(SocketClient sock, int uID, BotClient bc, string ownID)
		{
			this.sock = sock;
			this.uID = uID;
			this.ownID = ownID;
			this.bc = bc;
			doThread();
		}
		private bool _stopThread = false;
		private bool _isSpamming = false;
		private bool _isSpammingPM = false;
		private void doThread()
		{
			try
			{
				runningThread = new Thread( new ThreadStart(
				delegate
				{
					var commands = new Dictionary<string, Action>
					{
						{ "!say", () => SendMessage() },
						{ "!changename", () => this.SetName() },
						{ "!setavatar", () => this.SetAvatar() },
						{ "!setwebsite", () => this.SetWebsite() },
						{ "!choose", () => choose() },
						{ "!spam", () => spam() },
						{ "!stopspam", () => StopSpam()},
						{ "!pm", () => PM() },
						{ "!pmspam", () => pmspam() },
						{ "!stoppmspam", () => stoppmspam() },
						{ "!8ball", () => ball8() },
						{ "!insult", () => insult() },
						{ "!kick", () => kick() },
						{ "!lovetest", () => CheckLoveTest() },
						{ "!changeWMSG", () => ChangeWMSG() },
						{ "!fartsscmd", () => Fart() },
						{ "!bealertof", () => BeAlertOf() },
						{ "-sniffmode", () => SMO() },
						{ "-giveaccess", () => GAC() },
						{ "-removeaccess", () => RAC() }
					};
					allowed.Add(this.ownID);
					while((recv = sock.GetRecv()) != null && _stopThread == false)
					{
						if(recv.IndexOf("<m t=\"!") != -1 || recv.IndexOf("<p u=\"") != -1 || recv.IndexOf("<m t=\"-") != -1 || recv.IndexOf("<p u=\"") != -1)
						{
							if(readAttrValue(recv, "t").IndexOf("!") == -1 && readAttrValue(recv, "t").IndexOf("-") == -1)
							{
								return;
							}
							string user = readAttrValue(recv, "u");
							Console.WriteLine(" ******* " + user + " ****************");
							
							if(allowed.IndexOf(user) != -1)
							{
								Console.WriteLine("****** CONTAINS *****");
								string command = readAttrValue(recv, "t");
								
								if(commands.ContainsKey(command.Split(' ')[0]))
								{
									commands[command.Split(' ')[0]]();
								}
							} else {
								PM(user, "You are not on the access-list, please request someone from the access-list to allow you through!");
							}
						} else if(recv.StartsWith("<u "))
						{
							try
							{
								string username = readAttrValue(recv, "n");
								string ID = readAttrValue(recv, "u");
								this.PM(ID, welcomeMsg.Replace("[USR]", username).Replace("[BN]", this.bc.getName()));
								string rank = readAttrValue(recv, "rank");
								if(Int32.Parse(rank) > 4)
								{
									this.sock.Send("<c u=\"" + ID + "\" t=\"/e\" />");
								}
							} catch(NullReferenceException)
								{
									Console.WriteLine("Unable to send Welcome message!");
								}

						} else if(recv.StartsWith("<z "))
						{
							PM(readAttrValue(recv, "u"), "Whatcha want, " + readAttrValue(recv, "n") + "?");
						}
						while(_stopThread)
						{
							this.sock.GetRecv();
						}
					}
				}));
			runningThread.Start();
			}
			catch(ObjectDisposedException)
			{
			} catch(Exception e)
			{
				Environment.Exit(0);
			}
			
		}
		
		private void Sing()
		{
			try
			{
				string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + getMessage() + ".txt");
				foreach(string line in lines)
				{
					SendMessage(line);
					Thread.Sleep(500);
				}
			} catch(IOException)
			{
				SendMessage("Unable to locate song file - exiting loop");
			}
		}
		
		private void kick()
		{
			if(getMessage() == "1041") { return; }
			this.sock.Send("<c p=\"Owner request\" u=\"" + getMessage() + "\" t=\"/k\" />");
		}
		
		private void stoppmspam()
		{
			_isSpammingPM = false;
		}
		
		private void GAC()
		{
			string id = getMessage();
			this.allowed.Add(id);
			SendMessage(id + " has been added to the access list!");
		}
		
		private void RAC()
		{
			string id = getMessage();
			this.allowed.Remove(id);
			SendMessage(id + " has been removed from the access list!");
		}
		
		private void Fart()
		{
			string person = getMessage();
			if(person != string.Empty)
			{
				SendMessage(person + " farted.");
			} else {
				SendMessage("Somebody farted.");
			}
		}
		
		private void SetAvatar()
		{
			this.bc.SetAvatar(getMessage());
			this.bc = this.bc.refresh();
		}	
		private void SetWebsite()
		{
			this.bc.SetWebsite(getMessage());
			this.bc = this.bc.refresh();
		}
		private void SetName()
		{
			this.bc.SetName(getMessage());
			this.bc = this.bc.refresh();
		}
		
		private void SMO()
		{
			_stopThread = true;
			SendMessage("Sniff-mode has been turned on, no commands will be available for use!");
		}
		
		private void BeAlertOf()
		{
			SendMessage("THE MESSAGE BELOW IS OF GREAT IMPORTANCE");
			Thread.Sleep(500);
			SendMessage("NO ONE CAN DISLIKE IT EASILY WITHOUT");
			Thread.Sleep(500);
			SendMessage("GETTING WHIPPED ON HIS WHOLE BODY");
			Thread.Sleep(500);
			SendMessage("SO BEWARE..");
			Thread.Sleep(500);
			SendMessage(getMessage().ToUpper());
		}
		
		private void StopSpam() {  _isSpamming = false; }
		private void spam()
		{
			_isSpamming = true;
			string raidMsg = getMessage();
			new Thread( new ThreadStart( delegate
			                            {
			                            	while(_isSpamming)
			                            	{
			                            		SendMessage(raidMsg);
			                            		Thread.Sleep(500);
			                            	}
			                            }
			                           )
			                          ).Start();
		}
		private void pmspam()
		{
			try
			{
			string PMSG = getMessage();
			string[] elements = PMSG.Split(':');
			_isSpammingPM = true;
			new Thread(new ThreadStart( delegate
			{
				while(_isSpammingPM)
				{
					this.sock.Send("<z d=\"" + elements[0] + "\" u=\"" + this.uID + "\" t=\"" + elements[1] + "\" />");
					Thread.Sleep(500);
				}
			                           
			})).Start();
			} catch(Exception)
			{
				
			}
		}
		
		private void ChangeWMSG()
		{
			this.bc.SetWelcomeMessage(getMessage());
			SendMessage("Done");
		}
		
		private void CheckLoveTest()
		{
			try
			{
			string msg = getMessage();
			string[] parts = msg.Split(new string[] { " and " }, StringSplitOptions.None);
			string person1 = parts[0];
			string person2 = parts[1];
			int lpr = new Random().Next(0, 100);
			StringBuilder sb = new StringBuilder("Lovetest results: " + person1 + " and " + person2 + " have a " + lpr + "% " + "love connection, you two ");
			string[] possibilities;
			if(msg.IndexOf("Omar") != -1)
			{
				SendMessage("Lovetest results: " + person1 + " and " + person2 + " have a 0% love connections, this is one hell of a hate");
				return;
			}
			if(lpr >= 75)
			{
				possibilities = new string[] { "seem perfect!", "are like flying angels.", "really ADORE eachother!" };
			} else if(lpr >= 50)
			{
				possibilities = new string[] { "are halfway through", "like eachother", "MIGHT have a connection in the near future" };
			} else if(lpr >= 25)
			{
				possibilities = new string[] { "do not seem a good match", "doesn't seem likely to me that they'd have a good connection", "wouldn't really do favors for eachother", "do not hate eachother, but neither do you like eachother" };
			} else {
				possibilities = new string[] { "are flying enemies in the empty air!", "hate eachother that you'd bring a trumpet down eachothers' necks", "would love to choke eachother out", "are looking for your swiss knives", "are getting ready for the battle versus eachother" };
			}

			sb.Append(possibilities[new Random().Next(0, possibilities.Length - 1)]);
			this.SendMessage(sb.ToString());
			} catch(Exception)
			{
				SendMessage("Not even your toilet's smell can crash me.");
			}
		}
		
		private void PM(string target, string text)
		{
			this.sock.Send("<z d=\"" + target + "\" u=\"" + this.uID + "\" t=\"" + text + "\" />");
		}
		private void PM()
		{
			string PMSG = getMessage();
			string[] elements = PMSG.Split(':');
			this.sock.Send("<z d=\"" + elements[0] + "\" u=\"" + this.uID + "\" t=\"" + elements[1] + "\" />");
		}
		private void choose()
		{
			string message = this.getMessage();
			string[] options = message.Split(new string[] {" or "}, StringSplitOptions.None);
			SendMessage("I have chosen " + options[new Random().Next(0, (options.Length - 1))]);
		}
		private void ball8()
		{
			String[] arrResponse = new string[] { "That's for sure!", "Certainly", "That's certain!", "Ofcourse", "Is that even a question? YES!", "yes", "Yes!", "haha no", "never", "NO", "ofcourse not!", "Are you kidding me?", "Lol'd.. No.", "How about no?", "Oh please, we all know the answer to that is a certain no." };
			
		
			string chosen = arrResponse[new Random().Next(0, arrResponse.Length - 1)];
			SendMessage(chosen);
		}
		
		private void insult()
		{
			string user = this.getMessage();
			if(user == "iMix")
			{ user = "(INSERT WHOEVER COMMANDED ME TO DO SO HERE)"; }
			String[] arrResponse = new string[] { "What's that stinky smell? Oh sorry! I totally forgot " + user + " was here!", user + " is so fat when he jumps in a yellow t-shirt people think the sun is falling.", "Dang " + user + ", don't you have your own bathroom at your house?", "Scientists think " + user + "'s body size may exceed the sun's size.", "Scientists figured out lately that global warming has no effect on the ozone layer, but in fact, " + user + "'s breath is the core of the problem", "99% of car accidents do NOT happen when people are drunk, they happen when they see " + user + "'s face.", user + " is so fat that some people thought he beged Mehgan Trainor to sing all about that bass" };
			string chosen = arrResponse[new Random().Next(0, arrResponse.Length - 1)];
			SendMessage(chosen);
		}
		private string readAttrValue(string xml, string attrWanted)
		{
			string[] rd = xml.Split(new string[] { attrWanted + "=" }, StringSplitOptions.None);
			string value = rd[1].Split('"')[1].Split('"')[0];
			return value;
		}
		
		private string getMessage()
		{
			string xmlAttr = readAttrValue(recv, "t");
			string[] msgElements = xmlAttr.Split(' ');
			msgElements[0] = string.Empty;
			string msg = String.Join(" ", msgElements);
			if(msg.Substring(0, 1) == " ")
			{
				msg = msg.Substring(1);
			}
			return msg;
		}
		
		private void SendMessage()
		{
			string msgToSend = getMessage();
			this.SendMessage(msgToSend);
		}
		
		private void SendMessage(string msg)
		{
			string[] censor = new string[] { "fuck", "pussy", "dick", "bitch", "nigger", "porn", "p0rn", "pron", "pr0n", "shit", "nigga", "autist", "autism", "aut1sm", "pen1s", "pen15" };
			foreach(string word in censor)
			{
				if(msg.IndexOf(word) != -1)
				{
					this.sock.Send("<m t=\"I am not allowed to say that\" u=\""+this.uID+"\" />");
					return;
				}
			}
			this.sock.Send("<m t=\""+msg+"\" u=\""+this.uID+"\" />");
		}
	}
}

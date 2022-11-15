#define DEBUG

using System;

namespace SA2VsChatNET
{
	public abstract class TwitchClient
	{
		public string ip;
		public string username;
		public string channel;

		public int port;

		public bool connected = false;
		public bool printIRC = true;

		public void ProcessMessage(string message)
		{
			string lmessage = message.ToLower();
			if (message != null && message.Length > 1)
			{
#if DEBUG
				if (printIRC) Console.WriteLine(" > " + message);
#endif

				if (lmessage.Contains("ping :tmi.twitch.tv"))
				{
					SendIRCMessage("PONG :tmi.twitch.tv");
#if !DEBUG
                        Console.WriteLine("Ping");
#else
					if (printIRC) Console.WriteLine(" < PONG :tmi.twitch.tv");
#endif
				}
				if (message.ToLower().Contains("tmi.twitch.tv privmsg #"))
				{
					string name = "Unknown";
					try
					{
						name = message.Split(':')[1].Split('!')[0];
					}
					catch { }
					SA2VsChat.ProcessMessage(message.Split(new[] { ("#" + SA2VsChat.Room + " :") }, StringSplitOptions.None)[1], name);
				}
				if (message.ToLower().Contains("tmi.twitch.tv 366"))
				{
					// Comment the line below if you want to see what we're sending and receiving
					//irc.printIRC = false;
				}

			}
		}

		public void JoinChannel(string channel)
		{
			this.channel = channel;
			Console.WriteLine("Joining #" + channel);
			SendIRCMessage("JOIN #" + channel);
		}

		public void LeaveChannel(string channel)
		{
			this.channel = channel;
			Console.WriteLine("Leaving #" + channel);
			SendIRCMessage("PART #" + channel);
		}

		public void Disconnect()
		{
			Console.WriteLine("Disconnecting..");
			SendIRCMessage("QUIT");
			connected = false;
		}

		public void SendChatMessage(string message)
		{
			SendIRCMessage($":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{channel} :{message}");
		}

		public abstract void SendIRCMessage(string message);
	}
}

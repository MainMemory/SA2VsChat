using System;

namespace SA2VsChatNET
{
	public class TwitchWebSocketClient : TwitchClient
	{

		public bool errored = false;

		public WebSocket webClient;

		public TwitchWebSocketClient(string ip, int port, string username, string OAuth)
		{
			this.ip = ip;
			this.port = port;
			this.username = username;
			Console.WriteLine($"Connecting to {ip}:{port}... ");
			try
			{
				webClient = new WebSocket("wss://" + ip + ":" + port + "/");
				webClient.OnMessage += ReadMessage;
				webClient.Connect();
				// wait for connection
				while (!webClient.connected) ;
				if (OAuth.Length > 0) 
					webClient.Send("PASS oauth:" + OAuth);
				webClient.Send("NICK " + username);
				connected = true;
				Console.WriteLine("Connected");
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to connect.\n");
				Console.WriteLine("Exception: " + e);
				errored = true;
			}
		}

		public override void SendIRCMessage(string message)
		{
			webClient.Send(message);
		}

		public void ReadMessage(object sender, MessageEventArgs a)
		{
			if (a != EventArgs.Empty)
				ProcessMessage(a.Data);
		}
	}
}

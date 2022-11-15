using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace SA2VsChatNET
{
	public class MessageEventArgs: EventArgs
	{
		public MessageEventArgs(string text) { Data = text; }
		public string Data;
	}

	public class WebSocket
	{
		private Uri address;
		private ClientWebSocket webSocket;

		public bool connected = false;

		public event OnMessageHandler OnMessage;

		public WebSocket(string uri)
		{
			webSocket = new ClientWebSocket();
			address = new Uri(uri);
		}

		public async void Connect()
		{
			await webSocket.ConnectAsync(address, CancellationToken.None);
			Console.Write("Connecting Websocket");
			while (webSocket.State == WebSocketState.Connecting) Console.Write(".");
			Console.WriteLine();
			while (webSocket.State == WebSocketState.Open)
			{
				connected = true;
				try
				{
					ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[2048]);
					WebSocketReceiveResult result = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
					OnMessage?.Invoke(this, new MessageEventArgs(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count)));
				}
				catch (WebSocketException e)
				{
					Console.WriteLine("Error while receiving message:");
					Console.WriteLine(e.ToString());
					throw new Exception("Twitch WebSocket encountered an error!", e);
				}
			}
		}

		public void Send(string message)
		{
			if (webSocket.State == WebSocketState.Open)
			{
				ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
				webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		public delegate void OnMessageHandler(object sender, MessageEventArgs e);
	}
}

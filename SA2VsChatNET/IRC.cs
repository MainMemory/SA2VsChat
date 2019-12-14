using System;
using System.IO;
using System.Net.Sockets;

namespace SA2VsChatNET
{
    public class TwitchIRCClient
    {
        public string ip;
        public string username;
        public string channel;

        public int port;

        public bool connected;
        public bool printIRC = true;

        public TcpClient tcpClient;
        public StreamReader inputStream;
        public StreamWriter outputStream;

        public TwitchIRCClient(string ip, int port, string username, string OAuth)
        {
            this.ip = ip;
            this.port = port;
            this.username = username;
            Console.Write($"Connecting to {ip}:{port}... ");
            try
            {
                tcpClient = new TcpClient(ip, port);
                inputStream = new StreamReader(tcpClient.GetStream());
                outputStream = new StreamWriter(tcpClient.GetStream());
                if(OAuth.Length > 0) outputStream.WriteLine("PASS " + OAuth);
                outputStream.WriteLine("NICK " + username);
                connected = true;
                Console.WriteLine("Connected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect.\n");
                Console.WriteLine("Exception: "+e);
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
            ReadMessage();
            connected = false;
        }

        public void SendIRCMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void SendChatMessage(string message)
        {
            outputStream.WriteLine($":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{channel} :{message}");
            outputStream.Flush();
        }

        public string ReadMessage()
        {
            string message = inputStream.ReadLine();
            if (message != null && message.Length > 1)
            {
                #if DEBUG
                    if(printIRC) Console.WriteLine(" > " + message);
                #endif

                if (message.ToLower().Contains("ping :tmi.twitch.tv"))
                {
                    SendIRCMessage("PONG :tmi.twitch.tv");
                    #if !DEBUG
                        Console.WriteLine("Ping");
                    #else
                        if(printIRC) Console.WriteLine(" < PONG :tmi.twitch.tv");
                    #endif
                }
                        
            }
            return message;
        }
 }   
}
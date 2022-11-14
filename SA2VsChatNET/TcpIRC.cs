using System;
using System.IO;
using System.Net.Sockets;

namespace SA2VsChatNET
{
    public class TwitchTcpClient : TwitchClient
    {

        public TcpClient tcpClient;
        public StreamReader inputStream;
        public StreamWriter outputStream;

        public TwitchTcpClient(string ip, int port, string username, string OAuth)
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

        public override void SendIRCMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public string ReadMessage()
        {
            return inputStream.ReadLine();
        }
 }   
}
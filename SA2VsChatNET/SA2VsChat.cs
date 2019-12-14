using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace SA2VsChatNET
{
    public static class SA2VsChat
    {
        public static Random random = new Random();
        public static int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }
        // Generate a random string with a given size  
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        // Generate a random 14 characters    
        public static string Random14Char()
        {
            string s = "";
            for (int i = 0; i < 14; ++i)
                s += (char)(0x30 + random.Next(10));
            return s;
        }
        public static int Timer = 0;

        public static string ServerIP = "irc.twitch.tv";
        public static string Room = "";
        public static string User = $"justinfan{Random14Char()}";
        public static string Key = "";
        public static int Port = 6667;

        public static TwitchIRCClient TwitchClient = new TwitchIRCClient(ServerIP, Port, User, Key);
        
        public static void ProcessTwitchClient()
        {
            TwitchClient.JoinChannel(Room);
            while (TwitchClient.connected)
            {
                string message = TwitchClient.ReadMessage();
                if (message != null && message.Length > 1)
                {
                    if (message.ToLower().Contains("tmi.twitch.tv privmsg #"))
                    {
                        string name = "Unknown";
                        try
                        {
                            name = message.Split(':')[1].Split('!')[0];
                        }
                        catch { }
                        ProcessMessage(message.Split(new[] { ("#" + Room + " :") }, StringSplitOptions.None)[1], name);
                    }
                    else if (message.ToLower().Contains("tmi.twitch.tv 366"))
                    {
                        // Comment the line below if you want to see what we're sending and receiving
                        //irc.printIRC = false;
                    }
                }
                Thread.Sleep(50);
            }

        }

        public static void ProcessDiscord()
        {
            Discord.StartDiscord(MessageReceived);
        }

        private static Task MessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage userMessage))
                return Task.CompletedTask;

            if (userMessage.Author.IsBot)
                return Task.CompletedTask;


            ProcessMessage(userMessage.Content, userMessage.Author.Username + "#" + userMessage.Author.Discriminator);
            return Task.CompletedTask;
        }
        public static string RemoveBadChars(string word)
        {
            Regex reg = new Regex("[^a-zA-Z' ]");
            return reg.Replace(word, string.Empty);
        }

        public static List<string> ElevatedUsers = new List<string>();
    

        public static string AdminUsrName = "STEVEPOKERAINBOW";

        /*public static void DoBuildHTML()
        {
            StringBuilder strBldr = new StringBuilder();
            StringWriter strWriter = new StringWriter(strBldr);

            HtmlTextWriter writer = new HtmlTextWriter(strWriter);

            //<html>
            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            writer.Write("<meta http-equiv=\"Pragma\" content=\"no-cache\">\n<meta http-equiv=\"Expires\" content=\"-1\">\n<meta http-equiv=\"Refresh\" content=\"1\">");
            // <head>
            writer.RenderBeginTag(HtmlTextWriterTag.Head);
            writer.Write("<style>\n    body {\n      margin: 5px;\n      color: #FFFFFF;\n      text-shadow: 4px 1px #000000;\n      text-align: right;\n      font-size: 12pt;\n      font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif;\n      font-weight: bold;\n      font-style: normal;\n    }\n    \n    .squad {\n      padding: 10px;\n      background-color: rgba(0, 0, 0, 0.5);\n    }\n    \n    .pilot {\n      font-size: 1.1em;\n      margin-top: 5px;\n      font-weight: bold;\n    }\n    \n    .upgrades {\n      font-size: 0.9em;\n      margin-bottom: 5px;\n      font-weight: bold;\n    }\n    \n    .shields {\n      font-size: 0.95em;\n      color: cyan;\n    }\n    \n    .hull {\n      font-size: 0.95em;\n      color: yellow;\n    }\n    \n    .idtag {\n      background-image: url(images/Token-IDTag_Light.png);\n      background-size: 30px 30px;\n      background-repeat: no-repeat;\n      height: 30px;\n      width: 30px;\n      display: inline-block;\n      text-align: center;\n      text-shadow: 0px 0px #000000;\n      color: black;\n      padding-top: 5px;\n      vertical-align: middle;\n      font-size: 0.95em;\n    }\n  </style>");
            // </head>
            writer.RenderEndTag();
           
            // <body>
            writer.RenderBeginTag(HtmlTextWriterTag.Body);
            writer.Write("<h3><font color='White'>DISABLED COMMANDS </font><br> </h3>");
            writer.Write("<div class=\"pilot\">\n");
           
            if (!AllowSmall)
            {
                writer.WriteLine("<span>!Small<br></span>\n");
            }
           
            if (!AllowBig)
            {
                writer.WriteLine("<span>!Big<br> </span>\n");
            }
            if (!AllowShove)
            {
                writer.WriteLine("<span>!Shove<br> </span>\n");
            }
            if (!AllowRingLoss)
            {
                writer.WriteLine("<span>!RingLoss<br> </span>\n");
            }
            if (!AllowEggbox)
            {
                writer.WriteLine("<span>!Eggbox<br> </span>\n");
            }
            if (!AllowSpring)
            {
                writer.WriteLine("<span>!Spring<br> </span>\n");
            }
            if (!AllowPartner)
            {
                writer.WriteLine("<span>!!Partner<br> </span>\n");
            }
            if (!AllowRainbow)
            {
                writer.WriteLine("<span>!Rainbow<br> </span>\n");
            }
            if (!AllowRing)
            {
                writer.WriteLine("<span>!GiveRing<br> </span>\n");
            }
            if (!touchfluffy)
            {
                writer.WriteLine("<span>!TouchFluffy<br> </span>\n");
            }
            if (!AllowBigCat)
            {
                writer.WriteLine("<span>!ReallyBig<br> </span>\n");
            }
            if (!AllowEmoteCnG)
            {
                writer.WriteLine("<span>!CnG<br> </span>\n");
            }
            if (!AllowWelp)
            {
                writer.WriteLine("<span>!Welp<br> </span>\n");
            }
            if (!AllowEmote2)
            {
                writer.WriteLine("<span>!SHC<br> </span>\n");
            }
            if (!AllowEmote3)
            {
                writer.WriteLine("<span>----NOT IMPLEMENTED----<br> </span>\n");
            }
            if (!AllowColor)
            {
                writer.WriteLine("<span>!Color #112233<br> </span>\n");
            }
            if (!AllowMessage)
            {
                writer.WriteLine("<span>!Message<br> </span>\n");
            }
            if (!AllowGetDizzy)
            {
                writer.WriteLine("<span>!GetDizzy<br> </span>\n");
            }
            if (!AllowDemon)
            {
                writer.WriteLine("<span>!Demon<br> </span>\n");
            }

            writer.Write("</div>\n");

            // </body>
            writer.RenderEndTag();

            // </html>
            writer.RenderEndTag();
            try
            {
                System.IO.File.WriteAllText(@modPath + "/CommandStatus.html", strBldr.ToString());
            }
            catch (Exception e)
            {
               
            }
            
        }
        public static void DoBuildHTMLReady()
        {
            StringBuilder strBldr = new StringBuilder();
            StringWriter strWriter = new StringWriter(strBldr);

            HtmlTextWriter writer = new HtmlTextWriter(strWriter);

            //<html>
            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            writer.Write("<meta http-equiv=\"Pragma\" content=\"no-cache\">\n<meta http-equiv=\"Expires\" content=\"-1\">\n<meta http-equiv=\"Refresh\" content=\"1\">");
            // <head>
            writer.RenderBeginTag(HtmlTextWriterTag.Head);
            writer.Write("<style>\n    body {\n      margin: 5px;\n      color: #FFFFFF;\n      text-shadow: 4px 1px #000000;\n      text-align: left;\n      font-size: 12pt;\n      font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif;\n      font-weight: bold;\n      font-style: normal;\n    }\n    \n    .squad {\n      padding: 10px;\n      background-color: rgba(0, 0, 0, 0.5);\n    }\n    \n    .pilot {\n      font-size: 1.1em;\n      margin-top: 5px;\n      font-weight: bold;\n    }\n    \n    .upgrades {\n      font-size: 0.9em;\n      margin-bottom: 5px;\n      font-weight: bold;\n    }\n    \n    .shields {\n      font-size: 0.95em;\n      color: cyan;\n    }\n    \n    .hull {\n      font-size: 0.95em;\n      color: yellow;\n    }\n    \n    .idtag {\n      background-image: url(images/Token-IDTag_Light.png);\n      background-size: 30px 30px;\n      background-repeat: no-repeat;\n      height: 30px;\n      width: 30px;\n      display: inline-block;\n      text-align: center;\n      text-shadow: 0px 0px #000000;\n      color: black;\n      padding-top: 5px;\n      vertical-align: middle;\n      font-size: 0.95em;\n    }\n  </style>");
            // </head>
            writer.RenderEndTag();

            // <body>
            writer.RenderBeginTag(HtmlTextWriterTag.Body);
            writer.Write("<h3><font color='White'>Ready Commands:  </font></h3>");
            writer.Write("<div class=\"pilot\">\n");

            if (CheckBool(1))
            {
                if (AllowSmall)
                {
                    writer.WriteLine("<span>!Small </span>\n");
                }
                if (AllowBig)
                {
                    writer.WriteLine("<span>!Big </span>\n");
                }
                if (AllowBigCat)
                {
                    writer.WriteLine("<span>!ReallyBig </span>\n");
                }
               
            }
            if (CheckBool(2))
            {
                if (AllowRingLoss)
                {
                    writer.WriteLine("<span>!RingLoss </span>\n");
                }
            }
            if (CheckBool(3))
            {
                if (AllowDemon)
                {
                    writer.WriteLine("<span>!Demon </span>\n");
                }
            }
            if (CheckBool(4))
            {
                if (AllowMessage)
                {
                    writer.WriteLine("<span>!Message </span>\n");
                }
            }

            writer.Write("</div>\n");

            // </body>
            writer.RenderEndTag();

            // </html>
            writer.RenderEndTag();
            try
            {
                System.IO.File.WriteAllText(@modPath + "/ReadyCommands.html", strBldr.ToString());
            }
            catch
            {
            }
        }*/

        static Dictionary<string, int> itemDict = new Dictionary<string, int>()
        {
            { "speedshoes", 0 },
            { "5ring", 1 },
            { "1up", 2 },
            { "10ring", 3 },
            { "20ring", 4 },
            { "shield", 5 },
            { "bomb", 6 },
            { "health", 7 },
            { "magnet", 8 },
            { "invincibility", 10 }
        };

        public static void ProcessMessage(string message, string channelName)
		{
            channelName = channelName.ToLowerInvariant();
            // A shitty way of doing this. (Using Relection would look nicer but takes alot more work)
            message = message.ToLowerInvariant();
           // Console.WriteLine("From: {0} - Message:{1} ", channelName, msg);
            if (channelName == "streamlabs")
            {

                message = message.Substring(2);
                Console.WriteLine("Message:{0}", message);
            }

            if (!message.StartsWith("!"))
                return;

            string[] split = message.Substring(1).Split(' ');

            if (channelName == Room || channelName == AdminUsrName)
            {
                // admin stuff
            }

            switch (split[0])
            {
                case "item":
                    if (split.Length > 1 && itemDict.TryGetValue(split[1], out int v))
                        GiveItem(v);
                    else
                    {
                        int i = RandomNumber(0, 9);
                        if (i == 9)
                            ++i;
                        GiveItem(i);
                    }
                    break;
            }

            /*if (AllowBuildHTMLPagesForOverlay)
            {
                DoBuildHTML();
                DoBuildHTMLReady();
            }*/

        }
        public static bool EnableDiscordSupport = false;
        public static bool EnableTwitchSupport = false;
        public static bool EnableYoutubeSupport = false;
        public static bool AllowVideoIDForm = true;
        public static bool AllowBuildHTMLPagesForOverlay = true;
        public static string YoutubeVideoIDFromSettings = "";
        public static string YoutubeAPIKey = "";

        [DllExport(CallingConvention.Cdecl)]
        public static bool TwitchIRCStart()
        {
            LoadINIFile();
            //TwitchIRCThread Tirc = new Thread(PulseTwitchIRC);
            //Tirc.Start();
            
            if(EnableTwitchSupport)
            {
                Console.Write("------------------------------------------ \n");
                Console.Write("Starting Twitch IRC Client. \n");
                new Thread(() => ProcessTwitchClient()).Start();
                Console.Write("------------------------------------------ \n");
            }
            if(EnableDiscordSupport)
            {
                Console.Write("------------------------------------------ \n");
                ProcessDiscord();
                Console.Write("Starting Discord Client \n");
                Console.Write("------------------------------------------ \n");
            }
            if (EnableYoutubeSupport)
            {
                new Thread(() => Youtube.ProcessYoutubeThread()).Start();
            }
            return true;
        }

        public static void LoadINIFile()
        {
            Console.WriteLine("----------------------------------------------------------------------");
            Console.Write("Loading Settings. \n");
            if (!File.Exists("config.ini"))
            {
                Console.WriteLine("Settings file not found!");
                Console.WriteLine("Please use the \"Configure\" button in the mod manager to set up the mod.");
                return;
            }
            var MyIni = IniFile.IniSerializer.Deserialize<Settings>("config.ini");
            if (MyIni.General == null)
                MyIni.General = new SettingsGeneral();
            if (MyIni.Twitch == null)
                MyIni.Twitch = new SettingsTwitch();
            if (MyIni.Discord == null)
                MyIni.Discord = new SettingsDiscord();
            AdminUsrName = MyIni.General.AdminUsername.ToLowerInvariant();
           
            AllowBuildHTMLPagesForOverlay = MyIni.General.BuildHTMLPagesForOverlay;

            EnableTwitchSupport = MyIni.Twitch.Enable;
            Room = MyIni.Twitch.ChannelName.ToLowerInvariant();
            EnableDiscordSupport = MyIni.Discord.Enable;
            Discord.TempTokenToLoad = MyIni.Discord.BotKey;
            EnableYoutubeSupport = MyIni.YouTube.Enable;
            YoutubeVideoIDFromSettings = MyIni.YouTube.VideoID;
            AllowVideoIDForm = MyIni.YouTube.PromptForVideoID;
            YoutubeAPIKey = MyIni.YouTube.APIKey;
            Console.WriteLine("------------------------------------------ ");
            Console.WriteLine("Twitch Support - {0} ", EnableTwitchSupport);
            Console.WriteLine("Discord Support - {0} ", EnableDiscordSupport);
            Console.WriteLine("Youtube Support - {0} ", EnableYoutubeSupport);
            Console.WriteLine("------------------------------------------ ");
            Console.WriteLine("Done Loading Settings, ");
        }
        
        
        [DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GiveItem(int item);
    }

}

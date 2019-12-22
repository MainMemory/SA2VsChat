using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Timers;

namespace SA2VsChatNET
{
    class Youtube
    {
        const int EVENT_OTHER_SOCIAL = 70;
        const string LiveID = "lD7vXiGDJbQ";
        public static bool ReadyToCheck = false;
        public static YouTubeMessage YoutubeMessage = new YouTubeMessage();
        public static List<YouTubeMessage> YoutubeMessageList = new List<YouTubeMessage>();
        public static List<YouTubeMessage> SpentYoutubeMessages = new List<YouTubeMessage>();
        public struct YouTubeMessage
        {
            public string UUID;
            public string Author;
            public string Message;
            public DateTime PublishedTime;


            public YouTubeMessage(string UniqueID, string author, string msg, DateTime PubTime)
            {
                UUID = UniqueID;
                Author = author;
                Message = msg;
                PublishedTime = PubTime;
            }
        }
        public static DateTime TimeDateStarted;
    public static void ProcessYoutubeThread()
        {
            if (SA2VsChat.AllowVideoIDForm)
            {
                Console.Write("------------------------------------------ \n");
                Console.Write("Starting Youtube Client Form To Check Video ID \n");
                URLForm FrmURLCHECK = new URLForm();
                FrmURLCHECK.ShowDialog();

                Console.Write("------------------------------------------ \n");
                while (ReadyToCheck == false)
                {
                    Thread.Sleep(10);
                }
            }
            TimeDateStarted = DateTime.UtcNow;
            Console.Write("Starting Youtube Client With Video ID {0} \n", SA2VsChat.YoutubeVideoIDFromSettings);
            System.Timers.Timer GetMessages = new System.Timers.Timer();
            GetMessages.Interval = 10000;
            GetMessages.Elapsed += TimerOnElapsed;
            GetMessages.Start();
            while (true)
            {
                lock (YoutubeMessageList)
                {
                    //Console.Write("Codehit! \n");
                    CheckMessageList();

                }
                Thread.Sleep(10);
            }
        }
        public static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (YoutubeMessageList)
            {
                Console.Write("RanYoutubeCheck! \n");
                Get_Chat(SA2VsChat.YoutubeVideoIDFromSettings);
            }
        }
        public static void CheckMessageList()
        {
            if(YoutubeMessageList.Count > 0)
            {
                foreach (YouTubeMessage CurrentTYMessage in YoutubeMessageList)
                {
                    DateTime OffsetTime;
                    TimeSpan TimeDiffrence;
                    TimeDiffrence = DateTime.UtcNow - CurrentTYMessage.PublishedTime;
                    OffsetTime = DateTime.UtcNow + TimeDiffrence;
                    //Console.Write("MSG: {0}, DATEPUB: {1}, DATEOFF: {2}, DiffrenceL {3) \n",CurrentTYMessage.Message.ToString() , CurrentTYMessage.PublishedTime.ToShortTimeString(), OffsetTime.ToShortTimeString(), TimeDiffrence.ToString());
                   
                        SA2VsChat.ProcessMessage(CurrentTYMessage.Message, CurrentTYMessage.Author);
                        Console.Write("Processed Message - Removing");
                        SpentYoutubeMessages.Add(CurrentTYMessage); //We're done with it. 
                    Thread.Sleep(9000 / YoutubeMessageList.Count);

                

                }
                YoutubeMessageList.RemoveAll(x => SpentYoutubeMessages.Contains(x)); //Remove Dead Messages
            }
    
        }
        public static bool isCurrenctDateBetween(DateTime fromDate, DateTime toDate)
        {
            DateTime curent = DateTime.Now.Date;
            if (fromDate.CompareTo(toDate) >= 1)
            {
                
            }
            int cd_fd = curent.CompareTo(fromDate);
            int cd_td = curent.CompareTo(toDate);

            if (cd_fd == 0 || cd_td == 0)
            {
                return true;
            }

            if (cd_fd >= 1 && cd_td <= -1)
            {
                return true;
            }
            return false;
        }
        public static List<string> SeenUUIDs = new List<string>();
        public static string LastMessageUUID = "";
        public static bool CheckUUID(string MessageUUID)
        {

            foreach (string StoredUUID in SeenUUIDs)
            {
                if (MessageUUID == StoredUUID)
                {
                    return false;
                }
            }
            return true;
        }
        //string[] Timeformats = { "yyyy-MM-ddTHH\:mm\:ss.fffffffzzz" };
        //[DllExport("Get_Chat", CallingConvention.Cdecl)]
        public static void Get_Chat(string str)
        {

            string ReturnString = "";
            string MessageAuthor = "";
            string MessageUUID = "";
            DateTime MessageDateTime = DateTime.UtcNow;
            string Key = SA2VsChat.YoutubeAPIKey;
            string videoId = str;
            string videoLink;

            videoLink = "https://www.googleapis.com/youtube/v3/videos?id=" + videoId + "&key=" + Key + "&part=liveStreamingDetails,snippet";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoLink);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (System.IO.Stream stream = response.GetResponseStream())
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {

               
                JObject joResponse = JObject.Parse(reader.ReadToEnd());
                String liveChatId = joResponse["items"][0]["liveStreamingDetails"]["activeLiveChatId"].ToString();
                String chatListLink = "https://www.googleapis.com/youtube/v3/liveChat/messages?liveChatId=" + liveChatId + "&part=id%2C+snippet%2C+authorDetails&key=" + Key;

                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(chatListLink);
                using (HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse())
                using (System.IO.Stream stream2 = response2.GetResponseStream())
                using (System.IO.StreamReader reader2 = new System.IO.StreamReader(stream2))
                {
                   
                    JObject joResponse2 = JObject.Parse(reader2.ReadToEnd());
                    foreach (var item in joResponse2["items"])
                    {
                        ReturnString = Convert.ToString(item["snippet"]["displayMessage"]);
                        MessageUUID = Convert.ToString(item["id"]);
                        MessageAuthor = Convert.ToString(item["authorDetails"]["displayName"]);
                        MessageDateTime = DateTime.Parse(Convert.ToString(item["snippet"]["publishedAt"]));
                        if (MessageDateTime < TimeDateStarted) continue;
                        //Console.Write("YTU-UID: {0} - FROM: {1} - Message: {2} \n", MessageUUID, MessageAuthor, ReturnString);
                        if (SeenUUIDs.Count > 0)
                        {
                            if (CheckUUID(MessageUUID))
                            {
                                //Message Is Unique - send it!
                                //
                                
                                YoutubeMessage.UUID = MessageUUID;
                                YoutubeMessage.Author = MessageAuthor;
                                YoutubeMessage.Message = ReturnString;
                                YoutubeMessage.PublishedTime = MessageDateTime;
                                YoutubeMessageList.Add(YoutubeMessage);

                                Console.Write("YTU-UID: {0} - FROM: {1} - Message: {2} \n", MessageUUID, MessageAuthor, ReturnString);
                               
                                SeenUUIDs.Add(MessageUUID);
                            }

                        }
                        else
                        {
                            //we haven't seen any messages before...Lose it.
                            YoutubeMessage.UUID = MessageUUID;
                            YoutubeMessage.Author = MessageAuthor;
                            YoutubeMessage.Message = ReturnString;
                            YoutubeMessage.PublishedTime = MessageDateTime;

                            YoutubeMessageList.Add(YoutubeMessage);
                            Console.Write("YTU-UID: {0} - FROM: {1} - Message: {2} \n", MessageUUID, MessageAuthor, ReturnString);
                            SeenUUIDs.Add(MessageUUID);
                        }
                        if (SeenUUIDs.Count > 1000)
                        {
                            SeenUUIDs.Clear();
                            Console.Write("Cleared Seen Messages - Reached 1000");
                        }

                    }

                    Console.Write("AFTER foreachloop. \n");
                }

            }
        }
    }
}

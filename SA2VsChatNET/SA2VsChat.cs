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
using System.Security;
using System.Security.Permissions;

namespace SA2VsChatNET
{
	/// <summary>
	/// This class is simply a proxy for calling Init again from another AppDomain.
	/// </summary>
	public class InitProxy : MarshalByRefObject
	{
		public void Run()
		{
			SA2VsChat.Init();
		}
	}

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

		public static void ProcessTwitchClient()
		{
			TwitchIRCClient TwitchClient = new TwitchIRCClient(ServerIP, Port, User, Key);
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

		public class Vote
		{
			public StoryEntryType Type;
			public short ID;
			public bool Dark;

			public Vote(StoryEntryType type, short id = 0, bool dark = false)
			{
				Type = type;
				ID = id;
				Dark = dark;
			}

			public override bool Equals(object obj)
			{
				Vote v = obj as Vote;
				if (v == null) return false;
				return Type == v.Type && ID == v.ID && Dark == v.Dark;
			}

			public override int GetHashCode()
			{
				return (ID << 9) | ((int)Type << 1) | (Dark ? 1 : 0);
			}
		}

		public enum StoryEntryType
		{
			Event,
			Level,
			End,
			Credits
		}

		enum LevelIDs
		{
			BasicTest,
			KnucklesTest,
			SonicTest,
			GreenForest,
			WhiteJungle,
			PumpkinHill,
			SkyRail,
			AquaticMine,
			SecurityHall,
			PrisonLane,
			MetalHarbor,
			IronGate,
			WeaponsBed,
			CityEscape,
			RadicalHighway,
			WeaponsBed2P,
			WildCanyon,
			MissionStreet,
			DryLagoon,
			SonicVsShadow1,
			TailsVsEggman1,
			SandOcean,
			CrazyGadget,
			HiddenBase,
			EternalEngine,
			DeathChamber,
			EggQuarters,
			LostColony,
			PyramidCave,
			TailsVsEggman2,
			FinalRush,
			GreenHill,
			MeteorHerd,
			KnucklesVsRouge,
			CannonsCoreS,
			CannonsCoreE,
			CannonsCoreT,
			CannonsCoreR,
			CannonsCoreK,
			MissionStreet2P,
			FinalChase,
			WildCanyon2P,
			SonicVsShadow2,
			CosmicWall,
			MadSpace,
			SandOcean2P,
			DryLagoon2P,
			PyramidRace,
			HiddenBase2P,
			PoolQuest,
			PlanetQuest,
			DeckRace,
			DowntownRace,
			CosmicWall2P,
			GrindRace,
			LostColony2P,
			EternalEngine2P,
			MetalHarbor2P,
			IronGate2P,
			DeathChamber2P,
			BigFoot,
			HotShot,
			FlyingDog,
			KingBoomBoo,
			EggGolemS,
			Biolizard,
			FinalHazard,
			EggGolemE,
			Route101280 = 70,
			KartRace,
			ChaoWorld = 90,
			Invalid,
			Dark = 0x80
		}

		static readonly Dictionary<string, LevelIDs> levelmap = new Dictionary<string, LevelIDs>()
		{
			{ "gf", LevelIDs.GreenForest },
			{ "wj", LevelIDs.WhiteJungle },
			{ "ph", LevelIDs.PumpkinHill },
			{ "sr", LevelIDs.SkyRail },
			{ "am", LevelIDs.AquaticMine },
			{ "sh", LevelIDs.SecurityHall },
			{ "pl", LevelIDs.PrisonLane },
			{ "mha", LevelIDs.MetalHarbor },
			{ "ig", LevelIDs.IronGate },
			{ "wb", LevelIDs.WeaponsBed },
			{ "ce", LevelIDs.CityEscape },
			{ "rh", LevelIDs.RadicalHighway },
			{ "wc", LevelIDs.WildCanyon },
			{ "mst", LevelIDs.MissionStreet },
			{ "dl", LevelIDs.DryLagoon },
			{ "sovsh1", LevelIDs.SonicVsShadow1 },
			{ "shvso1", LevelIDs.SonicVsShadow1 | LevelIDs.Dark },
			{ "tve1", LevelIDs.TailsVsEggman1 },
			{ "evt1", LevelIDs.TailsVsEggman1 | LevelIDs.Dark },
			{ "so", LevelIDs.SandOcean },
			{ "cg", LevelIDs.CrazyGadget },
			{ "hb", LevelIDs.HiddenBase },
			{ "ee", LevelIDs.EternalEngine },
			{ "dc", LevelIDs.DeathChamber },
			{ "eq", LevelIDs.EggQuarters },
			{ "lc", LevelIDs.LostColony },
			{ "pc", LevelIDs.PyramidCave },
			{ "tve2", LevelIDs.TailsVsEggman2 },
			{ "evt2", LevelIDs.TailsVsEggman2 | LevelIDs.Dark },
			{ "fr", LevelIDs.FinalRush },
			{ "gh", LevelIDs.GreenHill },
			{ "mhe", LevelIDs.MeteorHerd },
			{ "kvr", LevelIDs.KnucklesVsRouge },
			{ "rvk", LevelIDs.KnucklesVsRouge | LevelIDs.Dark },
			{ "cc", LevelIDs.CannonsCoreT },
			{ "fc", LevelIDs.FinalChase },
			{ "sovsh2", LevelIDs.SonicVsShadow2 },
			{ "shvso2", LevelIDs.SonicVsShadow2 | LevelIDs.Dark },
			{ "cw", LevelIDs.CosmicWall },
			{ "msp", LevelIDs.MadSpace },
			{ "bf", LevelIDs.BigFoot },
			{ "hs", LevelIDs.HotShot },
			{ "fd", LevelIDs.FlyingDog },
			{ "kbb", LevelIDs.KingBoomBoo },
			{ "egs", LevelIDs.EggGolemS },
			{ "bl", LevelIDs.Biolizard },
			{ "fh", LevelIDs.FinalHazard },
			{ "ege", LevelIDs.EggGolemE },
			{ "r101", LevelIDs.Route101280 },
			{ "r280", LevelIDs.Route101280 | LevelIDs.Dark }
		};

		static readonly short[] events =
		{
			0000,
			0002,
			0003,
			0004,
			0005,
			0006,
			0011,
			0014,
			0015,
			0016,
			0017,
			0019,
			0021,
			0022,
			0024,
			0025,
			0026,
			0027,
			0028,
			0100,
			0101,
			0102,
			0103,
			0105,
			0106,
			0107,
			0109,
			0111,
			0112,
			0113,
			0116,
			0118,
			0119,
			0120,
			0122,
			0123,
			0124,
			0126,
			0127,
			0128,
			0129,
			0130,
			0131,
			0200,
			0201,
			0203,
			0204,
			0205,
			0206,
			0207,
			0208,
			0210,
			0211,
			0360,
			0361,
			0401,
			0409,
			0411,
			0420,
			0428,
			0429,
			0430,
			0524,
			0532,
			0602,
			0609
		};

		static bool allowCredits = true;
		static Dictionary<string, Vote> votes = new Dictionary<string, Vote>();
		static Dictionary<Vote, int> tally = new Dictionary<Vote, int>();
		static Vote topResult = new Vote(StoryEntryType.End);
		static int storyLength = 0;
		public static void TallyVotes()
		{
			tally.Clear();
			foreach (var item in votes.Values)
			{
				if (!tally.ContainsKey(item))
					tally.Add(item, 1);
				else
					++tally[item];
			}
			topResult = tally.OrderByDescending(a => a.Value).First().Key;
			SetNextStoryEvent((byte)topResult.Type, topResult.ID, topResult.Dark);
		}

		[DllExport(CallingConvention.Cdecl)]
		public static void ResetVotes(int eventno)
		{
			votes.Clear();
			tally.Clear();
			if (topResult.Type == StoryEntryType.Credits || (topResult.Type == StoryEntryType.Event && topResult.ID == 210))
				allowCredits = false;
			else if (eventno == 0)
				allowCredits = true;
			storyLength = eventno + 1;
			if (AllowBuildHTMLPagesForOverlay)
				DoBuildHTML();
		}

		static string modDir;
		public static void DoBuildHTML()
		{
			StringBuilder strBldr = new StringBuilder();
			StringWriter strWriter = new StringWriter(strBldr);

			HtmlTextWriter writer = new HtmlTextWriter(strWriter);

			//<html>
			writer.RenderBeginTag(HtmlTextWriterTag.Html);
			writer.Write("<meta http-equiv=\"Pragma\" content=\"no-cache\">\n<meta http-equiv=\"Expires\" content=\"-1\">\n<meta http-equiv=\"Refresh\" content=\"1\">");
			// <head>
			writer.RenderBeginTag(HtmlTextWriterTag.Head);
			writer.Write("<style>\n    body {\n      margin: 5px;\n      color: #FFFFFF;\n      text-shadow: 4px 1px #000000;\n      text-align: right;\n      font-size: 12pt;\n      font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif;\n      font-weight: bold;\n      font-style: normal;\n    }\n  </style>");
			// </head>
			writer.RenderEndTag();
		   
			// <body>
			writer.RenderBeginTag(HtmlTextWriterTag.Body);
			writer.Write($"<h3>Story Length: {storyLength}</h3>");
			writer.Write("<h3>Votes:</h3>");
			if (tally.Count > 0)
			{
				foreach (var item in tally.OrderByDescending(a=>a.Value).Take(Math.Min(tally.Count, MaximumVoteResults)))
				{
					writer.Write("<div>");
					switch (item.Key.Type)
					{
						case StoryEntryType.Event:
							writer.Write($"Event {item.Key.ID}");
							break;
						case StoryEntryType.Level:
							switch ((LevelIDs)item.Key.ID)
							{
								case LevelIDs.SonicVsShadow1:
									if (item.Key.Dark)
										writer.Write("Shadow Vs Sonic 1");
									else
										writer.Write("Sonic Vs Shadow 1");
									break;
								case LevelIDs.TailsVsEggman1:
									if (item.Key.Dark)
										writer.Write("Eggman Vs Tails 1");
									else
										writer.Write("Tails Vs Eggman 1");
									break;
								case LevelIDs.TailsVsEggman2:
									if (item.Key.Dark)
										writer.Write("Eggman Vs Tails 2");
									else
										writer.Write("Tails Vs Eggman 2");
									break;
								case LevelIDs.KnucklesVsRouge:
									if (item.Key.Dark)
										writer.Write("Rouge Vs Knuckles");
									else
										writer.Write("Knuckles Vs Rouge");
									break;
								case LevelIDs.CannonsCoreT:
									writer.Write("Cannon's Core");
									break;
								case LevelIDs.SonicVsShadow2:
									if (item.Key.Dark)
										writer.Write("Shadow Vs Sonic 2");
									else
										writer.Write("Sonic Vs Shadow 2");
									break;
								case LevelIDs.EggGolemS:
									writer.Write("Egg Golem (Sonic)");
									break;
								case LevelIDs.Biolizard:
									writer.Write("The Biolizard");
									break;
								case LevelIDs.FinalHazard:
									writer.Write("The FinalHazard");
									break;
								case LevelIDs.EggGolemE:
									writer.Write("Egg Golem (Eggman)");
									break;
								case LevelIDs.Route101280:
									if (item.Key.Dark)
										writer.Write("Route 280");
									else
										writer.Write("Route 101");
									break;
								default:
									string name = ((LevelIDs)item.Key.ID).ToString();
									StringBuilder sb = new StringBuilder(name.Length);
									sb.Append(name[0]);
									bool skip = false;
									for (int i = 1; i < name.Length - 1; i++)
									{
										if (!skip && (char.IsDigit(name, i) || char.IsUpper(name, i)) && char.IsLower(name, i + 1))
											sb.Append(' ');
										sb.Append(name[i]);
										if (char.IsLower(name, i) && (char.IsDigit(name, i + 1) || char.IsUpper(name, i + 1)))
										{
											sb.Append(' ');
											skip = true;
										}
										else
											skip = false;
									}
									sb.Append(name[name.Length - 1]);
									writer.Write(sb.ToString());
									break;
							}
							break;
						case StoryEntryType.End:
						case StoryEntryType.Credits:
							writer.Write(item.Key.Type);
							break;
					}
					writer.Write($": {item.Value}</div>");
				}
			}
			else
				writer.Write("<div>No votes!</div>");

			writer.Write("<h3>Die command is ");
			if (DateTime.Now - lastDieCommand <= dieCommandTimeout)
				writer.Write("not ");
			writer.Write("available.</h3>");

			writer.Write("<h3>Win command is ");
			if (DateTime.Now - lastWinCommand <= winCommandTimeout)
				writer.Write("not ");
			writer.Write("available.</h3>");

			// </body>
			writer.RenderEndTag();

			// </html>
			writer.RenderEndTag();
			try
			{
				File.WriteAllText(Path.Combine(modDir, "VoteStatus.html"), strBldr.ToString());
			}
			catch
			{
			   
			}
			
		}

		static readonly Dictionary<string, int> itemDict = new Dictionary<string, int>()
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

		static readonly Dictionary<string, int> bonusDict = new Dictionary<string, int>()
		{
			{ "good", 100 },
			{ "nice", 200 },
			{ "great", 300 },
			{ "jammin", 400 },
			{ "cool", 500 },
			{ "radical", 600 },
			{ "tight", 800 },
			{ "awesome", 1000 },
			{ "extreme", 1500 },
			{ "perfect", 2000 }
		};

		static TimeSpan dieCommandTimeout;
		static DateTime lastDieCommand;
		static TimeSpan winCommandTimeout;
		static DateTime lastWinCommand;
		public static void ProcessMessage(string message, string userName)
		{
			userName = userName.ToLowerInvariant();
			message = message.ToLowerInvariant();
		   // Console.WriteLine("From: {0} - Message:{1} ", channelName, msg);
			if (userName == "streamlabs")
			{

				message = message.Substring(2);
				Console.WriteLine("Message:{0}", message);
			}

			if (!message.StartsWith("!"))
				return;

			string[] split = message.Substring(1).Split(' ');

			if (userName == Room || userName == AdminUsrName)
			{
				// admin stuff
			}

			switch (split[0])
			{
				case "item":
					if (split.Length > 1)
					{
						if (itemDict.TryGetValue(split[1], out int v))
							GiveItem(v);
					}
					else
					{
						int i = RandomNumber(0, 10);
						if (i == 9)
							++i;
						GiveItem(i);
					}
					break;
				case "omochao":
					SpawnOmochao();
					break;
				case "voice":
					if (split.Length > 1)
					{
						if (int.TryParse(split[1], out int id) && id >= 0 && id < 2727)
							PlayVoice(Math.Min(Math.Max(0, id), 2726));
					}
					else
						PlayVoice(RandomNumber(0, 2727));
					break;
				case "stop":
					Stop();
					break;
				case "gottagofast":
					GottaGoFast();
					break;
				case "tsafogattog":
					TsafOgAttog();
					break;
				case "superjump":
					SuperJump();
					break;
				case "pmujrepus":
					PmujRepus();
					break;
				case "timestop":
					TimeStop();
					break;
				case "die":
					if (DateTime.Now - lastDieCommand > dieCommandTimeout && Die(userName))
						lastDieCommand = DateTime.Now;
					break;
				case "win":
					if (DateTime.Now - lastWinCommand > winCommandTimeout && Win(userName))
						lastWinCommand = DateTime.Now;
					break;
				case "grow":
					Grow();
					break;
				case "shrink":
					Shrink();
					break;
				case "bonus":
					if (split.Length > 1)
					{
						if (bonusDict.TryGetValue(split[1], out int val))
							Bonus(val);
						else if (int.TryParse(split[1], out val))
							Bonus(val);
					}
					else
						Bonus(bonusDict.Values.ElementAt(RandomNumber(0, bonusDict.Count)));
					break;
				case "music":
					if (split.Length > 1)
					{
						if (int.TryParse(split[1], out int mus) && mus >= 0 && mus < 157)
							PlayMusic(mus);
					}
					else
						PlayMusic(RandomNumber(0, 157));
					break;
				case "highgravity":
					HighGravity();
					break;
				case "lowgravity":
					LowGravity();
					break;
				case "healboss":
					HealBoss();
					break;
				case "confuse":
					Confuse();
					break;
				case "earthquake":
					Earthquake();
					break;
				case "chaokey":
					ToggleChaoKey();
					break;
				case "water":
					ToggleWater();
					break;
				case "level":
					if (split.Length > 1 && levelmap.TryGetValue(split[1], out LevelIDs lvl))
					{
						votes[userName] = new Vote(StoryEntryType.Level, (short)(lvl & ~LevelIDs.Dark), lvl.HasFlag(LevelIDs.Dark));
						TallyVotes();
					}
					break;
				case "event":
					if (split.Length > 1 && short.TryParse(split[1], out short ev) && events.Contains(ev))
					{
						if (!allowCredits && ev == 210) break;
						votes[userName] = new Vote(StoryEntryType.Event, ev);
						TallyVotes();
					}
					break;
				case "credits":
					if (allowCredits)
					{
						votes[userName] = new Vote(StoryEntryType.Credits);
						TallyVotes();
					}
					break;
				case "endstory":
					votes[userName] = new Vote(StoryEntryType.End);
					TallyVotes();
					break;  
			}

			if (AllowBuildHTMLPagesForOverlay)
			{
				DoBuildHTML();
			}
		}

		public static bool EnableDiscordSupport = false;
		public static bool EnableTwitchSupport = false;
		public static bool EnableYoutubeSupport = false;
		public static bool AllowVideoIDForm = true;
		public static bool AllowBuildHTMLPagesForOverlay = true;
		public static string YoutubeVideoIDFromSettings = "";
		public static string YoutubeAPIKey = "";
		public static int MaximumVoteResults = 3;

		static AppDomain _childDomain;
		[DllExport(CallingConvention.Cdecl)]
		public static void Main()
		{
			// Try restarting in another AppDomain if possible.
			try
			{
				// Give the new AppDomain full permissions.
				PermissionSet permissionSet = new PermissionSet(PermissionState.Unrestricted);
				permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.AllFlags));

				// The ApplicationBase of the new domain should be the directory containing the current DLL.
				AppDomainSetup appDomainSetup = new AppDomainSetup() { ApplicationBase = Path.GetDirectoryName(typeof(InitProxy).Assembly.Location) };
				_childDomain = AppDomain.CreateDomain("SA2VsChat", null, appDomainSetup, permissionSet);

				// Now make the new AppDomain load our code using our proxy.
				Type proxyType = typeof(InitProxy);
				dynamic initProxy = _childDomain.CreateInstanceFrom(proxyType.Assembly.Location, proxyType.FullName).Unwrap(); // Our AssemblyResolve will pick the missing DLL out.
				initProxy.Run();
			}
			catch
			{
				Init();
			}
		}

		public static void Init()
		{
			TwitchIRCStart();
		}

		public static void TwitchIRCStart()
		{
			modDir = Environment.CurrentDirectory;
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
			if (MyIni.YouTube == null)
				MyIni.YouTube = new SettingsYouTube();
			AdminUsrName = MyIni.General.AdminUsername?.ToLowerInvariant();
		   
			AllowBuildHTMLPagesForOverlay = MyIni.General.BuildHTMLPagesForOverlay;
			if (AllowBuildHTMLPagesForOverlay)
				DoBuildHTML();

			dieCommandTimeout = TimeSpan.FromMinutes(MyIni.General.DieCommandTimeout);
			lastDieCommand = DateTime.Now;

			winCommandTimeout = TimeSpan.FromMinutes(MyIni.General.WinCommandTimeout);
			lastWinCommand = DateTime.Now;

			MaximumVoteResults = MyIni.General.MaximumVoteResults;

			EnableTwitchSupport = MyIni.Twitch.Enable;
			Room = MyIni.Twitch.ChannelName?.ToLowerInvariant();
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
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SpawnOmochao();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void PlayVoice(int id);
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Stop();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void GottaGoFast();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TsafOgAttog();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SuperJump();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void PmujRepus();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void TimeStop();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool Die([MarshalAs(UnmanagedType.LPStr)] string user);
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool Win([MarshalAs(UnmanagedType.LPStr)] string user);
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Grow();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Shrink();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Bonus(int scr);
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void PlayMusic(int id);
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void HighGravity();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void LowGravity();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void HealBoss();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Confuse();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Earthquake();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void ToggleChaoKey();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void ToggleWater();
		[DllImport("SA2VsChat.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetNextStoryEvent(byte type, short id, bool dark);
	}

}

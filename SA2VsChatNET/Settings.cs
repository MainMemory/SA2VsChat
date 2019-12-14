using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA2VsChatNET
{
	public class Settings
	{
		public SettingsGeneral General { get; set; }
		public SettingsTwitch Twitch { get; set; }
		public SettingsDiscord Discord { get; set; }
		public SettingsYouTube YouTube { get; set; }
	}

	public class SettingsGeneral
	{
		public string AdminUsername { get; set; }
		public bool BuildHTMLPagesForOverlay { get; set; }
	}

	public class SettingsTwitch
	{
		public bool Enable { get; set; }
		public string ChannelName { get; set; }
	}

	public class SettingsDiscord
	{
		public bool Enable { get; set; }
		public string BotKey { get; set; }
	}

	public class SettingsYouTube
	{
		public bool Enable { get; set; }
		public string VideoID { get; set; }
		public bool PromptForVideoID { get; set; }
		public string APIKey { get; set; }
	}
}

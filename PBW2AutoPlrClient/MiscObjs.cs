using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace PBW2AutoPlrClient
{
	class GameTypeSettingsObj
	{

		public string GameTypeName { get; set; }
		public string GameExe { get; set; }
		public string GameArguments { get; set; }
		public string GamePreScript { get; set; }
		public string GamePostScript { get; set; }
		public string GameUploadFileMask { get; set; }
		public Dictionary<string, ModSettingsObj> GameMods = new Dictionary<string, ModSettingsObj>();         

	}

	class ModSettingsObj
	{
		public string ModName { get; set; }
		public string ModPath { get; set; }
		public string ModSavePath { get; set; }
        
	}

	static class ServerSettingsObj
	{

		private static string pbw_address;
		private static string pbw_loginpath;
		private static string pbw_gamelistpath;
		private static string pbw_uploadturnformparam;
		private static string pbw_uploadplrfilepath;
		private static string pbw_turndownloadpath;
		private static string pbw_gamepath;
		private static bool user_savelogin;
		private static string user_username;
		private static string user_password;
        private static string user_dl_dir;
		private static CookieContainer cookiejar;
        private static string connection_status;
        private static int refresh_rate;


        public static int Refresh_Rate
        {
            get { return refresh_rate; }
            set { refresh_rate = value; }
        }

        /// <summary>
        /// Sets or Gets connection status
        /// </summary>
        /// <remarks>
        /// set must be one of the following strings: 
        /// "Connected" 
        /// "Disconnected"
        /// "Auth Fail" 
        /// "Not Found" 
        /// "Time Out"
        /// </remarks>
        public static string Connection_Status
        {

            get { return connection_status; }
            set 
            {
                if ((value == "Connected") || (value == "Disconnected") || (value == "Auth Fail") || (value == "Not Found") || (value == "Time Out"))
                    { connection_status = value; }
                else
                {
                    throw new System.ArgumentException("Invalid argument for ConnectionStatus");        
                }
            }
        }
		public static CookieContainer CookieJar
		{
			get { return cookiejar; }
			set { cookiejar = value; }
		}
		public static string PBW_Address 
		{
			get { return pbw_address; }
			set { pbw_address = value; } 
		}
		public static string PBW_GamesListPath 
		{
			get { return pbw_gamelistpath; }
			set { pbw_gamelistpath = value; }
		}
		public static string PBW_GamePath
		{
			get { return pbw_gamepath; }
			set { pbw_gamepath = value; }
		}
		public static string PBW_LoginPath 
		{
			get { return pbw_loginpath; }
			set { pbw_loginpath = value; }
		}
		public static string PBW_UploadTurnFormParam
		{
			get { return pbw_uploadturnformparam; }
			set { pbw_uploadturnformparam = value; }
		}
		public static string PBW_UploadPlrFilePath
		{
			get { return pbw_uploadplrfilepath; }
			set { pbw_uploadplrfilepath = value; }
		}
		public static string PBW_TurnDownloadPath
		{ 
			get { return pbw_turndownloadpath; }
			set { pbw_turndownloadpath = value; }
		}

		public static bool User_SaveLogin
		{
			get { return user_savelogin; }
			set { user_savelogin = value; }
		}
		public static string User_UserName
		{
			get {return user_username; }
			set { user_username = value; }
		}
		public static string User_Password
		{
			get { return user_password; }
			set { user_password = value; }
		}
        public static string User_Download_Directory
        {
            get { return user_dl_dir; }
            set { user_dl_dir = value; }
        }

		public static void loadSettings()
		{
			var settings = PBW2AutoPlrClient.Properties.Settings.Default;
			pbw_address = settings.pbw_address;
			pbw_gamelistpath = settings.pbw_games_list_path;
			pbw_loginpath = settings.pbw_login_path;
			pbw_uploadturnformparam = settings.pbw_uploadFormParam_player;
			pbw_uploadplrfilepath = settings.pbw_upload_playerfile_path;
			pbw_turndownloadpath = settings.pbw_player_download_path;
			pbw_gamepath = settings.pbw_game_path;
 
			User_SaveLogin = settings.user_savelogin;
			User_UserName = settings.user_login;
			User_Password = settings.user_password;
            User_Download_Directory = settings.user_download_path;
		}

		public static void saveSettings()
		{
			var settings = PBW2AutoPlrClient.Properties.Settings.Default;
			settings.pbw_address = pbw_address;
			settings.pbw_login_path = pbw_loginpath;
			settings.pbw_player_download_path = pbw_turndownloadpath;
			settings.pbw_games_list_path = pbw_gamelistpath;
			settings.pbw_upload_playerfile_path = pbw_uploadplrfilepath;
			settings.pbw_uploadFormParam_player = pbw_uploadturnformparam;
			settings.pbw_game_path = pbw_gamepath;
			settings.user_savelogin = user_savelogin;
            settings.user_download_path = user_dl_dir;

			if (user_savelogin)
			{
				settings.user_login = user_username;
				settings.user_password = user_password;
			}
			else
			{
				settings.user_login = "Xintis";
				settings.user_password = "*****";
			}
			settings.Save();
		}


 
	}


	class Interpreter
	{
		public static string interpretString(string thestring, GameObject gameobj, GameTypeSettingsObj gamesetobj)
		{
			
			///$gamename
			///$plrnumber
			///$password 
			///$turnnumber
			///$savepath
			///$modpath
            string fmt = "000#";
            
            //thestring = thestring.Replace();
			thestring = thestring.Replace("$gamename", gameobj.GameName);
			thestring = thestring.Replace("$plrnumber", gameobj.GamePlrNumber.ToString(fmt));
			thestring = thestring.Replace("$password", gameobj.GamePlrEmpPassword);
            thestring = thestring.Replace("$turnnumber", gameobj.GameTurnNum.ToString());
			thestring = thestring.Replace("$savepath", gamesetobj.GameMods[gameobj.GameMod].ModSavePath);
			thestring = thestring.Replace("$modpath", gamesetobj.GameMods[gameobj.GameMod].ModPath);

			return thestring;                
		}

	}

	class logger
	{
		private static List<string> log = new List<string>();
		private static int readCount = 0;
		public static void logwrite(string text, bool newline = true)
		{
			if (newline) { text += "\r\n"; }
			log.Add(text);
			
		}
		public static IEnumerable<string> logreadall(bool markRead = true)
		{
			if (markRead)
				readCount = log.Count;
			return log;
		}
		public static IEnumerable<string> logread(bool markRead = true)
		{
			var result = log.Skip(readCount).ToArray();
			if (markRead)
				readCount = log.Count;
			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PBW2AutoPlrClient.Properties;

namespace PBW2AutoPlrClient
{
	class GameTypeSettingsObj
	{

		public string GameTypeName { get; set; }
		public string GameExe { get; set; }
        public string WorkingDirectory { get; set; }
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

		private static string _pbwAddress;
		private static string _pbwLoginpath;
		private static string _pbwGamelistpath;
		private static string _pbwUploadturnformparam;
		private static string _pbwUploadplrfilepath;
		private static string _pbwTurndownloadpath;
		private static string _pbwGamepath;
		private static bool _userSavelogin;
		private static string _userUsername;
		private static string _userPassword;
        private static string _userDlDir;
		private static CookieContainer _cookiejar;
        private static string _connectionStatus;
        private static int _refreshRate;


        public static int RefreshRate
        {
            get { return _refreshRate; }
            set { _refreshRate = value; }
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
        public static string ConnectionStatus
        {

            get { return _connectionStatus; }
            set 
            {
                if ((value == "Connected") || (value == "Disconnected") || (value == "Auth Fail") || (value == "Not Found") || (value == "Time Out"))
                    { _connectionStatus = value; }
                else
                {
                    throw new ArgumentException("Invalid argument for ConnectionStatus");        
                }
            }
        }
		public static CookieContainer CookieJar
		{
			get { return _cookiejar; }
			set { _cookiejar = value; }
		}
		public static string PbwAddress 
		{
			get { return _pbwAddress; }
			set { _pbwAddress = value; } 
		}
		public static string PbwGamesListPath 
		{
			get { return _pbwGamelistpath; }
			set { _pbwGamelistpath = value; }
		}
		public static string PbwGamePath
		{
			get { return _pbwGamepath; }
			set { _pbwGamepath = value; }
		}
		public static string PbwLoginPath 
		{
			get { return _pbwLoginpath; }
			set { _pbwLoginpath = value; }
		}
		public static string PbwUploadTurnFormParam
		{
			get { return _pbwUploadturnformparam; }
			set { _pbwUploadturnformparam = value; }
		}
		public static string PbwUploadPlrFilePath
		{
			get { return _pbwUploadplrfilepath; }
			set { _pbwUploadplrfilepath = value; }
		}
		public static string PbwTurnDownloadPath
		{ 
			get { return _pbwTurndownloadpath; }
			set { _pbwTurndownloadpath = value; }
		}

		public static bool UserSaveLogin
		{
			get { return _userSavelogin; }
			set { _userSavelogin = value; }
		}
		public static string UserUserName
		{
			get {return _userUsername; }
			set { _userUsername = value; }
		}
		public static string UserPassword
		{
			get { return _userPassword; }
			set { _userPassword = value; }
		}
        public static string UserDownloadDirectory
        {
            get { return _userDlDir; }
            set { _userDlDir = value; }
        }

		public static void LoadSettings()
		{
			var settings = Settings.Default;
			_pbwAddress = settings.pbw_address;
			_pbwGamelistpath = settings.pbw_games_list_path;
			_pbwLoginpath = settings.pbw_login_path;
			_pbwUploadturnformparam = settings.pbw_uploadFormParam_player;
			_pbwUploadplrfilepath = settings.pbw_upload_playerfile_path;
			_pbwTurndownloadpath = settings.pbw_player_download_path;
			_pbwGamepath = settings.pbw_game_path;
 
			UserSaveLogin = settings.user_savelogin;
			UserUserName = settings.user_login;
			UserPassword = settings.user_password;
            UserDownloadDirectory = settings.user_download_path;
		}

		public static void SaveSettings()
		{
			var settings = Settings.Default;
			settings.pbw_address = _pbwAddress;
			settings.pbw_login_path = _pbwLoginpath;
			settings.pbw_player_download_path = _pbwTurndownloadpath;
			settings.pbw_games_list_path = _pbwGamelistpath;
			settings.pbw_upload_playerfile_path = _pbwUploadplrfilepath;
			settings.pbw_uploadFormParam_player = _pbwUploadturnformparam;
			settings.pbw_game_path = _pbwGamepath;
			settings.user_savelogin = _userSavelogin;
            settings.user_download_path = _userDlDir;

			if (_userSavelogin)
			{
				settings.user_login = _userUsername;
				settings.user_password = _userPassword;
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
		public static string InterpretString(string thestring, GameObject gameobj, GameTypeSettingsObj gamesetobj)
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

	class Logger
	{
		private static List<string> _log = new List<string>();
		private static int _readCount;
		public static void Logwrite(string text, bool newline = true)
		{
			if (newline) { text += "\r\n"; }
			_log.Add(text);
			
		}
		public static IEnumerable<string> Logreadall(bool markRead = true)
		{
			if (markRead)
				_readCount = _log.Count;
			return _log;
		}
		public static IEnumerable<string> Logread(bool markRead = true)
		{
			var result = _log.Skip(_readCount).ToArray();
			if (markRead)
				_readCount = _log.Count;
			return result;
		}
	}
}

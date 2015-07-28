using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

//using System.Threading;

namespace PBW2AutoPlrClient
{
    public partial class FormPbwAutoPlrClient : Form
    {

        Dictionary<string, GameObject> _gameObjects = new Dictionary<string, GameObject>();
        Dictionary<string, GameTypeSettingsObj> _gamesSettings = new Dictionary<string, GameTypeSettingsObj>();
        string _selectedGame;
        Timer _timer1;        
        
        public FormPbwAutoPlrClient()
        {
            ServerSettingsObj.LoadSettings();
            InitializeComponent();
            GameSettingUpdate();

            _timer1 = new Timer();
            _timer1.Tick += timer1_Tick;
            _timer1.Interval = 120000; // in miliseconds
            _timer1.Start();

        }
              
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ServerSettingsObj.ConnectionStatus == "Connected")
            {
                PbwGamelist(); 
                Refreshlogview();
                PbwServerData();
            }
        }

        private void toolStripButton_opensettings_Click(object sender, EventArgs e)
        {
            FormSettings settingsForm;
            settingsForm = new FormSettings();
            settingsForm.ShowDialog();
            GameSettingUpdate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            PbwLogin();
            
        }

        private void dataGridView_games_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            _selectedGame = dataGridView_games[0, e.RowIndex].Value.ToString();
            button_download.Enabled = true;
            button_extract.Enabled = true;
            button_playGame.Enabled = true;
            button_upload.Enabled = true;
        }



        private void button_download_Click(object sender, EventArgs e)
        {
            if (_selectedGame != null)
            {
                download_gamefile();
            }  
        }

        private void button_upload_Click(object sender, EventArgs e)
        {
            if (_selectedGame != null)
            {
                upload_plrfile();
            }
        }
            
        private void button_extract_Click(object sender, EventArgs e)
        {
            if (_selectedGame != null)
            {
                extract_game();
            }
        }

        private void button_playGame_Click(object sender, EventArgs e)
        {
            if (_selectedGame != null)
            {
                string gameType = _gameObjects[_selectedGame].GameType;
                string pregamescript = (_gamesSettings[gameType].GamePreScript);
                if ((pregamescript != null) && (pregamescript != ""))
                {
                    PregameScriptProcessLauncher();
                }
                else
                {
                    GameProcessLauncher();
                }
            }
        }

        private void button_launchpbw_Click(object sender, EventArgs e)
        {
            OpenLink("http://" + ServerSettingsObj.PbwAddress);
        }

        /// <summary>
        /// deletes old downloaded files in the download dir.
        /// </summary>
        public void cleanup_download()
        {
            string downloadDir = Path.GetDirectoryName(ServerSettingsObj.UserDownloadDirectory);
            string gameName = _gameObjects[_selectedGame].GameName;

            string lastturnnum = (_gameObjects[_selectedGame].GameTurnNum -1).ToString();
            string filetodel = Path.GetFileName(gameName + lastturnnum + ".rar");
            string fullpathtodel = Path.GetFullPath(Path.Combine(downloadDir, filetodel));

            string[] files = Directory.GetFiles(downloadDir);

            if (File.Exists(fullpathtodel))
            {
                Logger.Logwrite("deleteing file: " + fullpathtodel);
                File.Delete(fullpathtodel);
                
            }
        }

        public void Refreshlogview()
        {
            richTextBox_log.Text += string.Join("\r\n", Logger.Logread().ToArray());
        }

        public void GameSettingUpdate()
        {
            Cursor = Cursors.WaitCursor;
            _gamesSettings = XmlHandler.GetGameSettings("gamesettings.ini");
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// collects nesicary information from serverSettingsObj and sends it to the coms handler.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// updates the ServerSettingsObj.Connection_Status (currently only with "Connected")
        /// 
        /// </remarks>
        private void PbwLogin()
        {

            string address = "https://" + ServerSettingsObj.PbwAddress;

            string loginPath = ServerSettingsObj.PbwLoginPath;
            string login = ServerSettingsObj.UserUserName;
            string pass = ServerSettingsObj.UserPassword;
            string loginAddress = address + loginPath;
            toolStripStatusLabel_acty.Text = "Attempting PBW Login via https";
            Refresh();
            Cursor = Cursors.WaitCursor;
            bool connectsuccess = PbwComsHandler.ConnectPbw(loginAddress, login, pass);
            toolStripStatusLabel_acty.Text = "";
            if (connectsuccess) 
            { 
                toolStripStatusLabel_connectionstate.Text = "Connected";
                ServerSettingsObj.ConnectionStatus = "Connected"; 
                PbwGamelist();
                PbwServerData();

            }
            else { toolStripStatusLabel_connectionstate.Text = "Disconnected"; }
            Cursor = Cursors.Default;
            Refreshlogview();
        }

        /// <summary>
        /// Gets serverdata xml from PBW via the comms handler
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// calls xmlServerData to interpret the xmldata. 
        /// </remarks>
        private void PbwServerData()
        {

            string address = "http://" + ServerSettingsObj.PbwAddress;
            string nodePath = "node/player";
            string url = address + nodePath;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            Cursor = Cursors.WaitCursor;

            string xmldata = PbwComsHandler.get_PbwXmlData(cookies, url);

            if (xmldata == null) //probibly throw an exception from PBW_ComsHandler would be better.
            {
                PbwLogin();
                xmldata = PbwComsHandler.get_PbwXmlData(cookies, url);
                if (xmldata == null) //still? login fail. redo this when give pbwLogin a return.
                {
                    //MessageBox.Show("unable to get ServerData from PBW.");
                    toolStripStatusLabel_connectionstate.Text = @"Unable to get ServerData from PBW.";
                }
            }
            else
            {
                XmlServerData(xmldata);
            }
            toolStripStatusLabel_acty.Text = "";
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Gets gamelist xmldata from PBW viathe coms handler
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// calls  XmlHandler.xmlToGameObj to interpret the xmldata and build/update a dictionary of gameobjects
        /// </remarks>
        private void PbwGamelist()
        {

            string address = "https://" + ServerSettingsObj.PbwAddress;
            string gamelistPath = ServerSettingsObj.PbwGamesListPath;
            string gamelisturl = address + gamelistPath;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            Cursor = Cursors.WaitCursor;

            string xmlgamelist = PbwComsHandler.get_PbwXmlData(cookies, gamelisturl);
            if (xmlgamelist == null)
            {
                PbwLogin();
                xmlgamelist = PbwComsHandler.get_PbwXmlData(cookies, gamelisturl);
                if (xmlgamelist == null) //still? login fail. redo this when give pbwLogin a return.
                {
                    //MessageBox.Show("Unable to get games list from PBW.");
                    toolStripStatusLabel_connectionstate.Text = @"Unable to get games list from PBW.";
                }
            }
            else
            {

                _gameObjects = XmlHandler.XmlToGameObj(xmlgamelist, _gameObjects);
                RefreshGamesList();
            }
            toolStripStatusLabel_acty.Text = "";
            Cursor = Cursors.Default; 

        }

        /// <summary>
        /// takes xml serverdata from pbw, and updates the serversettingsobj .
        /// </summary>
        /// <param name="xmlstring">xmlstring from PBW</param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void XmlServerData(string xmlstring)

        {

            Dictionary<string,string> serversettings = new Dictionary<string,string>();
            serversettings = XmlHandler.ServerSettings(xmlstring);

            ServerSettingsObj.RefreshRate = int.Parse(serversettings["update_interval"]);
            if (ServerSettingsObj.RefreshRate != 0)
            { _timer1.Interval = ServerSettingsObj.RefreshRate * 1000; }
            //also get server time and max file size?   
        }

        /// <summary>
        /// refreshes and orders the games list dataGridView.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// also hides the empire password colomn.
        /// </remarks>
        private void RefreshGamesList()
        {
            int selectedGameIndex = 0;
            List<GameObject> listofgames = new List<GameObject>();
            foreach (GameObject game in _gameObjects.Values)
            {
                listofgames.Add(game);
                if (game.GameName == _selectedGame) // if the game name == selected_game ( a class string) 
                {
                    selectedGameIndex = listofgames.Count - 1; //then we know the index of the row that needs to be selected.
                }
                if (game.GamePlrStatus == "waiting")
                {
                    PlaySimpleSound();
                }
            }

            dataGridView_games.DataSource = listofgames;
            dataGridView_games.Columns["GameName"].DisplayIndex = 0;
            dataGridView_games.Columns["GamePlrStatus"].DisplayIndex = 1;
            dataGridView_games.Columns["GameNextTurn"].DisplayIndex = 2;
            dataGridView_games.Columns["GamePlrEmpPassword"].Visible = false;
                      
            dataGridView_games.Rows[selectedGameIndex].Selected = true;
            dataGridView_games.CurrentCell = dataGridView_games.Rows[selectedGameIndex].Cells[1];
            dataGridView_games.Refresh();
        }
 
        /// <summary>
        /// downloads a turn from PBW via the comshandler. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// TODO: needs to check success. maybe via an exception thrown by the comshandler? I'd have to have the coms handler throw... 
        /// </remarks>
        private void download_gamefile()
        {

            
            string turnNum = _gameObjects[_selectedGame].GameTurnNum.ToString();
            string gameName = _gameObjects[_selectedGame].GameName;
            string downloadDir = Path.GetDirectoryName(ServerSettingsObj.UserDownloadDirectory);
            string pbwaddress = "http://" + ServerSettingsObj.PbwAddress;
            string turnDownloadUri = ServerSettingsObj.PbwTurnDownloadPath;
            string gamePathUri = ServerSettingsObj.PbwGamePath;
            
            string fulldownloadUri = pbwaddress + gamePathUri + gameName + "/" + turnDownloadUri;
            
            string downloadfilename = Path.GetFullPath(Path.Combine(downloadDir, Path.GetFileName(gameName + turnNum + ".rar")));
            
            if (!File.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }
            toolStripStatusLabel_acty.Text = "Attempting Gamefile Download";
            Refresh();
            Cursor = Cursors.WaitCursor;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            PbwComsHandler.DownloadGame(cookies, fulldownloadUri, downloadfilename);
            toolStripStatusLabel_acty.Text = "Done?";
            Refresh();
            Cursor = Cursors.Default;
            cleanup_download();
            Refreshlogview();
        }
 
        /// <summary>
        /// uploads the player file using the coms handler. coms handler not currently working. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void upload_plrfile()
        {

            string gameType = _gameObjects[_selectedGame].GameType;
            string gameMod = _gameObjects[_selectedGame].GameMod;
            string gameName = _gameObjects[_selectedGame].GameName;
            string savegamepath = _gamesSettings[gameType].GameMods[gameMod].ModSavePath;
            string upfilemask = _gamesSettings[gameType].GameUploadFileMask;
            string address = "http://" + ServerSettingsObj.PbwAddress;
            string uploadpath = ServerSettingsObj.PbwUploadPlrFilePath;
            string gamepath = ServerSettingsObj.PbwGamePath;
            string uploadurl = address + gamepath + gameName + "/" + uploadpath;
            string uploadformParam = ServerSettingsObj.PbwUploadTurnFormParam;

            upfilemask = Interpreter.InterpretString(upfilemask, _gameObjects[_selectedGame], _gamesSettings[gameType]);
            string upfile = savegamepath + upfilemask;
            CookieContainer cookies = ServerSettingsObj.CookieJar;

            toolStripStatusLabel_acty.Text = "Attempting Playerfile upload";
            Refresh();
            Cursor = Cursors.WaitCursor;

            var response = PbwComsHandler.UploadTurn(cookies, upfile, uploadurl, uploadformParam);

            if (response)
            {
 
                toolStripStatusLabel_acty.Text = "Done!";
                Refresh();
                Cursor = Cursors.Default;
            }
            else
            {
                //MessageBox.Show("Turn could not be uploaded.");
                toolStripStatusLabel_acty.Text = "Failed...";
                Refresh();
                Cursor = Cursors.Default;
            }

            PbwGamelist();
            Refreshlogview();
            PbwServerData();
        }

        /// <summary>
        /// exctracts rar file using the archivehandler. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void extract_game()
        {

            string turnNum = _gameObjects[_selectedGame].GameTurnNum.ToString();
            string gameName = _gameObjects[_selectedGame].GameName;
            string gameType = _gameObjects[_selectedGame].GameType;
            string gameMod = _gameObjects[_selectedGame].GameMod;

            string downloadDir = Path.GetDirectoryName(ServerSettingsObj.UserDownloadDirectory);
            string downloadfilename = Path.GetFullPath(Path.Combine(downloadDir, Path.GetFileName(gameName + turnNum + ".rar")));

            string savegamepath = _gamesSettings[gameType].GameMods[gameMod].ModSavePath;
            savegamepath = Path.GetDirectoryName(Interpreter.InterpretString(savegamepath, _gameObjects[_selectedGame], _gamesSettings[gameType]));
            int arc = ArchiveHandler.setLibPath();

            toolStripStatusLabel_acty.Text = "Extracting game files";
            Refresh();
            Cursor = Cursors.WaitCursor;

            arc = ArchiveHandler.extractArchive(downloadfilename, savegamepath);

            toolStripStatusLabel_acty.Text = "Extracting Done";
            Refresh();
            Cursor = Cursors.Default;
        }
 
        /// <summary>
        /// launches pregame scripts, such as se4 windowed mode. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void PregameScriptProcessLauncher(bool closeaftergame = true)
        {


            string gameType = _gameObjects[_selectedGame].GameType;
            string scriptname = Path.GetFullPath(_gamesSettings[gameType].GamePreScript);

            Thread prescriptthread = new Thread(delegate()
            {
                try
                {
                    process_preGameProcess.StartInfo.FileName = scriptname;
                    process_preGameProcess.StartInfo.UseShellExecute = false;
                    process_preGameProcess.StartInfo.RedirectStandardError = true;
                    process_preGameProcess.StartInfo.RedirectStandardOutput = false;
                    process_preGameProcess.Start();
                }
                catch (Exception e)
                {
                    Logger.Logwrite("Script thread Process Error: " + e.Message);
                    MessageBox.Show("Script thread Process Error: " + e.Message);
                }
                while ((!process_preGameProcess.StandardError.EndOfStream))// || (!process_GameLauncher.StandardOutput.EndOfStream))
                {
                    string line = process_preGameProcess.StandardError.ReadLine();
                    Logger.Logwrite("StdErr: ", false);
                    Logger.Logwrite(line);
                    //line = process_GameLauncher.StandardOutput.ReadLine();
                    //logger.logwrite("StdOut: ", false);
                    //logger.logwrite(line);
                    
                }

            });
            try
            {
                prescriptthread.Start();
            }
            catch (Exception e)
            {
                Logger.Logwrite("Script thread Process Error: " + e.Message);
                MessageBox.Show("Script thread Process Error: " + e.Message);
            }
            finally
            {
                Refreshlogview();
            }            
            GameProcessLauncher();

        }
 
        /// <summary>
        /// launches the game itself. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void GameProcessLauncher()
        {
            string turnNum = _gameObjects[_selectedGame].GameTurnNum.ToString();
            string gameName = _gameObjects[_selectedGame].GameName;
            string gameType = _gameObjects[_selectedGame].GameType;
            string gameMod = _gameObjects[_selectedGame].GameMod;

            

            string savegamepath = _gamesSettings[gameType].GameMods[gameMod].ModSavePath;
            savegamepath = Path.GetDirectoryName(Interpreter.InterpretString(savegamepath, _gameObjects[_selectedGame], _gamesSettings[gameType]));

            string launchexe = _gamesSettings[gameType].GameExe;
            launchexe = Path.GetFullPath(Interpreter.InterpretString(launchexe, _gameObjects[_selectedGame], _gamesSettings[gameType]));
            
            string workingdir; // = Path.GetDirectoryName(launchexe);
            if (_gamesSettings[gameType].WorkingDirectory != null ||
                _gamesSettings[gameType].WorkingDirectory != "")
            {
                workingdir = Path.GetDirectoryName(_gamesSettings[gameType].WorkingDirectory);
            }
            else
                workingdir = Path.GetDirectoryName(launchexe);

            string launchargs = _gamesSettings[gameType].GameArguments;
            launchargs = Interpreter.InterpretString(launchargs, _gameObjects[_selectedGame], _gamesSettings[gameType]);

            Logger.Logwrite("launching game");
            Logger.Logwrite(launchexe + " " + launchargs );
            Logger.Logwrite(workingdir);

            toolStripStatusLabel_acty.Text = "Launching " + gameType;
            Refresh();

            Thread gamethread = new Thread(delegate()
            {
                try
                {
                    process_GameLauncher.StartInfo.FileName = launchexe;
                    process_GameLauncher.StartInfo.Arguments = launchargs;
                    process_GameLauncher.StartInfo.WorkingDirectory = workingdir;
                    process_GameLauncher.StartInfo.UseShellExecute = false;
                    process_GameLauncher.StartInfo.RedirectStandardError = true;
                    process_GameLauncher.StartInfo.RedirectStandardOutput = false;
                    process_GameLauncher.Start();
                

                    while ((!process_GameLauncher.StandardError.EndOfStream)) //&& (!process_GameLauncher.StandardOutput.EndOfStream))
                    {
                    
                        //string linestdout = process_GameLauncher.StandardOutput.ReadLine();
                        //logger.logwrite("StdOut: ", false);
                        //logger.logwrite(linestdout);
                        string linestderr = process_GameLauncher.StandardError.ReadLine();
                        Logger.Logwrite("StdErr: ", false);
                        Logger.Logwrite(linestderr);
                    }
                }
                catch (Exception e)
                {
                    Logger.Logwrite("Game thread Process Error: " + e.Message);
                    MessageBox.Show("Game thread Process Error: " + e.Message);
                }
            });
            try
            {
                gamethread.Start();
            }
            catch (Exception e)
            {
                Logger.Logwrite("Game Process Error: " + e.Message);
                MessageBox.Show("Game Process Error: " + e.Message);
            }
            finally
            { 
                Refreshlogview();
            }
           
        }

        /// <summary>
        /// opens a url in the default browser 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// sometimes this can cause problems aparently, hense all the try cches.
        /// http://www.devtoolshed.com/content/launch-url-default-browser-using-c
        /// </remarks>
        private void OpenLink(string sUrl)
        {
            try
            {
                Process.Start(sUrl);
            }
            catch (Exception exc1)
            {
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                if (exc1.GetType().ToString() != "System.ComponentModel.Win32Exception")
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo("IExplore.exe", sUrl);
                        Process.Start(startInfo);
                        startInfo = null;
                    }
                    catch (Exception exc2)
                    {
                        // still nothing we can do so just show the error to the user here.
                    }
                }
            }
        }

        private void PlaySimpleSound()
        {
            SoundPlayer simpleSound = new SoundPlayer("newturn.wav");
            simpleSound.Play();
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            PbwGamelist();
        }

    }

}

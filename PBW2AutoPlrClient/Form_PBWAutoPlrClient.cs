using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
//using System.Threading;

namespace PBW2AutoPlrClient
{
    public partial class Form_PBWAutoPlrClient : Form
    {

        Dictionary<string, GameObject> dictionaryofgameobjects = new Dictionary<string, GameObject>();
        List<string> gamenames = new List<string>();
        Dictionary<string, GameTypeSettingsObj> dictionaryofgamessettings = new Dictionary<string, GameTypeSettingsObj>();
        string selected_game;
        Timer timer1;        
        
        public Form_PBWAutoPlrClient()
        {
            ServerSettingsObj.loadSettings();
            InitializeComponent();
            gameSettingUpdate();

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 120000; // in miliseconds
            timer1.Start();

        }
              
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ServerSettingsObj.Connection_Status == "Connected")
            {
                pbwGamelist(); 
                refreshlogview();
                pbwServerData();
            }
        }

        private void toolStripButton_opensettings_Click(object sender, EventArgs e)
        {
            FormSettings settingsForm;
            settingsForm = new FormSettings();
            settingsForm.ShowDialog();
            gameSettingUpdate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pbwLogin();
            
        }

        private void dataGridView_games_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            selected_game = dataGridView_games[0, e.RowIndex].Value.ToString();
            button_download.Enabled = true;
            button_extract.Enabled = true;
            button_playGame.Enabled = true;
            button_upload.Enabled = true;
        }



        private void button_download_Click(object sender, EventArgs e)
        {
            if (selected_game != null)
            {
                download_gamefile();
            }  
        }

        private void button_upload_Click(object sender, EventArgs e)
        {
            if (selected_game != null)
            {
                upload_plrfile();
            }
        }
            
        private void button_extract_Click(object sender, EventArgs e)
        {
            if (selected_game != null)
            {
                extract_game();
            }
        }

        private void button_playGame_Click(object sender, EventArgs e)
        {
            if (selected_game != null)
            {
                string game_type = dictionaryofgameobjects[selected_game.ToString()].GameType;
                string pregamescript = (dictionaryofgamessettings[game_type].GamePreScript);
                if ((pregamescript != null) && (pregamescript != ""))
                {
                    pregameScriptProcessLauncher();
                }
                else
                {
                    gameProcessLauncher();
                }
            }
        }

        private void button_launchpbw_Click(object sender, EventArgs e)
        {
            OpenLink("http://" + ServerSettingsObj.PBW_Address);
        }

        /// <summary>
        /// deletes old downloaded files in the download dir.
        /// </summary>
        public void cleanup_download()
        {
            string download_dir = Path.GetDirectoryName(ServerSettingsObj.User_Download_Directory);
            string game_name = dictionaryofgameobjects[selected_game].GameName;

            string lastturnnum = (dictionaryofgameobjects[selected_game].GameTurnNum -1).ToString();
            string filetodel = Path.GetFileName(game_name + lastturnnum + ".rar");
            string fullpathtodel = Path.GetFullPath(Path.Combine(download_dir, filetodel));

            string[] files = Directory.GetFiles(download_dir);

            if (File.Exists(fullpathtodel))
            {
                logger.logwrite("deleteing file: " + fullpathtodel);
                File.Delete(fullpathtodel);
                
            }
        }

        public void refreshlogview()
        {
            richTextBox_log.Text += string.Join("\r\n", logger.logread().ToArray());
        }

        public void gameSettingUpdate()
        {
            this.Cursor = Cursors.WaitCursor;
            dictionaryofgamessettings = XmlHandler.GetGameSettings("gamesettings.ini");
            this.Cursor = Cursors.Default;
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
        private void pbwLogin()
        {

            string address = "https://" + ServerSettingsObj.PBW_Address;

            string login_path = ServerSettingsObj.PBW_LoginPath;
            string login = ServerSettingsObj.User_UserName;
            string pass = ServerSettingsObj.User_Password;
            string login_address = address + login_path;
            toolStripStatusLabel_acty.Text = "Attempting PBW Login via https";
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;
            bool connectsuccess = PBW_ComsHandler.connectPBW(login_address, login, pass);
            toolStripStatusLabel_acty.Text = "";
            if (connectsuccess) 
            { 
                toolStripStatusLabel_connectionstate.Text = "Connected";
                ServerSettingsObj.Connection_Status = "Connected"; 
                pbwGamelist();
                pbwServerData();

            }
            else { toolStripStatusLabel_connectionstate.Text = "Disconnected"; }
            this.Cursor = Cursors.Default;
            refreshlogview();
        }

        /// <summary>
        /// Gets serverdata xml from PBW via the comms handler
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// calls xmlServerData to interpret the xmldata. 
        /// </remarks>
        private void pbwServerData()
        {

            string address = "http://" + ServerSettingsObj.PBW_Address;
            string node_path = "node/player";
            string url = address + node_path;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            this.Cursor = Cursors.WaitCursor;

            string xmldata = PBW_ComsHandler.get_PbwXmlData(cookies, url);

            if (xmldata == null) //probibly throw an exception from PBW_ComsHandler would be better.
            {
                pbwLogin();
                xmldata = PBW_ComsHandler.get_PbwXmlData(cookies, url);
                if (xmldata == null) //still? login fail. redo this when give pbwLogin a return.
                {
                    MessageBox.Show("unable to get ServerData from PBW.");
                }
            }
            else
            {
                xmlServerData(xmldata);
            }
            toolStripStatusLabel_acty.Text = "";
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Gets gamelist xmldata from PBW viathe coms handler
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// calls  XmlHandler.xmlToGameObj to interpret the xmldata and build/update a dictionary of gameobjects
        /// </remarks>
        private void pbwGamelist()
        {

            string address = "https://" + ServerSettingsObj.PBW_Address;
            string gamelist_path = ServerSettingsObj.PBW_GamesListPath;
            string gamelisturl = address + gamelist_path;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            this.Cursor = Cursors.WaitCursor;

            string xmlgamelist = PBW_ComsHandler.get_PbwXmlData(cookies, gamelisturl);
            if (xmlgamelist == null)
            {
                pbwLogin();
                xmlgamelist = PBW_ComsHandler.get_PbwXmlData(cookies, gamelisturl);
                if (xmlgamelist == null) //still? login fail. redo this when give pbwLogin a return.
                {
                    MessageBox.Show("unable to get games list from PBW.");
                }
            }
            else
            {

                dictionaryofgameobjects = XmlHandler.xmlToGameObj(xmlgamelist, dictionaryofgameobjects);
                refreshGamesList();
            }
            toolStripStatusLabel_acty.Text = "";
            this.Cursor = Cursors.Default; 

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
        private void xmlServerData(string xmlstring)

        {

            Dictionary<string,string> serversettings = new Dictionary<string,string>();
            serversettings = XmlHandler.ServerSettings(xmlstring);

            ServerSettingsObj.Refresh_Rate = int.Parse(serversettings["update_interval"]);
            if (ServerSettingsObj.Refresh_Rate != 0)
            { timer1.Interval = ServerSettingsObj.Refresh_Rate * 1000; }
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
        private void refreshGamesList()
        {
            int selected_gameIndex = 0;
            List<GameObject> listofgames = new List<GameObject>();
            foreach (GameObject game in dictionaryofgameobjects.Values)
            {
                listofgames.Add(game);
                if (game.GameName == selected_game) // if the game name == selected_game ( a class string) 
                {
                    selected_gameIndex = listofgames.Count - 1; //then we know the index of the row that needs to be selected.
                }
                if (game.GamePlrStatus == "waiting")
                {
                    playSimpleSound();
                }
            }

            dataGridView_games.DataSource = listofgames;
            dataGridView_games.Columns["GameName"].DisplayIndex = 0;
            dataGridView_games.Columns["GamePlrStatus"].DisplayIndex = 1;
            dataGridView_games.Columns["GameNextTurn"].DisplayIndex = 2;
            dataGridView_games.Columns["GamePlrEmpPassword"].Visible = false;
                      
            dataGridView_games.Rows[selected_gameIndex].Selected = true;
            dataGridView_games.CurrentCell = dataGridView_games.Rows[selected_gameIndex].Cells[1];
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

            
            string turn_num = dictionaryofgameobjects[selected_game].GameTurnNum.ToString();
            string game_name = dictionaryofgameobjects[selected_game].GameName;
            string download_dir = Path.GetDirectoryName(ServerSettingsObj.User_Download_Directory);
            string pbwaddress = "http://" + ServerSettingsObj.PBW_Address;
            string turn_download_URI = ServerSettingsObj.PBW_TurnDownloadPath;
            string game_path_URI = ServerSettingsObj.PBW_GamePath;
            
            string fulldownloadURI = pbwaddress + game_path_URI + game_name + "/" + turn_download_URI;
            
            string downloadfilename = Path.GetFullPath(Path.Combine(download_dir, Path.GetFileName(game_name + turn_num + ".rar")));
            
            if (!File.Exists(download_dir))
            {
                Directory.CreateDirectory(download_dir);
            }
            toolStripStatusLabel_acty.Text = "Attempting Gamefile Download";
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            PBW_ComsHandler.downloadGame(cookies, fulldownloadURI, downloadfilename);
            toolStripStatusLabel_acty.Text = "Done?";
            this.Refresh();
            this.Cursor = Cursors.Default;
            cleanup_download();
            refreshlogview();
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

            string game_type = dictionaryofgameobjects[selected_game.ToString()].GameType;
            string game_mod = dictionaryofgameobjects[selected_game.ToString()].GameMod;
            string game_name = dictionaryofgameobjects[selected_game.ToString()].GameName;
            string savegamepath = dictionaryofgamessettings[game_type].GameMods[game_mod].ModSavePath;
            string upfilemask = dictionaryofgamessettings[game_type].GameUploadFileMask;
            string address = "http://" + ServerSettingsObj.PBW_Address;
            string uploadpath = ServerSettingsObj.PBW_UploadPlrFilePath;
            string gamepath = ServerSettingsObj.PBW_GamePath;
            string uploadurl = address + gamepath + game_name + "/" + uploadpath;
            string uploadformParam = ServerSettingsObj.PBW_UploadTurnFormParam;

            upfilemask = Interpreter.interpretString(upfilemask, dictionaryofgameobjects[selected_game.ToString()], dictionaryofgamessettings[game_type]);
            string upfile = savegamepath + upfilemask;
            CookieContainer cookies = ServerSettingsObj.CookieJar;

            toolStripStatusLabel_acty.Text = "Attempting Playerfile upload";
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;

            var response = PBW_ComsHandler.uploadTurn(cookies, upfile, uploadurl, uploadformParam);

            if (response)
            {
 
                toolStripStatusLabel_acty.Text = "Done!";
                this.Refresh();
                this.Cursor = Cursors.Default;
            }
            else
            {
                //MessageBox.Show("Turn could not be uploaded.");
                toolStripStatusLabel_acty.Text = "Failed...";
                this.Refresh();
                this.Cursor = Cursors.Default;
            }

            pbwGamelist();
            refreshlogview();
            pbwServerData();
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

            string turn_num = dictionaryofgameobjects[selected_game.ToString()].GameTurnNum.ToString();
            string game_name = dictionaryofgameobjects[selected_game.ToString()].GameName;
            string game_type = dictionaryofgameobjects[selected_game.ToString()].GameType;
            string game_mod = dictionaryofgameobjects[selected_game.ToString()].GameMod;

            string download_dir = Path.GetDirectoryName(ServerSettingsObj.User_Download_Directory);
            string downloadfilename = Path.GetFullPath(Path.Combine(download_dir, Path.GetFileName(game_name + turn_num + ".rar")));

            string savegamepath = dictionaryofgamessettings[game_type].GameMods[game_mod].ModSavePath;
            savegamepath = Path.GetDirectoryName(Interpreter.interpretString(savegamepath, dictionaryofgameobjects[selected_game.ToString()], dictionaryofgamessettings[game_type]));
            int arc = ArchiveHandler.setLibPath();

            toolStripStatusLabel_acty.Text = "Extracting game files";
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;

            arc = ArchiveHandler.extractArchive(downloadfilename, savegamepath);

            toolStripStatusLabel_acty.Text = "Extracting Done";
            this.Refresh();
            this.Cursor = Cursors.Default;
        }
 
        /// <summary>
        /// launches pregame scripts, such as se4 windowed mode. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void pregameScriptProcessLauncher(bool closeaftergame = true)
        {


            string game_type = dictionaryofgameobjects[selected_game.ToString()].GameType;
            string scriptname = Path.GetFullPath(dictionaryofgamessettings[game_type].GamePreScript);

            System.Threading.Thread prescriptthread = new System.Threading.Thread(delegate()
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
                    logger.logwrite("Script thread Process Error: " + e.Message);
                    MessageBox.Show("Script thread Process Error: " + e.Message);
                }
                while ((!process_preGameProcess.StandardError.EndOfStream))// || (!process_GameLauncher.StandardOutput.EndOfStream))
                {
                    string line = process_preGameProcess.StandardError.ReadLine();
                    logger.logwrite("StdErr: ", false);
                    logger.logwrite(line);
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
                logger.logwrite("Script thread Process Error: " + e.Message);
                MessageBox.Show("Script thread Process Error: " + e.Message);
            }
            finally
            {
                refreshlogview();
            }            
            gameProcessLauncher();

        }
 
        /// <summary>
        /// launches the game itself. 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        private void gameProcessLauncher()
        {
            string turn_num = dictionaryofgameobjects[selected_game.ToString()].GameTurnNum.ToString();
            string game_name = dictionaryofgameobjects[selected_game.ToString()].GameName;
            string game_type = dictionaryofgameobjects[selected_game.ToString()].GameType;
            string game_mod = dictionaryofgameobjects[selected_game.ToString()].GameMod;

            

            string savegamepath = dictionaryofgamessettings[game_type].GameMods[game_mod].ModSavePath;
            savegamepath = Path.GetDirectoryName(Interpreter.interpretString(savegamepath, dictionaryofgameobjects[selected_game.ToString()], dictionaryofgamessettings[game_type]));

            string launchexe = dictionaryofgamessettings[game_type].GameExe;
            launchexe = Path.GetFullPath(Interpreter.interpretString(launchexe, dictionaryofgameobjects[selected_game.ToString()], dictionaryofgamessettings[game_type]));
            string workingdir = Path.GetDirectoryName(launchexe);
            string launchargs = dictionaryofgamessettings[game_type].GameArguments;
            launchargs = Interpreter.interpretString(launchargs, dictionaryofgameobjects[selected_game.ToString()], dictionaryofgamessettings[game_type]);

            logger.logwrite("launching game");
            logger.logwrite(launchexe + " " + launchargs );
            logger.logwrite(workingdir);

            toolStripStatusLabel_acty.Text = "Launching " + game_type;
            this.Refresh();

            System.Threading.Thread gamethread = new System.Threading.Thread(delegate()
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
                        logger.logwrite("StdErr: ", false);
                        logger.logwrite(linestderr);
                    }
                }
                catch (Exception e)
                {
                    logger.logwrite("Game thread Process Error: " + e.Message);
                    MessageBox.Show("Game thread Process Error: " + e.Message);
                }
            });
            try
            {
                gamethread.Start();
            }
            catch (Exception e)
            {
                logger.logwrite("Game Process Error: " + e.Message);
                MessageBox.Show("Game Process Error: " + e.Message);
            }
            finally
            { 
                refreshlogview();
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
                System.Diagnostics.Process.Start(sUrl);
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
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", sUrl);
                        System.Diagnostics.Process.Start(startInfo);
                        startInfo = null;
                    }
                    catch (Exception exc2)
                    {
                        // still nothing we can do so just show the error to the user here.
                    }
                }
            }
        }

        private void playSimpleSound()
        {
            System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(@"newturn.wav");
            simpleSound.Play();
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            pbwGamelist();
        }

    }

}

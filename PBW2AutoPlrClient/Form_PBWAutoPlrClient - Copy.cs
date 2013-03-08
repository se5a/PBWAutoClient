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

namespace PBW2AutoPlrClient
{
    public partial class Form_PBWAutoPlrClient : Form
    {
        List<GameObject> listofgameobjects = new List<GameObject>();
        //Dictionary<string, GameObject> dictionaryofgameobjects = new Dictionary<string, GameObject>();
        List<string> gamenames = new List<string>();
        Dictionary<string, GameTypeSettingsObj> dicofgamessettings = new Dictionary<string, GameTypeSettingsObj>();
        int selected_game;
        Timer timer1;        
        
        public Form_PBWAutoPlrClient()
        {
            ServerSettingsObj.loadSettings();
            InitializeComponent();
            gameSettingUpdate();

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 300000; // in miliseconds
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ServerSettingsObj.Connection_Status == "Connected")
            {
                //pbwGamelist(); currently this just keeps adding gameobjects using up memory. need to fix.
                showlog();
                pbwServerData();
            }
        }

        public void showlog()
        {
            richTextBox_log.Text += string.Join("\r\n", logger.logread().ToArray());
        }

        public void gameSettingUpdate()
        {

            dicofgamessettings = XmlHandler.GetGameSettings("gamesettings.ini");
        }


        private void toolStripButton_opensettings_Click(object sender, EventArgs e)
        {
            FormSettings settingsForm;
            settingsForm = new FormSettings();
            settingsForm.Show();
            gameSettingUpdate();
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pbwLogin();
            
        }

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
           
 
        }

        private void pbwServerData()
        {

            string address = "http://" + ServerSettingsObj.PBW_Address;
            string node_path = "node";
            string url = address + node_path;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            toolStripStatusLabel_acty.Text = "Attempting to get ServerData";
            this.Refresh();
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

        private void pbwGamelist()
        {

            string address = "http://" + ServerSettingsObj.PBW_Address;
            string gamelist_path = ServerSettingsObj.PBW_GamesListPath;
            string gamelisturl = address + gamelist_path;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            toolStripStatusLabel_acty.Text = "Attempting to get Games List";
            this.Refresh();
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
                xmlToGameObj(xmlgamelist);
            }
            toolStripStatusLabel_acty.Text = "";
            this.Cursor = Cursors.Default; 

        }
        


        private void xmlToGameObj(string xmlstring)
        {
            /// <summary>
            /// turns game xml data from pbw, cuts it into seperate games and creates a gamesObject.
            /// </summary>
            /// <param name="xmlstring">xmlstring from PBW</param>
            /// <returns>
            /// </returns>
            /// <remarks>
            /// 
            /// </remarks>
            string[] lines = xmlstring.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);       
            List<List<string>> listofgames = new List<List<string>>();
            int boxindex = 0;
            bool data_is_game = false;
            foreach (string line in lines)
            {   
                //List<string> thisgame = new List<string>();
                string line_trim = line.Trim();

                if (line_trim == @"</game>")
                {
                    data_is_game = false;
                    GameObject thisgameobj = new GameObject(listofgames[boxindex]);
                    //if thisgameobj.GameName does not exsist in listofgameobjects... should listofgameobjects be a dictionary maybe?
                    listofgameobjects.Add(thisgameobj);
                    boxindex += 1;
                }
                if (data_is_game)
                {
                    listofgames[boxindex].Add(line_trim);
                }
                
                if (line_trim == "<game>")
                {
                    data_is_game = true;
                    listofgames.Add(new List<string>());
                }
            }
            showGames();
        }

        private void xmlServerData(string xmlstring)
        {
            /// <summary>
            /// takes xml serverdata from pbw, and updates the serversettingsobj .
            /// </summary>
            /// <param name="xmlstring">xmlstring from PBW</param>
            /// <returns>
            /// </returns>
            /// <remarks>
            /// 
            /// </remarks>
            Dictionary<string,string> serversettings = new Dictionary<string,string>();
            serversettings = XmlHandler.ServerSettings(xmlstring);

            ServerSettingsObj.Refresh_Rate = int.Parse(serversettings["update_interval"]);
            if (ServerSettingsObj.Refresh_Rate != 0)
            { timer1.Interval = ServerSettingsObj.Refresh_Rate * 1000; }
            //also get server time and max file size?
            
        }

        private void showGames()
        {
            gamenames = new List<string>();
            foreach (GameObject game in listofgameobjects)
            {
                gamenames.Add(game.GameName);
            }
            //listBox_Games.DataSource = gamenames;
            dataGridView_games.DataSource = listofgameobjects;
            dataGridView_games.Columns["GameName"].DisplayIndex = 0;
            dataGridView_games.Columns["GamePlrStatus"].DisplayIndex = 1;
            dataGridView_games.Columns["GameNextTurn"].DisplayIndex = 2;
            dataGridView_games.Columns["GamePlrEmpPassword"].Visible = false;
            dataGridView_games.Refresh();
        }

        private void dataGridView_games_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            selected_game = e.RowIndex;
        }

        private void button_download_Click(object sender, EventArgs e)
        {
            richTextBox_log.Text += "download clicked for game " + listofgameobjects[selected_game].GameName;
            download_gamefile();
            
                
        }

        private void download_gamefile()
        {

            string address = "http://" + ServerSettingsObj.PBW_Address;
            string turn_download_path = ServerSettingsObj.PBW_TurnDownloadPath;
            string game_path = ServerSettingsObj.PBW_GamePath;
            string turn_num = listofgameobjects[selected_game].GameTurnNum.ToString();
            string game_name = listofgameobjects[selected_game].GameName;
            string download_dir = ServerSettingsObj.User_Downlod_Directory;
            string fulldownloadpath = address + game_path + game_name + "/" + turn_download_path;
            string downloadfilename = download_dir + game_name + turn_num + ".rar";

            toolStripStatusLabel_acty.Text = "Attempting Gamefile Download";
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            PBW_ComsHandler.downloadGame(cookies, fulldownloadpath, downloadfilename);
            toolStripStatusLabel_acty.Text = "Done?";
            this.Refresh();
            this.Cursor = Cursors.Default;
        }


        private void button_upload_Click(object sender, EventArgs e)
        {

            string game_type = listofgameobjects[selected_game].GameType;
            string game_mod = listofgameobjects[selected_game].GameMod;
            string game_name = listofgameobjects[selected_game].GameName;

            string address = "http://" + ServerSettingsObj.PBW_Address;
            string uploadpath = ServerSettingsObj.PBW_UploadPlrFilePath;
            string gamepath = ServerSettingsObj.PBW_GamePath;
            
            string uploadurl = address + gamepath + game_name + "/" + uploadpath;
            string uploadformParam = ServerSettingsObj.PBW_UploadTurnFormParam;

            toolStripStatusLabel_acty.Text = "Attempting Playerfile upload";
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;
            
            
            string savegamepath = dicofgamessettings[game_type].GameMods[game_mod].ModSavePath;
            string upfilemask = dicofgamessettings[game_type].GameUploadFileMask;
            upfilemask = Interpreter.interpretString(upfilemask, listofgameobjects[selected_game], dicofgamessettings[game_type]);
            string upfile = savegamepath + upfilemask;

            CookieContainer cookies = ServerSettingsObj.CookieJar;

            var response = PBW_ComsHandler.uploadTurn(cookies, upfile, uploadurl, uploadformParam);

            if (response)
            {
                MessageBox.Show("Turn uploaded successfully!");
                toolStripStatusLabel_acty.Text = "Done!";
                this.Refresh();
                this.Cursor = Cursors.Default;
            }
            else
            {
                MessageBox.Show("Turn could not be uploaded.");
                toolStripStatusLabel_acty.Text = "Done!";
                this.Refresh();
                this.Cursor = Cursors.Default;
            }
        }
        
       

        private void button_extract_Click(object sender, EventArgs e)
        {
            string turn_num = listofgameobjects[selected_game].GameTurnNum.ToString();
            string game_name = listofgameobjects[selected_game].GameName;
            string game_type = listofgameobjects[selected_game].GameType;
            string game_mod = listofgameobjects[selected_game].GameMod;
            string download_dir = ServerSettingsObj.User_Downlod_Directory;
            string downloadfilename = download_dir + game_name + turn_num + ".rar";

            string savegamepath = dicofgamessettings[game_type].GameMods[game_mod].ModSavePath;
            savegamepath = Interpreter.interpretString(savegamepath, listofgameobjects[selected_game], dicofgamessettings[game_type]);
            int arc = ArchiveHandler.setLibPath();
            arc = ArchiveHandler.extractArchive(downloadfilename, savegamepath);
            launchGame();
        }

        private void launchGame()
        {
            string turn_num = listofgameobjects[selected_game].GameTurnNum.ToString();
            string game_name = listofgameobjects[selected_game].GameName;
            string game_type = listofgameobjects[selected_game].GameType;
            string game_mod = listofgameobjects[selected_game].GameMod;
            string downloadfilename = game_name + turn_num + ".rar";

            string savegamepath = dicofgamessettings[game_type].GameMods[game_mod].ModSavePath;
            savegamepath = Interpreter.interpretString(savegamepath, listofgameobjects[selected_game], dicofgamessettings[game_type]);

            string launchexe = dicofgamessettings[game_type].GameExe;
            launchexe = Interpreter.interpretString(launchexe, listofgameobjects[selected_game], dicofgamessettings[game_type]);
            string launchargs = dicofgamessettings[game_type].GameArguments;
            launchargs = Interpreter.interpretString(launchargs, listofgameobjects[selected_game], dicofgamessettings[game_type]);
            
            //richTextBox_log.Text += dicofgamessettings[game_type].GameWorkingDirectory;
            //MessageBox.Show(dicofgamessettings[game_type].GameWorkingDirectory);

            var thisproc = new Process //I do not understand this syntax here at ALL
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = dicofgamessettings[game_type].GameWorkingDirectory, 
                    FileName = launchexe, 
                    Arguments = launchargs,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = true

                    
                }
                
            };
            try
            {
                thisproc.Start();
                while (!thisproc.StandardError.EndOfStream)
                {
                    string line = thisproc.StandardError.ReadLine();
                    richTextBox_log.Text += line + "\r\n";
                    //string line2 = thisproc.StandardOutput.ReadLine();
                    //richTextBox_log.Text += line + "\r\n";
                }
            }
            catch (Exception e)
            {
                richTextBox_log.Text += dicofgamessettings[game_type].GameWorkingDirectory + "\r\n";
                richTextBox_log.Text += launchexe + "\r\n";
                richTextBox_log.Text += launchargs + "\r\n";
                MessageBox.Show(e.Message.ToString() + "when attempting to launch game" );

            }

        }
    }
}

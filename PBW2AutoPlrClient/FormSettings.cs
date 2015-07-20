using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace PBW2AutoPlrClient
{
    public partial class FormSettings : Form
    {
        
        public FormSettings()
        {   

            InitializeComponent();
            
            FormSettings_Load();
            PopulateTreeview("gamesettings.ini");
            treeView_gmestn.ExpandAll();
            
        }


        private void FormSettings_Load()
        {
            //var settings = Settings.Default;
            textBox_pbwaddress.Text = ServerSettingsObj.PbwAddress;
            textBox_pbwgamelistpath.Text = ServerSettingsObj.PbwGamesListPath;
            textBox_pbwloginpath.Text = ServerSettingsObj.PbwLoginPath;
            textBox_pbwplayeruploadformparameter.Text = ServerSettingsObj.PbwUploadTurnFormParam;
            textBox_pbwplayeruploadpath.Text = ServerSettingsObj.PbwUploadPlrFilePath;
            textBox_pbwturndownloadpath.Text = ServerSettingsObj.PbwTurnDownloadPath;
            textBox_pbwgamepath.Text = ServerSettingsObj.PbwGamePath;

            checkBox_user_savelogin.Checked = ServerSettingsObj.UserSaveLogin;
            textBox_userlogin.Text = ServerSettingsObj.UserUserName;
            textBox_userpassword.PasswordChar = '*';
            textBox_userpassword.Text = ServerSettingsObj.UserPassword;
            textBox_user_dl_dir.Text = ServerSettingsObj.UserDownloadDirectory;

        }   

        private void buttonSave_Click(object sender, EventArgs e)
        {

            ServerSettingsObj.PbwAddress = textBox_pbwaddress.Text;
            ServerSettingsObj.PbwLoginPath = textBox_pbwloginpath.Text;
            ServerSettingsObj.PbwTurnDownloadPath = textBox_pbwturndownloadpath.Text;
            ServerSettingsObj.PbwGamesListPath = textBox_pbwgamelistpath.Text;
            ServerSettingsObj.PbwUploadPlrFilePath = textBox_pbwplayeruploadpath.Text;
            ServerSettingsObj.PbwUploadTurnFormParam = textBox_pbwplayeruploadformparameter.Text;
            ServerSettingsObj.PbwGamePath = textBox_pbwgamepath.Text;

            ServerSettingsObj.UserSaveLogin = checkBox_user_savelogin.Checked;

            ServerSettingsObj.UserUserName = textBox_userlogin.Text;
            ServerSettingsObj.UserPassword = textBox_userpassword.Text;
            ServerSettingsObj.SaveSettings();
            ServerSettingsObj.UserDownloadDirectory = textBox_user_dl_dir.Text;
            Close();        
        }

        private void tabPage_serversettings_Click(object sender, EventArgs e)
        {

        }

        private void treeView_GameSettings_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true; 
        }

        private void treeView_gmestn_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            textBox_gmestn_nodedata.Text = e.Node.Text;
            treeView_gmestn.SelectedNode = e.Node;
        }

        private void PopulateTreeview(string filename)
        {
            
            try
            {
                //Just a good practice -- change the cursor to a 
                //wait cursor while the nodes populate
                Cursor = Cursors.WaitCursor;
                //First, we'll load the Xml document
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filename);
                //Now, clear out the treeview, 
                //and add the first (root) node
                treeView_gmestn.Nodes.Clear();
                treeView_gmestn.Nodes.Add(new
                    TreeNode(xDoc.DocumentElement.Name));
                TreeNode tNode = new TreeNode();
                tNode = treeView_gmestn.Nodes[0];
                //We make a call to addTreeNode, 
                //where we'll add all of our nodes
                AddTreeNode(xDoc.DocumentElement, tNode);
                //Expand the treeview to show all nodes
                treeView_gmestn.ExpandAll();
            }
            catch (XmlException xExc)
            //Exception is thrown is there is an error in the Xml
            {
                MessageBox.Show(xExc.Message);
            }
            catch (Exception ex) //General exception
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default; //Change the cursor back
            }
            
        }
        //This function is called recursively until all nodes are loaded
        private void AddTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;
            if (xmlNode.HasChildNodes) //The current node has children
            {
                xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x <= xNodeList.Count - 1; x++)
                //Loop through the child nodes
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = treeNode.Nodes[x];
                    AddTreeNode(xNode, tNode);
                }
            }
            else //No children, so add the outer xml (trimming off whitespace)
                treeNode.Text = xmlNode.OuterXml.Trim();
        }
        //We use this in the export and the saveNode 
        //functions, though it's only instantiated once.
        private StreamWriter _sr;
        public void ExportToXml(TreeView tv, string filename) 
        {
            _sr = new StreamWriter(filename, false, Encoding.UTF8);
            //Write the header
            _sr.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            //Write our root node
            _sr.WriteLine("<" + tv.Nodes[0].Text + ">");
            foreach (TreeNode node in tv.Nodes)
            {
                SaveNode(node.Nodes);
            }
            //Close the root node
            _sr.WriteLine("</" + tv.Nodes[0].Text + ">");
            _sr.Close();
        }
        private void SaveNode(TreeNodeCollection tnc)
        {

            foreach (TreeNode node in tnc)
            {
                //If we have child nodes, we'll write 
                //a parent node, then iterrate through
                //the children
                int numtabs = node.Level;
                string tabs = new string(' ', numtabs * 4);
                _sr.Write(tabs);
                if (node.Nodes.Count > 0)
                {
                    _sr.WriteLine("<" + node.Text + ">");
                    SaveNode(node.Nodes);
                    _sr.Write(tabs);
                    _sr.WriteLine("</" + node.Text + ">");
                } 
                else //No child nodes, so we just write the text

                    _sr.WriteLine(node.Text);
            }
        }
        private void button_gmestn_save_Click(object sender, EventArgs e)
        {
            ExportToXml(treeView_gmestn, "gamesettings.ini");
            Close();
        }

        private void textBox_gmestn_nodedata_TextChanged(object sender, EventArgs e)
        {
            if (treeView_gmestn.SelectedNode != null && textBox_gmestn_nodedata.Focused)
            {
                treeView_gmestn.SelectedNode.Text = textBox_gmestn_nodedata.Text;
            }
        }

        private void textBox_gmestn_nodedata_Leave(object sender, EventArgs e)
        {

        }

        private void textBox_gmestn_nodedata_Enter(object sender, EventArgs e)
        {

        }

        private void button_gmestn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

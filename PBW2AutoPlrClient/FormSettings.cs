using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using PBW2AutoPlrClient.Properties;



namespace PBW2AutoPlrClient
{
    public partial class FormSettings : Form
    {
        
        public FormSettings()
        {   

            InitializeComponent();
            
            FormSettings_Load();
            populateTreeview("gamesettings.ini");
            treeView_gmestn.ExpandAll();
            
        }


        private void FormSettings_Load()
        {
            //var settings = Settings.Default;
            textBox_pbwaddress.Text = ServerSettingsObj.PBW_Address;
            textBox_pbwgamelistpath.Text = ServerSettingsObj.PBW_GamesListPath;
            textBox_pbwloginpath.Text = ServerSettingsObj.PBW_LoginPath;
            textBox_pbwplayeruploadformparameter.Text = ServerSettingsObj.PBW_UploadTurnFormParam;
            textBox_pbwplayeruploadpath.Text = ServerSettingsObj.PBW_UploadPlrFilePath;
            textBox_pbwturndownloadpath.Text = ServerSettingsObj.PBW_TurnDownloadPath;
            textBox_pbwgamepath.Text = ServerSettingsObj.PBW_GamePath;

            checkBox_user_savelogin.Checked = ServerSettingsObj.User_SaveLogin;
            textBox_userlogin.Text = ServerSettingsObj.User_UserName;
            textBox_userpassword.PasswordChar = '*';
            textBox_userpassword.Text = ServerSettingsObj.User_Password;
            textBox_user_dl_dir.Text = ServerSettingsObj.User_Download_Directory;

        }   

        private void buttonSave_Click(object sender, EventArgs e)
        {

            ServerSettingsObj.PBW_Address = textBox_pbwaddress.Text;
            ServerSettingsObj.PBW_LoginPath = textBox_pbwloginpath.Text;
            ServerSettingsObj.PBW_TurnDownloadPath = textBox_pbwturndownloadpath.Text;
            ServerSettingsObj.PBW_GamesListPath = textBox_pbwgamelistpath.Text;
            ServerSettingsObj.PBW_UploadPlrFilePath = textBox_pbwplayeruploadpath.Text;
            ServerSettingsObj.PBW_UploadTurnFormParam = textBox_pbwplayeruploadformparameter.Text;
            ServerSettingsObj.PBW_GamePath = textBox_pbwgamepath.Text;

            ServerSettingsObj.User_SaveLogin = checkBox_user_savelogin.Checked;

            ServerSettingsObj.User_UserName = textBox_userlogin.Text;
            ServerSettingsObj.User_Password = textBox_userpassword.Text;
            ServerSettingsObj.saveSettings();
            ServerSettingsObj.User_Download_Directory = textBox_user_dl_dir.Text;
            this.Close();        
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

        private void populateTreeview(string filename)
        {
            
            try
            {
                //Just a good practice -- change the cursor to a 
                //wait cursor while the nodes populate
                this.Cursor = Cursors.WaitCursor;
                //First, we'll load the Xml document
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filename);
                //Now, clear out the treeview, 
                //and add the first (root) node
                treeView_gmestn.Nodes.Clear();
                treeView_gmestn.Nodes.Add(new
                    TreeNode(xDoc.DocumentElement.Name));
                TreeNode tNode = new TreeNode();
                tNode = (TreeNode)treeView_gmestn.Nodes[0];
                //We make a call to addTreeNode, 
                //where we'll add all of our nodes
                addTreeNode(xDoc.DocumentElement, tNode);
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
                this.Cursor = Cursors.Default; //Change the cursor back
            }
            
        }
        //This function is called recursively until all nodes are loaded
        private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
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
                    addTreeNode(xNode, tNode);
                }
            }
            else //No children, so add the outer xml (trimming off whitespace)
                treeNode.Text = xmlNode.OuterXml.Trim();
        }
        //We use this in the export and the saveNode 
        //functions, though it's only instantiated once.
        private StreamWriter sr;
        public void exportToXml(TreeView tv, string filename) 
        {
            sr = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
            //Write the header
            sr.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            //Write our root node
            sr.WriteLine("<" + tv.Nodes[0].Text + ">");
            foreach (TreeNode node in tv.Nodes)
            {
                saveNode(node.Nodes);
            }
            //Close the root node
            sr.WriteLine("</" + tv.Nodes[0].Text + ">");
            sr.Close();
        }
        private void saveNode(TreeNodeCollection tnc)
        {

            foreach (TreeNode node in tnc)
            {
                //If we have child nodes, we'll write 
                //a parent node, then iterrate through
                //the children
                int numtabs = node.Level;
                string tabs = new string(' ', numtabs * 4);
                sr.Write(tabs);
                if (node.Nodes.Count > 0)
                {
                    sr.WriteLine("<" + node.Text + ">");
                    saveNode(node.Nodes);
                    sr.Write(tabs);
                    sr.WriteLine("</" + node.Text + ">");
                } 
                else //No child nodes, so we just write the text

                    sr.WriteLine(node.Text);
            }
        }
        private void button_gmestn_save_Click(object sender, EventArgs e)
        {
            exportToXml(treeView_gmestn, "gamesettings.ini");
            this.Close();
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
            this.Close();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

    }
}

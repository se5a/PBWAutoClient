namespace PBW2AutoPlrClient
{
    partial class Form_PBWAutoPlrClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_PBWAutoPlrClient));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_connectionstate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_acty = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_connect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_opensettings = new System.Windows.Forms.ToolStripButton();
            this.richTextBox_log = new System.Windows.Forms.RichTextBox();
            this.dataGridView_games = new System.Windows.Forms.DataGridView();
            this.button_extract = new System.Windows.Forms.Button();
            this.button_upload = new System.Windows.Forms.Button();
            this.button_download = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_playGame = new System.Windows.Forms.Button();
            this.button_launchpbw = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.process_GameLauncher = new System.Diagnostics.Process();
            this.process_preGameProcess = new System.Diagnostics.Process();
            this.button_refresh = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_games)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_connectionstate,
            this.toolStripStatusLabel_acty});
            this.statusStrip1.Location = new System.Drawing.Point(0, 366);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(570, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_connectionstate
            // 
            this.toolStripStatusLabel_connectionstate.Name = "toolStripStatusLabel_connectionstate";
            this.toolStripStatusLabel_connectionstate.Size = new System.Drawing.Size(71, 17);
            this.toolStripStatusLabel_connectionstate.Text = "Disconnected";
            // 
            // toolStripStatusLabel_acty
            // 
            this.toolStripStatusLabel_acty.Name = "toolStripStatusLabel_acty";
            this.toolStripStatusLabel_acty.Size = new System.Drawing.Size(41, 17);
            this.toolStripStatusLabel_acty.Text = "waiting";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_connect,
            this.toolStripButton_opensettings});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(570, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_connect
            // 
            this.toolStripButton_connect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_connect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_connect.Image")));
            this.toolStripButton_connect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_connect.Name = "toolStripButton_connect";
            this.toolStripButton_connect.Size = new System.Drawing.Size(91, 22);
            this.toolStripButton_connect.Text = "ConnectToPBW2";
            this.toolStripButton_connect.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton_opensettings
            // 
            this.toolStripButton_opensettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_opensettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_opensettings.Image")));
            this.toolStripButton_opensettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_opensettings.Name = "toolStripButton_opensettings";
            this.toolStripButton_opensettings.Size = new System.Drawing.Size(50, 22);
            this.toolStripButton_opensettings.Text = "Settings";
            this.toolStripButton_opensettings.Click += new System.EventHandler(this.toolStripButton_opensettings_Click);
            // 
            // richTextBox_log
            // 
            this.richTextBox_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_log.Location = new System.Drawing.Point(3, 3);
            this.richTextBox_log.Name = "richTextBox_log";
            this.richTextBox_log.Size = new System.Drawing.Size(556, 309);
            this.richTextBox_log.TabIndex = 3;
            this.richTextBox_log.Text = "";
            // 
            // dataGridView_games
            // 
            this.dataGridView_games.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_games.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_games.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView_games, 6);
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_games.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_games.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_games.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_games.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_games.MultiSelect = false;
            this.dataGridView_games.Name = "dataGridView_games";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_games.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_games.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_games.Size = new System.Drawing.Size(556, 276);
            this.dataGridView_games.TabIndex = 1;
            this.dataGridView_games.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_games_RowEnter);
            // 
            // button_extract
            // 
            this.button_extract.Enabled = false;
            this.button_extract.Location = new System.Drawing.Point(84, 285);
            this.button_extract.Name = "button_extract";
            this.button_extract.Size = new System.Drawing.Size(75, 21);
            this.button_extract.TabIndex = 2;
            this.button_extract.Text = "Extract";
            this.button_extract.UseVisualStyleBackColor = true;
            this.button_extract.Click += new System.EventHandler(this.button_extract_Click);
            // 
            // button_upload
            // 
            this.button_upload.Enabled = false;
            this.button_upload.Location = new System.Drawing.Point(246, 285);
            this.button_upload.Name = "button_upload";
            this.button_upload.Size = new System.Drawing.Size(75, 21);
            this.button_upload.TabIndex = 1;
            this.button_upload.Text = "Upload";
            this.button_upload.UseVisualStyleBackColor = true;
            this.button_upload.Click += new System.EventHandler(this.button_upload_Click);
            // 
            // button_download
            // 
            this.button_download.Enabled = false;
            this.button_download.Location = new System.Drawing.Point(3, 285);
            this.button_download.Name = "button_download";
            this.button_download.Size = new System.Drawing.Size(75, 21);
            this.button_download.TabIndex = 0;
            this.button_download.Text = "Download";
            this.button_download.UseVisualStyleBackColor = true;
            this.button_download.Click += new System.EventHandler(this.button_download_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(570, 341);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(562, 315);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Games";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.button_upload, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_playGame, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_extract, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_download, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView_games, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_launchpbw, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_refresh, 4, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(562, 309);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // button_playGame
            // 
            this.button_playGame.Enabled = false;
            this.button_playGame.Location = new System.Drawing.Point(165, 285);
            this.button_playGame.Name = "button_playGame";
            this.button_playGame.Size = new System.Drawing.Size(75, 21);
            this.button_playGame.TabIndex = 3;
            this.button_playGame.Text = "Play";
            this.button_playGame.UseVisualStyleBackColor = true;
            this.button_playGame.Click += new System.EventHandler(this.button_playGame_Click);
            // 
            // button_launchpbw
            // 
            this.button_launchpbw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_launchpbw.Location = new System.Drawing.Point(484, 285);
            this.button_launchpbw.Name = "button_launchpbw";
            this.button_launchpbw.Size = new System.Drawing.Size(75, 21);
            this.button_launchpbw.TabIndex = 4;
            this.button_launchpbw.Text = "PBW";
            this.button_launchpbw.UseVisualStyleBackColor = true;
            this.button_launchpbw.Click += new System.EventHandler(this.button_launchpbw_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox_log);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(562, 315);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // process_GameLauncher
            // 
            this.process_GameLauncher.StartInfo.Domain = "";
            this.process_GameLauncher.StartInfo.ErrorDialog = true;
            this.process_GameLauncher.StartInfo.LoadUserProfile = false;
            this.process_GameLauncher.StartInfo.Password = null;
            this.process_GameLauncher.StartInfo.StandardErrorEncoding = null;
            this.process_GameLauncher.StartInfo.StandardOutputEncoding = null;
            this.process_GameLauncher.StartInfo.UserName = "";
            this.process_GameLauncher.SynchronizingObject = this;
            // 
            // process_preGameProcess
            // 
            this.process_preGameProcess.EnableRaisingEvents = true;
            this.process_preGameProcess.StartInfo.Domain = "";
            this.process_preGameProcess.StartInfo.LoadUserProfile = false;
            this.process_preGameProcess.StartInfo.Password = null;
            this.process_preGameProcess.StartInfo.StandardErrorEncoding = null;
            this.process_preGameProcess.StartInfo.StandardOutputEncoding = null;
            this.process_preGameProcess.StartInfo.UserName = "";
            this.process_preGameProcess.SynchronizingObject = this;
            // 
            // button_refresh
            // 
            this.button_refresh.Location = new System.Drawing.Point(327, 285);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(75, 21);
            this.button_refresh.TabIndex = 5;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // Form_PBWAutoPlrClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 388);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form_PBWAutoPlrClient";
            this.Text = "PBW AutoPlr Client";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_games)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_connect;
        private System.Windows.Forms.ToolStripButton toolStripButton_opensettings;
        public System.Windows.Forms.RichTextBox richTextBox_log;
        private System.Windows.Forms.DataGridView dataGridView_games;
        private System.Windows.Forms.Button button_upload;
        private System.Windows.Forms.Button button_download;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button_extract;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_connectionstate;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_acty;
        private System.Windows.Forms.Button button_playGame;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Diagnostics.Process process_GameLauncher;
        private System.Diagnostics.Process process_preGameProcess;
        private System.Windows.Forms.Button button_launchpbw;
        private System.Windows.Forms.Button button_refresh;

    }
}


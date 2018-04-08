namespace MonPFE
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schedulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pBoxStatIndicator = new System.Windows.Forms.PictureBox();
            this.tView = new System.Windows.Forms.TreeView();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxStatIndicator)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(619, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.schedulesToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // schedulesToolStripMenuItem
            // 
            this.schedulesToolStripMenuItem.Name = "schedulesToolStripMenuItem";
            this.schedulesToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.schedulesToolStripMenuItem.Text = "Schedules";
            this.schedulesToolStripMenuItem.Click += new System.EventHandler(this.schedulesToolStripMenuItem_Click);
            // 
            // pBoxStatIndicator
            // 
            this.pBoxStatIndicator.Location = new System.Drawing.Point(557, 36);
            this.pBoxStatIndicator.Name = "pBoxStatIndicator";
            this.pBoxStatIndicator.Size = new System.Drawing.Size(26, 26);
            this.pBoxStatIndicator.TabIndex = 1;
            this.pBoxStatIndicator.TabStop = false;
            // 
            // tView
            // 
            this.tView.Location = new System.Drawing.Point(12, 36);
            this.tView.Name = "tView";
            this.tView.Size = new System.Drawing.Size(251, 244);
            this.tView.TabIndex = 2;
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(533, 78);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 3;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(533, 228);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(75, 23);
            this.btnAddFolder.TabIndex = 4;
            this.btnAddFolder.Text = "New Folder";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.Location = new System.Drawing.Point(533, 257);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(75, 23);
            this.btnAddFile.TabIndex = 5;
            this.btnAddFile.Text = "Add File";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(270, 36);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(257, 244);
            this.treeView1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "SQLServer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(375, 283);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "SQLite";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 309);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnAddFile);
            this.Controls.Add(this.btnAddFolder);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.tView);
            this.Controls.Add(this.pBoxStatIndicator);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxStatIndicator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem schedulesToolStripMenuItem;
        private System.Windows.Forms.PictureBox pBoxStatIndicator;
        private System.Windows.Forms.TreeView tView;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}


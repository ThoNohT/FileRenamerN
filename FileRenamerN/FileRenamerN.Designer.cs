namespace FileRenamerN
{
    partial class FileRenamerN
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
            this.status = new System.Windows.Forms.StatusStrip();
            this.currentDir = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBrowse = new System.Windows.Forms.ToolStripStatusLabel();
            this.progress = new System.Windows.Forms.ToolStripProgressBar();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.lstRenamers = new System.Windows.Forms.ListBox();
            this.argPan = new System.Windows.Forms.TableLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblToolDescription = new System.Windows.Forms.Label();
            this.txtFileFilter = new System.Windows.Forms.TextBox();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.status.SuspendLayout();
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentDir,
            this.lblBrowse,
            this.progress});
            this.status.Location = new System.Drawing.Point(0, 409);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(977, 22);
            this.status.TabIndex = 0;
            this.status.Text = "statusStrip1";
            // 
            // currentDir
            // 
            this.currentDir.Name = "currentDir";
            this.currentDir.Size = new System.Drawing.Size(0, 17);
            this.currentDir.TextChanged += new System.EventHandler(this.currentDir_TextChanged);
            // 
            // lblBrowse
            // 
            this.lblBrowse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblBrowse.IsLink = true;
            this.lblBrowse.Name = "lblBrowse";
            this.lblBrowse.Size = new System.Drawing.Size(16, 17);
            this.lblBrowse.Text = "...";
            this.lblBrowse.Click += new System.EventHandler(this.lblBrowse_Click);
            // 
            // progress
            // 
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(400, 16);
            // 
            // lstFiles
            // 
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(347, 368);
            this.lstFiles.TabIndex = 1;
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFiles_SelectedIndexChanged);
            this.lstFiles.DoubleClick += new System.EventHandler(this.lstFiles_DoubleClick);
            // 
            // lstRenamers
            // 
            this.lstRenamers.FormattingEnabled = true;
            this.lstRenamers.Location = new System.Drawing.Point(347, 0);
            this.lstRenamers.Name = "lstRenamers";
            this.lstRenamers.Size = new System.Drawing.Size(170, 368);
            this.lstRenamers.TabIndex = 2;
            this.lstRenamers.SelectedIndexChanged += new System.EventHandler(this.lstRenamers_SelectedIndexChanged);
            // 
            // argPan
            // 
            this.argPan.AutoScroll = true;
            this.argPan.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.argPan.ColumnCount = 2;
            this.argPan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.argPan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.argPan.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.argPan.Location = new System.Drawing.Point(516, 106);
            this.argPan.Name = "argPan";
            this.argPan.RowCount = 1;
            this.argPan.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.argPan.Size = new System.Drawing.Size(461, 262);
            this.argPan.TabIndex = 3;
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(584, 368);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(393, 40);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply Tool";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblToolDescription
            // 
            this.lblToolDescription.Location = new System.Drawing.Point(516, 0);
            this.lblToolDescription.Name = "lblToolDescription";
            this.lblToolDescription.Size = new System.Drawing.Size(461, 103);
            this.lblToolDescription.TabIndex = 5;
            this.lblToolDescription.Text = "No tool loaded";
            this.lblToolDescription.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtFileFilter
            // 
            this.txtFileFilter.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtFileFilter.Location = new System.Drawing.Point(0, 368);
            this.txtFileFilter.Multiline = true;
            this.txtFileFilter.Name = "txtFileFilter";
            this.txtFileFilter.Size = new System.Drawing.Size(293, 40);
            this.txtFileFilter.TabIndex = 6;
            this.txtFileFilter.Text = ".*";
            this.txtFileFilter.TextChanged += new System.EventHandler(this.txtFileFilter_TextChanged);
            // 
            // txtFilename
            // 
            this.txtFilename.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtFilename.Location = new System.Drawing.Point(292, 368);
            this.txtFilename.Multiline = true;
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(293, 40);
            this.txtFilename.TabIndex = 7;
            // 
            // FileRenamerN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 431);
            this.Controls.Add(this.txtFileFilter);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblToolDescription);
            this.Controls.Add(this.argPan);
            this.Controls.Add(this.lstRenamers);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.status);
            this.Controls.Add(this.txtFilename);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FileRenamerN";
            this.Text = "FileRenamer N";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileRenamerN_FormClosing);
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ToolStripStatusLabel currentDir;
        private System.Windows.Forms.ToolStripStatusLabel lblBrowse;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.ToolStripProgressBar progress;
        private System.Windows.Forms.ListBox lstRenamers;
        private System.Windows.Forms.TableLayoutPanel argPan;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblToolDescription;
        private System.Windows.Forms.TextBox txtFileFilter;
        private System.Windows.Forms.TextBox txtFilename;
    }
}


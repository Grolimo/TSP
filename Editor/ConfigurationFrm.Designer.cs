
namespace Editor
{
    partial class ConfigurationFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationFrm));
            this.tbSourceFiles = new System.Windows.Forms.TextBox();
            this.CbEnableDebug = new System.Windows.Forms.CheckBox();
            this.BtnChangeSourceLocation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.BtnOk = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.TbSourceFileExtension = new System.Windows.Forms.TextBox();
            this.BtnDefaultExtension = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbSourceFiles
            // 
            this.tbSourceFiles.Location = new System.Drawing.Point(24, 36);
            this.tbSourceFiles.Name = "tbSourceFiles";
            this.tbSourceFiles.ReadOnly = true;
            this.tbSourceFiles.Size = new System.Drawing.Size(697, 23);
            this.tbSourceFiles.TabIndex = 0;
            // 
            // CbEnableDebug
            // 
            this.CbEnableDebug.AutoSize = true;
            this.CbEnableDebug.Location = new System.Drawing.Point(28, 202);
            this.CbEnableDebug.Name = "CbEnableDebug";
            this.CbEnableDebug.Size = new System.Drawing.Size(135, 19);
            this.CbEnableDebug.TabIndex = 1;
            this.CbEnableDebug.Text = "Enable debug mode.";
            this.CbEnableDebug.UseVisualStyleBackColor = true;
            this.CbEnableDebug.CheckedChanged += new System.EventHandler(this.CbEnableDebug_CheckedChanged);
            // 
            // BtnChangeSourceLocation
            // 
            this.BtnChangeSourceLocation.Location = new System.Drawing.Point(738, 36);
            this.BtnChangeSourceLocation.Name = "BtnChangeSourceLocation";
            this.BtnChangeSourceLocation.Size = new System.Drawing.Size(50, 23);
            this.BtnChangeSourceLocation.TabIndex = 2;
            this.BtnChangeSourceLocation.Text = "...";
            this.BtnChangeSourceLocation.UseVisualStyleBackColor = true;
            this.BtnChangeSourceLocation.Click += new System.EventHandler(this.BtnChangeSourceLocation_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Source File Location";
            // 
            // BtnOk
            // 
            this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOk.Location = new System.Drawing.Point(619, 202);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(75, 23);
            this.BtnOk.TabIndex = 4;
            this.BtnOk.Text = "Ok";
            this.BtnOk.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(700, 202);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 5;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // TbSourceFileExtension
            // 
            this.TbSourceFileExtension.Location = new System.Drawing.Point(24, 89);
            this.TbSourceFileExtension.Name = "TbSourceFileExtension";
            this.TbSourceFileExtension.Size = new System.Drawing.Size(139, 23);
            this.TbSourceFileExtension.TabIndex = 6;
            this.TbSourceFileExtension.TextChanged += new System.EventHandler(this.TbSourceFileExtension_TextChanged);
            // 
            // BtnDefaultExtension
            // 
            this.BtnDefaultExtension.Location = new System.Drawing.Point(179, 88);
            this.BtnDefaultExtension.Name = "BtnDefaultExtension";
            this.BtnDefaultExtension.Size = new System.Drawing.Size(75, 23);
            this.BtnDefaultExtension.TabIndex = 7;
            this.BtnDefaultExtension.Text = "Default";
            this.BtnDefaultExtension.UseVisualStyleBackColor = true;
            this.BtnDefaultExtension.Click += new System.EventHandler(this.BtnDefaultExtension_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Source File Extension:";
            // 
            // ConfigurationFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(800, 237);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BtnDefaultExtension);
            this.Controls.Add(this.TbSourceFileExtension);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnChangeSourceLocation);
            this.Controls.Add(this.CbEnableDebug);
            this.Controls.Add(this.tbSourceFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationFrm";
            this.ShowInTaskbar = false;
            this.Text = "Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSourceFiles;
        private System.Windows.Forms.CheckBox CbEnableDebug;
        private System.Windows.Forms.Button BtnChangeSourceLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.TextBox TbSourceFileExtension;
        private System.Windows.Forms.Button BtnDefaultExtension;
        private System.Windows.Forms.Label label2;
    }
}
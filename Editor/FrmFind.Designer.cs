namespace Editor
{
    partial class FrmFind
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
        /// the contents of this method with the code Editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbText = new System.Windows.Forms.TextBox();
            this.cbCaseSensitive = new System.Windows.Forms.CheckBox();
            this.rbForward = new System.Windows.Forms.RadioButton();
            this.rbReverse = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(420, 14);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Find";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(502, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tbText
            // 
            this.tbText.Location = new System.Drawing.Point(12, 14);
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(370, 20);
            this.tbText.TabIndex = 2;
            // 
            // cbCaseSensitive
            // 
            this.cbCaseSensitive.AutoSize = true;
            this.cbCaseSensitive.Location = new System.Drawing.Point(12, 40);
            this.cbCaseSensitive.Name = "cbCaseSensitive";
            this.cbCaseSensitive.Size = new System.Drawing.Size(83, 17);
            this.cbCaseSensitive.TabIndex = 3;
            this.cbCaseSensitive.Text = "Match Case";
            this.cbCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // rbForward
            // 
            this.rbForward.AutoSize = true;
            this.rbForward.Checked = true;
            this.rbForward.Location = new System.Drawing.Point(101, 39);
            this.rbForward.Name = "rbForward";
            this.rbForward.Size = new System.Drawing.Size(63, 17);
            this.rbForward.TabIndex = 4;
            this.rbForward.TabStop = true;
            this.rbForward.Text = "Forward";
            this.rbForward.UseVisualStyleBackColor = true;
            // 
            // rbReverse
            // 
            this.rbReverse.AutoSize = true;
            this.rbReverse.Location = new System.Drawing.Point(170, 39);
            this.rbReverse.Name = "rbReverse";
            this.rbReverse.Size = new System.Drawing.Size(78, 17);
            this.rbReverse.TabIndex = 5;
            this.rbReverse.Text = "Backwards";
            this.rbReverse.UseVisualStyleBackColor = true;
            // 
            // FrmFind
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(580, 71);
            this.Controls.Add(this.rbReverse);
            this.Controls.Add(this.rbForward);
            this.Controls.Add(this.cbCaseSensitive);
            this.Controls.Add(this.tbText);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmFind";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Find";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox tbText;
        public System.Windows.Forms.CheckBox cbCaseSensitive;
        public System.Windows.Forms.RadioButton rbForward;
        public System.Windows.Forms.RadioButton rbReverse;
    }
}
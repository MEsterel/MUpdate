namespace MUpdate
{
    partial class MUpdateAcceptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MUpdateAcceptForm));
            this.lblUpdateAvailTitle = new System.Windows.Forms.Label();
            this.lblText1 = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.btnUpdateNow = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblAppName = new System.Windows.Forms.Label();
            this.patchNotesLbl = new System.Windows.Forms.Label();
            this.btnIgnore = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblUpdateAvailTitle
            // 
            resources.ApplyResources(this.lblUpdateAvailTitle, "lblUpdateAvailTitle");
            this.lblUpdateAvailTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUpdateAvailTitle.Name = "lblUpdateAvailTitle";
            // 
            // lblText1
            // 
            resources.ApplyResources(this.lblText1, "lblText1");
            this.lblText1.Name = "lblText1";
            // 
            // txtDetails
            // 
            resources.ApplyResources(this.txtDetails, "txtDetails");
            this.txtDetails.BackColor = System.Drawing.Color.White;
            this.txtDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.TabStop = false;
            // 
            // btnUpdateNow
            // 
            resources.ApplyResources(this.btnUpdateNow, "btnUpdateNow");
            this.btnUpdateNow.Name = "btnUpdateNow";
            this.btnUpdateNow.UseVisualStyleBackColor = true;
            this.btnUpdateNow.Click += new System.EventHandler(this.btnUpdateNow_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MUpdate.Properties.Resources.update;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lblAppName
            // 
            this.lblAppName.AutoEllipsis = true;
            resources.ApplyResources(this.lblAppName, "lblAppName");
            this.lblAppName.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblAppName.Name = "lblAppName";
            // 
            // patchNotesLbl
            // 
            resources.ApplyResources(this.patchNotesLbl, "patchNotesLbl");
            this.patchNotesLbl.Name = "patchNotesLbl";
            // 
            // btnIgnore
            // 
            resources.ApplyResources(this.btnIgnore, "btnIgnore");
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // MUpdateAcceptForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.patchNotesLbl);
            this.Controls.Add(this.btnUpdateNow);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.lblText1);
            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.lblUpdateAvailTitle);
            this.Name = "MUpdateAcceptForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblUpdateAvailTitle;
        private System.Windows.Forms.Label lblText1;
        private System.Windows.Forms.RichTextBox txtDetails;
        private System.Windows.Forms.Button btnUpdateNow;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label patchNotesLbl;
        private System.Windows.Forms.Button btnIgnore;
    }
}
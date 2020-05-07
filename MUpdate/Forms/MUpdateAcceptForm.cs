using MUpdate.File;
using MUpdate.Resources;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace MUpdate
{
    internal partial class MUpdateAcceptForm : Form
    {
        private IMUpdatable applicationInfo;

        private MUpdateXml updateInfo;

        public MUpdateAcceptForm(IMUpdatable applicationInfo, MUpdateXml updateInfo)
        {
            InitializeComponent();

            this.applicationInfo = applicationInfo;
            this.updateInfo = updateInfo;

            this.Text =lang.updateAvailable;
            this.lblAppName.Text = this.applicationInfo.ApplicationName;

            if (this.applicationInfo.ApplicationIcon != null)
                this.Icon = this.applicationInfo.ApplicationIcon;

            this.lblUpdateAvailTitle.Text = lang.updateAvailable + ": " + this.updateInfo.Version.ToString();
            //this.lblUpdaVers.Text = string.Format(lang.updateVersion + ": {0}", this.updateInfo.Version.ToString());

            //this.lblCurrVers.Text = string.Format(lang.currentVersion + ": {0}", this.applicationInfo.ApplicationAssembly.GetName().Version.ToString());

            this.txtDetails.Text = makeNewLines(this.updateInfo.Description);
        }

        private static string makeNewLines(string text)
        {
            string newText = text.Replace("\\r\\n", Environment.NewLine);

            return newText;
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnUpdateNow_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
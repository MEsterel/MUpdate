using MUpdate;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Test_App
{
    public partial class Form1 : Form, IMUpdatable
    {
        private string[] args;

        private MUpdater update;

        public Form1(string[] args)
        {
            InitializeComponent();
            label1.Text = "Version: " + ApplicationAssembly.GetName().Version.ToString();
            this.args = args;

            update = new MUpdater(this);
            MUpdate.Events.MUpdateEventBridge.AsyncUpdateFinished += MUpdateEventBridge_AsyncUpdateFinished;
        }

        /// <summary>
        /// Returns the executing assembly.
        /// </summary>
        public Assembly ApplicationAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        /// <summary>
        /// Returns the Application Icon.
        /// </summary>
        public Icon ApplicationIcon
        {
            get { return this.Icon; }
        }

        /// <summary>
        /// Returns the Application ID.
        /// </summary>
        public string ApplicationID
        {
            get { return "TestApp"; }
        }

        /// <summary>
        /// Returns the Application Name.
        /// </summary>
        public string ApplicationName
        {
            get { return "TestApp"; }
        }

        /// <summary>
        /// Returns the context.
        /// </summary>
        public Form Context
        {
            get { return this; }
        }

        public string Language
        {
            get { return "fr"; }
        }

        /// <summary>
        /// Returns Uri location of the update.xml file.
        /// </summary>
        public Uri UpdateXmlLocation
        {
            get { return new Uri("https://dl.dropbox.com/s/mkrc2gjyu94s0bw/update.xml?dl=0"); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            update.DoUpdateAsync();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                if (args[0] == "-updatedMessage")
                {
                    string appName = Assembly.GetExecutingAssembly().GetName().Name;
                    string message = appName + " has been successfully updated to version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "!";
                    MessageBox.Show(message, appName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }

        private void MUpdateEventBridge_AsyncUpdateFinished(object sender, EventArgs e)
        {
            MUpdate.Events.AsyncUpdateFinishedEventArgs args = (MUpdate.Events.AsyncUpdateFinishedEventArgs)e;

            if (!args.Successful && args.Ex != null)
            {
                MessageBox.Show(this, args.Ex.Message, "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
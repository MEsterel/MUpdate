using MUpdate.File;
using MUpdate.Resources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace MUpdate
{
    internal partial class MUpdateDownloadForm : Form
    {
        /// <summary>
        /// Backgroundworker of download
        /// </summary>
        private BackgroundWorker bgWorker;

        /// <summary>
        /// MD5 private string
        /// </summary>
        private string md5;

        /// <summary>
        /// Stopwatch to calculate download speed.
        /// </summary>
        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Path of the temporary downloaded file
        /// </summary>
        private string tempFile;

        /// <summary>
        /// Web client to download file
        /// </summary>
        private WebClient webClient;

        /// <summary>
        /// Creates a new MUpdateDownloadForm
        /// </summary>
        internal MUpdateDownloadForm(Uri location, string md5, Icon programIcon, string applicationName)
        {
            InitializeComponent();

            if (programIcon != null)
                this.Icon = programIcon;

            lblAppName.Text = applicationName;

            lblProgress.Text = lang.startingDownload3P;

            tempFile = Path.GetTempFileName();

            this.md5 = md5;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;

            sw.Start();

            try { webClient.DownloadFileAsync(location, this.tempFile); }
            catch { this.DialogResult = DialogResult.No; this.Close(); }
        }

        /// <summary>
        /// Returns the path of the temporary downloaded file
        /// </summary>
        internal string TempFilePath
        {
            get { return this.tempFile; }
        }

        /// <summary>
        /// Make bgWorker doing his work, hashing file.
        /// </summary>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            string updateMd5 = ((string[])e.Argument)[1];

            if (Hasher.HashFile(file, HashType.MD5) != updateMd5)
                e.Result = DialogResult.No;
            else
                e.Result = DialogResult.OK;
        }

        /// <summary>
        /// When the bgWorker has finished
        /// </summary>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }

        /// <summary>
        /// If user clicks on Cancel button
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }

        /// <summary>
        /// Fortmat bytes to a readable state.
        /// </summary>
        /// <param name="bytes">Bytes to update.</param>
        /// <param name="decimalPlaces">Number of decimals.</param>
        /// <param name="showByteType">Show the byte type (ex : MB).</param>
        /// <returns></returns>
        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "8";

            if (newBytes >= 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes >= 1048576 && newBytes < 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }

            if (decimalPlaces > 0)
            {
                formatString += ":0.";

                for (int i = 0; i < decimalPlaces; i++)
                    formatString += "0";
            }
            else
            {
                formatString += ":0.";
            }

            formatString += "}";

            if (showByteType)
                formatString += byteType;

            return string.Format(formatString, newBytes);
        }

        /// <summary>
        /// If user closes window
        /// </summary>
        private void MUpdateDownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
            }
            else
            {
                lblProgress.Text = lang.verifyingDownload3P;
                progressBar1.Style = ProgressBarStyle.Marquee;

                bgWorker.RunWorkerAsync(new string[] { this.tempFile, this.md5 });
            }
        }

        /// <summary>
        /// Progress changed of the download
        /// </summary>
        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;

            long bytesSecond = e.BytesReceived / Convert.ToInt64(sw.Elapsed.TotalSeconds); // Get Bytes/Sec
            //long remainingTime = (Convert.ToInt64(sw.Elapsed.TotalSeconds, ) / e.BytesReceived) * (e.TotalBytesToReceive - e.BytesReceived); // Get remaining time in seconds
            long remainingTime = (e.TotalBytesToReceive - e.BytesReceived) / bytesSecond; // Get remaining time in seconds

            this.lblProgress.Text = String.Format("{0}{1} {2} {3} {4} {5}\r\n{6} {7}",
                lang.downloadedTag,
                FormatBytes(e.BytesReceived, 1, true),
                lang.ofTag,
                FormatBytes(e.TotalBytesToReceive, 1, true),
                lang.atTag,
                FormatBytes(bytesSecond, 0, true) + "/s",
                lang.estimatedTimeRemainingLbl,
                remainingTime + " sec");
        }

        private void MUpdateDownloadForm_Load(object sender, EventArgs e)
        {

        }
    }
}
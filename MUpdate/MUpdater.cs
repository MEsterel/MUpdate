using MUpdate.Events;
using MUpdate.File;
using MUpdate.Resources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MUpdate
{
    public class MUpdater : IDisposable
    {
        /// <summary>
        /// Stores application's infos
        /// </summary>
        private IMUpdatable applicationInfo;

        /// <summary>
        /// Definition of a bgWorker
        /// </summary>
        private BackgroundWorker bgWorker;

        /// <summary>
        /// Initializes the update.
        /// </summary>
        /// <param name="applicationInfo">Specify the calling form (ex: this).</param>
        public MUpdater(IMUpdatable applicationInfo)
        {
            // 0. LOAD DLLS
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            this.applicationInfo = applicationInfo;

            if (applicationInfo.Language == "fr")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }

            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += bgWorker_DoWork;
            this.bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName MissingAssembly = new AssemblyName(args.Name);
            CultureInfo ci = MissingAssembly.CultureInfo;

            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            string resourceName = "MUpdate.Resources." + ci.Name.Replace("-", "_") + "." + MissingAssembly.Name + ".dll";
            //string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        /// <summary>
        /// Indicates if the calling app was already up to date, after looking for updates.
        /// </summary>
        public bool CheckedAlreadyUpToDate { get; private set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <para>Launch the update process in background.</para>
        /// <para>When update process is finished (with error or not), the event AsyncUpdateFinished is raised with all the results for the calling application to exploit (e.g. if network error).</para>
        /// </summary>
        /// <param name="notificateNoUpdates">If false, do not show message box saying 'You already have the latest version'.</param>
        public void DoUpdateAsync()
        {
            try
            {
                if (!this.bgWorker.IsBusy)
                {
                    this.bgWorker.RunWorkerAsync(this.applicationInfo);
                }
                else
                {
                    MUpdateEventBridge.RaiseAsyncUpdateFinished(false, new Exception(lang.asyncAlreadyRunning));
                }
            }
            catch (Exception ex)
            {
                MUpdateEventBridge.RaiseAsyncUpdateFinished(false, ex);
            }
        }

        /// <summary>
        /// <para>Launch the update process and wait for answer.</para>
        /// <para>Better wrap this function in a Try-Catch block or it may fail the calling application (e.g. internet connection problems).</para>
        /// <para>Returns True if updated successfully, False if no updates are available; throws Exception if error (e.g. no internet connection).<br/>
        /// If user already have last version installed, returns false because no updates were made. Use CheckedAlreadyUpToDate property.</para>
        /// </summary>
        /// <param name="notificateNoUpdates">If false, do not show message box saying 'You already have the latest version'.</param>
        /// <returns>True if updated successfully, False if not updated, Exception if error (e.g. no internet connection)</returns>
        public bool DoUpdateSync(bool notificateNoUpdates = true)
        {
            if (!Tools.CheckForInternetConnection())
            {
                throw new WebException(lang.noInternetConnectionError, WebExceptionStatus.ConnectFailure);
            }

            if (MUpdateXml.ExistsOnServer(applicationInfo.UpdateXmlLocation)) // Si le fichier de mise à jour existe
            {
                return CompareVersions(MUpdateXml.Parse(applicationInfo.UpdateXmlLocation, applicationInfo.ApplicationID),
                    notificateNoUpdates); // Retourne si une nouvelle mise à jour est disponible et demande à l'utilisateur ce qu'il souhaite faire.
            }

            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                bgWorker.Dispose();
            }
            // free native resources
        }

        /// <summary>
        /// Parses data from update.xml located on the specified Uri
        /// </summary>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Tools.CheckForInternetConnection())
            {
                throw new WebException(lang.noInternetConnectionError, WebExceptionStatus.ConnectFailure);
            }

            IMUpdatable application = (IMUpdatable)e.Argument;

            if (!MUpdateXml.ExistsOnServer(application.UpdateXmlLocation))
                e.Cancel = true;
            else
                e.Result = MUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationID);
        }

        /// <summary>
        /// When bgWorker finished parsing dats from update.xml
        /// </summary>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null) // If no errors were reported
            {
                MUpdateXml update = (MUpdateXml)e.Result; // Gets the result sent as argument

                bool updated = CompareVersions(update, false); // Compare and finish

                MUpdateEventBridge.RaiseAsyncUpdateFinished(updated, (!updated && CheckedAlreadyUpToDate) ? new Exception(lang.latestVersionInstalled) : null);
            }
            else // If an error was reported
            {
                if (e.Error == null)
                {
                    MUpdateEventBridge.RaiseAsyncUpdateFinished(false, new Exception(lang.checkError));
                }
                else
                {
                    MUpdateEventBridge.RaiseAsyncUpdateFinished(false, e.Error);
                }
            }
        }

        private bool CompareVersions(MUpdateXml update, bool notificateNoUpdates = true)
        {
            if (update != null && update.IsNewerThan(applicationInfo.ApplicationAssembly.GetName().Version)) // If the update is not null and the version is newer than the current one
            {
                if (new MUpdateAcceptForm(this.applicationInfo, update).ShowDialog(this.applicationInfo.Context) == DialogResult.Yes) // If user accepts to do the update now
                {
                    DownloadUpdate(update); // Download update
                }

                CheckedAlreadyUpToDate = false;
                return true;
            }
            else if (update != null && update.Version <= applicationInfo.ApplicationAssembly.GetName().Version)
            {
                if (notificateNoUpdates)
                {
                    MessageBox.Show(lang.latestVersionInstalledLbl + " " + applicationInfo.ApplicationAssembly.GetName().Version.ToString(),
                       applicationInfo.ApplicationName + " - " + lang.updateChecking,
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CheckedAlreadyUpToDate = true;
            }
            else
            {
                CheckedAlreadyUpToDate = false;
            }

            return false;
        }

        private void DownloadUpdate(MUpdateXml update)
        {
            MUpdateDownloadForm form = new MUpdateDownloadForm(update.Uri, update.MD5, this.applicationInfo.ApplicationIcon, this.applicationInfo.ApplicationName); // Initialze a new Download form
            DialogResult res = form.ShowDialog(this.applicationInfo.Context); //Show it by getting the dialog result

            if (res == DialogResult.OK)
            {
                string currentPath = this.applicationInfo.ApplicationAssembly.Location; // Get current path
                string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.FileName;

                if (!update.IsInstaller)
                    UpdateStandalone(form.TempFilePath, currentPath, newPath, update.LaunchArgs); // Launch cmd
                else
                    LaunchInstaller(form.TempFilePath, update.LaunchArgs);

                Application.Exit();
            }
            else if (res == DialogResult.Abort)
            {
                MessageBox.Show(lang.downloadCancelled,
                    lang.downloadCancelledTitle,
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show(lang.downloadError,
                    lang.downloadErrorTitle,
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
        }

        private void UpdateStandalone(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            string MY_FORMAT = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & /C Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = string.Format(MY_FORMAT, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };

            Process.Start(info);
        }

        private void LaunchInstaller(string tempFilePath, string launchArgs)
        {
            string MY_FORMAT = "/C Choice /C Y /N /D Y /T 4 & Start /WAIT \"\" /D \"{0}\" \"{1}\" {2} & /C Choice /C Y /N /D Y /T 2 & Del /F /Q \"{3}\" ";

            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = string.Format(MY_FORMAT, Path.GetDirectoryName(tempFilePath), Path.GetFileName(tempFilePath), launchArgs, tempFilePath),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };

            Process.Start(info);
        }
    }
}
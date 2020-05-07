using System;
using System.Net;
using System.Xml;

namespace MUpdate.File
{
    internal class MUpdateXml
    {
        private string description;
        private string fileName;
        private string launchArgs;
        private string md5;
        private Uri uri;
        private Version version;
        private bool isInstaller;

        /// <summary>
        /// Creates a new SharpUpdateXml
        /// </summary>
        internal MUpdateXml(Version version, Uri uri, string fileName, string md5, string description, string launchArgs, bool isInstaller)
        {
            this.version = version;
            this.uri = uri;
            this.fileName = fileName;
            this.md5 = md5;
            this.description = description;
            this.launchArgs = launchArgs;
            this.isInstaller = isInstaller;
        }

        /// <summary>
        /// Returns the description.
        /// </summary>
        internal string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// Returns the file's name update.
        /// </summary>
        internal string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// Returns the specified launch args.
        /// </summary>
        internal string LaunchArgs
        {
            get { return this.launchArgs; }
        }

        /// <summary>
        /// Returns MD5.
        /// </summary>
        internal string MD5
        {
            get { return this.md5; }
        }

        /// <summary>
        /// Returns the specified Uri.
        /// </summary>
        internal Uri Uri
        {
            get { return this.uri; }
        }

        /// <summary>
        /// Returns the new version.
        /// </summary>
        internal Version Version
        {
            get { return this.version; }
        }

        internal bool IsInstaller
        {
            get { return this.isInstaller; }
        }

        /// <summary>
        /// Checks the Uri to make sure the file exists.
        /// </summary>
        /// <param name="location">The Uri of the update.xml</param>
        /// <returns>If the file exists.</returns>
        internal static bool ExistsOnServer(Uri location)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                resp.Close();

                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch { return false; }
        }

        internal static MUpdateXml Parse(Uri location, string appID)
        {
            Version version = null;
            string url = "", fileName = "", md5 = "", description = "", launchArgs = "";
            bool isInstaller = false;

            XmlDocument doc = new XmlDocument(); // Request the update.xml
            doc.Load(location.AbsoluteUri);

            XmlNode node = doc.DocumentElement.SelectSingleNode("//update[@appId='" + appID + "']");

            if (node == null) // If the node doesn't exists there is no update.
                return null;

            // Parse data
            version = Version.Parse(node["version"].InnerText);
            url = node["url"].InnerText;
            fileName = node["fileName"].InnerText;
            md5 = node["md5"].InnerText;
            description = node["description"].InnerText;
            launchArgs = node["launchArgs"].InnerText;
            try { isInstaller = bool.Parse(node["isInstaller"].InnerText); } catch { } // Adapt to older versions

            return new MUpdateXml(version, new Uri(url), fileName, md5, description, launchArgs, isInstaller);
        }

        /// <summary>
        /// Checks if update's version is newer than the old version.
        /// </summary>
        /// <param name="version">Application's current version.</param>
        /// <returns>If the update's version is newer.</returns>
        internal bool IsNewerThan(Version version)
        {
            return this.version > version;
        }
    }
}
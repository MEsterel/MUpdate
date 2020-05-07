using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace MUpdate
{
    public interface IMUpdatable
    {
        /// <summary>
        /// Returns executing assembly.
        /// </summary>
        Assembly ApplicationAssembly { get; }

        /// <summary>
        /// Returns App Icon
        /// </summary>
        Icon ApplicationIcon { get; }

        /// <summary>
        /// Returns App Id (here : "Test App")
        /// </summary>
        string ApplicationID { get; }

        /// <summary>
        /// Returns App Name (here : "Test App")
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// Returns form (this).
        /// </summary>
        Form Context { get; }

        /// <summary>
        /// Returns the language.
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Returns Uri location.
        /// </summary>
        Uri UpdateXmlLocation { get; }
    }
}
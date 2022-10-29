using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace MacroScript
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("189ad47f-1f71-4811-aeb5-b7340c594a91")]
    public class MainToolWindo : ToolWindowPane
    {
      

        /// <summary>
        /// Initializes a new instance of the <see cref="MainToolWindo"/> class.
        /// </summary>
        public MainToolWindo() : base(null)
        {
            this.Caption = "MainToolWindo";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new MainToolWindoControl();
        }
    }
}

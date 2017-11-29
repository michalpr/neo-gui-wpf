using System.Diagnostics;
using System.Windows;
using Neo.Gui.Base.Helpers.Interfaces;

namespace Neo.Gui.Wpf.Helpers
{
    public class ProcessHelper : IProcessHelper
    {
        #region IProcessHelper implementation 
        public void OpenInExternalBrowser(string url)
        {
            Process.Start(url);
        }

        public void Restart()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        #endregion
    }
}
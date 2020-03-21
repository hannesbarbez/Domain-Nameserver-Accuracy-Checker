using System.Diagnostics;

namespace Domain_Nameserver_Accuracy_Checker.Logic
{
    internal static class Generic
    {
        internal static void OpenDefaultBrowser(string url)
        {
            RunExternalProcess("explorer.exe", url);
        }

        internal static void RunExternalProcess(string appPath, string args)
        {
            ProcessStartInfo info = new ProcessStartInfo(appPath, @args);
            info.UseShellExecute = false;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();
        }
    }
}

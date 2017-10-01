﻿using Neo.Core;
using Neo.Implementations.Blockchains.LevelDB;
using Neo.Network;
using Neo.Properties;
using Neo.UI.Views.Updater;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Neo
{
    internal static class Program
    {
        private const string PeerStatePath = "peers.dat";

        public static LocalNode LocalNode;

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            using (var fileStream = new FileStream("error.log", FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                PrintErrorLogs(writer, (Exception) e.ExceptionObject);
            }
        }

        private static bool InstallCertificate()
        {
            if (!Settings.Default.InstallCertificate) return true;
            using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            using (var cert = new X509Certificate2(Resources.OnchainCertificate))
            {
                store.Open(OpenFlags.ReadOnly);
                if (store.Certificates.Contains(cert)) return true;
            }
            using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            using (var cert = new X509Certificate2(Resources.OnchainCertificate))
            {
                try
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(cert);
                    return true;
                }
                catch (CryptographicException) { }
                if (MessageBox.Show(Strings.InstallCertificateText, Strings.InstallCertificateCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    Settings.Default.InstallCertificate = false;
                    Settings.Default.Save();
                    return true;
                }
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        UseShellExecute = true,
                        Verb = "runas",
                        WorkingDirectory = Environment.CurrentDirectory
                    });
                    return false;
                }
                catch (Win32Exception) { }
                MessageBox.Show(Strings.InstallCertificateCancel);
                return true;
            }
        }

        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            XDocument xdoc = null;
            try
            {
                xdoc = XDocument.Load("https://neo.org/client/update.xml");
            }
            catch { }
            if (xdoc != null)
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var minimum = Version.Parse(xdoc.Element("update").Attribute("minimum").Value);
                if (version < minimum)
                {
                    var updateDialog = new UpdateView(xdoc);

                    updateDialog.ShowDialog();
                    return;
                }
            }
            if (!InstallCertificate()) return;
            
            if (File.Exists(PeerStatePath))
                using (var fileStream = new FileStream(PeerStatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    LocalNode.LoadState(fileStream);
                }
            using (Blockchain.RegisterBlockchain(new LevelDBBlockchain(Settings.Default.DataDirectoryPath)))
            using (LocalNode = new LocalNode())
            {
                LocalNode.UpnpEnabled = true;

                // Start NEO GUI Application
                var app = new App(xdoc);

                app.Run();
            }
            using (var fileStream = new FileStream(PeerStatePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                LocalNode.SaveState(fileStream);
            }
        }

        private static void PrintErrorLogs(StreamWriter writer, Exception ex)
        {
            writer.WriteLine(ex.GetType());
            writer.WriteLine(ex.Message);
            writer.WriteLine(ex.StackTrace);
            if (ex is AggregateException ex2)
            {
                foreach (var innerException in ex2.InnerExceptions)
                {
                    writer.WriteLine();
                    PrintErrorLogs(writer, innerException);
                }
            }
            else if (ex.InnerException != null)
            {
                writer.WriteLine();
                PrintErrorLogs(writer, ex.InnerException);
            }
        }
    }
}
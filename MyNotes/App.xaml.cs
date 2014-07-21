using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Xml.Linq;

namespace MyNotes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string notesfilename = "notes.xml";
        private static readonly string notesfilepath;

        private static string applicationDataPath;
        public static string ApplicationDataPath
        {
            get { return applicationDataPath; }
        }

        private bool allowClose = false;
        public bool AllowClose
        {
            get { return allowClose; }
        }

        static App()
        {
            var roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            roamingPath = Path.Combine(roamingPath, "DeadDog", "MyNotes");
            ensurePath(roamingPath);
            applicationDataPath = roamingPath;

            notesfilepath = Path.Combine(roamingPath, notesfilename);
        }

        private static void ensurePath(string path)
        {
            string[] levels = path.Split(Path.DirectorySeparatorChar);
            var drive = new DriveInfo(levels[0]);
            if (drive.DriveType == DriveType.NoRootDirectory ||
                drive.DriveType == DriveType.Unknown)
                throw new ArgumentException("Unable to evaluate path drive; " + levels[0], "path");

            if (!drive.IsReady)
                throw new ArgumentException("Drive '" + levels[0] + "' is not ready.", "path");

            path = levels[0] + "\\";
            for (int i = 1; i < levels.Length; i++)
            {
                path = Path.Combine(path, levels[i]);
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                    dir.Create();
            }
        }

        private readonly double waitMinutes = 10;

        private XDocument doc;
        private Timer saveTimer;

        private Window hideWindow;

        public App()
        {
            this.saveTimer = new Timer(1000 * 60 * waitMinutes)
            {
                AutoReset = false
            };
            this.saveTimer.Elapsed += (s, e) => doc.Save(notesfilepath);

            this.hideWindow = new Window()
            {
                Top = -100,
                Left = -100,
                Width = 1,
                Height = 1,
                ShowActivated = false,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow
            };
            hideWindow.Show();
            hideWindow.Hide();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            doc = new FileInfo(notesfilepath).Exists ? XDocument.Load(notesfilepath) : new XDocument(new XElement("notes"));
            XElement notes = doc.Element("notes");

            if (notes == null)
                throw new InvalidDataException("The " + notesfilename + " must contain a 'notes' root element.");
            if (!notes.HasElement("note"))
                notes.Add(new XElement("note"));

            var icon = new Hardcodet.Wpf.TaskbarNotification.TaskbarIcon();
            icon.Icon = MyNotes.Properties.Resources.toolbar;

            foreach (var n in doc.Element("notes").Elements("note"))
                if (!n.Attribute("deleted", false))
                    new MainWindow(n) { Owner = hideWindow }.Show();

            doc.Changed += (s, ee) => { saveTimer.Stop(); saveTimer.Start(); };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            doc.Save(notesfilepath);
            allowClose = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyNotes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string applicationDataPath;
        public static string ApplicationDataPath
        {
            get { return applicationDataPath; }
        }

        static App()
        {
            var roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            roamingPath = Path.Combine(roamingPath, "DeadDog", "MyNotes");
            ensurePath(roamingPath);
            applicationDataPath = roamingPath;
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
    }
}

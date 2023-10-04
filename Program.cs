using Cobbs_Engine.Input;
using System;
using System.Diagnostics;
using System.IO;

namespace Cobbs_Engine
{
    internal static partial class Program
    {
        internal static MainGame Game { get; private set; }

        internal static class Properties
        {
            internal static bool DebugEnabled { get; set; } = true;
            internal static bool PortableEnabled { get; set; }
            internal static bool LoggingEnabled { get; set; } = true;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            ValidateArguments(args);
            ValidatePaths();

            Diagnostics.DiagnosticsPath = IO.Paths[PathType.Diagnostics];
            Diagnostics.Initialize();

            Console.ShowWindow(Console.Window, Properties.DebugEnabled ? (int)ConsoleState.Open : (int)ConsoleState.Closed);

            Diagnostics.LogMessage("Game starting...");

            AppDomain.CurrentDomain.ProcessExit += OnExit;
            try
            {
                using (Game = new MainGame())
                {
                    MainGame.Instance = Game;
                    Game.Run();
                }
            }
            catch (Exception ex)
            {
                Diagnostics.LogException(ex, "Exception caught");
                Diagnostics.LogMessage("The game has crashed due to an exception");
            }
        }

        internal static void ValidateArguments(string[] args)
        {
            foreach (string arg in args)
            {
                switch (arg.ToLower())
                {
                    case "-portable":
                        Properties.PortableEnabled = true;
                        break;

                    case "-debug":
                        Properties.DebugEnabled = true;
                        break;

                    default:
                        return;
                }
                Diagnostics.LogMessage($"Argument '{arg}' recognized");
            }
        }

        internal static void ValidatePaths()
        {
            IO.Paths[PathType.Executable] = Directory.GetCurrentDirectory();

            if (!Properties.PortableEnabled)
            {
                string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                switch (Environment.OSVersion.Platform)
                {
                    // Windows
                    case (PlatformID.Win32NT):
                    case (PlatformID.Win32S):
                    case (PlatformID.Win32Windows):
                    case (PlatformID.WinCE):
                        IO.Paths[PathType.UserData] = Path.Combine(baseDirectory, "AppData\\LocalLow\\" + Configuration.GameName);
                        break;
                    // Linux
                    case (PlatformID.Unix):
                        IO.Paths[PathType.UserData] = Path.Combine(baseDirectory, "." + Configuration.GameName.ToLowerInvariant());
                        break;
                    // Apple
                    case (PlatformID.MacOSX):
                        IO.Paths[PathType.UserData] = Path.Combine(baseDirectory, "Library/Application Support/" + Configuration.GameName);
                        break;
                    default:
                        throw new PlatformNotSupportedException("Platform not supported.");
                }
            }
            else
            {
                IO.Paths[PathType.UserData] = Directory.GetCurrentDirectory();
            }

            string pluginPath = Configuration.PluginsPath.Replace("~\\", $"{IO.Paths[PathType.UserData]}\\").Replace("@\\", $"{IO.Paths[PathType.Executable]}\\");
            string contentPath = Configuration.ContentPath.Replace("~\\", $"{IO.Paths[PathType.UserData]}\\").Replace("@\\", $"{IO.Paths[PathType.Executable]}\\");
            string savePath = Configuration.SavePath.Replace("~\\", $"{IO.Paths[PathType.UserData]}\\").Replace("@\\", $"{IO.Paths[PathType.Executable]}\\");
            string diagnosticPath = Configuration.DiagnosticsPath.Replace("~\\", $"{IO.Paths[PathType.UserData]}\\").Replace("@\\", $"{IO.Paths[PathType.Executable]}\\");
            string settingsPath = Configuration.SettingsPath.Replace("~\\", $"{IO.Paths[PathType.UserData]}\\").Replace("@\\", $"{IO.Paths[PathType.Executable]}\\");
            string mediaPath = Configuration.MediaPath.Replace("~\\", $"{IO.Paths[PathType.UserData]}\\").Replace("@\\", $"{IO.Paths[PathType.Executable]}\\");

            IO.Paths[PathType.Plugins] = pluginPath;
            IO.Paths[PathType.Content] = contentPath;
            IO.Paths[PathType.Save] = savePath;
            IO.Paths[PathType.Diagnostics] =diagnosticPath;
            IO.Paths[PathType.Settings] = settingsPath;
            IO.Paths[PathType.Media] = mediaPath;

            IO.ValidatePath(IO.Paths[PathType.Plugins]);
            IO.ValidatePath(IO.Paths[PathType.Content]);
            IO.ValidatePath(IO.Paths[PathType.Save]);
            IO.ValidatePath(IO.Paths[PathType.Diagnostics]);
            IO.ValidatePath(IO.Paths[PathType.Settings]);
            IO.ValidatePath(IO.Paths[PathType.Media]);
        }

        internal static void OnExit(object sender, EventArgs e)
        {
            Diagnostics.LogMessage("Program Exiting...");
            try
            {
                IO.SaveSettings(MainGame.Settings);
                Diagnostics.Flush();
                Diagnostics.Prune();
            }
            catch (Exception ex)
            {
                Diagnostics.LogException(ex);
            }

            System.Console.WriteLine("\nPress any key to close the console.\nType '`' to open log in console");
            ConsoleKeyInfo keypress = System.Console.ReadKey();
            try
            {
                if (keypress.Key == ConsoleKey.Oem3)
                {
                    System.Console.WriteLine(Diagnostics.LogPath);
                    Process process = new();
                    process.StartInfo = new ProcessStartInfo(Diagnostics.LogPath)
                    {
                        UseShellExecute = true,
                    };
                    process.Start();
                }
                    
            }
            catch { }
        }
    }
}
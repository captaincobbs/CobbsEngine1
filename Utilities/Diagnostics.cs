using Pastel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cobbs_Engine
{
    public static partial class Diagnostics
    {
        public static string DiagnosticsPath { get; set; }
        public static string LogPath { get; set; }
        public static string LogTime { get; private set; } = string.Empty;
        private static int logIndex;
        private static string logDate = string.Empty;
        private static string log = string.Empty;
        private static List<string> queuedLogs = new();

        public static readonly Dictionary<MessageType, string> Colors = new() {
            { MessageType.Message, "#EEEEEE" },
            { MessageType.Warning, "#FF8F00" },
            { MessageType.Error, "#FF2A2A" },
            { MessageType.Exception, "#C2185B" },
            { MessageType.Assert, "#4527A0" },
            { MessageType.Debug, "#212AAD" },
        };

        public static void Initialize()
        {
            LogTime = $"{DateTime.Now:yyyy-MM-dd @HH-mm-ss}";
            logDate = $"{DateTime.Now:U}";
            LogPath = Path.Combine(IO.Paths[PathType.Diagnostics], $"Diagnostic - {LogTime}.{Configuration.DiagnosticsFileType}");
            if (Program.Properties.LoggingEnabled)
            {
                InitializeLog();
            }
        }

        private static void InitializeLog()
        {
            string template = htmlTemplate.Replace("{GameName}", Configuration.GameName).Replace("{InitialTime}", logDate);
            logIndex = template.IndexOf("</content>");
            log = template;

            if (queuedLogs != null && queuedLogs.Count > 0)
            {
                foreach (string queuedLog in queuedLogs)
                {
                    log = log.Insert(logIndex, queuedLog);
                    logIndex += queuedLog.Length;
                }
                queuedLogs = null;
            }
        }

        public static void LogMessage(string message, bool write = true, bool console = true, bool debug = true)
        {
            if (Program.Properties.LoggingEnabled)
            {
                if (debug)
                    Debug.WriteLine(message);

                if (console)
                    Console.WriteLine(message.Pastel(Colors[MessageType.Message]));

                if (write)
                    Write(message, MessageType.Message, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogWarning(string message, bool write = true, bool console = true, bool debug = true)
        {
            if (Program.Properties.LoggingEnabled)
            {
                if (debug)
                    Debug.WriteLine(message);

                if (console)
                    Console.WriteLine(message.Pastel(Colors[MessageType.Warning]));

                if (write)
                    Write(message, MessageType.Warning, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogError(string message, bool write = true, bool console = true, bool debug = true)
        {
            if (Program.Properties.LoggingEnabled)
            {
                if (debug)
                    Debug.WriteLine(message);

                if (console)
                    Console.WriteLine(message.Pastel(Colors[MessageType.Error]));

                if (write)
                    Write(message, MessageType.Error, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogException(Exception ex, string errorMessage = null, bool write = true, bool console = true, bool debug = true)
        {
            if (Program.Properties.LoggingEnabled)
            {
                string message = string.Empty;

                message += $"{(errorMessage != null ? $"{errorMessage}: " : "")}{ex.Message}\n";
                if (!string.IsNullOrEmpty(ex.InnerException?.Message))
                    message += $"Inner exception: {ex.InnerException?.Message}\n";
                if (!string.IsNullOrEmpty(ex.ToString()))
                    message += $"Stack Trace: {ex}";

                if (debug)
                    Debug.WriteLine(message.TrimEnd('\n'));

                Console.WriteLine(message.TrimEnd('\n').Pastel(Colors[MessageType.Exception]));

                if (write)
                    Write($"{(errorMessage != null ? $"{errorMessage}: " : "")}{ex.Message}\n", MessageType.Exception, new StackTrace(ex, skipFrames: 1, true));
            }
        }

        public static void LogDebug(string message, bool write = true, bool console = true, bool debug = true)
        {
            if (Program.Properties.DebugEnabled && Program.Properties.LoggingEnabled)
            {
                if (debug)
                    Debug.WriteLine(message);

                if (console)
                    Console.WriteLine(message.Pastel(Colors[MessageType.Debug]));

                if (write)
                    Write(message, MessageType.Debug, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogAction(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                LogException(ex, "Logging action caused exception");
                throw;
            }
        }

        public static void LogAssert(bool condition, string message, bool write = true, bool console = true, bool debug = true)
        {
            if (Program.Properties.LoggingEnabled && !condition)
            {
                if (debug)
                    Debug.WriteLine(message);

                if (console)
                    Console.WriteLine($"{message}".Pastel(Colors[MessageType.Assert]));

                if (write)
                    Write($"{message}", MessageType.Assert, new StackTrace(skipFrames: 1, true));
            }
        }

        private static void Write(string message, MessageType messageType, StackTrace stack)
        {
            message = GenerateLogEvent(message, messageType, stack);
            if (logIndex != 0)
            {
                log = log.Insert(logIndex, message);
                logIndex += message.Length;
            }
            else
            {
                queuedLogs.Add(message);
            }
        }

        private static string GenerateLogEvent(string text, MessageType logType, StackTrace stack)
        {
            string stackTrace = string.Empty;
            foreach (StackFrame frame in stack.GetFrames())
            {
                MethodBase method = frame.GetMethod();
                string methodName = method.Name;
                string className = method.DeclaringType.FullName;
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                stackTrace += $"at {className}.{methodName}{(!string.IsNullOrEmpty(fileName)? $", in {fileName} on line {lineNumber}" : " from external namespace")}\n";
            }

            // Generate a log event as a string
            string logEvent = $"<div class='{Enum.GetName(logType).ToLower()} log'>" +
                              $"<span class='time'>{DateTime.Now:HH:mm:ss:fff}</span>" +
                              $"<span class='flags'>{Enum.GetName(logType)}</span>" +
                              $"<a onclick='toggleStackTrace(this)'>STACK</a>" +
                              $"{text}" +
                              $"<pre>{stackTrace.TrimEnd('\n')}</pre>" +
                              $"</div>";

            return logEvent;
        }

        public static void Flush()
        {
            if (log?.Length == 0)
            {
                return;
            }

            try
            {
                IO.SaveDiagnostics(log);
                log = string.Empty;
                LogTime = $"{DateTime.Now:yyyy-MM-dd @HH-mm-ss}";
                logDate = $"{DateTime.Now:U}";
                LogPath = Path.Combine(IO.Paths[PathType.Diagnostics], $"Diagnostic - {LogTime}.{Configuration.DiagnosticsFileType}");
                InitializeLog();
                LogMessage("Sucessfully flushed log", false);
            }
            catch (Exception ex)
            {
                LogException(ex, "Log has failed to save log", false);
            }
        }

        public static void Prune(bool all = false)
        {
            try
            {
                if (!all)
                {
                    if (Directory.GetFiles(IO.Paths[PathType.Diagnostics], "*", SearchOption.TopDirectoryOnly).Length > Configuration.MaximumDiagnostics)
                    {
                        var oldestFiles = Directory.EnumerateFiles(IO.Paths[PathType.Diagnostics])
                            .Select(fileName => new FileInfo(fileName))
                            .OrderByDescending(fileInfo => fileInfo.LastWriteTime)
                            .Skip((int)Configuration.MaximumDiagnostics)
                            .Select(fileInfo => fileInfo.FullName);

                        foreach (var file in oldestFiles)
                            File.Delete(file);
                    }
                }
                else
                {
                    foreach (string file in Directory.EnumerateFiles(IO.Paths[PathType.Diagnostics]))
                    {
                        LogMessage($"File deleted: {file}");
                        File.Delete(file);
                    }
                } 
            }
            catch
            {
                // Too late to log
            }
        }

        public enum MessageType
        {
            Message = 0,
            Warning = 1,
            Error = 2,
            Exception = 3,
            Assert = 4,
            Debug = 5,
        }
    }
}

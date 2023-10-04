using Pastel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cobbs_Engine
{
    public static partial class Diagnostics
    {
        public static string DiagnosticsPath { get; set; }
        public static string LogTime { get; private set; } = string.Empty;
        private static int logIndex;
        private static string logDate = string.Empty;
        private static string log = string.Empty;
        private static List<string> queuedLogs = new();

        private static readonly Dictionary<MessageType, string> Colors = new() {
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

            foreach (string queuedLog in queuedLogs)
            {
                log = log.Insert(logIndex, queuedLog);
                logIndex += queuedLog.Length;
            }
            queuedLogs = null;
        }

        public static void LogMessage(string message)
        {
            if (Program.Properties.LoggingEnabled)
            {
                Debug.WriteLine(message);
                Console.WriteLine(message.Pastel(Colors[MessageType.Message]));
                Write(message, MessageType.Message, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogWarning(string message)
        {
            if (Program.Properties.LoggingEnabled)
            {
                Debug.WriteLine(message);
                Console.WriteLine(message.Pastel(Colors[MessageType.Warning]));
                Write(message, MessageType.Warning, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogError(string message)
        {
            if (Program.Properties.LoggingEnabled)
            {
                Debug.WriteLine(message);
                Console.WriteLine(message.Pastel(Colors[MessageType.Error]));
                Write(message, MessageType.Error, new StackTrace(skipFrames: 1, true));
            }
        }

        public static void LogException(Exception ex, string errorMessage = null)
        {
            if (Program.Properties.LoggingEnabled)
            {
                string message = string.Empty;

                message += $"{(errorMessage != null ? $"{errorMessage}: " : "")}{ex.Message}\n";
                if (!string.IsNullOrEmpty(ex.InnerException?.Message))
                    message += $"Inner exception: {ex.InnerException?.Message}\n";
                if (!string.IsNullOrEmpty(ex.ToString()))
                    message += $"Stack Trace: {ex}";

                Debug.WriteLine(message.TrimEnd('\n'));
                Console.WriteLine(message.TrimEnd('\n').Pastel(Colors[MessageType.Exception]));
                Write($"{(errorMessage != null ? $"{errorMessage}: " : "")}{ex.Message}\n", MessageType.Exception, new StackTrace(ex, skipFrames: 1, true));
            }
        }

        public static void LogDebug(string message)
        {
            if (Program.Properties.DebugEnabled && Program.Properties.LoggingEnabled)
            {
                Debug.WriteLine(message);
                Console.WriteLine(message.Pastel(Colors[MessageType.Debug]));
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
                LogException(ex);
                throw;
            }
        }

        public static void LogAssert(bool condition, string message)
        {
            if (Program.Properties.LoggingEnabled && !condition)
            {
                Debug.WriteLine($"{message}");
                Console.WriteLine($"{message}".Pastel(Colors[MessageType.Assert]));
                Write($"{message}", MessageType.Assert, new StackTrace(skipFrames: 1, true));
            }
        }

        private static void Write(string message, MessageType messageType, StackTrace stack)
        {
            message = GenerateLogEvent(message, messageType, stack);
            if (log != string.Empty)
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
            }
            catch (Exception ex)
            {
                LogException(ex, "Log has failed to save log");
            }
            log = string.Empty;
        }

        public static void Prune()
        {
            try
            {
                if (Directory.GetFiles(IO.Paths[PathType.Diagnostics], "*", SearchOption.TopDirectoryOnly).Length > 10)
                {
                    var oldestFiles = Directory.EnumerateFiles(IO.Paths[PathType.Diagnostics])
                        .Select(fileName => new FileInfo(fileName))
                        .OrderByDescending(fileInfo => fileInfo.LastWriteTime)
                        .Skip(10)
                        .Select(fileInfo => fileInfo.FullName);

                    foreach (var file in oldestFiles)
                        File.Delete(file);
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

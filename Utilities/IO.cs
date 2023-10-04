using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;

namespace Cobbs_Engine
{
    public static class IO
    {
        public static Dictionary<PathType, string> Paths { get; set; } = new Dictionary<PathType, string>();
        #region JSON
        public static string SerializeJson(object obj, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None, JsonConverter[] convertors = null)
        {
            return JsonConvert.SerializeObject(
                obj,
                formatting,
                convertors);
        }

        public static T DeserializeJSON<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
        #endregion

        #region XML
        public static string SerializeXML<T>(this T value, bool removeDefaultXmlNamespaces = true, bool omitXmlDeclaration = true) where T : class
        {
            XmlSerializerNamespaces namespaces = removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null;

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = omitXmlDeclaration,
                CheckCharacters = false
            };

            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, settings);
            var serializer = new XmlSerializer(value.GetType());
            serializer.Serialize(writer, value, namespaces);
            return stream.ToString();
        }

        public static T DeserializeXML<T>(string str)
        {
            using StringReader reader = new(str);
            XmlSerializer serializer = new(typeof(T));
            return (T)serializer.Deserialize(reader);
        }
        #endregion

        #region Binary
        public static string SerializeBinary(object obj)
        {
            using MemoryStream stream = new();
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, obj);
            stream.Flush();
            stream.Position = 0;
            return Convert.ToBase64String(stream.ToArray());
        }

        public static T DeserializeBinary<T>(string str)
        {
            using MemoryStream stream = new(Convert.FromBase64String(str));
            BinaryFormatter formatter = new();
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
        #endregion

        #region File Utilities
        public static bool ValidatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Diagnostics.LogException(ex);
                    return false;
                }
                Diagnostics.LogMessage($"Created folder at {path}");
                return false;
            }
            return true;
        }

        public static string ReadFile(string path, bool throwError = false)
        {
            string output = "";

            try
            {
                output = File.ReadAllText(path);
            }
            catch (Exception ex) when (throwError)
            {
                Diagnostics.LogException(ex, $"The file \"{Path.GetFileName(path)}\" could not be saved.");
            }
            return output;
        }

        public static void SaveFile(string path, string str, bool throwError = false)
        {
            try
            {
                File.WriteAllTextAsync(path, str);
            }
            catch (Exception ex) when (throwError)
            {

                string message = $"The file \"{Path.GetFileName(path)}\" could not be saved.";
                Diagnostics.LogException(ex, message);
            }
        }
        #endregion

        #region Settings
        public static void SaveSettings(Settings settings)
        {
            settings ??= Settings.Default;

            string path = Path.Combine(Paths[PathType.Settings], $"settings.{Configuration.SettingsFileType}");

            string output = null;
            switch (Configuration.SettingsFileSerializer)
            {
                case SerializerType.Json:
                    output = SerializeJson(settings, Newtonsoft.Json.Formatting.Indented, new JsonConverter[] { new StringEnumConverter() });
                    break;

                case SerializerType.XML:
                    output = SerializeXML(settings);
                    break;

                case SerializerType.Binary:
                    output = SerializeBinary(settings);
                    break;
            }

            Diagnostics.LogMessage("Settings Saved");
            SaveFile(path, output);
        }

        public static Settings LoadSettings()
        {
            string path = Path.Combine(Paths[PathType.Settings], $"settings.{Configuration.SettingsFileType}");

            if (!File.Exists(path))
                SaveSettings(Settings.Default);
            string input = ReadFile(path);

            Settings output = null;

            switch (Configuration.SettingsFileSerializer)
            {
                case SerializerType.Json:
                    output = DeserializeJSON<Settings>(input);
                    break;

                case SerializerType.XML:
                    output = DeserializeXML<Settings>(input);
                    break;

                case SerializerType.Binary:
                    output = DeserializeBinary<Settings>(input);
                    break;
            }

            Diagnostics.LogDebug("Settings Loaded");
            return output;
        }
        #endregion

        #region Diagnostics
        public static void SaveDiagnostics(string logs)
        {
            string path = "";
            try
            {
                path = Path.Combine(Paths[PathType.Diagnostics], $"Diagnostic - {Diagnostics.LogTime}.{Configuration.DiagnosticsFileType}");
                File.AppendAllTextAsync(path, logs);
            }
            catch (Exception)
            {
                Diagnostics.LogError($"Log could not be saved at \"{path}\"");
            }
        }
        #endregion
    }

    internal enum SerializerType
    {
        Json = 0,
        XML = 1,
        Binary = 2,
    }

    public enum PathType
    {
        Executable,
        UserData,
        Plugins,
        Content,
        Diagnostics,
        Settings,
        Save,
        Media,
    }
}

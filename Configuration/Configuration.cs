namespace Cobbs_Engine
{
    internal static class Configuration
    {
        internal const string GameName = "MonoGameEngine4";

        #region IO
        // @ to be replaced with the relative path of the game's executable folder
        // ~ to be replaced with the relative path of the game's documents folder
        internal const string PluginsPath = "@\\plugins";
        internal const string ContentPath = "@\\content";
        internal const string DiagnosticsPath = "~\\diagnostics";
        internal const string SettingsPath = "~\\settings";
        internal const string SavePath = "~\\saves";
        internal const string MediaPath = "~\\media";

        // Settings Output
        internal const string SettingsFileType = "json";
        internal const SerializerType SettingsFileSerializer = SerializerType.Json;

        // Save Output
        internal const string SaveFileType = "sav";
        internal const bool SaveFileEncoded = true;
        internal const SerializerType SaveFileSerializer = SerializerType.Binary;

        // Diagnostics Output
        internal const string DiagnosticsFileType = "html";
        #endregion

        #region Controls
        internal const float CameraZoomMinimum = 10f;
        internal const float CameraZoomMaximum = 0.75f;
        internal const float CameraScrollInertia = 0.04f;
        internal const float CameraZoomInertia = 0.15f;
        internal const float CameraZoomThreshold = 0.75f;
        internal const float InputSensitivity = 15f;
        internal const float ScrollSensitivity = 10f;
        #endregion
    }
}

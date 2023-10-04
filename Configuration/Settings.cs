using Newtonsoft.Json;
using System;

namespace Cobbs_Engine
{
    [Serializable]
    public class Settings
    {
        [JsonProperty(PropertyName = "WIDTH")]
        public int Width { get; set; } = 1280;

        [JsonProperty(PropertyName = "HEIGHT")]
        public int Height { get; set; } = 720;

        [JsonProperty(PropertyName = "VSYNC")]
        public bool IsVsync { get; set; } = true;

        [JsonProperty(PropertyName = "FULLSCREEN")]
        public bool IsFullscreen { get; set; } = false;

        [JsonProperty(PropertyName = "BORDERLESS")]
        public bool IsBorderless { get; set; } = true;

        public static Settings Default
        {
            get
            {
                return new Settings
                {
                    Width = 1280,
                    Height = 720,
                    IsVsync = true,
                    IsFullscreen = false,
                    IsBorderless = true,
                };
            }
        }
    }
}

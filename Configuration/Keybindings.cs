using Cobbs_Engine.Input;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cobbs_Engine
{
    public class Keybindings
    {
        [JsonProperty(PropertyName = "UP")]           public List<Keybinding> Up           = new List<Keybinding>();
        [JsonProperty(PropertyName = "DOWN")]         public List<Keybinding> Down         = new List<Keybinding>();
        [JsonProperty(PropertyName = "LEFT")]         public List<Keybinding> Left         = new List<Keybinding>();
        [JsonProperty(PropertyName = "RIGHT")]        public List<Keybinding> Right        = new List<Keybinding>();

        [JsonProperty(PropertyName = "EXIT")]         public List<Keybinding> Exit         = new List<Keybinding>();
        [JsonProperty(PropertyName = "DEBUGOVERLAY")] public List<Keybinding> DebugOverlay = new List<Keybinding>();
        [JsonProperty(PropertyName = "FULLSCREEN")]   public List<Keybinding> Fullscreen   = new List<Keybinding>();
        [JsonProperty(PropertyName = "SCREENSHOT")]   public List<Keybinding> Screenshot   = new List<Keybinding>();

        [JsonProperty(PropertyName = "ZOOMIN")]       public List<Keybinding> ZoomIn       = new List<Keybinding>();
        [JsonProperty(PropertyName = "ZOOMOUT")]      public List<Keybinding> ZoomOut      = new List<Keybinding>();
        [JsonProperty(PropertyName = "ZOOMRESET")]    public List<Keybinding> ZoomReset    = new List<Keybinding>();

        public static Keybindings Default
        {
            get
            {
                Keybindings def = new Keybindings();

                def.Up.Add(new Keybinding() { KeyboardKeys = new() { Keys.Up } });
                def.Down.Add(new Keybinding() { KeyboardKeys = new() { Keys.Down } });
                def.Left.Add(new Keybinding() { KeyboardKeys = new() { Keys.Left } });
                def.Right.Add(new Keybinding() { KeyboardKeys = new() { Keys.Right } });

                def.Exit.Add(new Keybinding() { KeyboardKeys = new() { Keys.Escape } });
                def.DebugOverlay.Add(new Keybinding() { KeyboardKeys = new() { Keys.F1 } });
                def.Fullscreen.Add(new Keybinding() { KeyboardKeys = new() { Keys.F11 } });
                def.Screenshot.Add(new Keybinding() { KeyboardKeys = new() { Keys.F12 } });

                def.ZoomIn.Add(new Keybinding() { MouseActions = new() { MouseAction.ScrollUp } });
                def.ZoomOut.Add(new Keybinding() { MouseActions = new() { MouseAction.ScrollDown } });
                def.ZoomReset.Add(new Keybinding() { MouseActions = new() { MouseAction.ClickMiddle } });

                return def;
            }
        }
    }
}

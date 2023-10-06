using Cobbs_Engine.Input;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cobbs_Engine
{
    public class Keybindings
    {
        [JsonProperty(PropertyName = "UP")]           public List<Keybinding> Up           = new();
        [JsonProperty(PropertyName = "DOWN")]         public List<Keybinding> Down         = new();
        [JsonProperty(PropertyName = "LEFT")]         public List<Keybinding> Left         = new();
        [JsonProperty(PropertyName = "RIGHT")]        public List<Keybinding> Right        = new();

        [JsonProperty(PropertyName = "EXIT")]         public List<Keybinding> Exit         = new();
        [JsonProperty(PropertyName = "DEBUGOVERLAY")] public List<Keybinding> DebugOverlay = new();
        [JsonProperty(PropertyName = "CONSOLE")]      public List<Keybinding> Console      = new();
        [JsonProperty(PropertyName = "FULLSCREEN")]   public List<Keybinding> Fullscreen   = new();
        [JsonProperty(PropertyName = "SCREENSHOT")]   public List<Keybinding> Screenshot   = new();

        [JsonProperty(PropertyName = "ZOOMIN")]       public List<Keybinding> ZoomIn       = new();
        [JsonProperty(PropertyName = "ZOOMOUT")]      public List<Keybinding> ZoomOut      = new();
        [JsonProperty(PropertyName = "ZOOMRESET")]    public List<Keybinding> ZoomReset    = new();

        public static Keybindings Default
        {
            get
            {
                Keybindings def = new();

                def.Up.Add(new Keybinding() { KeyboardKeys = new() { Keys.Up } });
                def.Down.Add(new Keybinding() { KeyboardKeys = new() { Keys.Down } });
                def.Left.Add(new Keybinding() { KeyboardKeys = new() { Keys.Left } });
                def.Right.Add(new Keybinding() { KeyboardKeys = new() { Keys.Right } });

                def.Exit.Add(new Keybinding() { KeyboardKeys = new() { Keys.Escape } });
                def.DebugOverlay.Add(new Keybinding() { KeyboardKeys = new() { Keys.F1 } });
                def.Console.Add(new Keybinding() { KeyboardKeys = new() { Keys.F2 } });
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

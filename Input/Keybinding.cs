using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Cobbs_Engine.Input
{
    [Serializable]
    public class Keybinding
    {
        public List<Keys> KeyboardKeys { get; set; }        = new List<Keys>();
        public List<Buttons> GamePadButtons { get; set; }   = new List<Buttons>();
        public List<MouseAction> MouseActions { get; set; } = new List<MouseAction>();
    }
}

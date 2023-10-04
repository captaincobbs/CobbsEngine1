using System;

namespace Cobbs_Engine.Input
{
    [Flags]
    public enum MouseAction
    {
        None = 0,

        ClickLeft = 1,
        ClickMiddle = 2,
        ClickRight = 4,

        ClickSideButton1 = 8,
        ClickSideButton2 = 16,

        ScrollUp = 32,
        ScrollDown = 64,
        ScrollLeft = 128,
        ScrollRight = 256,
    }
}

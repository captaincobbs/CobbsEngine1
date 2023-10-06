using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cobbs_Engine.Input
{
    public readonly struct MouseStateExtended
    {
        private readonly MouseState currentMouseState;
        private readonly MouseState previousMouseState;

        public MouseStateExtended(MouseState currentMouseState, MouseState previousMouseState)
        {
            this.currentMouseState = currentMouseState;
            this.previousMouseState = previousMouseState;
        }

        public int X => currentMouseState.X;
        public int Y => currentMouseState.Y;
        public Vector2 Position => new(currentMouseState.Position.X, currentMouseState.Position.Y);
        public bool PositionChanged => currentMouseState.Position != previousMouseState.Position;
        public int DeltaX => previousMouseState.X - currentMouseState.X;
        public int DeltaY => previousMouseState.Y - currentMouseState.Y;
        public Vector2 DeltaPosition => new(DeltaX, DeltaY);
        public int HorizontalScrollWheelValue => currentMouseState.HorizontalScrollWheelValue;
        public int VerticalScrollWheelValue => currentMouseState.ScrollWheelValue;
        public int DeltaHorizontalScrollWheelValue => previousMouseState.HorizontalScrollWheelValue - currentMouseState.HorizontalScrollWheelValue;
        public int DeltaVerticalScrollWheelValue => previousMouseState.ScrollWheelValue - currentMouseState.ScrollWheelValue;
        public ButtonState LeftButton => currentMouseState.LeftButton;
        public ButtonState MiddleButton => currentMouseState.MiddleButton;
        public ButtonState RightButton => currentMouseState.RightButton;
        public ButtonState SideButton1 => currentMouseState.XButton1;
        public ButtonState SideButton2 => currentMouseState.XButton2;
        public bool ScrollingUp => DeltaVerticalScrollWheelValue < 0;
        public bool ScrollingDown => DeltaVerticalScrollWheelValue > 0;
        public bool ScrollingLeft => DeltaHorizontalScrollWheelValue < 0;
        public bool ScrollingRight => DeltaHorizontalScrollWheelValue > 0;

        public bool IsButtonPressed(MouseAction button)
        {
            switch (button)
            {
                case MouseAction.ClickLeft:
                    return currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseAction.ClickRight:
                    return currentMouseState.RightButton == ButtonState.Pressed;
                case MouseAction.ClickMiddle:
                    return currentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseAction.ClickSideButton1:
                    return currentMouseState.XButton1 == ButtonState.Pressed;
                case MouseAction.ClickSideButton2:
                    return currentMouseState.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        public bool IsButtonReleased(MouseAction button)
        {
            switch (button)
            {
                case MouseAction.ClickLeft:
                    return currentMouseState.LeftButton == ButtonState.Released;
                case MouseAction.ClickRight:
                    return currentMouseState.RightButton == ButtonState.Released;
                case MouseAction.ClickMiddle:
                    return currentMouseState.MiddleButton == ButtonState.Released;
                case MouseAction.ClickSideButton1:
                    return currentMouseState.XButton1 == ButtonState.Released;
                case MouseAction.ClickSideButton2:
                    return currentMouseState.XButton2 == ButtonState.Released;
            }
            return false;
        }

        public bool WasButtonPressed(MouseAction button)
        {
            switch (button)
            {
                case MouseAction.ClickLeft:
                    return previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;

                case MouseAction.ClickRight:
                    return previousMouseState.RightButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;

                case MouseAction.ClickMiddle:
                    return previousMouseState.MiddleButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;

                case MouseAction.ClickSideButton1:
                    return previousMouseState.XButton1 == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;

                case MouseAction.ClickSideButton2:
                    return previousMouseState.XButton2 == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;
            }
            return false;
        }

        public bool WasButtonReleased(MouseAction button)
        {
            switch (button)
            {
                case MouseAction.ClickLeft:
                    return previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;

                case MouseAction.ClickRight:
                    return previousMouseState.RightButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;

                case MouseAction.ClickMiddle:
                    return previousMouseState.MiddleButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;

                case MouseAction.ClickSideButton1:
                    return previousMouseState.XButton1 == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;

                case MouseAction.ClickSideButton2:
                    return previousMouseState.XButton2 == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
            }
            return false;
        }
    }
}

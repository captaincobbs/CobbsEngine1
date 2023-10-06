using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Cobbs_Engine.Input
{
    public readonly struct GamePadStateExtended
    {
        private readonly GamePadState currentGamePadState;
        private readonly GamePadState previousGamePadState;

        public GamePadStateExtended(GamePadState currentGamePadState, GamePadState previousGamePadState)
        {
            this.currentGamePadState = currentGamePadState;
            this.previousGamePadState = previousGamePadState;
        }

        public GamePadThumbSticks Thumbsticks => currentGamePadState.ThumbSticks;
        public GamePadTriggers Triggers => currentGamePadState.Triggers;
        public GamePadDPad DPad => currentGamePadState.DPad;
        public GamePadButtons Buttons => currentGamePadState.Buttons;
        public int PacketNumber => currentGamePadState.PacketNumber;
        public bool IsConnected => currentGamePadState.IsConnected;

        public bool DPadUpPressed => currentGamePadState.DPad.Up == ButtonState.Pressed;
        public bool DPadUpWasPressed => currentGamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released;
        public bool DPadUpWasReleased => currentGamePadState.DPad.Up == ButtonState.Released && previousGamePadState.DPad.Up == ButtonState.Pressed;

        public bool DPadDownPressed => currentGamePadState.DPad.Down == ButtonState.Pressed;
        public bool DPadDownWasPressed => currentGamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released;
        public bool DPadDownWasReleased => currentGamePadState.DPad.Down == ButtonState.Released && previousGamePadState.DPad.Down == ButtonState.Pressed;

        public bool DPadLeftPressed => currentGamePadState.DPad.Left == ButtonState.Pressed;
        public bool DPadLeftWasPressed => currentGamePadState.DPad.Left == ButtonState.Pressed && previousGamePadState.DPad.Left == ButtonState.Released;
        public bool DPadLeftWasReleased => currentGamePadState.DPad.Left == ButtonState.Released && previousGamePadState.DPad.Left == ButtonState.Pressed;

        public bool DPadRightPressed => currentGamePadState.DPad.Right == ButtonState.Pressed;
        public bool DPadRightWasPressed => currentGamePadState.DPad.Right == ButtonState.Pressed && previousGamePadState.DPad.Right == ButtonState.Released;
        public bool DPadRightWasReleased => currentGamePadState.DPad.Right == ButtonState.Released && previousGamePadState.DPad.Right == ButtonState.Pressed;

        public bool LeftThumbstickUp => LeftThumbStickY < 0;
        public bool LeftThumbstickDown => LeftThumbStickY > 0;
        public bool LeftThumbstickLeft => LeftThumbStickX < 0;
        public bool LeftThumbstickRight => LeftThumbStickX > 0;
        public float LeftThumbStickX => currentGamePadState.ThumbSticks.Left.X;
        public float LeftThumbStickY => currentGamePadState.ThumbSticks.Left.Y;
        public float LeftThumbStickDeltaX => currentGamePadState.ThumbSticks.Left.X - previousGamePadState.ThumbSticks.Left.X;
        public float LeftThumbStickDeltaY => currentGamePadState.ThumbSticks.Left.Y - previousGamePadState.ThumbSticks.Left.Y;
        public Vector2 LeftThumbstick => new(LeftThumbStickX, LeftThumbStickY);
        public Vector2 LeftThumbstickDelta => new(LeftThumbStickX, LeftThumbStickX);

        public bool RightThumbstickUp => RightThumbStickY < 0;
        public bool RightThumbstickDown => RightThumbStickY > 0;
        public bool RightThumbstickLeft => RightThumbStickX < 0;
        public bool RightThumbstickRight => RightThumbStickX > 0;
        public float RightThumbStickX => currentGamePadState.ThumbSticks.Right.X;
        public float RightThumbStickY => currentGamePadState.ThumbSticks.Right.Y;
        public float RightThumbStickDeltaX => currentGamePadState.ThumbSticks.Right.X - previousGamePadState.ThumbSticks.Right.X;
        public float RightThumbStickDeltaY => currentGamePadState.ThumbSticks.Right.Y - previousGamePadState.ThumbSticks.Right.Y;
        public Vector2 RightThumbstick => new(RightThumbStickX, RightThumbStickY);
        public Vector2 RightThumbstickDelta => new(RightThumbStickX, RightThumbStickX);

        public float LeftTrigger => currentGamePadState.Triggers.Left;
        public float LeftTriggerDelta => currentGamePadState.Triggers.Left - previousGamePadState.Triggers.Left;
        public bool LeftTriggerState => currentGamePadState.Triggers.Left >= MainGame.Settings.GamePadTriggerThreshold;

        public float RightTrigger => currentGamePadState.Triggers.Right;
        public float RightTriggerDelta => currentGamePadState.Triggers.Right - previousGamePadState.Triggers.Right;
        public bool RightTriggerState => currentGamePadState.Triggers.Right >= MainGame.Settings.GamePadTriggerThreshold;

        public bool IsButtonPressed(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button);
        }

        public bool IsButtonReleased(Buttons button)
        {
            return currentGamePadState.IsButtonUp(button);
        }

        public bool WasButtonPressed(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button) && previousGamePadState.IsButtonUp(button);
        }

        public bool WasButtonReleased(Buttons button)
        {
            return currentGamePadState.IsButtonUp(button) && previousGamePadState.IsButtonDown(button);
        }
    }
}

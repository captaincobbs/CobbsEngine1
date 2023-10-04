using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Cobbs_Engine.Input
{
    public struct KeyboardStateExtended
    {
        public readonly KeyboardState currentKeyboardState;
        public readonly KeyboardState previousKeyboardState;

        public KeyboardStateExtended(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState)
        {
            this.currentKeyboardState = currentKeyboardState;
            this.previousKeyboardState = previousKeyboardState;
        }

        public Keys[] PressedKeys => currentKeyboardState.GetPressedKeys();
        public int PressedKeyCount => currentKeyboardState.GetPressedKeyCount();
        public List<Keys> HeldKeys => (List<Keys>)currentKeyboardState.GetPressedKeys().Intersect(previousKeyboardState.GetPressedKeys());
        public List<Keys> ReleasedKeys => (List<Keys>)previousKeyboardState.GetPressedKeys().Except(currentKeyboardState.GetPressedKeys());
        public List<Keys> TriggeredKeys => (List<Keys>)currentKeyboardState.GetPressedKeys().Except(previousKeyboardState.GetPressedKeys());
        public bool NumLock => currentKeyboardState.NumLock;
        public bool CapsLock => currentKeyboardState.CapsLock;

        public bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key);
        }

        public bool WasKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        public bool WasKeyReleased(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key);
        }
    }
}

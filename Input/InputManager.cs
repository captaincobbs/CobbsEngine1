using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Cobbs_Engine.Input
{
    public class InputManager
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        public MouseStateExtended MouseState;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        public KeyboardStateExtended KeyboardState;

        private Dictionary<PlayerIndex, GamePadState> currentGamePadState;
        private Dictionary<PlayerIndex, GamePadState> previousGamePadState;
        public Dictionary<PlayerIndex, GamePadStateExtended> GamePadState;

        public Dictionary<InputAction, List<Keybinding>> Keybindings;

        public Dictionary<InputAction, bool> CurrentInputActionsOccuring;
        public Dictionary<InputAction, bool> PreviousInputActionsOccuring;

        public event EventHandler<InputAction> InputActionTriggered;
        public event EventHandler<InputAction> InputActionReleased;

        public IntPtr WindowHandle
        {
            get => Mouse.WindowHandle;
            set => Mouse.WindowHandle = value;
        }

        public InputManager()
        {
            currentMouseState = previousMouseState = Mouse.GetState();
            MouseState = new(currentMouseState, previousMouseState);

            currentKeyboardState = previousKeyboardState = Keyboard.GetState();
            KeyboardState = new(currentKeyboardState, previousKeyboardState);

            currentGamePadState = new();
            previousGamePadState = new();
            GamePadState = new();
            foreach (PlayerIndex player in Enum.GetValues(typeof(PlayerIndex)))
            {
                currentGamePadState.Add(player, GamePad.GetState(player));
                previousGamePadState.Add(player, GamePad.GetState(player));
                GamePadState.Add(player, new(currentGamePadState[player], previousGamePadState[player]));
            }

            CurrentInputActionsOccuring = new();
            PreviousInputActionsOccuring = new();
            if (Keybindings is null || Keybindings.Count == 0)
                Keybindings = new();
            
                
            foreach (InputAction inputAction in Enum.GetValues(typeof(InputAction)))
            {
                CurrentInputActionsOccuring.Add(inputAction, false);
                PreviousInputActionsOccuring.Add(inputAction, false);
                Keybindings.Add(inputAction, new());
            }
        }

        public void SetMousePosition(int x, int y) => Mouse.SetPosition(x, y);
        public void SetMousePosition(Point point) => Mouse.SetPosition(point.X, point.Y);
        public void SetCursor(MouseCursor cursor) => Mouse.SetCursor(cursor);

        public void SetVibration(PlayerIndex index, float leftMotor, float rightMotor, float leftTrigger, float rightTrigger) => GamePad.SetVibration(index, leftMotor, rightMotor, leftTrigger, rightTrigger);
        public void SetVibration(PlayerIndex index, float leftMotor, float rightMotor) => GamePad.SetVibration(index, leftMotor, rightMotor);

        public void UpdateMouseState()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            MouseState = new(currentMouseState, previousMouseState);
        }

        public void UpdateKeyboardState()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            KeyboardState = new(currentKeyboardState, previousKeyboardState);
        }

        public void UpdateGamePadState()
        {
            previousGamePadState = currentGamePadState;
            currentGamePadState = new();
            GamePadState = new();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                currentGamePadState[index] = GamePad.GetState(index);
                GamePadState[index] = new(currentGamePadState[index], previousGamePadState[index]);
            }
        }

        public void UpdateCurrentInputActions()
        {
            PreviousInputActionsOccuring = new(CurrentInputActionsOccuring);
            CurrentInputActionsOccuring = new Dictionary<InputAction, bool>();

            foreach (InputAction inputAction in Enum.GetValues(typeof(InputAction)))
            {
                CurrentInputActionsOccuring[inputAction] = false;
            }
        }

        public bool KeybindingSatisfied(Keybinding keyBinding, PlayerIndex player)
        {
            bool noKeys = false;
            bool noButtons = false;
            bool noActions = false;

            if (keyBinding.KeyboardKeys != null && keyBinding.KeyboardKeys.Count > 0)
            {
                foreach (Keys key in keyBinding.KeyboardKeys)
                {
                    if (!KeyboardState.IsKeyPressed(key))
                    {
                        return false;
                    }
                }
            }
            else
                noKeys = true;

            if (keyBinding.GamePadButtons != null && keyBinding.GamePadButtons.Count > 0)
            {
                foreach (Buttons button in keyBinding.GamePadButtons)
                {
                    if (!GamePadState[player].IsButtonPressed(button))
                    {
                        return false;
                    }
                }
            }
            else
                noButtons = true;

            if (keyBinding.GamePadButtons != null && keyBinding.GamePadButtons.Count > 0)
            {
                foreach (MouseAction action in keyBinding.MouseActions)
                {
                    if (!MouseState.IsButtonPressed(action))
                    {
                        return false;
                    }
                }
            }
            else
                noActions = true;

            // If there are no bindings of any category, return false, otherwise, return true
            return !(noKeys == noButtons == noActions);
        }

        public void AddKeybindings(Keybindings keybinding)
        {
            if (keybinding == null)
            {
                return;
            }

            Keybindings[InputAction.Up] = keybinding.Up;
            Keybindings[InputAction.Down] = keybinding.Down;
            Keybindings[InputAction.Left] = keybinding.Left;
            Keybindings[InputAction.Right] = keybinding.Right;

            Keybindings[InputAction.Exit] = keybinding.Exit;
            Keybindings[InputAction.Console] = keybinding.Console;
            Keybindings[InputAction.DebugOverlay] = keybinding.DebugOverlay;
            Keybindings[InputAction.Fullscreen] = keybinding.Fullscreen;
            Keybindings[InputAction.Screenshot] = keybinding.Screenshot;

            Diagnostics.LogMessage("Keybindings Applied");
        }

        public void Update(GameTime gameTime)
        {
            UpdateMouseState();
            UpdateKeyboardState();
            UpdateGamePadState();
            UpdateCurrentInputActions();

            foreach (InputAction inputAction in Enum.GetValues(typeof(InputAction)))
            {
                foreach (Keybinding keybindings in Keybindings[inputAction])
                {
                    if (KeybindingSatisfied(keybindings, PlayerIndex.One))
                    {
                        CurrentInputActionsOccuring[inputAction] = true;

                        if (CurrentInputActionsOccuring[inputAction] && !PreviousInputActionsOccuring[inputAction])
                        {
                            InvokeInputActionTriggered(inputAction);
                        }
                        
                        continue;
                    }
                    if (!CurrentInputActionsOccuring[inputAction] && PreviousInputActionsOccuring[inputAction])
                    {
                        InvokeInputActionReleased(inputAction);
                    }
                }
            }
        }

        public void InvokeInputActionTriggered(InputAction inputAction)
        {
            EventHandler<InputAction> handler = InputActionTriggered;

            if (handler != null)
            {
                handler.Invoke(this, inputAction);
            }
        }

        public void InvokeInputActionReleased(InputAction inputAction)
        {
            EventHandler<InputAction> handler = InputActionReleased;

            if (handler != null)
            {
                handler.Invoke(this, inputAction);
            }
        }
    }
}

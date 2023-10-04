using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Cobbs_Engine.Components
{
    public class Camera
    {
        public Matrix Matrix { get; private set; } = Matrix.Identity;
        public static IFocusObject CameraFocus { get; set; }

        public float ZoomValue { get; private set; } = 2f;
        public float ZoomTarget { get; private set; } = 2f;

        public Vector2 Coordinates { get; set; } = Vector2.Zero;
        public Vector2 TargetCoordinates { get; private set; } = Vector2.Zero;

        public Viewport Viewport { get; private set; }

        public Camera(Viewport viewport, IFocusObject cameraFocus)
        {
            CameraFocus = cameraFocus;
            Viewport = viewport;
        }

        public void Update()
        {
            // Apply zoom delta to current zoom value
            ZoomValue += ((ZoomTarget - ZoomValue) * Configuration.CameraZoomInertia);

            // If camera isn't manually controlled, interporate between target position and current position
            if (CameraFocus.GetType() != typeof(PlayerControlled))
            {
                TargetCoordinates = Vector2.Round(new Vector2(
                    CameraFocus.GetPosition().X,
                    CameraFocus.GetPosition().Y));
                Coordinates = new Vector2(
                    Coordinates.X + ((TargetCoordinates.X - Coordinates.X) * Configuration.CameraScrollInertia),
                    Coordinates.Y + ((TargetCoordinates.Y - Coordinates.Y) * Configuration.CameraScrollInertia));
            }

            // Change camera matrix properties with updated information
            Matrix =
                Matrix.CreateTranslation(new Vector3(-Coordinates.X, -Coordinates.Y, 0)) *
                Matrix.CreateScale(new Vector3(ZoomValue, ZoomValue, 1)) *
                Matrix.CreateTranslation(new Vector3(MainGame.GameWindow.ClientBounds.Width * 0.5f, MainGame.GameWindow.ClientBounds.Height * 0.5f, 0));
        }

        public void HandleInput()
        {
            if (CameraFocus.GetType() == typeof(PlayerControlled))
            {
                // TODO MANUAL CAMERA
            }

            // Scroll In
            if (InputManager.IsActionPressed(InputManager.InputAction.ZoomIn))
            {
                ZoomTarget += Configuration.CameraZoomThreshold * InputManager.GetActionScroll(InputManager.InputAction.ZoomIn);
            }

            // Scroll Out
            if (InputManager.IsActionPressed(InputManager.InputAction.ZoomOut))
            {
                ZoomTarget -= Configuration.CameraZoomThreshold * InputManager.GetActionScroll(InputManager.InputAction.ZoomOut);
            }

            // Reset camera zoom
            if (InputManager.IsActionPressed(InputManager.InputAction.ZoomReset))
            {
                ZoomTarget = 2f;
            }

            // Keep camera zoom within a specific range
            ZoomTarget = Math.Clamp(ZoomTarget, Configuration.CameraZoomMinimum, Configuration.CameraZoomMaximum);
        }
    }

    public interface IFocusObject
    {
        public Vector2 GetPosition()
        {
            return Vector2.Zero;
        }
    }

    public class GameObjectFocus : IFocusObject
    {
        private GameObject focusObject { get; }

        public GameObjectFocus(GameObject focusObject)
        {
            this.focusObject = focusObject;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(focusObject.Bounds.X, focusObject.Bounds.Y);
        }
    }

    public class StaticFocus : IFocusObject
    {
        private Vector2 Position { get; set; }

        public StaticFocus(Vector2 position)
        {
            this.Position = position;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }
    }

    public class PlayerControlled : IFocusObject
    {
        private Vector2 Position { get; set; }

        public PlayerControlled(Vector2 position)
        {
            this.Position = position;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }
    }
}

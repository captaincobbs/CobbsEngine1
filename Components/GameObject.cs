using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Cobbs_Engine.Components
{
    public abstract class GameObject
    {
        private readonly Scene parent;
        private Texture2D texture;
        private Vector2 position = Vector2.Zero;
        private List<ISceneEvent> subscribedEvents = new();

        public int ZIndex { get; set; }
        public int Priority { get; set; }

        public Rectangle Bounds => new((int)position.X, (int)position.Y, texture.Width, texture.Height);
        public Vector2 Position => position;
        public bool Enabled { get; set; } = true;
        public bool Hidden { get; set; } = false;

        public virtual void Update(GameTime gameTime){ }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void HandleEvent(ISceneEvent sceneEvent)
        {

        }
    }
}

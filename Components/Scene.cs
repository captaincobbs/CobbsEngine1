using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cobbs_Engine.Components
{
    public abstract class Scene
    {
        protected List<GameObject> gameObjects = new();
        protected Dictionary<ISceneEvent, List<GameObject>> eventSubscribers = new();
        protected ContentManager contentManager;
        protected GraphicsDevice graphicsDevice;

        public Camera Camera;

        public event EventHandler<Scene> OnSceneSwitched;
        public event EventHandler<ISceneEvent> OnEventNotification;

        public virtual void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice, Camera camera = null)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;

            this.Camera = camera ?? new Camera(graphicsDevice.Viewport, new StaticFocus(Vector2.Zero));

        }

        public virtual void LoadContent()
        {

        }

        public virtual void UnloadContent()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var gameObject in gameObjects.OrderBy(o => o.ZIndex))
            {
                if (!gameObject.Hidden)
                {
                    gameObject.Draw(spriteBatch);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var gameObject in gameObjects.OrderBy(o => o.Priority))
            {
                if (!gameObject.Enabled)
                {
                    gameObject.Update(gameTime);
                }
            }
        }

        public void SubscribeEvent(GameObject gameObject, List<ISceneEvent> events)
        {
            foreach (ISceneEvent sceneEvent in events)
            {
                if (!eventSubscribers.ContainsKey(sceneEvent))
                {
                    List<GameObject> eventObjects = new()
                    {
                        gameObject
                    };

                    eventSubscribers.Add(sceneEvent, eventObjects);
                }
                else
                {
                    eventSubscribers[sceneEvent].Add(gameObject);
                }
            }
        }

        public virtual void HandleInput(GameTime gameTime)
        {

        }

        public void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Physics = FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Tiled = Squared.Tiled;
using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;
using System.IO;
using Sputnik.Game;
using Sputnik.Menus;

namespace Sputnik {
	public class GameEnvironment : Environment {
		// Spawning/culling.
		public const float k_cullRadius = 300.0f; // Must be greater than spawn radius.
		public const float k_spawnRadius = 100.0f; // Must be less than cull radius.
		public SpawnController SpawnController { get; private set; }

		// Camera.
		public static Vector2 k_idealScreenSize { get { return new Vector2(1680, 1024); } }
		public Vector2 ScreenVirtualSize = new Vector2(1680, 1050);
		public Camera2D Camera;
        public Vector2 defaultCameraMoveSpeed = new Vector2(100.0f, 0.0f);

        /// <summary>
        /// The player. May be null if the player is not spawned at the moment.
        /// </summary>
        public Balloon Balloon;

		// Drawing.
		private SpriteBatch m_spriteBatch;
		private Tiled.Map m_map;

		private Matrix m_projection;

		// Particles.
		public SpriteBatchRenderer ParticleRenderer;
		public List<ParticleEntity> ParticleKeepalive = new List<ParticleEntity>();

		// Physics.
		private Physics.DebugViewXNA m_debugView;

		public static float k_physicsScale = 1.0f / 50.0f; // 50 pixels = 1 meter.
		public static float k_invPhysicsScale = 50.0f; // ^ must be inverse.

		// Update loop.
		public float m_updateAccum; // How much time has passed relative to the physics world.

        //Pausing
        private bool m_paused;
        PopUp m_popUp;

		// HUD.
		public Menus.HUD HUD;

		// Screen effects.
		private Effect m_tintEffect;

		// Atmosphere.
		public float Pressure { get; private set; }
		public float Temperature { get; private set; }

		public GameEnvironment(Controller ctrl)
				: base(ctrl) {

			Pressure = 0.0f;

			Sound.StopAll(true);

			// Camera.
			Camera = new Camera2D(this);
			//Camera.Position = new Vector2(1280.0f, k_idealScreenSize.Y * 0.5f);
            Camera.MoveSpeed = defaultCameraMoveSpeed;
			Camera.ResetEffectScale(1.0f); // Set at slightly higher than 1.0 so we can do a zoom out pressure effect.

			// Window.
			Controller.Window.ClientSizeChanged += WindowSizeChanged;
			WindowSizeChanged(null, null);
			Controller.IsMouseVisible = true;

			// Collision.
			CollisionWorld = new Physics.Dynamics.World(Vector2.Zero);

			// Create a new SpriteBatch which can be used to draw textures.
			m_spriteBatch = new SpriteBatch(ctrl.GraphicsDevice);

			ParticleRenderer = new SpriteBatchRenderer {
				GraphicsDeviceService = ctrl.Graphics
			};

			ParticleRenderer.LoadContent(contentManager);

			// Create collision notification callbacks.
			CollisionWorld.ContactManager.PreSolve += PreSolve;
			CollisionWorld.ContactManager.BeginContact += BeginContact;
			CollisionWorld.ContactManager.EndContact += EndContact;
			CollisionWorld.ContactManager.ContactFilter += ContactFilter;

			// HUD.
			HUD = new Menus.HUD(this);

			// Effects.
			m_tintEffect = contentManager.Load<Effect>("TintEffect");

			// Farseer freaks out unless we call Update here when changing Environments.  FIXME: Why?
			Update(0.0f);
		}

		/// <summary>
		/// Called when the window size changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WindowSizeChanged(object sender, EventArgs e) {
			Rectangle rect = Controller.Window.ClientBounds;
			if (rect.Width == 0 || rect.Height == 0) return; // Do nothing, window was minimized.

			Controller.Graphics.PreferredBackBufferWidth = rect.Width;
			Controller.Graphics.PreferredBackBufferHeight = rect.Height;
			Controller.Graphics.ApplyChanges();

			// Correct virtual screen aspect ratio.
			float ratio = (float) rect.Width / rect.Height;
			ScreenVirtualSize = k_idealScreenSize;
			ScreenVirtualSize.X = ScreenVirtualSize.Y * ratio;

			// TODO: Make SpriteBatch drawing use this projection too.
			m_projection = Matrix.CreateOrthographicOffCenter(0.0f, rect.Width, rect.Height, 0.0f, -1.0f, 1.0f);

			Camera.WindowSizeChanged();
		}


		private Tiled.Tileset.TilePropertyList GetTileProperties(int tileId) {
			foreach (Tiled.Tileset t in m_map.Tilesets.Values) {
				Tiled.Tileset.TilePropertyList props = t.GetTileProperties(tileId);
				if (props != null) return props;
			}

			return null;
		}

		/// <summary>
		/// Load a map from file and create collision objects for it.
		/// </summary>
		/// <param name="filename">File to load map from.</param>
		public void LoadMap(string filename) {
			m_map = Tiled.Map.Load(Path.Combine(Controller.Content.RootDirectory, filename), Controller.Content);

			// Destroy and re-create collision body for map.
			DestroyCollisionBody();
			CreateCollisionBody(CollisionWorld, Physics.Dynamics.BodyType.Static);

			Vector2 tileHalfSize = new Vector2(m_map.TileWidth, m_map.TileHeight) / 2;
			Vector2 tileSize = new Vector2(m_map.TileWidth, m_map.TileHeight);

			bool[,] levelCollision = new bool[m_map.Width, m_map.Height];

			float defaultZVal = 0.96f;

			// 2 = collision. 1 = no collision. 0 = unknown.
			List<byte> collision = new List<byte>();

			foreach (KeyValuePair<string, Tiled.Layer> layer in m_map.Layers) {
				defaultZVal -= 0.001f;

				for (int x = 0; x < layer.Value.Width; ++x)
				for (int y = 0; y < layer.Value.Height; ++y) {
					int tileId = layer.Value.GetTile(x, y) - 1;
					if (tileId < 0) continue;

					if (tileId >= collision.Count || collision[tileId] == 0) {
						Tiled.Tileset.TilePropertyList props = GetTileProperties(tileId);
						
						// The only way to add new elements at arbitrary indices is to fill up the rest of the array.  Do so.
						for (int i = collision.Count; i < tileId + 1; ++i) collision.Add(0);

						if (props != null && props.ContainsKey("collision")) {
							collision[tileId] = (byte) (props["collision"].Equals("true", StringComparison.OrdinalIgnoreCase) ? 2 : 1);
						} else {
							collision[tileId] = 1;
						}
					}

					levelCollision[x, y] |= (collision[tileId] != 1);
				}

				float z = defaultZVal;

				if (layer.Value.Properties.ContainsKey("zindex")) {
					if (!float.TryParse(layer.Value.Properties["zindex"], out z)) {
						z = defaultZVal;
					}
				}

				AddChild(new MapLayer(this, m_map, layer.Key, z));
			}

			// Go through collision and try to create large horizontal collision shapes.
			for (int y = 0; y < m_map.Height; ++y) {
				int firstX = 0;
				bool hasCollision = false;

				for (int x = 0; x < m_map.Width; ++x) {
					if (levelCollision[x, y]) {
						if (hasCollision) continue;
						else {
							hasCollision = true;
							firstX = x;
						}
					} else {
						if (hasCollision) {
							hasCollision = false;
							int tilesWide = x - firstX;
							if (tilesWide == 1) continue;

							for (int i = firstX; i <= x; ++i) levelCollision[i, y] = false;

							AddCollisionRectangle(
								tileHalfSize * new Vector2(tilesWide, 1.0f)
								, new Vector2(tileSize.X * (x - (float) tilesWide / 2), tileSize.Y * (y + 0.5f))
							);
						}
					}
				}

				// Create final collision.
				if (hasCollision) {
					for (int i = firstX; i < m_map.Width; ++i) levelCollision[i, y] = false;

					int tilesWide = m_map.Width - firstX;
					AddCollisionRectangle(
						tileHalfSize * new Vector2(tilesWide, 1.0f)
						, new Vector2(tileSize.X * (m_map.Width - (float) tilesWide / 2), tileSize.Y * (y + 0.5f))
					);
				}
			}

			// Go through collision and try to create large vertical collision shapes.
			for (int x = 0; x < m_map.Width; ++x) {
				int firstY = 0;
				bool hasCollision = false;

				for (int y = 0; y < m_map.Height; ++y) {
					if (levelCollision[x, y]) {
						if (hasCollision) continue;
						else {
							hasCollision = true;
							firstY = y;
						}
					} else {
						if (hasCollision) {
							hasCollision = false;
							int tilesTall = y - firstY;

							AddCollisionRectangle(
								tileHalfSize * new Vector2(1.0f, tilesTall)
								, new Vector2(tileSize.X * (x + 0.5f), tileSize.Y * (y - (float) tilesTall / 2))
							);
						}
					}
				}

				// Create final collision.
				if (hasCollision) {
					int tilesTall = m_map.Height - firstY;
					AddCollisionRectangle(
						tileHalfSize * new Vector2(1.0f, tilesTall)
						, new Vector2(tileSize.X * (x + 0.5f), tileSize.Y * (m_map.Height - (float) tilesTall / 2))
					);
				}
			}

			SpawnController = new SpawnController(this, m_map.ObjectGroups.Values);
		}


        public void pause(PopUp popup)
        {
            m_paused = true;
            Camera.MoveSpeed = new Vector2(0, 0);
            m_popUp = popup;
        }
        public void unPause()
        {
            m_popUp = null;
            Camera.MoveSpeed = new Vector2(100.0f, 0.0f);
            m_paused = false;
        }

		/// <summary>
		/// Update the Environment each frame.
		/// </summary>
		/// <param name="elapsedTime">Time since last Update() call.</param>
		public override void Update(float elapsedTime) {
			// Toggle debug view = F1.
			if (Keyboard.GetState().IsKeyDown(Keys.F1) && !OldKeyboard.GetState().IsKeyDown(Keys.F1)) {
				if (m_debugView != null) m_debugView = null;
				else m_debugView = new Physics.DebugViewXNA(CollisionWorld);
			}
            
            //Pause with Escape
            if (!m_paused)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !OldKeyboard.GetState().IsKeyDown(Keys.Escape))
                    pause(new GamePauseMenu(Controller, this));
            }

			// Debug Menu = F10.
			if (Keyboard.GetState().IsKeyDown(Keys.F10) && !OldKeyboard.GetState().IsKeyDown(Keys.F10)) {
				Controller.ChangeEnvironment(new Menus.DebugMenu(Controller));
			}

			// Exit game.
			if (Keyboard.GetState().IsKeyDown(Keys.F5) && !OldKeyboard.GetState().IsKeyDown(Keys.F5)) {
				Controller.Exit();
			}

            if (!m_paused)
            {
                HUD.enabled = true;
                if (elapsedTime > 0.0f)
                {
                    // Update physics.
                    CollisionWorld.Step(elapsedTime);

                    if (SpawnController != null) SpawnController.Update(elapsedTime);

                    // Update entities.
                    Camera.Update(elapsedTime);
                    base.Update(elapsedTime);
					
                    // Particle keepalive.  Update remaining and try to remove if they are done.
                    ParticleKeepalive.RemoveAll(ent => {
						ent.Update(elapsedTime);
						if (ent.Effect.ActiveParticlesCount == 0) {
							ent.Remove();
							return true;
						} else {
							return false;
						}
					});
                }
            }
            else
            {
                Camera.Update(elapsedTime);
                m_popUp.Update(elapsedTime);
                HUD.enabled = false;
            }

            HUD.Update(elapsedTime);
		}

		/// <summary>
		/// Draw the world.
		/// </summary>
		public override void Draw() {
			Matrix tform = Camera.Transform;

			// Draw entities.
			/*if (false) {
				// TODO: Draw effect.
				m_tintEffect.Parameters["TintColor"].SetValue(new Color(1.5f, 5.0f, 1.0f).ToVector3());
				m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, m_tintEffect, tform);
			} else {*/
				m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, tform);
			//}
			Draw(m_spriteBatch);
			m_spriteBatch.End();

			if (m_debugView != null) {
				// Debug drawing.
				Matrix debugMatrix = Matrix.CreateScale(k_invPhysicsScale) * tform;
				m_debugView.RenderDebugData(ref m_projection, ref debugMatrix);
			}

			// Draw HUD.
			HUD.Draw();

            //Draw popup
            if (m_popUp != null)
                m_popUp.Draw();
		}

		/// <summary>
		/// Farseer Physics callback.  Called when a contact point is created.
		/// </summary>
		/// <param name="contact">Contact point.</param>
		public void BeginContact(Physics.Dynamics.Contacts.Contact contact) {
			// PreSolve performs the same function as this, only continue if one is a sensor.
			if (!contact.FixtureA.IsSensor && !contact.FixtureB.IsSensor) return;
			
			HandleContact(contact);
		}

		/// <summary>
		/// Farseer Physics callback.  Called when two AABB's overlap to determine if they should collide.
		/// </summary>
		/// <param name="fixtureA">First fixture involved.</param>
		/// <param name="fixtureB">Second fixture involved.</param>
		/// <returns></returns>
		private bool ContactFilter(Physics.Dynamics.Fixture fixtureA, Physics.Dynamics.Fixture fixtureB) {
			// Get Entities from both shapes.
			Entity entA = (Entity) fixtureA.Body.UserData;
			Entity entB = (Entity) fixtureB.Body.UserData;

			// Determine if shapes agree to collide.
			return entA.ShouldCollide(entB, fixtureA, fixtureB) && entB.ShouldCollide(entA, fixtureB, fixtureA);
		}

		/// <summary>
		/// Farseer Physics callback.  Handles the case where two objects collide.
		/// </summary>
		/// <param name="contact">Contact point.</param>
		/// <param name="oldManifold">Manifold from last update.</param>
		private void PreSolve(Physics.Dynamics.Contacts.Contact contact, ref Physics.Collision.Manifold oldManifold) {
			HandleContact(contact);
		}

		/// <summary>
		/// Farseer Physics callback.  Called when a contact is destroyed.
		/// </summary>
		/// <param name="contact">Contact point.</param>
		private void EndContact(Physics.Dynamics.Contacts.Contact contact) {
			// Get Entities from both shapes.
			Entity entA = (Entity) contact.FixtureA.Body.UserData;
			Entity entB = (Entity) contact.FixtureB.Body.UserData;

			entA.OnSeparate(entB, contact);
			entB.OnSeparate(entA, contact);
		}

		/// <summary>
		/// Handle contact interactions.
		/// </summary>
		/// <param name="contact">Contact point.</param>
		private void HandleContact(Physics.Dynamics.Contacts.Contact contact) {
			if (!contact.IsTouching()) return;

			// Get Entities from both shapes.
			Entity entA = (Entity) contact.FixtureA.Body.UserData;
			Entity entB = (Entity) contact.FixtureB.Body.UserData;

			entA.OnCollide(entB, contact);
			entB.OnCollide(entA, contact);
		}

		/*********************************************************************/
		// Wind/pressure reaction.

		public virtual void OnTempChange(float amount) {
			Temperature = amount;

			// Call for children.
			Children.ForEach((Entity ent) => { if (ent is GameEntity) ((GameEntity) ent).OnTempChange(amount); });
		}

		public virtual void OnPressureChange(float amount) {
			Pressure = amount;

			// Call for children.
			Children.ForEach((Entity ent) => { if (ent is GameEntity) ((GameEntity) ent).OnPressureChange(amount); });
		}
	}
}
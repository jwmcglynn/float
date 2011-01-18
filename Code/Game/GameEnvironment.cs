using System;
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

		public const float k_scrollSpeed = 150.0f;

		// Map.
		private float m_mapStart = 0.0f;

		// Camera.
		public static Vector2 k_idealScreenSize { get { return new Vector2(1680, 1024); } }
		public Vector2 ScreenVirtualSize = new Vector2(1680, 1050);
        public Vector2 defaultCameraMoveSpeed = new Vector2(k_scrollSpeed, 0.0f);

        /// <summary>
        /// The player. May be null if the player is not spawned at the moment.
        /// </summary>
        public Balloon Balloon;

		// Drawing.
		private SpriteBatch m_spriteBatch;

		private Matrix m_projection;

		public FaderOuter FadeOut;
		public bool fade = false;

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
        public bool paused;
        PopUp m_popUp;

		// HUD.
		public Menus.HUD HUD;

		// Screen effects.
		private Effect m_tintEffect;
		private float heatMultiplier;

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

			FadeOut = new FaderOuter(this, ScreenVirtualSize.X, ScreenVirtualSize.Y);

			SpawnController = new SpawnController(this);

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


		private Tiled.Tileset.TilePropertyList GetTileProperties(Tiled.Map map, int tileId) {
			foreach (Tiled.Tileset t in map.Tilesets.Values) {
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
			m_mapStart = 0.0f;
			DestroyCollisionBody();
			LoadOrExtendMap(filename, true);
		}

		/// <summary>
		/// Load a map from file and create collision objects for it.  Appends map horizontally if one exists.
		/// </summary>
		/// <param name="filename">File to load map from.</param>
		public void LoadOrExtendMap(string filename, bool spawnPlayer = false) {
			Tiled.Map map = Tiled.Map.Load(Path.Combine(Controller.Content.RootDirectory, filename), Controller.Content);

			// Destroy and re-create collision body for map.
			if (CollisionBody == null) CreateCollisionBody(CollisionWorld, Physics.Dynamics.BodyType.Static);

			Vector2 tileHalfSize = new Vector2(map.TileWidth, map.TileHeight) / 2;
			Vector2 tileSize = new Vector2(map.TileWidth, map.TileHeight);

			bool[,] levelCollision = new bool[map.Width, map.Height];

			float defaultZVal = ZSettings.Ground;

			// 2 = collision. 1 = no collision. 0 = unknown.
			List<byte> collision = new List<byte>();

			foreach (KeyValuePair<string, Tiled.Layer> layer in map.Layers) {
				defaultZVal -= 0.001f;

				for (int x = 0; x < layer.Value.Width; ++x)
				for (int y = 0; y < layer.Value.Height; ++y) {
					int tileId = layer.Value.GetTile(x, y);

					if (tileId >= collision.Count || collision[tileId] == 0) {
						Tiled.Tileset.TilePropertyList props = GetTileProperties(map, tileId);
						
						// The only way to add new elements at arbitrary indices is to fill up the rest of the array.  Do so.
						for (int i = collision.Count; i < tileId + 1; ++i) collision.Add(0);

						if (props != null && props.ContainsKey("collision")) {
							collision[tileId] = (byte) (props["collision"].Equals("true", StringComparison.OrdinalIgnoreCase) ? 2 : 1);
						} else {
							collision[tileId] = 1;
						}
					}

					levelCollision[x, y] |= (collision[tileId] > 1);
				}

				float z = defaultZVal;

				if (layer.Value.Properties.ContainsKey("zindex")) {
					if (!float.TryParse(layer.Value.Properties["zindex"], out z)) {
						z = defaultZVal;
					}
				}

				MapLayer ml = new MapLayer(this, map, layer.Key, z);
				ml.Position = new Vector2(m_mapStart, 0.0f);
				AddChild(ml);
			}

			// Go through collision and try to create large horizontal collision shapes.
			for (int y = 0; y < map.Height; ++y) {
				int firstX = 0;
				bool hasCollision = false;

				for (int x = 0; x < map.Width; ++x) {
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
								, new Vector2(tileSize.X * (x - (float) tilesWide / 2) + m_mapStart, tileSize.Y * (y + 0.5f))
							);
						}
					}
				}

				// Create final collision.
				if (hasCollision) {
					for (int i = firstX; i < map.Width; ++i) levelCollision[i, y] = false;

					int tilesWide = map.Width - firstX;
					AddCollisionRectangle(
						tileHalfSize * new Vector2(tilesWide, 1.0f)
						, new Vector2(tileSize.X * (map.Width - (float) tilesWide / 2) + m_mapStart, tileSize.Y * (y + 0.5f))
					);
				}
			}

			// Go through collision and try to create large vertical collision shapes.
			for (int x = 0; x < map.Width; ++x) {
				int firstY = 0;
				bool hasCollision = false;

				for (int y = 0; y < map.Height; ++y) {
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
					int tilesTall = map.Height - firstY;
					AddCollisionRectangle(
						tileHalfSize * new Vector2(1.0f, tilesTall)
						, new Vector2(tileSize.X * (x + 0.5f), tileSize.Y * (map.Height - (float) tilesTall / 2))
					);
				}
			}

			SpawnController.CreateSpawnPoints(map.ObjectGroups.Values, new Vector2(m_mapStart, 0.0f), spawnPlayer);
			m_mapStart += map.Width * map.TileWidth;
		}


        public void pause(PopUp popup)
        {
            paused = true;
            Camera.MoveSpeed = new Vector2(0, 0);
            m_popUp = popup;
        }
        public void unPause()
        {
            m_popUp = null;
			Camera.MoveSpeed = defaultCameraMoveSpeed;
            paused = false;
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
            if (!paused)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !OldKeyboard.GetState().IsKeyDown(Keys.Escape)
                    || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start) && !OldGamePad.GetState().IsButtonDown(Buttons.Start))
                )
				{
                    pause(new GamePauseMenu(Controller, this));
					Sound.PlayCue("scroll");
				}
            }

			// Debug Menu = F10.
			if (Keyboard.GetState().IsKeyDown(Keys.F10) && !OldKeyboard.GetState().IsKeyDown(Keys.F10)) {
				Controller.ChangeEnvironment(new Menus.DebugMenu(Controller));
			}

			// Exit game.
			if (Keyboard.GetState().IsKeyDown(Keys.F5) && !OldKeyboard.GetState().IsKeyDown(Keys.F5)) {
				Controller.Exit();
			}

            if (!paused)
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
			FadeOut.Update(elapsedTime);

			if (Balloon != null)
				heatMultiplier = (this.ScreenVirtualSize.Y/2 - Balloon.Position.Y)* 0.0007f;
			else heatMultiplier = 0;
		}


		public void SpriteBatchPush(bool effect = true) {
			if (effect && heatMultiplier != 0) {
				// TODO: Draw effect.
				if(heatMultiplier > 0)
					m_tintEffect.Parameters["TintColor"].SetValue(new Color(1.0f, 1.0f * (1 - heatMultiplier), 1.0f * (1 - heatMultiplier)).ToVector3());
				else
					m_tintEffect.Parameters["TintColor"].SetValue(new Color(1.0f * (1 + heatMultiplier), 1.0f * (1 + heatMultiplier), 1.0f).ToVector3());
				m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, m_tintEffect, Camera.Transform);
			} else {
			m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Camera.Transform);
			}
		}

		public void SpriteBatchPop() {
			m_spriteBatch.End();
		}

		/// <summary>
		/// Draw the world.
		/// </summary>
		public override void Draw() {
			Matrix tform = Camera.Transform;

			// Draw entities.
			SpriteBatchPush();
			DrawChildren(m_spriteBatch);
			SpriteBatchPop();

			if (m_debugView != null) {
				// Debug drawing.
				Matrix debugMatrix = Matrix.CreateScale(k_invPhysicsScale) * tform;
				m_debugView.RenderDebugData(ref m_projection, ref debugMatrix);
			}

			// Draw HUD.
			HUD.Draw();

			if(fade)
			{
				FadeOut.fadeOut(0.3f);
			}
			FadeOut.Draw(m_spriteBatch);

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

		//Trick entities into restarting
		public void restartEntities()
		{
			Entity[] copyOfChildren = new Entity[Children.Count];
			Children.CopyTo(copyOfChildren);
			foreach (Entity ent in copyOfChildren)
			{
				if (!((ent is Moon) || (ent is RepeatingBackground) || (ent is MapLayer)))
					ent.Dispose();
			}

			ParticleKeepalive.RemoveAll((p) => { p.Dispose(); return true; });

			foreach (SpawnPoint sp in SpawnController.SpawnPoints)
				sp.HasBeenOffscreen = true;
		}

	}
}
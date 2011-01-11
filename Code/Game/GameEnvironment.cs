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

namespace Sputnik {
	public class GameEnvironment : Environment {
		// Spawning/culling.
		public const float k_cullRadius = 300.0f; // Must be greater than spawn radius.
		public const float k_spawnRadius = 100.0f; // Must be less than cull radius.
		public SpawnController SpawnController { get; private set; }

		// Camera.
		public static Vector2 k_maxVirtualSize { get { return new Vector2(1680, 1050) * 1.25f; } }
		public Vector2 ScreenVirtualSize = new Vector2(1680, 1050);
		public Camera2D Camera;

		// Drawing.
		private SpriteBatch m_spriteBatch;
		private Tiled.Map m_map;

		private Matrix m_projection;

		// Particles.
		public SpriteBatchRenderer ParticleRenderer;
		private List<ParticleEffect> Effects = new List<ParticleEffect>();

		// Physics.
		private Physics.DebugViewXNA m_debugView;

		public static float k_physicsScale = 1.0f / 50.0f; // 50 pixels = 1 meter.
		public static float k_invPhysicsScale = 50.0f; // ^ must be inverse.

		// Update loop.
		public float m_updateAccum; // How much time has passed relative to the physics world.

		// HUD.
		public Menus.HUD HUD;

		public GameEnvironment(Controller ctrl)
				: base(ctrl) {

			Sound.StopAll(true);

			Controller.IsMouseVisible = true;

			CollisionWorld = new Physics.Dynamics.World(Vector2.Zero);
			Controller.Window.ClientSizeChanged += WindowSizeChanged;
			Camera = new Camera2D(this);
			WindowSizeChanged(null, null);

			// Create a new SpriteBatch which can be used to draw textures.
			m_spriteBatch = new SpriteBatch(ctrl.GraphicsDevice);

			ParticleRenderer = new SpriteBatchRenderer {
				GraphicsDeviceService = ctrl.Graphics
			};

			ParticleRenderer.LoadContent(contentManager);

			foreach (var e in Effects) {
				e.Initialise();
				e.LoadContent(contentManager);
			}

			// Create collision notification callbacks.
			CollisionWorld.ContactManager.PreSolve += PreSolve;
			CollisionWorld.ContactManager.BeginContact += BeginContact;
			CollisionWorld.ContactManager.EndContact += EndContact;
			CollisionWorld.ContactManager.ContactFilter += ContactFilter;

			// HUD.
			HUD = new Menus.HUD(this);

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
			ScreenVirtualSize = k_maxVirtualSize;
			if (ratio <= 16.0f / 10.0f) ScreenVirtualSize.X = ScreenVirtualSize.Y * ratio;
			else ScreenVirtualSize.Y = ScreenVirtualSize.X / ratio;

			// TODO: Make SpriteBatch drawing use this projection too.
			m_projection = Matrix.CreateOrthographicOffCenter(0.0f, rect.Width, rect.Height, 0.0f, -1.0f, 1.0f);

			Camera.WindowSizeChanged();
		}

		/// <summary>
		/// Load a map from file and create collision objects for it.
		/// </summary>
		/// <param name="filename">File to load map from.</param>
		public void LoadMap(string filename) {
			m_map = Tiled.Map.Load(Path.Combine(Controller.Content.RootDirectory, filename), Controller.Content);
			
			// CASESENSITIVE_TODO: Create world collision here.

			SpawnController = new SpawnController(this, m_map.ObjectGroups.Values);
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

			// Debug Menu = F10.
			if (Keyboard.GetState().IsKeyDown(Keys.F10) && !OldKeyboard.GetState().IsKeyDown(Keys.F10)) {
				Controller.ChangeEnvironment(new Menus.DebugMenu(Controller));
			}

			// Exit game.
			if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !OldKeyboard.GetState().IsKeyDown(Keys.Escape)) {
				Controller.Exit();
			}

			if (elapsedTime > 0.0f) {
				// Update physics.
				CollisionWorld.Step(elapsedTime);

				if (SpawnController != null) SpawnController.Update(elapsedTime);

				// Update entities.
				base.Update(elapsedTime);
				Camera.Update(elapsedTime);
				HUD.Update(elapsedTime);

				// Particles.
				foreach (var effect in Effects) effect.Update(elapsedTime);
			}
		}

		/// <summary>
		/// Draw the world.
		/// </summary>
		public override void Draw() {
			Matrix tform = Camera.Transform;

			// Draw map.
			if (m_map != null) {
				m_map.Draw(m_spriteBatch, Camera.Rect, () => {
					m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, tform);
				});
			}

			// Draw entities.
			m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, tform);
			Draw(m_spriteBatch);
			m_spriteBatch.End();

			// Draw particles on top.  CASESENSITIVE_TODO: Do we want this?
			foreach (var effect in Effects) {
				ParticleRenderer.RenderEffect(effect, ref tform);
			}

			if (m_debugView != null) {
				// Debug drawing.
				Matrix debugMatrix = Matrix.CreateScale(k_invPhysicsScale) * tform;
				m_debugView.RenderDebugData(ref m_projection, ref debugMatrix);
			}

			// Draw HUD.
			HUD.Draw();
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
	}
}
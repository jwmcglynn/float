using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sputnik.Menus {
	public class Menu : Environment {
		private SpriteBatch m_spriteBatch;
		internal List<Widget> Buttons = new List<Widget>();
		public Vector2 ScreenSize { get; private set; }

		public Vector2 ScreenOffset = Vector2.Zero;
		public Vector2 ScreenScale = Vector2.One;

		// Mouse tracking.
		private Widget m_activeButton;
		private bool m_mouseWasPressed = false;

		public Menu(Controller ctrl)
				: base(ctrl) {
			Controller.Window.ClientSizeChanged += WindowSizeChanged;
			WindowSizeChanged(null, null); // Prime the ScreenSize.

			// Create a new SpriteBatch which can be used to draw textures.
			m_spriteBatch = new SpriteBatch(ctrl.GraphicsDevice);
		}

		/// <summary>
		/// Called when the window size changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WindowSizeChanged(object sender, EventArgs e) {
			Rectangle rect = Controller.Window.ClientBounds;
			ScreenSize = new Vector2(rect.Width, rect.Height);
		}

		/// <summary>
		/// Update the Environment each frame.
		/// </summary>
		/// <param name="elapsedTime">Time since last Update() call.</param>
		public override void Update(float elapsedTime) {
			MouseState mouse = Mouse.GetState();
			Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
			bool mousePressed = (mouse.LeftButton == ButtonState.Pressed);

			List<Widget> collidingButtons = Buttons.FindAll(b => b.Visible && b.Collides(mousePos));
			Widget button = null;
			if (collidingButtons.Count > 0) button = collidingButtons.OrderBy(b => b.Zindex).First();

			// We switched to a new button.
			if (m_activeButton != button) {
				// Unset current button.
				if (m_activeButton != null) {
					m_activeButton.DispatchOnMouseOut();
					if (m_mouseWasPressed) m_activeButton.DispatchOnMouseUp(false);
				}

				// Activate new one.
				m_activeButton = button;
				if (m_activeButton != null) {
					m_activeButton.DispatchOnMouseOver();
					
					if (mousePressed && !m_mouseWasPressed) {
						m_activeButton.DispatchOnMouseDown();
					}
				}
			} else if (m_activeButton != null) {
				// Button is the same.  Update it.
				if (m_mouseWasPressed && !mousePressed) {
					m_activeButton.DispatchOnMouseUp(true);
				} else if (!m_mouseWasPressed && mousePressed) {
					m_activeButton.DispatchOnMouseDown();
				}
			}

			m_mouseWasPressed = mousePressed;
			base.Update(elapsedTime);
		}

		/// <summary>
		/// Draw the world.
		/// </summary>
		public override void Draw() {
			// Draw entities.
			m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			DrawChildren(m_spriteBatch);
			m_spriteBatch.End();
		}

		public bool canAdvance()
		{
			KeyboardState kb = Keyboard.GetState();
			MouseState ms = Mouse.GetState();
			GamePadState gp = GamePad.GetState(PlayerIndex.One);

			KeyboardState okb = OldKeyboard.GetState();
			MouseState oms = OldMouse.GetState();
			GamePadState ogp = OldGamePad.GetState();

			return (
				((kb.IsKeyDown(Keys.Escape) && !okb.IsKeyDown(Keys.Escape))
				|| (kb.IsKeyDown(Keys.Space) && !okb.IsKeyDown(Keys.Space))
				|| ((ms.LeftButton == ButtonState.Pressed &&
						ms.X >= 0 && ms.Y >= 0 && ms.X < ScreenSize.X && ms.Y < ScreenSize.Y)
					&& !(oms.LeftButton == ButtonState.Pressed &&
						oms.X >= 0 && oms.Y >= 0 && oms.X < ScreenSize.X && oms.Y < ScreenSize.Y))

				|| (gp.Buttons.Start == ButtonState.Pressed && ogp.Buttons.Start != ButtonState.Pressed)
				|| (gp.Buttons.A == ButtonState.Pressed && ogp.Buttons.A != ButtonState.Pressed)
				|| (gp.Buttons.B == ButtonState.Pressed && ogp.Buttons.B != ButtonState.Pressed)
				|| ((kb.IsKeyDown(Keys.Enter) && !kb.IsKeyDown(Keys.LeftAlt) && !kb.IsKeyDown(Keys.RightAlt))
					&& !(okb.IsKeyDown(Keys.Enter) && !okb.IsKeyDown(Keys.LeftAlt) && !okb.IsKeyDown(Keys.RightAlt))
					)
				)
			);
		}

	}
}

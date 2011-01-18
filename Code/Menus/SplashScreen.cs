using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sputnik.Menus {
	class SplashScreen : Menu {
		private Widget m_logo;

		public SplashScreen(Controller cntl)
				: base(cntl) {

			m_logo = new Widget(this);
			m_logo.LoadTexture(contentManager, "Splash");
			m_logo.PositionPercent = new Vector2(0.5f, 0.5f);
			m_logo.Registration = m_logo.Size / 2;
			AddChild(m_logo);
		}

		private const float k_stayTime = 3.0f;
		private const float k_fadeTime = 0.5f;

		private float m_timer = k_stayTime;

		enum State {
			Waiting,
			Fading
		}

		private State m_state;

		public override void Update(float elapsedTime) {
			KeyboardState kb = Keyboard.GetState();
			MouseState ms = Mouse.GetState();
			GamePadState gp = GamePad.GetState(PlayerIndex.One);

			switch (m_state) {
				case State.Waiting:
					if (m_timer <= 0.0f
							|| kb.IsKeyDown(Keys.Escape)
							|| kb.IsKeyDown(Keys.Space)
							|| (ms.LeftButton == ButtonState.Pressed &&
									ms.X >= 0 && ms.Y >= 0 && ms.X < ScreenSize.X && ms.Y < ScreenSize.Y)
							|| gp.Buttons.Start == ButtonState.Pressed
							|| gp.Buttons.A == ButtonState.Pressed
							|| gp.Buttons.B == ButtonState.Pressed) {
						m_state = State.Fading;
						m_timer = k_fadeTime;
					}

					break;

				case State.Fading:
					m_logo.Alpha = MathHelper.Clamp(m_timer / k_fadeTime, 0.0f, 1.0f);
					if (m_timer <= 0.0f) {
						Controller.ChangeEnvironment(new MainMenu(Controller));
					}

					break;
			}
			
			m_timer -= elapsedTime;

			base.Update(elapsedTime);
		}
	}
}

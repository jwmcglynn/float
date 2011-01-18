using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sputnik.Menus {
	class Credits : Menu {
		private Widget m_credits;

		private static Vector2 k_moveSpeed = new Vector2(0.0f, -50.0f);

		class CreditBox : Widget {
			public CreditBox(Menu menu)
					: base(menu) {
				LoadTexture(menu.contentManager, "Credit2");
				PositionPercent = new Vector2(0.5f, 1.0f);
				Registration = new Vector2(Size.X / 2, 0.0f);
				DesiredVelocity = k_moveSpeed;
			}
		}

		public Credits(Controller cntl)
				: base(cntl) {

			m_credits = new CreditBox(this);
			AddChild(m_credits);
		}

		private bool CreditsDone() {
			return m_credits.AbsolutePosition.Y + m_credits.Size.Y < 0.0f;
		}

		public override void Update(float elapsedTime) {
			KeyboardState kb = Keyboard.GetState();
			MouseState ms = Mouse.GetState();
			GamePadState gp = GamePad.GetState(PlayerIndex.One);
			
			if (CreditsDone()
					|| kb.IsKeyDown(Keys.Escape)
					|| kb.IsKeyDown(Keys.Space)
					|| (ms.LeftButton == ButtonState.Pressed &&
							ms.X >= 0 && ms.Y >= 0 && ms.X < ScreenSize.X && ms.Y < ScreenSize.Y)
					|| gp.Buttons.Start == ButtonState.Pressed
					|| gp.Buttons.A == ButtonState.Pressed
					|| gp.Buttons.B == ButtonState.Pressed) {
				Controller.ChangeEnvironment(new MainMenu(Controller));
			}

			base.Update(elapsedTime);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Input = Microsoft.Xna.Framework.Input;
using Sputnik.Game;

namespace Sputnik.Menus
{
    /// <summary>
    /// Any pop up that should pause the game.
    /// </summary>
    public class PopUp : Menu
    {
        protected GameEnvironment m_game;
        private bool m_justPaused;
        protected Keys quitKey;
        public PopUp(Controller cntl, GameEnvironment game)
            : base(cntl)
        {
            Controller.IsMouseVisible = true;
            m_game = game;
            m_justPaused = true;
            quitKey = Keys.Escape;
		}

        public override void Update(float elapsedTime)
        {
            if (!m_justPaused)
            {
				KeyboardState kb = Keyboard.GetState();
				GamePadState gp = GamePad.GetState(PlayerIndex.One);

				const float threshold = 0.4f;
                switch (quitKey)
                {
					case Keys.Enter:
						if (canAdvance()) {
							Sound.PlayCue("scroll");
							unPause();
						}
						break;
                    case Keys.Up:
						if (kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.W)
								|| gp.IsButtonDown(Input.Buttons.DPadUp)
								|| gp.ThumbSticks.Left.Y > threshold
								|| gp.ThumbSticks.Right.Y > threshold) {
							Sound.PlayCue("scroll");
							unPause();
						}
						break;
					case Keys.Down:
						if (kb.IsKeyDown(Keys.Down) || kb.IsKeyDown(Keys.S)
								|| gp.IsButtonDown(Input.Buttons.DPadDown)
								|| gp.ThumbSticks.Left.Y < -threshold
								|| gp.ThumbSticks.Right.Y < -threshold)
						{
							Sound.PlayCue("scroll");
							unPause();
						}
						break;
					case Keys.Right:
						if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D)
								|| gp.IsButtonDown(Input.Buttons.DPadRight)
								|| gp.ThumbSticks.Left.X > threshold
								|| gp.ThumbSticks.Right.X > threshold)
							{
							Sound.PlayCue("scroll");
							unPause();
						}
						break;
					case Keys.Left:
						if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A)
								|| gp.IsButtonDown(Input.Buttons.DPadLeft)
								|| gp.ThumbSticks.Left.X < -threshold
								|| gp.ThumbSticks.Right.X < -threshold)
						{
							Sound.PlayCue("scroll");
							unPause();
						}
						break;
                }
            }
            m_justPaused = false;
            base.Update(elapsedTime);
        }

		protected virtual void unPause()
		{
			m_game.unPause();
		}

		public override void Dispose() {
			Controller.IsMouseVisible = false;
			base.Dispose();
        }
    }
}

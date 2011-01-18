﻿using System;
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
				//Code taken from credits

					if (quitKey == Keys.Enter && canAdvance())
					{
						unPause();
					}

                // Try unpausing with keyboard.
				if ((Keyboard.GetState().IsKeyDown(Keys.Escape) && !OldKeyboard.GetState().IsKeyDown(Keys.Escape))
					|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.Start)
							&& !OldGamePad.GetState().IsButtonDown(Input.Buttons.Start)))
				{
					unPause();
					Sound.PlayCue("scroll");
				}
				const float threshold = 0.4f;
                switch (quitKey)
                {
                    case Keys.Up:
						if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadUp)
							|| GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > threshold)
							unPause();
						break;
					case Keys.Down:
						if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadDown)
							|| GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -threshold)
							unPause();
						break;
					case Keys.Right:
						if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadRight)
							|| GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > threshold)
							unPause();
						break;
					case Keys.Left:
						if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadLeft)
							|| GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -threshold)
							unPause();
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

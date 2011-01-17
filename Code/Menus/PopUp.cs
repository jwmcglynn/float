using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
				if (Keyboard.GetState().IsKeyDown(quitKey) && !OldKeyboard.GetState().IsKeyDown(quitKey))
					unPause();
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

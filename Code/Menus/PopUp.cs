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
        GameEnvironment Game;
        private bool m_justPaused;
        public PopUp(Controller cntl, GameEnvironment game)
            : base(cntl)
        {
            Controller.IsMouseVisible = true;
            Game = game;
            m_justPaused = true;

			TextWidget title = new TextWidget(this, "font", "Paused");
			title.PositionPercent = new Vector2(0.5f, 0.3f);
			AddChild(title);

			float ypos = 50.0f;
		}

        public override void Update(float elapsedTime)
        {
            if (!m_justPaused)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !OldKeyboard.GetState().IsKeyDown(Keys.Enter))
                    Game.unPause();
            }
            m_justPaused = false;
            base.Update(elapsedTime);
        }

		public override void Dispose() {
			Controller.IsMouseVisible = false;
			base.Dispose();
        }
    }
}

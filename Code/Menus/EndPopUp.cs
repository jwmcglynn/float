using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sputnik.Menus
{
	class EndPopUp : PopUp
	{
		GameEnvironment currentGame;
		public EndPopUp(Controller cntl, GameEnvironment game)
			: base(cntl, game)
		{
			currentGame = game;
			quitKey = Microsoft.Xna.Framework.Input.Keys.Enter;

			Widget letterBackground = new Widget(this);
			letterBackground.Texture = contentManager.Load<Texture2D>("Letterforsanta");
			letterBackground.Registration = letterBackground.Size / 2;
			letterBackground.PositionPercent = new Vector2(0.5f, 0.5f);
			letterBackground.Scale = 0.7f;
			letterBackground.Zindex = 0.01f;
			letterBackground.Visible = true;
			AddChild(letterBackground);
		}

		protected override void unPause()
		{
			// Go back to main menu.
		}
	}
}

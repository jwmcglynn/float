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
		//GameEnvironment currentGame;
		Controller Control;
		Widget m_letterBackground;

		public EndPopUp(Controller cntl, GameEnvironment game)
			: base(cntl, game)
		{
			Control = cntl;
			quitKey = Microsoft.Xna.Framework.Input.Keys.Enter;

			m_letterBackground = new Widget(this);
			m_letterBackground.Texture = contentManager.Load<Texture2D>("Letterforsanta");
			m_letterBackground.Registration = m_letterBackground.Size / 2;
			m_letterBackground.PositionPercent = new Vector2(0.5f, 0.5f);
			m_letterBackground.Scale = 0.7f;
			m_letterBackground.Zindex = 0.01f;
			m_letterBackground.Visible = true;
			AddChild(m_letterBackground);
		}

		protected override void unPause()
		{
			Control.ChangeEnvironment(new Credits(Control));
		}

		public override void Update(float elapsedTime) {

			float scale = Math.Min(ScreenSize.X / m_letterBackground.Texture.Width, ScreenSize.Y / m_letterBackground.Texture.Height);
			m_letterBackground.Scale = scale;
			base.Update(elapsedTime);
		}
	}
}

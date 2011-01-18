using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sputnik.Menus
{
	class Instructions : Menu
	{

		public Instructions(Controller control)
			: base(control)
		{

			Widget intructions = new Widget(this);
			intructions.Texture = contentManager.Load<Texture2D>("instructions3");
			intructions.Registration = intructions.Size / 2;
			intructions.PositionPercent = new Vector2(0.5f, 0.5f);
			intructions.Scale = 0.7f;
			intructions.Zindex = 0.01f;
			intructions.Visible = true;
			AddChild(intructions);
		}

		public override void Update(float elapsedTime)
		{
			base.Update(elapsedTime);
			if(canAdvance())
			{
				Controller.ChangeEnvironment(new MainMenu(Controller));
			}
			
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sputnik.Game;

namespace Sputnik.Menus
{
	public class FaderOuter : Entity
	{
		Environment environment;
		private bool fadingOut;
		private float fadePerSecond;
		public bool fadedOut;

		public FaderOuter(Environment env)
		{
			environment = env;
			Registration = new Vector2(0.5f, 0.5f);
			Alpha = 0.0f;
			fadingOut = false;
			Zindex = 0.1f;
			fadedOut = false;

			Texture2D dummyTexture = new Texture2D(env.Controller.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.White });

			Texture = dummyTexture;
		}

		public void fadeOut(float _fadePerSecond)
		{
			fadingOut = true;
			fadePerSecond = _fadePerSecond;
		}

		public override void Update(float elapsedTime)
		{
			base.Update(elapsedTime);
			if (fadingOut)
			{
				Alpha += elapsedTime * fadePerSecond;
				if (Alpha >= 1.0f)
					fadedOut = true;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(Texture, new Vector2(0, 0), environment.Controller.Window.ClientBounds,
		 Color.Black * Alpha, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, Zindex);
			spriteBatch.End();
		}


	}
}

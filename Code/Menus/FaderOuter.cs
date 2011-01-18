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
		private float fadePerSecond;

		public FaderOuter(Environment env)
		{
			environment = env;
			Registration = new Vector2(0.5f, 0.5f);
			Alpha = 0.0f;
			Zindex = 0.1f;

			Texture2D dummyTexture = new Texture2D(env.Controller.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.White });

			Texture = dummyTexture;
		}

		public bool doneFading
		{
			get { return fadePerSecond == 0; }
		}

		public void fadeOut(float _fadePerSecond)
		{
			fadePerSecond = _fadePerSecond;
		}

		public void fadeIn(float _fadePerSecond)
		{
			fadePerSecond = -_fadePerSecond;
		}

		public void reset()
		{
			Alpha = 0.0f;
			fadePerSecond = 0.0f;
		}

		public void resetOpaque()
		{
			Alpha = 1.0f;
			fadePerSecond = 0.0f;
		}

		public override void Update(float elapsedTime)
		{
			base.Update(elapsedTime);

			Alpha += elapsedTime * fadePerSecond;
			if (fadePerSecond > 0 && Alpha >= 1.0f)
			{
				Alpha = 1.0f;
				fadePerSecond = 0;
			}
			
			if (fadePerSecond < 0 && Alpha <= 0.0f)
			{
				Alpha = 0.0f;
				fadePerSecond = 0;
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

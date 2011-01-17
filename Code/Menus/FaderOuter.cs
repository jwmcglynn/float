﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sputnik.Game;

namespace Sputnik.Menus
{
	class FaderOuter : Entity
	{
		public float Height;
		public float FullWidth;
		Environment environment;
		private bool fading;
		private float fadePerSecond;

		public float FillPercent = 1.0f;
		public FaderOuter(Environment env, float width, float height)
		{
			environment = env;
			Registration = new Vector2(0.5f, 0.5f);
			Alpha = 0.0f;
			Height = height;
			FullWidth = width;
			fading = false;
			Zindex = 0.1f;

			Texture2D dummyTexture = new Texture2D(env.Controller.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.White });

			Texture = dummyTexture;
		}

		public void fadeOut(float _fadePerSecond)
		{
			fading = true;
			fadePerSecond = _fadePerSecond;
		}

		public override void Update(float elapsedTime)
		{
			base.Update(elapsedTime);
			if (fading)
			{
				Alpha += elapsedTime * fadePerSecond;
				GameEnvironment game = ((GameEnvironment)environment);
				if (Alpha > 1.0f && !game.paused)
				{	
					game.pause(new EndPopUp(game.Controller,game));
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(Texture, new Vector2(0,0), environment.Camera.Rect,
		 Color.Black * Alpha, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, Zindex);
			spriteBatch.End();
		}


	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sputnik.Menus {
	class RectangleWidget : Widget {
		public float Height;
		public float FullWidth;

		public float FillPercent = 1.0f;

		public RectangleWidget(Menu env, float width, float height)
			: base(env) {

			Height = height;
			FullWidth = width;

			Texture2D dummyTexture = new Texture2D(env.Controller.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.White });

			Texture = dummyTexture;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			spriteBatch.Draw(Texture, AbsolutePosition, new Rectangle(
				(int) Math.Round(-FullWidth / 2)
				, (int) Math.Round(-Height / 2)
				, (int) Math.Round(FullWidth * FillPercent)
				, (int) Math.Round(Height)
			), VertexColor * Alpha, 0.0f, new Vector2(FullWidth / 2, Height / 2), 1.0f, SpriteEffects.None, Zindex);
		}
	}

	public class HUD : Menu {
		private GameEnvironment Environment;

		private RectangleWidget ShipHealth;
		private RectangleWidget ShipHealthBG;

		public HUD(GameEnvironment env)
				: base(env.Controller) {

			Environment = env;

			/////

			// Ship health meter.
			ShipHealth = new RectangleWidget(this, ScreenSize.X * 0.25f - 4.0f, 20.0f - 4.0f);
			ShipHealth.PositionPercent = new Vector2(0.0f, 1.0f);
			ShipHealth.Position = new Vector2(10.0f + ScreenSize.X * 0.25f / 2, -20.0f);
			ShipHealth.VertexColor = Color.Green;
			ShipHealth.Zindex = 0.5f;
			ShipHealth.Alpha = 0.0f;
			AddChild(ShipHealth);

			// Ship health background.
			ShipHealthBG = new RectangleWidget(this, ScreenSize.X * 0.25f, 20.0f);
			ShipHealthBG.PositionPercent = new Vector2(0.0f, 1.0f);
			ShipHealthBG.Position = new Vector2(10.0f + ScreenSize.X * 0.25f / 2, -20.0f);
			ShipHealthBG.VertexColor = Color.Black;
			ShipHealthBG.Zindex = 1.0f;
			ShipHealthBG.Alpha = 0.0f;
			AddChild(ShipHealthBG);
		}

		public override void Update(float elapsedTime) {
			ShipHealth.FullWidth = ScreenSize.X * 0.25f - 4.0f;
			ShipHealth.Position = new Vector2(10.0f + ScreenSize.X * 0.25f / 2, -20.0f);
			ShipHealthBG.FullWidth = ScreenSize.X * 0.25f;
			ShipHealthBG.Position = new Vector2(10.0f + ScreenSize.X * 0.25f / 2, -20.0f);

			// Ship health bar.
			ShipHealth.FillPercent = 1.0f; // TODO: Make health meter work.

			base.Update(elapsedTime);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

		private RectangleWidget up;
		private RectangleWidget down;
        private RectangleWidget left;
        private RectangleWidget right;

        private RectangleWidget hudB;

        private RectangleWidget upPress;
        private RectangleWidget downPress;
        private RectangleWidget leftPress;
        private RectangleWidget rightPress;

		public HUD(GameEnvironment env)
				: base(env.Controller) {

			Environment = env;

			/////

            hudB = new RectangleWidget(this, 200.0f, 200.0f);
            hudB.PositionPercent = new Vector2(0.8f, 0.8f);
            hudB.Position = new Vector2(0.8f, 0.8f);
            hudB.VertexColor = Color.White;
            hudB.Zindex = 0.7f;
            hudB.FullWidth = 200.0f;
            hudB.FillPercent = 1.0f;
            AddChild(hudB);

			// Control Placeholder - Pomi edited this
			up = new RectangleWidget(this, 0.1f, 10.0f);
            //new RectangleWidget(this, ScreenSize.X * 0.25f - 4.0f, 20.0f - 4.0f);
            up.PositionPercent = new Vector2(0.8f, 0.75f);
            up.Position = new Vector2(0.8f, 0.8f);
			up.VertexColor = Color.Yellow;
			up.Zindex = 0.5f;
            up.FullWidth = 20.0f;
            up.FillPercent = 1.0f;
			AddChild(up);

            down = new RectangleWidget(this, 0.1f, 10.0f);
            down.PositionPercent = new Vector2(0.8f, 0.85f);
            down.Position = new Vector2(0.8f, 0.8f);
            down.VertexColor = Color.Pink;
            down.Zindex = 0.5f;
            down.FullWidth = 20.0f;
            down.FillPercent = 1.0f;
            AddChild(down);

            left = new RectangleWidget(this, 0.1f, 10.0f);
            left.PositionPercent = new Vector2(0.75f, 0.8f);
            left.Position = new Vector2(0.8f, 0.8f);
            left.VertexColor = Color.Red;
            left.Zindex = 0.5f;
            left.FullWidth = 20.0f;
            left.FillPercent = 1.0f;
            AddChild(left);

            right = new RectangleWidget(this, 0.1f, 10.0f);
            right.PositionPercent = new Vector2(0.85f, 0.8f);
            right.Position = new Vector2(0.8f, 0.8f);
            right.VertexColor = Color.Blue;
            right.Zindex = 0.5f;
            right.FullWidth = 20.0f;
            right.FillPercent = 1.0f;
            AddChild(right);

            upPress = new RectangleWidget(this, 2.0f, 20.0f);
            upPress.PositionPercent = new Vector2(0.8f, 0.75f);
            upPress.Position = new Vector2(0.8f, 0.8f);
            upPress.VertexColor = Color.Black;
            upPress.Zindex = 0.6f;
            upPress.FullWidth = 30.0f;
            upPress.FillPercent = 0.0f;
            AddChild(upPress);

            downPress = new RectangleWidget(this, 2.0f, 20.0f);
            downPress.PositionPercent = new Vector2(0.8f, 0.85f);
            downPress.Position = new Vector2(0.8f, 0.8f);
            downPress.VertexColor = Color.Black;
            downPress.Zindex = 0.6f;
            downPress.FullWidth = 30.0f;
            downPress.FillPercent = 0.0f;
            AddChild(downPress);

            leftPress = new RectangleWidget(this, 2.0f, 20.0f);
            leftPress.PositionPercent = new Vector2(0.75f, 0.8f);
            leftPress.Position = new Vector2(0.8f, 0.8f);
            leftPress.VertexColor = Color.Black;
            leftPress.Zindex = 0.6f;
            leftPress.FullWidth = 30.0f;
            leftPress.FillPercent = 0.0f;
            AddChild(leftPress);

            rightPress = new RectangleWidget(this, 2.0f, 20.0f);
            rightPress.PositionPercent = new Vector2(0.85f, 0.8f);
            rightPress.Position = new Vector2(0.8f, 0.8f);
            rightPress.VertexColor = Color.Black;
            rightPress.Zindex = 0.6f;
            rightPress.FullWidth = 30.0f;
            rightPress.FillPercent = 0.0f;
            AddChild(rightPress);



            /*
			upBG = new RectangleWidget(this, ScreenSize.X * 0.25f, 20.0f);
			upBG.PositionPercent = new Vector2(0.0f, 1.0f);
			upBG.Position = new Vector2(10.0f + ScreenSize.X * 0.25f / 2, -20.0f);
			upBG.VertexColor = Color.Black;
			upBG.Zindex = 1.0f;
			AddChild(upBG);
             */
		}

		public override void Update(float elapsedTime) {

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
                upPress.FillPercent = 1.0f;
            else
                upPress.FillPercent = 0.0f;
            if (keyState.IsKeyDown(Keys.Down))
                downPress.FillPercent = 1.0f;
            else
                downPress.FillPercent = 0.0f;
            if (keyState.IsKeyDown(Keys.Left))
                leftPress.FillPercent = 1.0f;
            else
                leftPress.FillPercent = 0.0f;
            if (keyState.IsKeyDown(Keys.Right))
                rightPress.FillPercent = 1.0f;
            else
                rightPress.FillPercent = 0.0f;


            //upBG.FullWidth = ScreenSize.X * 0.25f;
            //upBG.Position = new Vector2(10.0f + ScreenSize.X * 0.25f / 2, -20.0f);
			base.Update(elapsedTime);
		}
	}
}

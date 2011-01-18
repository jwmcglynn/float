using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Input = Microsoft.Xna.Framework.Input;
using Sputnik.Game;

namespace Sputnik.Menus {
	class HUDButton : Widget {

        
        private Color fillColor = Color.LightGray;
        private Color highlightColor = Color.White;
		public HUDButton(Menu env, string asset)
			: base(env) {
     
            Texture = env.contentManager.Load<Texture2D>(asset);
			Registration = new Vector2(169.0f, 152.0f);
            PositionPercent = new Vector2(0.9f, 0.85f);
            Zindex = 0.5f;
            Scale = 0.65f;
            VertexColor = fillColor;
            Alpha = 0.8f;
            
            /* Are we going to implement controls here? -Reza
            OnMouseOver += () => { Alpha = 1.0f; };
            OnMouseOut += () => { deHighlight(); };
            OnMouseDown += () => { highlight(); };
             * */
		}

        public void highlight()
        {
            VertexColor = highlightColor;
            Alpha = 1.0f;
			Scale = 0.69f;
        }
        public void deHighlight()
        {
            VertexColor = fillColor;
            Alpha = 0.7f;
			Scale = 0.65f;
        }
	}

	public class HUD : Menu {
		private GameEnvironment Environment;

		private HUDButton up;
		private HUDButton down;
        private HUDButton left;
        private HUDButton right;
        private HUDButton hudB;

        public bool enabled;

		public HUD(GameEnvironment env)
				: base(env.Controller) {

			Environment = env;
            enabled = true;
            hudB = new HUDButton(this, "buttons\\HUDOutline");
            hudB.Zindex = 0.7f;
            AddChild(hudB);

			up = new HUDButton(this, "buttons\\TopButton");
			AddChild(up);

            down = new HUDButton(this, "buttons\\BottomButton");
            AddChild(down);

            left = new HUDButton(this, "buttons\\LeftButton");
            AddChild(left);

            right = new HUDButton(this, "buttons\\RightButton");
            AddChild(right);

		}

        const float threshold = 0.4f;

		public override void Update(float elapsedTime) {

            if (enabled)
            {
                KeyboardState keyState = Keyboard.GetState();
                GamePadState padState = GamePad.GetState(PlayerIndex.One);
                GamePadState oldpadState = OldGamePad.GetState();

                if (keyState.IsKeyDown(Keys.Up)
					|| keyState.IsKeyDown(Keys.W)
					|| padState.IsButtonDown(Input.Buttons.DPadUp)
                    || padState.ThumbSticks.Left.Y > threshold
					|| padState.ThumbSticks.Right.Y > threshold
					)
                    up.highlight();
                else
                    up.deHighlight();

                if (keyState.IsKeyDown(Keys.Down)
					|| keyState.IsKeyDown(Keys.S)
                    || padState.IsButtonDown(Input.Buttons.DPadDown)
                    || (padState.ThumbSticks.Left.Y < -threshold)
					|| (padState.ThumbSticks.Right.Y < -threshold)
					)
                    down.highlight();
                else
                    down.deHighlight();

                if (keyState.IsKeyDown(Keys.Left)
					|| keyState.IsKeyDown(Keys.D)
                    || padState.IsButtonDown(Input.Buttons.DPadLeft)
                    || padState.ThumbSticks.Left.X < -threshold
					|| padState.ThumbSticks.Right.X < -threshold
					)
                    left.highlight();
                else
                    left.deHighlight();

                if (keyState.IsKeyDown(Keys.Right)
					|| keyState.IsKeyDown(Keys.A)
                    || padState.IsButtonDown(Input.Buttons.DPadRight)
                    || padState.ThumbSticks.Left.X > threshold
					|| padState.ThumbSticks.Right.X > threshold
					)

                    right.highlight();
                else
                    right.deHighlight();
            }
            else
            {
                up.deHighlight();
                down.deHighlight();
                left.deHighlight();
                right.deHighlight();
            }
            
			    base.Update(elapsedTime);
		}
	}
}

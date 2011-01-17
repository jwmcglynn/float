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

		public override void Update(float elapsedTime) {

            if (enabled)
            {
                KeyboardState keyState = Keyboard.GetState();
                GamePadState padState = GamePad.GetState(PlayerIndex.One);
                GamePadState oldpadState = OldGamePad.GetState();
                if (keyState.IsKeyDown(Keys.Up)
                    || padState.IsButtonDown(Input.Buttons.DPadUp)
                    || padState.IsButtonDown(Input.Buttons.LeftThumbstickUp))
                    up.highlight();
                else
                    up.deHighlight();

                if (keyState.IsKeyDown(Keys.Down)
                    || padState.IsButtonDown(Input.Buttons.DPadDown)
                    || padState.IsButtonDown(Input.Buttons.LeftThumbstickDown))
                    down.highlight();
                else
                    down.deHighlight();

                if (keyState.IsKeyDown(Keys.Left)
                    || padState.IsButtonDown(Input.Buttons.DPadLeft)
                    || padState.IsButtonDown(Input.Buttons.LeftThumbstickLeft))
                    left.highlight();
                else
                    left.deHighlight();

                if (keyState.IsKeyDown(Keys.Right)
                    || padState.IsButtonDown(Input.Buttons.DPadRight)
                    || padState.IsButtonDown(Input.Buttons.LeftThumbstickRight))
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

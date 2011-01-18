﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Input = Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Sputnik.Game;
using System.Threading;
namespace Sputnik.Menus
{
	internal class TutorialButton : Widget
	{
		private Animation m_anim;
		private Sequence blink;
		public TutorialButton(Menu menu, Keys key)
			:base(menu)
		{
			Scale = 0.1f;
			string keyStandard = null;
			string keyPressed = null;
			switch (key)
			{
				case Keys.Down:
					keyStandard = "tutorial\\Keydown_standard";
					keyPressed = "tutorial\\Keydown_pressed";
					break;
				case Keys.Up:
					keyStandard = "tutorial\\Keyup_standard";
					keyPressed = "tutorial\\Keyup_pressed";
					break;
				case Keys.Right:
					keyStandard = "tutorial\\Keyright_standard";
					keyPressed = "tutorial\\Keyright_pressed";
					break;
				case Keys.Left:
					keyStandard = "tutorial\\Keyleft_standard";
					keyPressed = "tutorial\\Keyleft_pressed";
					break;
			}

			blink = new Sequence(menu.contentManager);
			float pressDuration = 1.0f;
			if (keyPressed != null && keyStandard != null)
			{
				Texture = menu.contentManager.Load<Texture2D>(keyStandard);
				blink.AddFrame(keyStandard, pressDuration);
				blink.AddFrame(keyPressed, pressDuration);
			}
			blink.Loop = true;
			m_anim = new Animation();
			m_anim.PlaySequence(blink);

			Registration = new Vector2(171.0f, 171.0f);
		}

		public override void Update(float elapsedTime)
		{
			base.Update(elapsedTime);
			Texture = m_anim.CurrentFrame;
			m_anim.Update(elapsedTime);
		}
	}

    public class TutorialPopUp : PopUp
    {
		GameEnvironment Game;
        Widget thePopUp;
		TutorialButton button;
        public TutorialPopUp(Controller cntl, GameEnvironment game, Keys key)
            : base(cntl, game)
        {
			Game = game;
            quitKey = key;
            thePopUp = new Widget(this);
            switch (key)
            {
               case Keys.Down:
                   thePopUp.Texture = contentManager.Load<Texture2D>("buttons\\LowTemperature");
                   break;
               case Keys.Up:
                   thePopUp.Texture = contentManager.Load<Texture2D>("buttons\\HighTemperature");
                   break;
               case Keys.Right:
                   thePopUp.Texture = contentManager.Load<Texture2D>("buttons\\LowPressure");
                   break;
               case Keys.Left:
                   thePopUp.Texture = contentManager.Load<Texture2D>("buttons\\HighPressure");
                   break;
			   case Keys.Enter:
				   thePopUp.Texture = contentManager.Load<Texture2D>("Letterforsanta");
				   break;
            }
            thePopUp.Registration = (thePopUp.Size / 2);
            thePopUp.PositionPercent = new Vector2(0.5f, 0.5f);
            thePopUp.Zindex = 0.1f;
            thePopUp.Visible = true;
			thePopUp.VertexColor = Color.LightGoldenrodYellow;
            AddChild(thePopUp);

			button = new TutorialButton(this, key);
			button.PositionPercent = thePopUp.PositionPercent;
			button.Position = new Vector2(190, 60);
			button.Zindex = 0.05f;
			button.Visible = true;
			AddChild(button);
        }

		protected override void  unPause()
		{
			switch (quitKey)
			{
				case Keys.Up:
					Game.Balloon.enableUp = true;
					break;
				case Keys.Down:
					Game.Balloon.enableDown = true;
					break;
				case Keys.Right:
					Game.Balloon.enableRight = true;
					break;
				case Keys.Left:
					Game.Balloon.enableLeft = true;
					break;
			}
			base.unPause();
		}

    }

}

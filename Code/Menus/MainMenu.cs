using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Input = Microsoft.Xna.Framework.Input;
using Sputnik.Game;
using Sputnik.Menus;

namespace Sputnik.Menus
{
	internal class MainMenuButton : Widget
	{
		public MainMenuButton prevButton { get; set; }
		private MainMenuButton nButton;
		private MainMenu theMenu;
		public MainMenuButton nextButton
		{
			get { return nButton; }
			set
			{
				nButton = value;
				value.prevButton = this;
			}
		}
		public bool isSelected;
		public bool isPressed;
		public MainMenuButton(MainMenu menu, string textureName)
			: base(menu)
		{
			theMenu = menu;
			isSelected = false;
			isPressed = false;
			prevButton = this;
			nextButton = this;
			Texture = menu.contentManager.Load<Texture2D>(textureName);
			OnMouseOver += () =>
			{
				if (!isSelected)
				{
					menu.unSelectCurrentButton();
					menu.selectButton(this);
				}
			};
			OnMouseDown += () =>
			{
				isPressed = true;
				theMenu.buttonPressed = true;
			};
			OnMouseOut += () =>
			{
				if (isPressed)
				{
					isPressed = false;
					menu.selectButton(this);
					theMenu.buttonPressed = false;
				}
			};
		}
		public override void Update(float elapsedTime)
		{
			if (isSelected)
				VertexColor = Color.LightGreen;
			else VertexColor = Color.White;
			if (isPressed)
				VertexColor = Color.Green;

			base.Update(elapsedTime);
		}
	}

	public class MainMenu : Menu
	{
		MainMenuButton currentButton;
		MainMenuButton menuBackground;
		MainMenuButton playButton;
		MainMenuButton instructionButton; 
		MainMenuButton creditsButton;
		MainMenuButton quitButton;

		public bool buttonPressed;

		public MainMenu(Controller cntl)
			: base(cntl)
		{
			Sound.StopAll();
			Sound.PlayCue("music_title");
			Controller.IsMouseVisible = true;
			menuBackground = new MainMenuButton(this, "main-menu");
			menuBackground.Registration = menuBackground.Size / 2;
			menuBackground.PositionPercent = new Vector2(0.5f, 0.5f);
			menuBackground.Zindex = 0.6f;
			AddChild(menuBackground);

			//The buttons:
			playButton = new MainMenuButton(this, "buttons\\introButton1");
			instructionButton = new MainMenuButton(this, "buttons\\introButton2");
			creditsButton = new MainMenuButton(this, "buttons\\introButton3");
			quitButton = new MainMenuButton(this, "buttons\\introButton4");

			playButton.nextButton = instructionButton;
			instructionButton.nextButton = creditsButton;
			creditsButton.nextButton = quitButton;
			quitButton.nextButton = playButton;

			float distanceBetweenButtons = 0.12f;
			float prevButton = 0.4f;
			float xPercentage = 0.52f;

			playButton.PositionPercent = new Vector2(xPercentage, prevButton);
			playButton.Registration = playButton.Size / 2;
			playButton.CreateButton(new Rectangle(0, 0,
				(int)playButton.Size.X, (int)playButton.Size.Y));
			playButton.Zindex = 0.4f;
			playButton.OnActivate += () =>
			{
				onPressPlay();
			};

			AddChild(playButton);

			prevButton += distanceBetweenButtons;
			instructionButton.PositionPercent = new Vector2(xPercentage, prevButton);
			instructionButton.Registration = instructionButton.Size / 2;
			instructionButton.CreateButton(new Rectangle(0, 0,
				 (int)instructionButton.Size.X, (int)instructionButton.Size.Y));
			instructionButton.Zindex = 0.5f;
			instructionButton.OnActivate += () =>
			{
				onPressInstructions();
			};
			AddChild(instructionButton);

			prevButton += distanceBetweenButtons;
			creditsButton.PositionPercent = new Vector2(xPercentage, prevButton);
			creditsButton.Registration = creditsButton.Size / 2;
			creditsButton.CreateButton(new Rectangle(0, 0,
				 (int)creditsButton.Size.X, (int)creditsButton.Size.Y));
			creditsButton.Zindex = 0.5f;
			creditsButton.OnActivate += () =>
			{
				onPressCredits();
			};
			AddChild(creditsButton);

			prevButton += distanceBetweenButtons;
			quitButton.PositionPercent = new Vector2(xPercentage, prevButton);
			quitButton.Registration = quitButton.Size / 2;
			quitButton.CreateButton(new Rectangle(0, 0,
				 (int)quitButton.Size.X, (int)quitButton.Size.Y));
			quitButton.Zindex = 0.5f;
			quitButton.OnActivate += () =>
			{
				onPressQuit();
			};
			AddChild(quitButton);

			selectButton(playButton);
		}

		void onPressPlay()
		{
			Controller.ChangeEnvironment(new Level1Environment(Controller));
		}

		void onPressInstructions()
		{
			Controller.ChangeEnvironment(new Instructions(Controller));
		}

		void onPressCredits()
		{
			Controller.ChangeEnvironment(new Credits(Controller));
		}

		void onPressQuit()
		{
			Controller.Exit();
		}

		public override void Update(float elapsedTime)
		{
			//Scaling
			float scaleX = ScreenSize.X / menuBackground.Texture.Width;
			float scaleY = ScreenSize.Y / menuBackground.Texture.Height;

			if (scaleX < scaleY) {
				ScreenScale = new Vector2(1.0f, scaleX / scaleY);
				ScreenOffset = new Vector2(0.0f, (ScreenSize.Y - ScreenScale.Y * ScreenSize.Y) / 2);
			} else {
				ScreenScale = new Vector2(scaleY / scaleX, 1.0f);
				ScreenOffset = new Vector2((ScreenSize.X - ScreenScale.X * ScreenSize.X) / 2, 1.0f);
			}

			float scale = Math.Min(scaleX, scaleY);
			menuBackground.Scale = scale;
			playButton.Scale = scale;
			instructionButton.Scale = scale;
			creditsButton.Scale = scale;
			quitButton.Scale = scale;

			if ((Keyboard.GetState().IsKeyDown(Keys.Down) && !OldKeyboard.GetState().IsKeyDown(Keys.Down))
				|| (Keyboard.GetState().IsKeyDown(Keys.S) && !OldKeyboard.GetState().IsKeyDown(Keys.S))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadDown) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.DPadDown))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.LeftThumbstickDown) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.LeftThumbstickDown))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.RightThumbstickDown) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.RightThumbstickDown))
				)
			{
				unSelectCurrentButton();
				selectButton(currentButton.nextButton);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up) && !OldKeyboard.GetState().IsKeyDown(Keys.Up)
				|| Keyboard.GetState().IsKeyDown(Keys.W) && !OldKeyboard.GetState().IsKeyDown(Keys.W)
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadUp) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.DPadUp))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.LeftThumbstickUp) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.LeftThumbstickUp))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.RightThumbstickUp) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.RightThumbstickUp))
				)
			{
				unSelectCurrentButton();
				selectButton(currentButton.prevButton);
			}

			if (!(Keyboard.GetState().IsKeyDown(Keys.RightAlt) || Keyboard.GetState().IsKeyDown(Keys.LeftAlt)))
			{
				if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !OldKeyboard.GetState().IsKeyDown(Keys.Enter)
					|| GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.A) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.A))
				{
					currentButton.isPressed = true;
				}
				if (Keyboard.GetState().IsKeyUp(Keys.Enter) && !OldKeyboard.GetState().IsKeyUp(Keys.Enter)
					|| GamePad.GetState(PlayerIndex.One).IsButtonUp(Input.Buttons.A) && !OldGamePad.GetState().IsButtonUp(Input.Buttons.A))
				{
					if (currentButton.isPressed)
						currentButton.DispatchOnMouseUp(true);
				}
			}

			base.Update(elapsedTime);
		}

		internal void selectButton(MainMenuButton button)
		{
			if (!buttonPressed)
			{
				button.isSelected = true;
				currentButton = button;
			}
		}

		//This is to be called by a button, when the mouse hovers over a different button.
		internal void unSelectCurrentButton()
		{
			currentButton.isSelected = false;
		}

	}
}

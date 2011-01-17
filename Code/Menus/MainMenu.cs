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
			isSelected = false;
			isPressed = false;
			prevButton = this;
			nextButton = this;
			Texture = menu.contentManager.Load<Texture2D>(textureName);
			OnMouseOver += () =>
			{
				menu.unSelectCurrentButton();
				menu.selectButton(this);
			};
			OnMouseDown += () => { isPressed = true; };
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
		GameEnvironment currentGame;

		public MainMenu(Controller cntl)
			: base(cntl)
		{

			MainMenuButton letterBackground = new MainMenuButton(this, "buttons\\pause_menu");
			letterBackground.Registration = letterBackground.Size / 2;
			letterBackground.PositionPercent = new Vector2(0.5f, 0.45f);
			letterBackground.Zindex = 0.6f;
			AddChild(letterBackground);

			//The buttons:
			MainMenuButton playButton = new MainMenuButton(this, "buttons\\back_to_game");
			MainMenuButton instructionButton = new MainMenuButton(this, "buttons\\back_to_checkpoint");
			MainMenuButton creditsButton = new MainMenuButton(this, "buttons\\back_to_start");
			MainMenuButton quitButton = new MainMenuButton(this, "buttons\\back_to_title");

			playButton.nextButton = instructionButton;
			instructionButton.nextButton = creditsButton;
			creditsButton.nextButton = quitButton;
			quitButton.nextButton = playButton;

			float distanceBetweenButtons = 65.0f;
			float prevButton = -100.0f;

			playButton.PositionPercent = new Vector2(0.5f, 0.45f);
			playButton.Registration = playButton.Size / 2;
			playButton.Position = (new Vector2(0.0f, prevButton));
			playButton.CreateButton(new Rectangle(0, 0,
				(int)playButton.Size.X, (int)playButton.Size.Y));
			playButton.Zindex = 0.4f;
			playButton.OnActivate += () =>
			{
				onPressPlay();
			};

			AddChild(playButton);

			instructionButton.PositionPercent = new Vector2(0.5f, 0.45f);
			instructionButton.Registration = instructionButton.Size / 2;
			prevButton += distanceBetweenButtons;
			instructionButton.Position = (new Vector2(15.0f, prevButton));
			instructionButton.CreateButton(new Rectangle(0, 0,
				 (int)instructionButton.Size.X, (int)instructionButton.Size.Y));
			instructionButton.Zindex = 0.5f;
			instructionButton.OnActivate += () =>
			{
				onPressInstructions();
			};
			AddChild(instructionButton);

			creditsButton.PositionPercent = new Vector2(0.5f, 0.45f);
			creditsButton.Registration = creditsButton.Size / 2;
			prevButton += distanceBetweenButtons;
			creditsButton.Position = (new Vector2(0.0f, prevButton));
			creditsButton.CreateButton(new Rectangle(0, 0,
				 (int)creditsButton.Size.X, (int)creditsButton.Size.Y));
			creditsButton.Zindex = 0.5f;
			creditsButton.OnActivate += () =>
			{
				onPressCredits();
			};
			AddChild(creditsButton);

			quitButton.PositionPercent = new Vector2(0.5f, 0.45f);
			quitButton.Registration = quitButton.Size / 2;
			prevButton += distanceBetweenButtons;
			quitButton.Position = (new Vector2(0.0f, prevButton));
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
			//need intructions;
		}

		void onPressCredits()
		{
			//need credits
		}

		void onPressQuit()
		{
			Controller.Exit();
		}

		public override void Update(float elapsedTime)
		{
			if ((Keyboard.GetState().IsKeyDown(Keys.Down) && !OldKeyboard.GetState().IsKeyDown(Keys.Down))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadDown) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.DPadDown))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.LeftThumbstickDown) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.LeftThumbstickDown)))
			{
				unSelectCurrentButton();
				selectButton(currentButton.nextButton);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up) && !OldKeyboard.GetState().IsKeyDown(Keys.Up)
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.DPadUp) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.DPadUp))
				|| (GamePad.GetState(PlayerIndex.One).IsButtonDown(Input.Buttons.LeftThumbstickUp) && !OldGamePad.GetState().IsButtonDown(Input.Buttons.LeftThumbstickUp)))
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

			button.isSelected = true;
			currentButton = button;
		}

		//This is to be called by a button, when the mouse hovers over a different button.
		internal void unSelectCurrentButton()
		{
			currentButton.isSelected = false;
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sputnik.Game;

namespace Sputnik.Menus
{
    internal class MenuButton : Widget
    {
        public MenuButton prevButton { get; set; }
        private MenuButton nButton;
        public MenuButton nextButton
        {
            get{ return nButton; }
            set
            {
                nButton = value;
                value.prevButton = this;
            }
        }
        public bool isSelected;
        public bool isPressed;
        public MenuButton(GamePauseMenu menu, string textureName)
            : base(menu)
        {
            isSelected = false;
            isPressed = false;
            prevButton = this;
            nextButton = this;
            Texture = menu.contentManager.Load<Texture2D>(textureName);
            OnMouseOver += () => {
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
            if(isPressed)
                VertexColor = Color.Green;
            base.Update(elapsedTime);
        }
    }

    public class GamePauseMenu : PopUp
    {
        MenuButton currentButton;
        
        public GamePauseMenu(Controller cntl, GameEnvironment game)
            :base(cntl, game)
        {

            MenuButton letterBackground = new MenuButton(this, "buttons\\pause_menu");
            letterBackground.Registration = letterBackground.Size / 2;
            letterBackground.PositionPercent = new Vector2(0.5f, 0.45f);
            letterBackground.Zindex = 0.6f;
            AddChild(letterBackground);

            //The buttons:
            MenuButton resumeGameButton = new MenuButton(this, "buttons\\back_to_game");
            MenuButton checkpointButton = new MenuButton(this, "buttons\\back_to_checkpoint");
            MenuButton startButton = new MenuButton(this, "buttons\\back_to_start");
            MenuButton titleButton = new MenuButton(this, "buttons\\back_to_title");

            resumeGameButton.nextButton = checkpointButton;
            checkpointButton.nextButton = startButton;
            startButton.nextButton = titleButton;
            titleButton.nextButton = resumeGameButton;

            float distanceBetweenButtons = 65.0f;
            float prevButton = -100.0f;

            resumeGameButton.PositionPercent = new Vector2(0.5f, 0.45f);
            resumeGameButton.Registration = resumeGameButton.Size / 2;
            resumeGameButton.Position = (new Vector2(0.0f, prevButton)); 
            resumeGameButton.CreateButton(new Rectangle(0, 0,
                (int)resumeGameButton.Size.X , (int)resumeGameButton.Size.Y ));
            resumeGameButton.Zindex = 0.4f;
            resumeGameButton.OnActivate += () => {
                onPressResume();
            };
            
			AddChild(resumeGameButton);

            checkpointButton.PositionPercent = new Vector2(0.5f, 0.45f);
            checkpointButton.Registration = checkpointButton.Size / 2;
            prevButton += distanceBetweenButtons;
            checkpointButton.Position = (new Vector2(15.0f, prevButton)); 
            checkpointButton.CreateButton(new Rectangle(0, 0,
                 (int)checkpointButton.Size.X, (int)checkpointButton.Size.Y ));
            checkpointButton.Zindex = 0.5f;
            checkpointButton.OnActivate += () =>
            {
                onPressCheckpoint();
            };
            AddChild(checkpointButton);

            startButton.PositionPercent = new Vector2(0.5f, 0.45f);
            startButton.Registration = startButton.Size / 2;
            prevButton += distanceBetweenButtons;
            startButton.Position = (new Vector2(0.0f, prevButton)); 
            startButton.CreateButton(new Rectangle(0, 0,
                 (int)startButton.Size.X, (int)startButton.Size.Y));
            startButton.Zindex = 0.5f;
            startButton.OnActivate += () =>
            {
                onPressBackToStart();
            };
            AddChild(startButton);

            titleButton.PositionPercent = new Vector2(0.5f, 0.45f);
            titleButton.Registration = titleButton.Size / 2;
            prevButton += distanceBetweenButtons;
            titleButton.Position = (new Vector2(0.0f, prevButton)); 
            titleButton.CreateButton(new Rectangle(0, 0,
                 (int)titleButton.Size.X, (int)titleButton.Size.Y));
            titleButton.Zindex = 0.5f;
            titleButton.OnActivate += () =>
            {
                onPressTitle();
            };
            AddChild(titleButton);

            selectButton(resumeGameButton);
        }

        void onPressResume()
        {
            m_game.unPause();
        }

        void onPressCheckpoint()
        {
            m_game.unPause(); 
            //TODO: make this go restart from last checkpoint
        }

        void onPressBackToStart()
        {
            m_game.unPause();
            Controller.ChangeEnvironment(new Level1Environment(Controller));
        }

        void onPressTitle()
        {
            m_game.unPause();
            //TODO: go back to the main menu.
        }

        public override void Update(float elapsedTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !OldKeyboard.GetState().IsKeyDown(Keys.Down))
            {
                unSelectCurrentButton();
                selectButton(currentButton.nextButton);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !OldKeyboard.GetState().IsKeyDown(Keys.Up))
            {
                unSelectCurrentButton();
                selectButton(currentButton.prevButton);
            }

            if(!( Keyboard.GetState().IsKeyDown(Keys.RightAlt) || Keyboard.GetState().IsKeyDown(Keys.LeftAlt) ))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !OldKeyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    currentButton.isPressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.Enter) && !OldKeyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    if (currentButton.isPressed)
                        currentButton.DispatchOnMouseUp(true);
                }
            }

            base.Update(elapsedTime);
        }

        internal void selectButton(MenuButton button)
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

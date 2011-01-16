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
    class MenuButton : Widget
    {
        public MenuButton(Menu menu, string textureName)
            : base(menu)
        {
            Texture = menu.contentManager.Load<Texture2D>(textureName);
            OnMouseOver += () => { VertexColor = Color.LightGreen; };
            OnMouseOut += () => { VertexColor = Color.White; };
            OnMouseDown += () => { VertexColor = Color.Green; };
        }
    }

    public class GamePauseMenu : PopUp
    {
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

            float distanceBetweenButtons = 65.0f;
            float prevButton = -100.0f;

            resumeGameButton.PositionPercent = new Vector2(0.5f, 0.45f);
            resumeGameButton.Registration = resumeGameButton.Size / 2;
            resumeGameButton.Position = (new Vector2(0.0f, prevButton)); 
            resumeGameButton.CreateButton(new Rectangle(0, 0,
                 (int)resumeGameButton.Size.X / 2, (int)resumeGameButton.Size.Y / 2));
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
                 (int)checkpointButton.Size.X / 2, (int)checkpointButton.Size.Y / 2));
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
                 (int)startButton.Size.X / 2, (int)startButton.Size.Y / 2));
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
                 (int)titleButton.Size.X / 2, (int)titleButton.Size.Y / 2));
            titleButton.Zindex = 0.5f;
            titleButton.OnActivate += () =>
            {
                onPressTitle();
            };
            AddChild(titleButton);
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

    }
}

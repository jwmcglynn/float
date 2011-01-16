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
            OnMouseOver += () => { VertexColor = Color.Aquamarine; };
            OnMouseOut += () => { VertexColor = Color.White; };
            OnMouseDown += () => { VertexColor = Color.Blue; };
        }
    }

    public class GamePauseMenu : PopUp
    {
        public GamePauseMenu(Controller cntl, GameEnvironment game)
            :base(cntl, game)
        {

            float halfButtonTextureHeight;
            float halfButtonTextureWidth;

            MenuButton letterBackground = new MenuButton(this, "buttons\\pause_menu");
            halfButtonTextureHeight = letterBackground.Texture.Height / 2;
            halfButtonTextureWidth = letterBackground.Texture.Width / 2;
            letterBackground.Registration = letterBackground.Size / 2;
            letterBackground.PositionPercent = new Vector2(0.5f, 0.45f);
            letterBackground.Zindex = 0.6f;
            AddChild(letterBackground);

            //TODO make the buttons.
            MenuButton resumeGameButton = new MenuButton(this, "buttons\\back_to_game");
            MenuButton checkpointButton = new MenuButton(this, "buttons\\back_to_checkpoint");
            MenuButton startButton = new MenuButton(this, "buttons\\back_to_start");
            MenuButton titleButton = new MenuButton(this, "buttons\\back_to_title");

            halfButtonTextureHeight = resumeGameButton.Texture.Height/2;
            halfButtonTextureWidth = resumeGameButton.Texture.Width/2;
			resumeGameButton.PositionPercent = new Vector2(0.5f,0.28f);
            resumeGameButton.Registration = resumeGameButton.Size / 2;
            resumeGameButton.CreateButton(new Rectangle(0, 0, 
                (int)halfButtonTextureWidth * 2, (int)halfButtonTextureHeight * 2));
            resumeGameButton.Zindex = 0.4f;
            resumeGameButton.OnActivate += () => {
                onPressResume();
            };
			AddChild(resumeGameButton);


            halfButtonTextureHeight = checkpointButton.Texture.Height / 2;
            halfButtonTextureWidth = checkpointButton.Texture.Width / 2;
            checkpointButton.PositionPercent = new Vector2(0.5f, 0.58f);
            checkpointButton.Registration = checkpointButton.Size / 2;
            checkpointButton.CreateButton(new Rectangle(0, 0,
                (int)halfButtonTextureWidth * 2, (int)halfButtonTextureHeight * 2));
            checkpointButton.Zindex = 0.5f;
            checkpointButton.OnActivate += () =>
            {
                onPressCheckpoint();
            };
            AddChild(checkpointButton);

            halfButtonTextureHeight = startButton.Texture.Height / 2;
            halfButtonTextureWidth = startButton.Texture.Width / 2;
            startButton.PositionPercent = new Vector2(0.5f, 0.38f);
            startButton.Registration = startButton.Size / 2;
            startButton.CreateButton(new Rectangle(0, 0,
                 (int)halfButtonTextureWidth * 2, (int)halfButtonTextureHeight * 2));
            startButton.Zindex = 0.5f;
            startButton.OnActivate += () =>
            {
                onPressBackToStart();
            };
            AddChild(startButton);

            halfButtonTextureHeight = titleButton.Texture.Height / 2;
            halfButtonTextureWidth = titleButton.Texture.Width / 2;
            titleButton.PositionPercent = new Vector2(0.5f, 0.48f);
            titleButton.Registration = titleButton.Size / 2;
            titleButton.CreateButton(new Rectangle(0, 0,
                 (int)halfButtonTextureWidth * 2, (int)halfButtonTextureHeight * 2));
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

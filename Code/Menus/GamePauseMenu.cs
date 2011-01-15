using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            Texture = contentManager.Load<Texture2D>("pause_menu");

            float spaceBetweenButtons = Texture.Height * 0.6f;

            //TODO make the buttons.
            //MenuButton playButton = new MenuButton(this, "buttons\\play_button");
            //MenuButton instructionButton = new MenuButton(this, "buttons\\inst_button");
           // MenuButton creditsButton = new MenuButton(this, "buttons\\credits_button");
            //MenuButton quitButton = new MenuButton(this, "buttons\\quit_button");

			/*playButton.PositionPercent = new Vector2(0.5f,0.5f);
			playButton.Position = new Vector2(0.0f, spaceBetweenButtons);
			playButton.CreateButton(new Rectangle(-50, -16, 100, 32));
			//activation funtion goes here. Makes something happen.
			AddChild(playButton);*/

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace Sputnik.Menus
{
    public class TutorialPopUp : PopUp
    {
        Widget thePopUp;
        public TutorialPopUp(Controller cntl, GameEnvironment game, Keys key)
            : base(cntl, game)
        {
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
            }
            thePopUp.Registration = (thePopUp.Size / 2);
            thePopUp.PositionPercent = new Vector2(0.5f, 0.5f);
            thePopUp.Zindex = 0.1f;
            thePopUp.Visible = true;
            AddChild(thePopUp);
           
        }
    }
}
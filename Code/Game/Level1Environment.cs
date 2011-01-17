using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class Level1Environment : GameEnvironment{
		public Level1Environment(Controller ctrl)
				: base(ctrl) {

			LoadMap("LevelDesignThree.tmx");

			AddChild(new RepeatingBackground(this));
			AddChild(new Moon(this));

            //Balloon.enableUp = false;
            //Balloon.enableDown = false;
            //Balloon.enableRight = false;
            //Balloon.enableLeft = false;
		}
	}
}

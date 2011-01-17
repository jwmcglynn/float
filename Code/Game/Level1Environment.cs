using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class Level1Environment : GameEnvironment{
		public Level1Environment(Controller ctrl)
				: base(ctrl) {

			LoadMap("LevelDesignOne.tmx");

			AddChild(new RepeatingBackground(this));
			AddChild(new Moon(this));
		}
	}
}

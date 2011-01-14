﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class Level1Environment : GameEnvironment{
		public Level1Environment(Controller ctrl)
				: base(ctrl) {

			LoadMap("Level1.tmx");

			RepeatingBackground bg = new RepeatingBackground(this);
			AddChild(bg);
		}
	}
}
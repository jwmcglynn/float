using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class TestLevelEnvironment : GameEnvironment {
		public TestLevelEnvironment(Controller ctrl)
			: base(ctrl) {

			LoadMap("TestLevel.tmx");
			LoadOrExtendMap("TestLevel.tmx");

			AddChild(new RepeatingBackground(this));
			AddChild(new Moon(this));
            Sound.PlayCue("music_dreaming");
		}
	}
}

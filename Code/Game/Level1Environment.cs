using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class Level1Environment : GameEnvironment{
		public Level1Environment(Controller ctrl)
				: base(ctrl) {

            LoadMap("LevelDesignSix.tmx");
            LoadOrExtendMap("LevelDesignFour.tmx");
            LoadOrExtendMap("LevelDesignOne.tmx");
            LoadOrExtendMap("LevelDesignThree.tmx");
            LoadOrExtendMap("LevelDesignTwo.tmx");

			AddChild(new RepeatingBackground(this));
			AddChild(new Moon(this));
            Sound.PlayCue("music_dreaming");

            //Balloon.enableUp = false;
            //Balloon.enableDown = false;
            //Balloon.enableRight = false;
            //Balloon.enableLeft = false;
		}
	}
}

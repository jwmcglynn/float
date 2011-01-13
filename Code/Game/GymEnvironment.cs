﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;

namespace Sputnik.Game {
	class GymEnvironment : GameEnvironment {
		public GymEnvironment(Controller ctrl)
				: base(ctrl) {

			/*Entity e = new SaphereBoss(this);
			AddChild(e);

			LoadMap("gym.tmx");

			Sound.PlayCue("bg_music");
			*/

            //I want to create a balloon in this world.
            GameEntity balloon = new Balloon(this);
            //I then want to add it as a child.
            GameEntity cloud = new Cloud(this);
            AddChild(balloon);
            AddChild(cloud);


			Bird b = new Bird(this);
			b.Position = new Vector2(500.0f, 200.0f);
			AddChild(b);
		}
	}
}
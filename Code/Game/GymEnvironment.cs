﻿﻿using System;
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
			//GameEntity balloon = new Balloon(this);
			////I then want to add it as a child.
			//AddChild(balloon);

			Balloon newBalloon = new Balloon(this);
			newBalloon.WarpPosition(new Vector2(0.0f, 500.0f));
			AddChild(newBalloon);


            GameEntity cloud = new Cloud(this);

            AddChild(cloud);


			Bird b = new Bird(this);
			b.Position = new Vector2(500.0f, 300.0f);
			AddChild(b);

            //Kaushik made a plane enter into the world :D
            GameEntity plane = new AirPlane(this);
            plane.Position = new Vector2(500.0f, 200.0f);
			AddChild(plane);

			RepeatingBackground bg = new RepeatingBackground(this);
			AddChild(bg);
		}
	}
}
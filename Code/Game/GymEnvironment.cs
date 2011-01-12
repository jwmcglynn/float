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

			for (int i = 0; i < 50; ++i) {
				GameEntity e = new GameEntity(this);
				e.LoadTexture(contentManager, "arrow");
				e.Registration = new Vector2(e.Texture.Width, e.Texture.Height) / 2;
				e.Position = new Vector2(RandomUtil.NextFloat(0.0f, ScreenVirtualSize.X), RandomUtil.NextFloat(0.0f, ScreenVirtualSize.Y)) - ScreenVirtualSize / 2;

				e.CreateCollisionBody(CollisionWorld, BodyType.Dynamic);
				
                //e.AddCollisionCircle(e.Texture.Width / 2, Vector2.Zero);
                //This is just a basic circle, lets try something cooler
                float width = e.Texture.Width / 2 * GameEnvironment.k_physicsScale, height = e.Texture.Height / 2 * GameEnvironment.k_physicsScale;
                e.AddCollisionShape(new PolygonShape(new Vertices(new Vector2[] {
                                                                        new Vector2(-width, 0),
                                                                        new Vector2(width, 0),
                                                                        new Vector2(0, height)
                                                                                 })));


				e.SetPhysicsVelocityOnce(new Vector2(RandomUtil.NextFloat(-50.0f, 50.0f), RandomUtil.NextFloat(-50.0f, 50.0f)));
                AddChild(e);
			}

            GameEntity balloon = new Balloon(this);
            AddChild(balloon);

		}
	}
}
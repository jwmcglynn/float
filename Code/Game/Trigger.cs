using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Sputnik.Menus;

namespace Sputnik.Game
{
    /// <summary>
    /// Triggers collide only with the Balloon, and they're collision box is determined
    /// through the editer
    /// </summary>
    class Trigger : GameEntity
    {
        private Rectangle shape;
        public Trigger(GameEnvironment env, SpawnPoint sp)
				: base(env, sp) {
            shape = sp.Rect;
			Initialize();
			Position = sp.Position;
		}

		private void Initialize() {

			CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

            AddCollisionRectangle(new Vector2(shape.Width/2, shape.Height/2), Position);
		}

        public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture)
        {
            return (entB is Balloon);
        }
    }
}

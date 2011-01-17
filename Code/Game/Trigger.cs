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
    abstract class Trigger : GameEntity
    {
		protected bool passed;
        private Rectangle shape;
        public Trigger(GameEnvironment env, SpawnPoint sp)
				: base(env, sp) {
            shape = sp.Rect;
			Initialize();
			Position = sp.Position;
			passed = false;
		}

		private void Initialize() {

			CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

            AddCollisionRectangle(new Vector2(shape.Width/2, shape.Height/2), Position);
		}

        public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture)
        {
			return !passed && (entB is Balloon);
        }

		public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
			if (!passed) {
				passed = true;
				OnTrigger((Balloon) entB);
			}

			base.OnCollide(entB, contact);
		}

		// Implement this.
		public abstract void OnTrigger(Balloon balloon);
    }
}

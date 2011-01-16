﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

namespace Sputnik.Game
{
    class AirPlane : GameEntity
    {
        public static float k_defaultVelX = -150.0f;
		private ParticleEntity m_smokeTrail;

        /***************************************************************************/

        // Debug constructor.
        public AirPlane(GameEnvironment env)
            : base(env)
        {
            Initialize();
        }

        // Regular constructor.
        public AirPlane(GameEnvironment env, SpawnPoint sp)
            : base(env, sp)
        {
            Initialize();
			Position = sp.Position;
        }

        private void Initialize() {
			// TODO: PROPER Particles?
			m_smokeTrail = new ParticleEntity(Environment, "MagicTrail");
			m_smokeTrail.Zindex = 0.9f;
			AddChild(m_smokeTrail);

            LoadTexture(Environment.contentManager, "plane");
            Registration = new Vector2(Texture.Width, Texture.Height) / 2; // temp.

            Scale = 0.5f;           
            CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

            AddCollisionCircle(50.0f, Vector2.Zero);
            DesiredVelocity = new Vector2(k_defaultVelX, 0.0f);
			SnapToRung();

			Zindex = 0.7f;
        }

        /***************************************************************************/

        public override void Update(float elapsedTime)
        {
			m_smokeTrail.Effect.Trigger(Position + new Vector2(110.0f, -5.0f));
            base.Update(elapsedTime);
        }

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}
    }
}

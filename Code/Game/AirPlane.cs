using System;
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

		private Animation m_anim = new Animation();

        /***************************************************************************/

        // Debug constructor.
        public AirPlane(GameEnvironment env)
            : base(env)
        {
            Initialize();
        }

        // Regular constructor.
        public AirPlane(GameEnvironment env, SpawnPoint sp)
				: base(env, sp) {
			Position = sp.Position;
            Initialize();
        }

        private void Initialize() {

			m_smokeTrail = new ParticleEntity(Environment, "smokeTrail");
			m_smokeTrail.Zindex = ZSettings.Plane + 0.01f; // Just behind plane.
			AddChild(m_smokeTrail);

			Sequence seq = new Sequence(Environment.contentManager);
			seq.AddFrame("plane\\Plan1", 0.1f);
			seq.AddFrame("plane\\Plan2", 0.1f);
			seq.AddFrame("plane\\Plan3", 0.2f);
			seq.AddFrame("plane\\Plan4", 0.1f);
			seq.Loop = true;

			m_anim.PlaySequence(seq);

            Registration = new Vector2(250.0f, 250.0f);

            Scale = 0.5f;
            CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

			AddCollisionCircle(50.0f, new Vector2(-50.0f, 0.0f));
			AddCollisionCircle(50.0f, Vector2.Zero);
			AddCollisionCircle(50.0f, new Vector2(50.0f, 0.0f));
            DesiredVelocity = new Vector2(k_defaultVelX, 0.0f);
			SnapToRung();

			Zindex = ZSettings.Plane;
        }

        /***************************************************************************/

        public override void Update(float elapsedTime)
        {
			m_anim.Update(elapsedTime);
			Texture = m_anim.CurrentFrame;

			m_smokeTrail.Effect.Trigger(Position + new Vector2(110.0f, -5.0f));
            base.Update(elapsedTime);
        }

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}
    }
}

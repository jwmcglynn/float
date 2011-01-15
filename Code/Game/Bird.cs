using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

namespace Sputnik.Game {
	class Bird : GameEntity {
		public static float k_defaultVelX = 50.0f;
		public static float[] k_horizVels = {-100.0f, -70.0f, -40.0f};
		
		private int m_speedLevel = 1;

		private Animation m_anim = new Animation();

		/***************************************************************************/

		// Debug constructor.
		public Bird(GameEnvironment env)
				: base(env) {
			Initialize();
		}

		// Regular constructor.
		public Bird(GameEnvironment env, SpawnPoint sp)
				: base(env, sp) {
			Initialize();
			Position = sp.Position;
		}

		private void Initialize() {
			Sequence seq = new Sequence(Environment.contentManager);
			seq.AddFrame("bird\\Bird1", 0.33f);
			seq.AddFrame("bird\\Bird2", 0.33f);
			seq.Loop = true;
			m_anim.PlaySequence(seq);

			Texture = m_anim.CurrentFrame;
			Scale = 0.20f;

			Registration = new Vector2(Texture.Width, Texture.Height) / 2; // temp.

			CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

			AddCollisionCircle(100.0f * Scale, new Vector2(0.0f, 10.0f));
			DesiredVelocity = new Vector2(k_horizVels[m_speedLevel], 0.0f);
		}

		/***************************************************************************/

		public override void Update(float elapsedTime) {
			// TODO: Update animation.
			m_anim.Update(elapsedTime);
			Texture = m_anim.CurrentFrame;
			base.Update(elapsedTime);
		}

		
		/***************************************************************************/

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}

		public override void OnPressureChange(float amount) {
			if (amount < 0) {
				--m_speedLevel;
				if (m_speedLevel < 0) m_speedLevel = 0;
			} else {
				++m_speedLevel;
				if (m_speedLevel > k_horizVels.Length - 1) m_speedLevel = k_horizVels.Length - 1;
			}

			DesiredVelocity = new Vector2(k_horizVels[m_speedLevel], 0.0f);

			base.OnPressureChange(amount);
		}
	}
}

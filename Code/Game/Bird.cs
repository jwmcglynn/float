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
		public static float[] k_horizVels = {-200.0f, -100.0f, -50.0f};
		
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
			Position = sp.Position;
			Initialize();
		}

		private void Initialize() {
			SnapToRung();

			Sequence seq = new Sequence(Environment.contentManager);
			seq.AddFrame("bird\\Bird1", 0.33f);
			seq.AddFrame("bird\\Bird2", 0.33f);
			seq.Loop = true;
			m_anim.PlaySequence(seq);

			Texture = m_anim.CurrentFrame;
			Scale = 0.20f;
			Zindex = ZSettings.Bird;

			Registration = new Vector2(221.0f, 331.0f);

			CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);
			AddCollisionCircle(100.0f * Scale, Vector2.Zero);

			SetBirdVel(Environment.Pressure);
		}

		/***************************************************************************/

		public override void Update(float elapsedTime) {
			m_anim.Update(elapsedTime);
			Texture = m_anim.CurrentFrame;
			base.Update(elapsedTime);
		}

		
		/***************************************************************************/

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}

		private void SetBirdVel(float amount) {
			if (amount < 0) {
				DesiredVelocity = new Vector2(k_horizVels[0], 0.0f);
			} else if (amount > 0) {
				DesiredVelocity = new Vector2(k_horizVels[2], 0.0f);
			} else {
				DesiredVelocity = new Vector2(k_horizVels[1], 0.0f);
			}
		}

		public override void OnPressureChange(float amount) {
			SetBirdVel(amount);
			base.OnPressureChange(amount);
		}
	}
}

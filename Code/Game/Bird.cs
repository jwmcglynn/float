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
		public static float[] k_horizVels = {-30.0f, -50.0f, -70.0f};
		
		private int m_speedLevel = 1;

		/***************************************************************************/

		// Debug constructor.
		public Bird(GameEnvironment env)
				: base(env) {
			Initialize();
			Position = Vector2.Zero;
		}

		// Regular constructor.
		public Bird(GameEnvironment env, SpawnPoint sp)
				: base(env, sp) {
			Initialize();
		}

		private void Initialize() {
			LoadTexture(Environment.contentManager, "bird");
			Registration = new Vector2(Texture.Width, Texture.Height) / 2; // temp.

			CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);

			AddCollisionCircle(Texture.Width / 4, Vector2.Zero);
			DesiredVelocity = new Vector2(k_horizVels[m_speedLevel], 0.0f);
		}

		/***************************************************************************/

		public override void Update(float elapsedTime) {
			// TODO: Update animation.
			base.Update(elapsedTime);
		}

		
		/***************************************************************************/

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

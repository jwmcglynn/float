using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class Moon : GameEntity {
		/***************************************************************************/
		private static Vector2 k_posOffset = new Vector2(600.0f, -400.0f);

		public Moon(GameEnvironment env)
				: base(env) {

			Zindex = 0.99f;

			LoadTexture(Environment.contentManager, "Moon");
			Registration = Size * 0.5f; // Set registration before size is altered by Scale.
			
			Scale = 1.0f;
		}

		/***************************************************************************/

		public override void Update(float elapsedTime) {
			Position = Environment.Camera.Position + k_posOffset;
			base.Update(elapsedTime);
		}
	}
}

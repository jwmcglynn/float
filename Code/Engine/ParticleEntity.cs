using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiled = Squared.Tiled;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace Sputnik {
	public class ParticleEntity : Entity {
		public GameEnvironment Environment;
		public ParticleEffect Effect;

		public ParticleEntity(GameEnvironment env, string effectName) {
			Environment = env;

			Effect = Environment.contentManager.Load<ParticleEffect>(effectName).DeepCopy();
			Effect.Initialise();
			Effect.LoadContent(Environment.contentManager);
		}

		public override void Update(float elapsedTime) {
			Effect.Update(elapsedTime);
			base.Update(elapsedTime);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			Environment.SpriteBatchPop();
			Matrix tform = Environment.Camera.Transform;
			Environment.ParticleRenderer.RenderEffect(Effect, ref tform);
			Environment.SpriteBatchPush();
			base.Draw(spriteBatch);
		}

		/// <summary>
		/// If this particle effect is still emitting keep and it is Disposed() from
		/// the parent being destroyed keep it alive within the ParticleKeepalive list.
		/// </summary>
		public override void Dispose() {
			if (!(Parent is Environment) && Effect.ActiveParticlesCount != 0) {
				Environment.AddChild(this);
				Environment.ParticleKeepalive.Add(this);
			} else {
				base.Dispose();
			}
		}
	}
}

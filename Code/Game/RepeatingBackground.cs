using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sputnik.Game {
	class RepeatingBackground : GameEntity {
		private List<Entity> m_bg = new List<Entity>();
		private float m_width = 0.0f;

		private string[] k_bgNames = {"sky1", "sky2", "sky3"};

		private Texture2D[] m_textures;
		private int[] m_strides;

		public RepeatingBackground(GameEnvironment env)
				: base(env) {

			m_textures = new Texture2D[k_bgNames.Length];
			m_strides = new int[k_bgNames.Length];

			for (int i = 0; i < k_bgNames.Length; ++i) {
				m_textures[i] = Environment.contentManager.Load<Texture2D>(k_bgNames[i]);
				m_strides[i] = m_textures[i].Width;

				m_width += m_strides[i];
			}

			Zindex = 1.0f; // Very back.
		}

		public override bool ShouldCull() {
			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			float left = Environment.Camera.Rect.Left;
			float right = Environment.Camera.Rect.Right;

			float pos = 0.0f;
			float origPos = pos;

			int tex = 0;
			while (pos < right) {
				// Draw if visible.
				if (pos + m_strides[tex] > left) {
					spriteBatch.Draw(m_textures[tex], new Vector2(pos, 0.0f), null, VertexColor * Alpha, Rotation, Registration, Scale, SpriteEffects.None, Zindex);
				}

				pos += m_strides[tex];
				++tex;
				if (tex == m_textures.Length) tex = 0; // Wrap around.
			}

			base.Draw(spriteBatch);
		}
	}
}

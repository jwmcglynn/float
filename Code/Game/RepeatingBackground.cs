using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game {
	class RepeatingBackground : GameEntity {
		private List<Entity> m_bg = new List<Entity>();
		private float m_width = 0.0f;

		private string[] k_bgNames = {"sky1", "sky2", "sky3", "sky1", "sky2", "sky3", "sky1", "sky2", "sky3"};

		public RepeatingBackground(GameEnvironment env)
				: base(env) {
			
			for (int i = 0; i < k_bgNames.Length; ++i) {
				Entity bg = new Entity();
				bg.LoadTexture(Environment.contentManager, k_bgNames[i]);
				bg.Zindex = 1.0f; // Lowest.
				bg.Position = new Vector2(m_width, 0.0f);
				AddChild(bg);

				m_width += bg.Size.X;
				m_bg.Add(bg);
			}

			// Update positioning.
			Update(0.0f);
		}

		public override bool ShouldCull() {
			return false;
		}

		public override void Update(float elapsedTime) {
			while (m_bg.First().VisibleRect.Right < Environment.Camera.Rect.Left) {
				// Push to back of array.
				Entity first = m_bg.First();
				m_bg.Remove(first);
				m_bg.Add(first);

				// Move to the right.
				first.Position = first.Position + new Vector2(m_width, 0.0f);
			}

			// Make sure leftmost part of screen has texture.  If this happens the window must have been resized or the texture is too small.
			while (m_bg.First().VisibleRect.Left > Environment.Camera.Rect.Left) {
				// Push to back of array.
				Entity last = m_bg.Last();
				m_bg.Remove(last);
				m_bg.Insert(0, last);

				// Move to the left.
				last.Position = last.Position - new Vector2(m_width, 0.0f);
			}

			base.Update(elapsedTime);
		}
	}
}

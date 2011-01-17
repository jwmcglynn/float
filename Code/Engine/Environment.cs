using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Sputnik {
	public abstract class Environment : Entity {
		public Controller Controller { get; private set; }

		public Camera2D Camera;

		public Environment(Controller ctrl) {
			Controller = ctrl;
		}

		public ContentManager contentManager {
			get {
				return Controller.Content;
			}
		}

		private void GetDrawList(List<Entity> list, Entity ent) {
			list.Add(ent);
			foreach (Entity c in ent.Children) {
				GetDrawList(list, c);
			}
		}

		public void DrawChildren(SpriteBatch spriteBatch) {
			List<Entity> list = new List<Entity>();
			GetDrawList(list, this);

			foreach (Entity ent in list.OrderByDescending((ent) => ent.Zindex)) {
				ent.Draw(spriteBatch);
			}
		}

		/// <summary>
		/// Draw the environment.
		/// </summary>
		public abstract void Draw();
	}
}

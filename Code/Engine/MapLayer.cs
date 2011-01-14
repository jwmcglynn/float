using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiled = Squared.Tiled;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sputnik {
	class MapLayer : Entity {
		public GameEnvironment Environment;
		private Tiled.Map m_map;
		private string m_layer;

		public MapLayer(GameEnvironment env, Tiled.Map map, string layer, float zindex) {
			Environment = env;
			m_map = map;
			m_layer = layer;
			Zindex = zindex;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			Rectangle rectangle = Environment.Camera.Rect;

			// Draw map.
			m_map.Layers[m_layer].Draw(spriteBatch, m_map.Tilesets.Values, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), m_map.TileWidth, m_map.TileHeight, Zindex);
		}
	}
}

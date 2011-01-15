using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace Sputnik
{
	class Sequence
	{
		private ContentManager m_contentManager;

		public struct Frame {
			public Texture2D Texture;
			public float Time;
		}

		private List<Frame> m_frames = new List<Frame>();

		public bool Loop { get; set; }

		public List<Frame> Frames { get { return m_frames; } }
		public int Count { get { return m_frames.Count; } }

		/***************************************************************************/

		public Sequence(ContentManager manager) {
			m_contentManager = manager;
		}

		public void AddFrame(string filename, float duration)
		{
			Frame f;
			f.Texture = m_contentManager.Load<Texture2D>(filename);
			f.Time = duration;

			m_frames.Add(f);
		}

		public Frame FrameAt(int index)
		{
			return m_frames[index];
		}
	}
}

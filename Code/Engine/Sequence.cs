﻿using System;
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
			public Vector2 Offset;
		}

		private List<Frame> m_frames = new List<Frame>();

		public bool Loop { get; set; }

		public List<Frame> Frames { get { return m_frames; } }
		public int Count { get { return m_frames.Count; } }

		/***************************************************************************/

		public Sequence(ContentManager manager) {
			m_contentManager = manager;
		}

		public void AddFrame(string filename, float duration, Vector2? offset = null)
		{
			Frame f;
			if (filename != null) f.Texture = m_contentManager.Load<Texture2D>(filename);
			else f.Texture = null;
			f.Time = duration;

			f.Offset = offset == null ? Vector2.Zero: (Vector2)offset;

			m_frames.Add(f);
		}

		public Frame FrameAt(int index)
		{
			return m_frames[index];
		}
	}
}

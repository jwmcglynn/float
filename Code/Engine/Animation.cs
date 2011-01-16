using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Sputnik
{
	class Animation
	{
		private Sequence m_currentSequence;
		private int currentframe;
		private float frametime;

		public bool Done { get; private set; }

		// Change to a new sequence.
		public void PlaySequence(Sequence sequence)
		{
			if (m_currentSequence != sequence) {
				PlayOrRestartSequence(sequence);
			}
		}

		public void PlayOrRestartSequence(Sequence sequence) {
			Done = false;
			m_currentSequence = sequence;
			currentframe = 0;
			frametime = 0.0f;
		}

		// Get current texture.
		public Texture2D CurrentFrame { get { return m_currentSequence.FrameAt(currentframe).Texture; } }

		// Update currently displayed frame.
		public void Update(float elapsedTime)
		{
			frametime += elapsedTime;

			while (currentframe < m_currentSequence.Count && frametime > m_currentSequence.FrameAt(currentframe).Time) {
				frametime -= m_currentSequence.FrameAt(currentframe).Time;
				currentframe++;

				if (currentframe == m_currentSequence.Count) {
					if (m_currentSequence.Loop) {
						// go back to first frame 
						currentframe = 0;
					} else {
						currentframe = m_currentSequence.Count - 1;
						Done = true;
					}
				}
			}
		}
	}
}

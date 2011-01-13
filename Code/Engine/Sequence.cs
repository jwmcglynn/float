using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace Sputnik.Engine
{
    class Sequence
    {
        private List<Texture2D> frames = new List<Texture2D>();
        private List<float> timeframes = new List<float>();
		public bool Loop; 

        void AddFrame(ContentManager contentManager, string filename, float duration)
        {
            frames.Add( contentManager.Load<Texture2D>(filename));

            timeframes.Add(duration);
        }

        public List<Texture2D> getFrames()
        {
    
         return frames;
         }

        public List<float> getTime()
        {
            return timeframes;
        }


    }
}

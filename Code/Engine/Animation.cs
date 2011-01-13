using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Sputnik.Engine
{
    class Animation
    {
        Sequence m_currentSequence;
		bool isdone=false;
        int currentframe; 
        float frametime; 
         // Change to a new sequence.
         void PlaySequence(Sequence sequence)
         {
			 isdone=false;
         m_currentSequence=sequence;
             currentframe=0;
             frametime=0;

             
         }

           // Get current texture.
           Texture2D CurrentFrame()
            {      
            
			return  m_currentSequence.getFrames().ElementAt(currentframe);
            }

         // Update currently displayed frame.
         void Update(float elapsedTime)
         {
             frametime+=elapsedTime;

			 if (frametime > m_currentSequence.getTime().ElementAt(currentframe))
			 {
				 currentframe++;
				 frametime=0;
			 }




			 if (currentframe == m_currentSequence.getFrames().Count)
			 {

				 if (m_currentSequence.Loop)
				 {   // go back to first frame 
					 currentframe = 0;
				 }
				 else
				 {
					 currentframe = currentframe - 1;
					 isdone = true;
				 }
			 }


		
			 
			
			 
         }
    }
}

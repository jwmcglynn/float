using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game
{
    class Checkpoint : Trigger
    {

        public Checkpoint(GameEnvironment env, SpawnPoint sp)
            : base(env, sp)
        {
        }

		public override void OnTrigger(Balloon balloon) {
			if (balloon.isBalloonAlive) {
				balloon.SpawnPoint.Position = balloon.PreviousPosition;
			}
		}
    }
}

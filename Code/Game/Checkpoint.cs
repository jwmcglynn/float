using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sputnik.Game
{
    class Checkpoint : Trigger
    {
		bool passed;
        public Checkpoint(GameEnvironment env, SpawnPoint sp)
            : base(env, sp)
        {
			passed = false;
        }

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            base.OnCollide(entB, contact);
			if (!passed)
			{
				((Balloon)entB).SpawnPoint.Position = entB.Position;
				passed = true;
			}
        }
    }
}

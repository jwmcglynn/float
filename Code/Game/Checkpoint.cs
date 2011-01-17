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

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            base.OnCollide(entB, contact);
             FarseerPhysics.Collision.WorldManifold manifold;
			 contact.GetWorldManifold(out manifold);
			 Vector2 posOfCollision = manifold.Points[0]*GameEnvironment.k_invPhysicsScale;
             ((Balloon)entB).SpawnPoint.Position = entB.Position;
        }
    }
}

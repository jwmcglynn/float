﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

namespace Sputnik.Game
{
    class Plane : GameEntity
    {
        public static float k_defaultVelX = -100.0f;

        /***************************************************************************/

        // Debug constructor.
        public Plane(GameEnvironment env)
            : base(env)
        {
            Initialize();
            Position = Vector2.Zero;
        }

        // Regular constructor.
        public Plane(GameEnvironment env, SpawnPoint sp)
            : base(env, sp)
        {
            Initialize();
        }

        private void Initialize()
        {
            LoadTexture(Environment.contentManager, "plane");
            Registration = new Vector2(Texture.Width, Texture.Height) / 2; // temp.

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);

            AddCollisionCircle(Texture.Width / 4, Vector2.Zero);
            DesiredVelocity = new Vector2(k_defaultVelX, 0.0f);
        }

        /***************************************************************************/

        public override void Update(float elapsedTime)
        {
            // TODO: Update animation.
            base.Update(elapsedTime);
        }


        /***************************************************************************/

        public override void OnPressureChange(float amount)
        {
            base.OnPressureChange(amount);
        }
    }
}
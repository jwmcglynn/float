﻿using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sputnik.Game
{
    enum BALLOON_STATE {
        DEAD, ALIVE, INVULNERABLE
    }
    class Balloon : GameEntity
    {
        public const float MAX_SPEED = 10.0f, SPECIAL_STATE_DURATION_IN_SECONDS = 5.0f;

        private BALLOON_STATE currentState;
        private float currentSpecialStateRemainingTime;
        private bool visible;

       	public Balloon(GameEnvironment env) : base(env) {
            
            Initialize();
		}

		public Balloon(GameEnvironment env, SpawnPoint sp) : base(env, sp) {
            Initialize();
		}

        private void Initialize()
        {
            currentState = BALLOON_STATE.INVULNERABLE;
            currentSpecialStateRemainingTime = SPECIAL_STATE_DURATION_IN_SECONDS;
            LoadTexture(Environment.contentManager, "redball");
            Registration = new Vector2(Texture.Width, Texture.Height) / 2;
            Position = Vector2.Zero;

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);

            AddCollisionCircle(Texture.Width / 2, Vector2.Zero);
            DesiredVelocity = new Vector2(250.0f, 0.0f);

            Environment.Camera.Focus = this;
        }

        public BALLOON_STATE stateOfBalloon
        {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
            }
        }

        public bool isBalloonAlive
        {
            get
            {
                return currentState != BALLOON_STATE.DEAD;
            }
        }

        public override void Update(float elapsedTime)
        {
            currentSpecialStateRemainingTime -= elapsedTime;

            base.Update(elapsedTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (currentSpecialStateRemainingTime > 0.0f && currentState == BALLOON_STATE.INVULNERABLE)
            {
                visible = !visible;
                if (visible)
                    base.Draw(spriteBatch);
            }
            else
            {
                base.Draw(spriteBatch);
            }
        }
        public override bool ShouldCull()
        {
            return false;
        }

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            base.OnCollide(entB, contact);
        }
    }
}

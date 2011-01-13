using FarseerPhysics.Collision.Shapes;
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
    /**
     * Created: Tuesday Jan 11 2011 by student.kaushik@gmail.com
     * 
     * BALLOON_STATE enum stores the possible states the balloon could be.
     * It may become more useful in future editions of the game, where other factors of the balloon may come into play.
     * For now it indicates the play-state of the balloon (invulnerable, alive, or dead).
     *
     */
    enum BALLOON_STATE {
        DEAD, ALIVE, INVULNERABLE
    }

    /**
     * Created: Tuesday Jan 11 2011 by student.kaushik@gmail.com
     * 
     * Balloon is the entity which the entity to which the player associates.
     * It interacts with pressure and temperature, as well as birds, and wind.
     */
    class Balloon : GameEntity
    {
        public const float MAX_SPEED = 10.0f, SPECIAL_STATE_DURATION_IN_SECONDS = 5.0f;

        private BALLOON_STATE currentState;
        private float currentSpecialStateRemainingTime;
        private bool visible;
        private Vector2 currentVelocity;
        public static Vector2 RIGHT = new Vector2(1, 0);
        public static Vector2 UP = new Vector2(0, -1);

       	public Balloon(GameEnvironment env) : base(env) {
            
            Initialize();
		}

		public Balloon(GameEnvironment env, SpawnPoint sp) : base(env, sp) {
            Initialize();
		}

        public override void OnPressureChange(float amount)
        {
            base.OnPressureChange(amount);

        }
        private void Initialize()
        {
            currentState = BALLOON_STATE.INVULNERABLE;
            currentSpecialStateRemainingTime = SPECIAL_STATE_DURATION_IN_SECONDS;
            LoadTexture(Environment.contentManager, "Balloon\\BalloonNorm1");
            Registration = new Vector2(Texture.Width, Texture.Height) / 2;
            Position = Vector2.Zero;

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);

            AddCollisionCircle(Texture.Width / 5 - 5, 85*UP + 34*RIGHT);

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

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
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

    enum BALLOON_MOTION_STATE
    {
       UNCHANGED, TEMP_INCREASED, TEMP_DECREASED, PRESSURE_INCREASED, PRESSURE_DECREASED
    }
    /**
     * Created: Tuesday Jan 11 2011 by student.kaushik@gmail.com
     * 
     * Balloon is the entity which the entity to which the player associates.
     * It interacts with pressure and temperature, as well as birds, and wind.
     */
    class Balloon : GameEntity
    {
        public static Vector2 RIGHT = Vector2.UnitX;
        public static Vector2 UP = -Vector2.UnitY;
        private static Vector2 DEFAULT_VELOCITY = 100 * RIGHT;
        private static Vector2 MAX_VELOCITY = 3 * DEFAULT_VELOCITY, MIN_VELOCITY = -1 * DEFAULT_VELOCITY;
        private static Vector2 MOVE_UP = new Vector2(0, -DEFAULT_VELOCITY.X / 1.1f);
        private static Vector2 MOVE_DOWN = new Vector2(0, DEFAULT_VELOCITY.X / 1.1f);
        public static float DEFAULT_MOVE_DURATION = 1;

        private static int INITIAL_TRACK = 4;
        public const float SPECIAL_STATE_DURATION_IN_SECONDS = 0.2f;
        private static Vector2 PRESSURE_SPEED_STEP = new Vector2(50.0f, 0.0f);
        public const float DEFAULT_DISTANCE_FROM_LEFT_SCREEN = 50.0f;

        private BALLOON_STATE currentState;
        private List<BALLOON_MOTION_STATE> currentMotionStates;

        private float currentSpecialStateRemainingTime;

        private int track;
        private float forseenDistance;


        private float moveDuration;

        private bool visible;
        private Vector2 currentVelocity;

		private bool m_dead = false;


       	public Balloon(GameEnvironment env) : base(env) {
            Initialize();
		}

		public Balloon(GameEnvironment env, SpawnPoint sp) : base(env, sp) {
            Initialize();
			Position = sp.Position;
		}

        private void Initialize()
        {
			Scale = 0.5f;

            currentState = BALLOON_STATE.INVULNERABLE;
            currentMotionStates = new List<BALLOON_MOTION_STATE>();
            currentMotionStates.Add(BALLOON_MOTION_STATE.UNCHANGED);

            currentSpecialStateRemainingTime = SPECIAL_STATE_DURATION_IN_SECONDS;

            DesiredVelocity = currentVelocity = DEFAULT_VELOCITY;

            track = INITIAL_TRACK;
            forseenDistance = 2.5f * TRACK_DISTANCE;
            moveDuration = 0;
            LoadTexture(Environment.contentManager, "Balloon\\BalloonNorm1");
			Registration = new Vector2(285.0f, 165.0f);
            Position = new Vector2(DEFAULT_DISTANCE_FROM_LEFT_SCREEN, TRACK_0 + track*TRACK_DISTANCE);
            //Position = Vector2.Zero;

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);

            AddCollisionCircle(90.0f * Scale, Vector2.Zero);

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
        public List<BALLOON_MOTION_STATE> stateOfBalloonMotion
        {
            get
            {
                return currentMotionStates;
            }
            set
            {
                currentMotionStates = value;
            }
        }
        public bool isBalloonAlive
        {
            get
            {
                return currentState != BALLOON_STATE.DEAD;
            }
        }

        public override void OnPressureChange(float amount)
        {
            base.OnPressureChange(amount);
            currentMotionStates.Add( amount < 0 ? BALLOON_MOTION_STATE.PRESSURE_DECREASED: BALLOON_MOTION_STATE.PRESSURE_INCREASED);
        }

        public override void OnTempChange(float amount)
        {
            base.OnTempChange(amount);
            currentMotionStates.Add(amount < 0 ? BALLOON_MOTION_STATE.TEMP_DECREASED : BALLOON_MOTION_STATE.TEMP_INCREASED);
        }
        public override void Update(float elapsedTime)
        {
            if(currentSpecialStateRemainingTime > 0)
                currentSpecialStateRemainingTime -= elapsedTime;
            if (moveDuration > 0)
                moveDuration -= elapsedTime;
            

            foreach (BALLOON_MOTION_STATE state in currentMotionStates)
            {
                switch (state)
                {
                    case BALLOON_MOTION_STATE.PRESSURE_DECREASED:
                        currentVelocity = MinX(currentVelocity + PRESSURE_SPEED_STEP, MAX_VELOCITY);
                        break;
                    case BALLOON_MOTION_STATE.PRESSURE_INCREASED:
                        currentVelocity = MaxX(currentVelocity - PRESSURE_SPEED_STEP,  MIN_VELOCITY);
                        break;
                    case BALLOON_MOTION_STATE.TEMP_DECREASED:
                        // if position of next track is less than K units away
                        if (Position.Y + forseenDistance > (track+1)*TRACK_DISTANCE + TRACK_0)
                        {
                            track++;
                            currentVelocity += MOVE_DOWN;
                        }
                        break;
                    case BALLOON_MOTION_STATE.TEMP_INCREASED:
                        if (Position.Y - forseenDistance < TRACK_0 + (track-1)*TRACK_DISTANCE )
                        {
                            track--;
                            currentVelocity += MOVE_UP;
                        }
                        break;
                    case BALLOON_MOTION_STATE.UNCHANGED:
                        
                        continue;
                }

                DesiredVelocity = currentVelocity;

                
            }

            currentMotionStates.Clear();

            if ((DesiredVelocity.Y > 0.0f && Position.Y >= TRACK_0 + track * TRACK_DISTANCE)
                    || (DesiredVelocity.Y < 0.0f && Position.Y <= TRACK_0 + track * TRACK_DISTANCE))
            {
                DesiredVelocity = new Vector2(DesiredVelocity.X, 0.0f);
                currentVelocity.Y = 0;
            }

            base.Update(elapsedTime);
        }

        private Vector2 MinX(Vector2 m1, Vector2 m2)
        {
            return m1.X < m2.X ? m1 : m2;
        }
        private Vector2 MaxX(Vector2 m1, Vector2 m2)
        {
            return m1.X > m2.X ? m1 : m2;
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
            return m_dead;
        }

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
			// TODO: Explode!
			m_dead = true;
			contact.Enabled = false;
            base.OnCollide(entB, contact);
        }
    }
}

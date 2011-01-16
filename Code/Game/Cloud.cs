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
    enum CLOUD_STATE {
        NORM, THUNDER, RAIN, HAIL, LIGHTNING
    }

    class Cloud : GameEntity
    {
        /*
         Clouds

            React to temperature change.
            Do NOT react to pressure.
            React to temperature button presses (up and down).

            Different types of clouds- 5 states and different collision properties
            Normal and Thunder - no collision
            Lightning - has collision and will kill you. [create collision for this and use a special body type - kinematic]
            Rain - collision extending from cloud to bottom of screen. Will push you down, but not slow horizontally.
            Hail - collision extending from cloud to bottom of screen. Will kill you on collision.

            Balloon interaction - talk to Kaushik about this.

            F1 debugs collision shapes.

            Hot or cold moves it one "stage".

            Lightning -> Thunder -> Normal -> Rain -> Hail
            Hottest to coldest 
         */


        public const float MAX_SPEED = 0.0f, SPECIAL_STATE_DURATION_IN_SECONDS = 5.0f;

        private CLOUD_STATE currentState;
        private int cloudState;
        public static Vector2 RIGHT = new Vector2(1, 0);
		public static Vector2 UP = new Vector2(0, -1);

		private Fixture m_rain;


       	public Cloud(GameEnvironment env) : base(env) {
            
            Initialize();
		}

		public Cloud(GameEnvironment env, SpawnPoint sp) : base(env, sp) {
            Initialize();
			Position = sp.Position;
		}
        public override void OnTempChange(float amount)
        {
            /*
			 * Going up -> amount > 0 -> cloudState dec
			 * Going down -> amount < 0 -> cloudState inc
			 */
            if (amount > 0 && cloudState < 2)
            {
                cloudState++;
            }
            else if (amount < 0 && cloudState > -2)
            {
                cloudState--;
            }

            switch (cloudState)
            {
                case 2: LoadTexture(Environment.contentManager, "cloud\\Lighting1");
                    currentState = CLOUD_STATE.LIGHTNING;
                    break;
                case 1: LoadTexture(Environment.contentManager, "cloud\\thunder1");
                    currentState = CLOUD_STATE.THUNDER;
                    break;
                case 0: LoadTexture(Environment.contentManager, "cloud\\CloudX1");
                    currentState = CLOUD_STATE.NORM;
                    break;
                case -1: LoadTexture(Environment.contentManager, "cloud\\Rain");
                    currentState = CLOUD_STATE.RAIN;
                    break;
                case -2: LoadTexture(Environment.contentManager, "cloud\\Hail1");
                    currentState = CLOUD_STATE.HAIL;
                    break;
            }
            base.OnPressureChange(amount);
            /**this is where the main portion of the environment interaction will occur. */
            base.OnTempChange(amount);
        }
        private void Initialize()
        {
            cloudState = 0;
            currentState = CLOUD_STATE.NORM;
            LoadTexture(Environment.contentManager, "cloud\\CloudX1");
            Registration = new Vector2(295.0f, 236.0f);

			Scale = 1.0f;

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

            Console.WriteLine(Environment.ScreenVirtualSize.Y);
            Console.WriteLine(Position.Y);

			Vector2 topleft = new Vector2(168.0f, 290.0f) - Registration;
			Vector2 bottomright = new Vector2(431.0f, 1016.0f) - Registration;
            m_rain = AddCollisionRectangle((bottomright - topleft) * 0.5f, (topleft + bottomright) * 0.5f); 

            SnapToRung();
            //300 x 240
            AddCollisionCircle(200.0f, Vector2.Zero);
        }

        public CLOUD_STATE stateOfCloud
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

        
        public override void Update(float elapsedTime)
        {
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !OldKeyboard.GetState().IsKeyDown(Keys.Up))
            {
                OnPressureChange(-1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !OldKeyboard.GetState().IsKeyDown(Keys.Down))
            {
                OnPressureChange(1);
            }
            */ // KEYBOARD input is handled in Balloon.cs for now.
            base.Update(elapsedTime);
        }

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (entB is Balloon)
            {
				Balloon b = (Balloon) entB;

				bool hitRain = (contact.FixtureA == m_rain || contact.FixtureB == m_rain);

                switch(stateOfCloud) {
					case CLOUD_STATE.LIGHTNING:
						if (!hitRain) OnNextUpdate += () => b.Kill();
						break;
                    case CLOUD_STATE.HAIL:
						if (hitRain) OnNextUpdate += () => b.Kill();
						break;
                    case CLOUD_STATE.RAIN:
						if (hitRain) OnNextUpdate += () => b.HitByRain();
						break;
                }
			}

			contact.Enabled = false;
			base.OnCollide(entB, contact);
        }
    }
}

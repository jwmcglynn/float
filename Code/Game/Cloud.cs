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

       	public Cloud(GameEnvironment env) : base(env) {
            
            Initialize();
		}

		public Cloud(GameEnvironment env, SpawnPoint sp) : base(env, sp) {
            Initialize();
			Position = sp.Position;
		}
        public override void OnTempChange(float amount)
        {
            
            if (amount == 1 && cloudState < 2)
            {
                cloudState++;
            }
            else if (amount == -1 && cloudState > -2)
            {
                cloudState--;
            }

            switch (cloudState)
            {
                case -2: LoadTexture(Environment.contentManager, "cloud\\Lighting1");
                    currentState = CLOUD_STATE.LIGHTNING;
                    CollisionBody.Active = true;
                    break;
                case -1: LoadTexture(Environment.contentManager, "cloud\\thunder1");
                    currentState = CLOUD_STATE.THUNDER;
                    CollisionBody.Active = false;
                    break;
                case 0: LoadTexture(Environment.contentManager, "cloud\\CloudX1");
                    currentState = CLOUD_STATE.NORM;
                    break;
                case 1: LoadTexture(Environment.contentManager, "cloud\\Rain");
                    currentState = CLOUD_STATE.RAIN;
                    CollisionBody.Active = false;
                    break;
                case 2: LoadTexture(Environment.contentManager, "cloud\\Hail1");
                    currentState = CLOUD_STATE.HAIL;
                    CollisionBody.Active = true;
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
            Registration = new Vector2(Texture.Width, Texture.Height) / 2;

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

            Console.WriteLine(Environment.ScreenVirtualSize.Y);
            Console.WriteLine(Position.Y);
            AddCollisionRectangle(new Vector2(300, Texture.Height) / 2, new Vector2(0, 200), 0.0f, 1.0f); 
            SnapToRung();
            //300 x 240
            CollisionBody.Active = false;
            AddCollisionCircle(220, new Vector2(0, -280));
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

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override bool ShouldCull()
        {
            return false;
        }

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            base.OnCollide(entB, contact);
            if (entB.Equals(typeof (Game.Balloon)))
            {
                switch(stateOfCloud)
                {
                    case CLOUD_STATE.HAIL: entB.Dispose(); break;
                    case CLOUD_STATE.LIGHTNING: entB.Dispose(); break;
                    case CLOUD_STATE.NORM: ; break;
                    case CLOUD_STATE.RAIN: entB.SetPhysicsVelocityOnce(new Vector2(0, 300)); break;
                    case CLOUD_STATE.THUNDER: ; break;
                }
            }
        }
    }
}

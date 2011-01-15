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
        WHITE, DARK, RAIN, HAIL, LIGHTING
    }

    class Cloud : GameEntity
    {
        public const float MAX_SPEED = 10.0f, SPECIAL_STATE_DURATION_IN_SECONDS = 5.0f;

        private CLOUD_STATE currentState;
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
            /**this is where the main portion of the environment interaction will occur. */
            base.OnTempChange(amount);
        }
        private void Initialize()
        {
            currentState = CLOUD_STATE.WHITE;
            LoadTexture(Environment.contentManager, "cloud\\NormCloud");
            Registration = new Vector2(Texture.Width, Texture.Height) / 2;

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);

            AddCollisionCircle(Texture.Width / 3, Vector2.Zero);
			SnapToRung();
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
        }
    }
}

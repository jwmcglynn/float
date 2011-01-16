using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Sputnik.Menus;

namespace Sputnik.Game
{
    class PopUpTrigger : GameEntity
    {
        private Rectangle shape;
        private GameEnvironment game;
        private SpawnPoint spawnPoint;
        private string TutorialType;
        public PopUpTrigger(GameEnvironment env, SpawnPoint sp, string tutorialType)
				: base(env, sp) {
            shape = sp.Rect;
			Initialize();
            game = env;
			Position = sp.Position;
            spawnPoint = sp;
            TutorialType = tutorialType;
		}

		private void Initialize() {

			//Registration = new Vector2(Texture.Width, Texture.Height) / 2; // temp.

			CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

            AddCollisionRectangle(new Vector2(shape.Width/2, shape.Height/2), Position);
		}

        public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture)
        {
            return (entB is Balloon);
        }

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            base.OnCollide(entB, contact);
            if (entB is Balloon) 
            {
                if (TutorialType == "HighPressure")
                    game.pause(new TutorialPopUp(game.Controller, game, Keys.Left));
                if (TutorialType == "LowPressure")
                    game.pause(new TutorialPopUp(game.Controller, game, Keys.Right));
                if (TutorialType == "TempUp")
                    game.pause(new TutorialPopUp(game.Controller, game, Keys.Up));
                if (TutorialType == "TempDown")
                    game.pause(new TutorialPopUp(game.Controller, game, Keys.Down));
                CollisionBody.Active = false;
            }
        }
    }
}

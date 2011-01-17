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
    class PopUpTrigger : Trigger
    {
        private GameEnvironment game;
        private SpawnPoint spawnPoint;
        private string TutorialType;
        public PopUpTrigger(GameEnvironment env, SpawnPoint sp, string tutorialType)
				: base(env, sp) {
            game = env;
            spawnPoint = sp;
            TutorialType = tutorialType;
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

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
using Sputnik.Game;

namespace Sputnik.Game
{
	class EndGameTrigger : Trigger
	{
		private GameEnvironment game;
		private SpawnPoint spawnPoint;
		float delay;
		float delayAfterFade = 2.0f;
		bool changedMusic = false;

		public EndGameTrigger(GameEnvironment env, SpawnPoint sp)
			:base(env, sp)
		{
			spawnPoint = sp;
			game = env;
		}

		public override void OnTrigger(Balloon balloon) {
			if (spawnPoint.Properties.ContainsKey("delay"))
				float.TryParse(spawnPoint.Properties["delay"], out delay);
			else delay = 0.0f;
			balloon.goToEndSequence(delay);
			Environment.fade = true;
		}

		public override void Update(float elapsedTime)
		{
			base.Update(elapsedTime);
			if (game.FadeOut.Alpha == 1.0f)
			{
				if (!changedMusic)
				{
					Sound.StopAllExceptMusic();
					Sound.PlayCue("music_title");
				}
				else
				{ changedMusic = true; }

				if (delayAfterFade <= 0.0f)
				{
					game.pause(new EndPopUp(game.Controller, game));
					Sound.PlayCue("scroll");
				}
				else delayAfterFade -= elapsedTime;
			}
		}
	}

}

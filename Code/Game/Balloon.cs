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
using Microsoft.Xna.Framework.Audio;

namespace Sputnik.Game
{
	class Balloon : GameEntity
	{
		///DEFAULT CONSTANTS FOR BALLOON
		/**
		 * Created: Tuesday Jan 11 2011 by student.kaushik@gmail.com
		 * 
		 * BALLOON_STATE enum stores the possible states the balloon could be.
		 * It may become more useful in future editions of the game, where other factors of the balloon may come into play.
		 * For now it indicates the play-state of the balloon (invulnerable, alive, or dead).
		 *
		 */
		public enum BALLOON_STATE {
			DEAD, ALIVE, INVULNERABLE
		}

		//DEFAULT SPEED CONSTANTS
		private static Vector2 DEFAULT_SPEED = new Vector2(100.0f, 0.0f);
		public const float MOVE_VEL = 250.0f;

        //DEFAULT POSITION CONSTANTS
		
		public const float INVULNERABILITY_TIME = 0.5f;

		public const float SPEED_UP = 2.0f;

		public const float CLOSE_TO_EPSILON = 5.0f;

		/// <summary>
		/// Instance variables for Balloon:
		/// </summary>

		private BALLOON_STATE currentState;
		private float currentSpecialStateRemainingTime;

		private Vector2 previousPosition;

		private bool m_dead;

		public Balloon(GameEnvironment env)
			: base(env)
		{
			Initialize();
		}
		public Balloon(GameEnvironment env, SpawnPoint sp)
			: base(env, sp)
		{
			Position = sp.Position;
			Initialize();
		}
		private void up()
		{

			// Update controls.
			KeyboardState keyState = Keyboard.GetState();
			KeyboardState oldkeyState = OldKeyboard.GetState();
		}
		private void Initialize()
		{
			Scale = 0.5f;
            m_dead = false;

			currentState = BALLOON_STATE.INVULNERABLE;
			currentSpecialStateRemainingTime = INVULNERABILITY_TIME;
			DesiredVelocity = DEFAULT_SPEED;

			LoadTexture(Environment.contentManager, "Balloon\\BalloonNorm1");
			Registration = new Vector2(285.0f, 165.0f);
			//Position = new Vector2(DEFAULT_DISTANCE_FROM_LEFT_SCREEN, TRACK_0 + track*TRACK_DISTANCE);
			//Position = Vector2.Zero;
			CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);
			AddCollisionCircle(90.0f * Scale, Vector2.Zero);

			previousPosition = Position;
			SnapToRung();
		}

		/// <summary>
		/// Set position in local space.
		/// </summary>
		public void WarpPosition(Vector2 newPos)
		{
			previousPosition = Position;
			Position = newPos;
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

        public virtual bool isCloseTo(float x, float y) {
            return Math.Abs(x - y) < CLOSE_TO_EPSILON;
        }

		public virtual int onTrack()
		{
			for(int i = 0; i < tracks.Length; i++)
                if( isCloseTo (Position.Y,tracks[i]) )
                    return i;

            return -1;
		}

        Cue downSound, upSound, leftSound, rightSound;

		int lastDirX = 0;
		int lastDirY = 0;
		/**
			* I will be editing this very  soon but I will be back in 2 hours.
			*/
		public override void Update(float elapsedTime)
		{
			if (currentSpecialStateRemainingTime > 0)
				currentSpecialStateRemainingTime -= elapsedTime;

			KeyboardState keyState = Keyboard.GetState();
			KeyboardState oldKeyState = OldKeyboard.GetState();

			int dirX = 0;
			int dirY = 0;

            //POMI: I realize this should be in Environment, but I'm throwing in cloud controls here
            //      for testing purposes. This part can and will be moved.

			if (keyState.IsKeyDown(Keys.Up))
            {
                if (!OldKeyboard.GetState().IsKeyDown(Keys.Up))
                {
                    Environment.OnTempChange(-1);
                }
                --dirY;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                if (!OldKeyboard.GetState().IsKeyDown(Keys.Down))
                {
                    Environment.OnTempChange(1);
                }
                ++dirY;
            }

			if (keyState.IsKeyDown(Keys.Left)) --dirX;
			if (keyState.IsKeyDown(Keys.Right)) ++dirX;

			if (dirY == -1 && dirY != lastDirY)
			{
				upSound = Sound.PlayCue("up");
			}
			if (dirY == 1 && dirY != lastDirY) {
				downSound = Sound.PlayCue("down");
			}

			// Events on key down.
			if (dirX == -1 && dirX != lastDirX)
			{
				leftSound = Sound.PlayCue("left");
				Environment.OnPressureChange(1.0f);
			}
			if (dirX == 1 && dirX != lastDirX)
			{
				rightSound = Sound.PlayCue("right");
				Environment.OnPressureChange(-1.0f);
			}

			if (downSound != null && dirY != 1)
			{
				downSound.Stop(AudioStopOptions.AsAuthored);
				downSound = null;
			}
			if (upSound != null && dirY != -1)
			{
				upSound.Stop(AudioStopOptions.AsAuthored);
				upSound = null;
			}
			if (leftSound != null && dirX != -1)
			{
				leftSound.Stop(AudioStopOptions.AsAuthored);
				leftSound = null;
				Environment.OnPressureChange(0.0f);

			}
			if (rightSound != null && dirX != 1)
			{
				rightSound.Stop(AudioStopOptions.AsAuthored);
				rightSound = null;
				Environment.OnPressureChange(0.0f);
			}

			//DEFAULT ACTION

			Vector2 vel = DesiredVelocity;
			Vector2 pos = Position;

			if (dirY == 0)
			{
				for (int i = 0; i < tracks.Length; i++)
				{
					if (Math.Sign(tracks[i] - pos.Y) != Math.Sign(tracks[i] - previousPosition.Y))
					{
						vel.Y = DEFAULT_SPEED.Y;
						pos.Y = tracks[i];
					}
				}
			}
			else
			{
				if ((dirY < 0 && pos.Y < tracks.First()) || (dirY > 0 && pos.Y > tracks.Last()))
				{
					vel.Y = DEFAULT_SPEED.Y;
					pos.Y = MathUtils.Clamp(pos.Y, tracks.First(), tracks.Last());
				}
				else
				{
					vel.Y = MOVE_VEL * dirY;
				}
			}

			bool atLeft = Environment.Camera.Rect.Left + LEFT_POSITION_BUFFER >= Position.X;
			bool atRight = Environment.Camera.Rect.Right - RIGHT_POSITION_BUFFER <= Position.X;

			if ((dirX < 0 && atLeft) || (dirX > 0 && atRight))
			{
				vel.X = DEFAULT_SPEED.X;
				pos.X = MathUtils.Clamp(pos.X, Environment.Camera.Rect.Left + LEFT_POSITION_BUFFER, Environment.Camera.Rect.Right - RIGHT_POSITION_BUFFER);
			}
			else
			{
				vel.X = DEFAULT_SPEED.X + MOVE_VEL * dirX;
			}

			previousPosition = pos;
			Position = pos;
			DesiredVelocity = vel;

			lastDirX = dirX;
			lastDirY = dirY;

			base.Update(elapsedTime);
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
            Sound.StopAll();
            Sound.PlayCue("pop");
		}
	}
}
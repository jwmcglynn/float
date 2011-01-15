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

		public const float LEFT_MOST_POSITION = 100;
		public const float RIGHT_MOST_POSITION = 100;

		public const float INVULNERABILITY_TIME = 0.5f;

		public const float SPEED_UP = 2.0f;

		public const float ON_TRACK_EPSILON = 0.01f;
		/// <summary>
		/// Instance variables for Balloon:
		/// </summary>

		private BALLOON_STATE currentState;
		private float currentSpecialStateRemainingTime;
		private int currentTrack;

		private Vector2 previousPosition;

		private bool m_dead = false;

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

		public virtual bool onTrack()
		{
			return (Position.Y - TRACK_0) % TRACK_DISTANCE <= 0.5f;// ActualVelocity.Y;
		}

        Cue downSound, upSound, leftSound, rightSound;


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

            if (keyState.IsKeyDown(Keys.Up)) --dirY;
            if (keyState.IsKeyDown(Keys.Down)) ++dirY;

            if (keyState.IsKeyDown(Keys.Left)) --dirX;
            if (keyState.IsKeyDown(Keys.Right)) ++dirX;

            if (!oldKeyState.IsKeyDown(Keys.Up) && keyState.IsKeyDown(Keys.Up))
            {
                upSound = Sound.PlayCue("up");
            }
			if (!oldKeyState.IsKeyDown(Keys.Down) && keyState.IsKeyDown(Keys.Down)) {
                downSound = Sound.PlayCue("down");
            }
            if (!oldKeyState.IsKeyDown(Keys.Left) && keyState.IsKeyDown(Keys.Left))
            {
                leftSound = Sound.PlayCue("left");
            }
            if (!oldKeyState.IsKeyDown(Keys.Right) && keyState.IsKeyDown(Keys.Right))
            {
                rightSound = Sound.PlayCue("right");
            }

            if (downSound != null && !keyState.IsKeyDown(Keys.Down))
            {
                downSound.Stop(AudioStopOptions.AsAuthored);
                downSound = null;
            }
            if (upSound != null && !keyState.IsKeyDown(Keys.Up))
            {
                upSound.Stop(AudioStopOptions.AsAuthored);
                upSound = null;
            }
            if (leftSound != null && !keyState.IsKeyDown(Keys.Left))
            {
                leftSound.Stop(AudioStopOptions.AsAuthored);
                leftSound = null;
            }
            if (rightSound != null && !keyState.IsKeyDown(Keys.Right))
            {
                rightSound.Stop(AudioStopOptions.AsAuthored);
                rightSound = null;
            }

			bool passedATrack = false;

			for (int i = 0; i < NUMBER_OF_TRACKS; i++)
			{
				float yOfTracki = TRACK_0 + TRACK_DISTANCE * i;
				bool crossedUp =
						previousPosition.Y > yOfTracki && yOfTracki > Position.Y;
				bool crossedDown =
						previousPosition.Y < yOfTracki && yOfTracki < Position.Y;
				if (crossedDown || crossedUp)
					passedATrack = true;
				if (crossedUp)
					currentTrack = i;
				if (crossedDown)
					currentTrack = i;
			}

			Console.WriteLine("track = " + currentTrack);

			bool onATrack = onTrack() || passedATrack;

			if (onATrack && dirX == 0 && dirY == 0)
			{
				DesiredVelocity = DEFAULT_SPEED;
			}

			else
			{
				bool onLastTrack = onATrack && currentTrack >= NUMBER_OF_TRACKS - 1;
				bool onFirstTrack = onATrack && currentTrack <= 0;
				bool atLeft = Environment.Camera.Rect.Left + LEFT_MOST_POSITION >= Position.X;
				bool atRight = Environment.Camera.Rect.Right - RIGHT_MOST_POSITION <= Position.X;

				DesiredVelocity = DEFAULT_SPEED + new Vector2(MOVE_VEL * dirX, MOVE_VEL * dirY);

				if (dirY > 0 && onLastTrack)
				{
					DesiredVelocity = new Vector2(DesiredVelocity.X, DEFAULT_SPEED.Y);
				}
				else if (dirY < 0 && onFirstTrack)
				{
					DesiredVelocity = new Vector2(DesiredVelocity.X, DEFAULT_SPEED.Y);
				}
				//if(!onFirstTrack && !onLastTrack) {
				//    if (dirX != 0 && onATrack) Environment.OnPressureChange(-dirX); // Left = pressure increase.
				//    if (dirY != 0 && onATrack) Environment.OnTempChange(-dirY); // Up = temp increase.

				//    DesiredVelocity = DesiredVelocity + new Vector2(STEP_RATE*dirX, STEP_RATE*dirY);
				//}

				if (dirX < 0 && atLeft)
				{
					DesiredVelocity = new Vector2(DEFAULT_SPEED.X, DesiredVelocity.Y);
				}
				else if (dirX > 0 && atRight)
				{
					DesiredVelocity = new Vector2(DEFAULT_SPEED.X, DesiredVelocity.Y);
				}
			}



			previousPosition = Position;
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
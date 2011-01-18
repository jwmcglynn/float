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
	public class Balloon : GameEntity
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
			DYING, ALIVE, INVULNERABLE, SUPER_GUST, DEAD, ENDING_SEQUENCE
		}

		//DEFAULT SPEED CONSTANTS
		private static Vector2 DEFAULT_SPEED = new Vector2(GameEnvironment.k_scrollSpeed, 0.0f);
		public const float MOVE_VEL = 250.0f;

        //DEFAULT POSITION CONSTANTS
		
		public const float INVULNERABILITY_TIME = 1.5f;

		public const float SPEED_UP = 2.0f;

		public const float CLOSE_TO_EPSILON = 5.0f;

		public const int NUMBER_OF_ANIMATION_STATES = 7;
		public const int DEAD_ANIM_INDEX = 0, ALIVE_ANIM_INDEX = 1, INVULNERABLE_ANIM_INDEX = 2,
						 UP_ANIM_INDEX = 3, DOWN_ANIM_INDEX = 4, FORWARD_ANIM_INDEX = 5,
						 BACK_ANIM_INDEX = 6;

		/// <summary>
		/// Instance variables for Balloon:
		/// </summary>

		private BALLOON_STATE currentState;
		private float currentSpecialStateRemainingTime;

		public Vector2 PreviousPosition;

		private Vector2 originalRegistration;

		private float m_distortedPosition = 0.0f;
		private float m_distortedVel = 0.0f;

		private const float k_rainRestoreDelay = 0.5f;
		private float m_distortRestoreDelay = 0.5f;
		private const float k_distortAmount = 1.5f * RUNG_HEIGHT;
		private const float k_distortedResetVel = -MOVE_VEL / 6.0f;
		private const float k_distortedFallVel = MOVE_VEL / 2;

		private float zoomAmount = 0.0125f;

		private ParticleEntity m_endExplosion; 

		enum DistortState {
			NONE,
			FALLING,
			FALLEN,
			RISING
		}

		private DistortState m_distortState = DistortState.NONE, m_lastDistortState = DistortState.NONE;

		private bool m_dead;

		private ParticleEntity drippingWet;

		private int m_lastRung = -1;
		private Animation anim = new Animation();
		private Sequence [] animations;

		//Enabling and disabling controls
		public bool enableUp;
		public bool enableDown;
		public bool enableRight;
		public bool enableLeft;
		private bool endingDescent;
		private float descentDelay;

		public Balloon(GameEnvironment env)
			: base(env)
		{
			Initialize();
		}
		public Balloon(GameEnvironment env, SpawnPoint sp)
			: base(env, sp)
		{
            env.Balloon = this;
			Position = sp.Position;
            Environment.Camera.MoveSpeed = Environment.defaultCameraMoveSpeed;
            Environment.Camera.Position = new Vector2(Position.X + 350, GameEnvironment.k_idealScreenSize.Y * 0.5f);
			Initialize();
		}
		private void up()
		{
			// Update controls.
			KeyboardState keyState = Keyboard.GetState();
			KeyboardState oldkeyState = OldKeyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            GamePadState oldpadState = OldGamePad.GetState();
		}
		private void Initialize()
		{
			Zindex = ZSettings.Balloon;
			Scale = 0.5f;
			m_dead = false;

			enableUp = true;
			enableDown = true;
			enableRight = true;
			enableLeft = true;
			endingDescent = false;

			currentState = BALLOON_STATE.INVULNERABLE;
			currentSpecialStateRemainingTime = INVULNERABILITY_TIME;

			DesiredVelocity = DEFAULT_SPEED;
			originalRegistration = new Vector2(285.0f, 165.0f);
			Registration = originalRegistration;

			//Position = new Vector2(DEFAULT_DISTANCE_FROM_LEFT_SCREEN, TRACK_0 + track*TRACK_DISTANCE);
			//Position = Vector2.Zero;
			CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);
			AddCollisionCircle(30.0f, Vector2.Zero);

			drippingWet = new ParticleEntity(Environment, "DrippingWet");
			drippingWet.Zindex = ZSettings.Balloon - 0.01f; // Just in front of balloon.
			AddChild(drippingWet);

			animations = new Sequence[NUMBER_OF_ANIMATION_STATES];

			float defaultDuration = 0.1f;

			//LOADING DEAD ANIMATION
			Sequence seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonPop1", 0.2f);
			seq.AddFrame("balloon\\BalloonPop2", 0.4f, new Vector2(291 - 279, 215 - 171));
			seq.AddFrame(null, 2.0f, new Vector2(291 - 279, 215 - 171));
			animations[DEAD_ANIM_INDEX] = seq;

			//LOADING ALIVE ANIMATION
			seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonNorm1", 0.3f);
			seq.AddFrame("balloon\\BalloonNorm2", 0.3f);
			seq.Loop = true;
			animations[ALIVE_ANIM_INDEX] = seq;

	
			//LOADING INVULNERABLE ANIMATION
			seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonNorm2", defaultDuration);

			seq.Loop = false;
			animations[INVULNERABLE_ANIM_INDEX] = seq;


			//LOADING UP ANIMATION
			seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonUp", float.PositiveInfinity);
			seq.Loop = true;
			animations[UP_ANIM_INDEX] = seq;
			
			//LOADING DOWN ANIMATION
			seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonFall", float.PositiveInfinity, new Vector2(0.0f, -10.0f));
			seq.Loop = true;
			animations[DOWN_ANIM_INDEX] = seq;

			//LOADING FORWARD ANIMATION
			seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonForward1", 0.15f, new Vector2(50, 20));
			seq.AddFrame("balloon\\BalloonForward2", 0.15f, new Vector2(50, 20));
			seq.Loop = true;
			animations[FORWARD_ANIM_INDEX] = seq;

			//LOADING BACK ANIMATION
			seq = new Sequence(Environment.contentManager);
			seq.AddFrame("balloon\\BalloonBack1", 0.3f);
			seq.AddFrame("balloon\\BalloonBack2", 0.3f);
			seq.Loop = true;
			animations[BACK_ANIM_INDEX] = seq;

			//particle effect
			m_endExplosion = new ParticleEntity(Environment, "balloon Explosion"); // here is where i add the particel effect
			m_endExplosion.Zindex = ZSettings.Balloon - 0.01f;
			AddChild(m_endExplosion);

			anim.PlaySequence(animations[ALIVE_ANIM_INDEX]);
			Texture = anim.CurrentFrame;
			PreviousPosition = Position;
			SnapToRung();
		}

		/// <summary>
		/// Set position in local space.
		/// </summary>
		public void WarpPosition(Vector2 newPos)
		{
			PreviousPosition = Position;
			Position = newPos;
            Environment.Camera.Position = new Vector2(newPos.X, GameEnvironment.k_idealScreenSize.Y * 0.5f);
		}

		public bool isBalloonAlive
		{
			get
			{
                return currentState != BALLOON_STATE.DYING && currentState != BALLOON_STATE.DEAD;
			}
		}

		Cue downSound, upSound, leftSound, rightSound;

		int lastDirX = 0;
		int lastDirY = 0;

		public override void Update(float elapsedTime) {
			switch (currentState) {
				case BALLOON_STATE.DYING:
					DesiredVelocity = Vector2.Zero;
					anim.Update(elapsedTime);
					Texture = anim.CurrentFrame;
					if (anim.Done) currentState = BALLOON_STATE.DEAD;
					break;
					
				case BALLOON_STATE.DEAD:
					DesiredVelocity = Vector2.Zero;
					m_dead = true;
					Environment.restartEntities();
					break;

				case BALLOON_STATE.ENDING_SEQUENCE:
					enableDown = false;
					enableUp = false;
					enableLeft = false;
					enableRight = false;

					descentDelay -= elapsedTime;
					if (descentDelay <= 0.0f) {
						endingDescent = true;
					}
					goto case BALLOON_STATE.ALIVE;

				case BALLOON_STATE.INVULNERABLE:
					currentSpecialStateRemainingTime -= elapsedTime;
			
					if (currentSpecialStateRemainingTime <= 0) {
						currentState = BALLOON_STATE.ALIVE;
						Visible = true;
					} else {
						Visible = !Visible;
					}

					goto case BALLOON_STATE.ALIVE;
					
				case BALLOON_STATE.ALIVE: {
					int dirX, dirY;
					ProcessControls(out dirX, out dirY);

					//DEFAULT ACTION
					Vector2 origVel = DesiredVelocity;
					Vector2 vel = DesiredVelocity;
					Vector2 pos = Position;

					pos.Y -= m_distortedPosition;

					if (dirY == 0)
					{
						for (int i = 0; i < tracks.Length; i++)
						{
							if (Math.Sign(tracks[i] - pos.Y) != Math.Sign(tracks[i] - PreviousPosition.Y))
							{
								vel.Y = DEFAULT_SPEED.Y;
								pos.Y = tracks[i];
							}
						}
					}
					else {
						if ((dirY < 0 && pos.Y + m_distortedPosition < tracks.First()) || (dirY > 0 && pos.Y + m_distortedPosition > tracks.Last())) {
							vel.Y = DEFAULT_SPEED.Y;
							pos.Y = Math.Min(MathUtils.Clamp(pos.Y + m_distortedPosition, tracks.First(), tracks.Last()) - m_distortedPosition, TOP_POSITION_BUFFER) ;
						} else {
							if(endingDescent)
								vel.Y = MOVE_VEL/2 * dirY;
							else
								vel.Y = MOVE_VEL * dirY;
						}

						if (CLOSE_TO_EPSILON > Math.Abs(TOP_POSITION_BUFFER - pos.Y) && vel.Y < 0)
						{
							vel.Y = DEFAULT_SPEED.Y;
						}


					}

					bool atLeft = Environment.Camera.Rect.Left + LEFT_POSITION_BUFFER >= Position.X;
					bool atRight = Environment.Camera.Rect.Right - RIGHT_POSITION_BUFFER <= Position.X;

					if ((atLeft && vel.X < DEFAULT_SPEED.X) || (atRight && vel.X > DEFAULT_SPEED.X))
					{
						vel.X = DEFAULT_SPEED.X;
						pos.X = MathUtils.Clamp(pos.X, Environment.Camera.Rect.Left + LEFT_POSITION_BUFFER, Environment.Camera.Rect.Right - RIGHT_POSITION_BUFFER);
					}
					else
					{
						if(endingDescent)
							vel.X = DEFAULT_SPEED.X  * dirX;
						else
							vel.X = DEFAULT_SPEED.X + MOVE_VEL * dirX;
					}

					// Distorted position.
					UpdateDistortion(elapsedTime);

					// Trigger OnTempChange.
					if (PositionToRung(pos.Y) != m_lastRung) {
						int last = m_lastRung;
						m_lastRung = PositionToRung(pos.Y);
						Environment.OnTempChange(m_lastRung);
					}

					PreviousPosition = pos; // previousPosition always reflects the real un-distorted position.
					pos.Y += m_distortedPosition;

					Position = pos;
					DesiredVelocity = vel;

					Vector2 cameraRelativeVelocity = origVel - DEFAULT_SPEED;
			
					if (cameraRelativeVelocity.Y > CLOSE_TO_EPSILON)
					{
						if (tracks.Last() > Position.Y)
						{
							anim.PlaySequence(animations[DOWN_ANIM_INDEX]);
						}
					} else if (cameraRelativeVelocity.Y < -CLOSE_TO_EPSILON)
					{
						if (tracks.First() < Position.Y)
						{
							anim.PlaySequence(animations[UP_ANIM_INDEX]);
						}
					}
					else if (cameraRelativeVelocity.X < -CLOSE_TO_EPSILON)
					{
						if (!atLeft)
						{
							anim.PlaySequence(animations[BACK_ANIM_INDEX]);
						}
					}
					else if (cameraRelativeVelocity.X > CLOSE_TO_EPSILON)
					{
						if (!atRight)
						{
							anim.PlaySequence(animations[FORWARD_ANIM_INDEX]);
						}
					}
					else
					{
						anim.PlaySequence(animations[ALIVE_ANIM_INDEX]);
					}

					lastDirX = dirX;
					lastDirY = dirY;

					if (m_distortState != DistortState.NONE)
						drippingWet.Effect.Trigger(Position);
				} break;
				
			}

			anim.Update(elapsedTime);
			Registration = originalRegistration + anim.CurrentOffset;
			Texture = anim.CurrentFrame;

			base.Update(elapsedTime);
		}

        const float threshold = 0.4f;

		private void ProcessControls(out int dirX, out int dirY) {
			KeyboardState keyState = Keyboard.GetState();
			KeyboardState oldKeyState = OldKeyboard.GetState();
			GamePadState padState = GamePad.GetState(PlayerIndex.One);
			GamePadState oldpadState = OldGamePad.GetState();

			dirX = 0;
			dirY = 0;

			if (((keyState.IsKeyDown(Keys.Up))
				|| padState.IsButtonDown(Buttons.DPadUp)
                || padState.ThumbSticks.Left.Y > threshold) 
				&& enableUp) --dirY;
			if ((keyState.IsKeyDown(Keys.Down) 
				|| padState.IsButtonDown(Buttons.DPadDown) 
                || (padState.ThumbSticks.Left.Y < -threshold)
				&& enableDown) 
				|| endingDescent ) ++dirY;

			if ((keyState.IsKeyDown(Keys.Left)
				|| padState.IsButtonDown(Buttons.DPadLeft)
				|| padState.ThumbSticks.Left.X < -threshold)
				&& enableLeft)
			{
				Environment.Camera.EffectScale = 1.0f + 2*zoomAmount;
				--dirX;
			} 

			if (((keyState.IsKeyDown(Keys.Right)
				|| padState.IsButtonDown(Buttons.DPadRight)
				|| padState.ThumbSticks.Left.X > threshold)
				&& enableRight)
				|| endingDescent)
			{
				Environment.Camera.EffectScale = 1.0f;
				++dirX;
			}
			if(!keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left))
				Environment.Camera.EffectScale = 1.0f + zoomAmount;
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
		}

		Cue rainSound;

		private void UpdateDistortion(float elapsedTime) {
			bool init = (m_lastDistortState != m_distortState);
			m_lastDistortState = m_distortState;

			m_distortedPosition += m_distortedVel * elapsedTime;

			switch (m_distortState) {
				case DistortState.NONE:
					if (init) {
						m_distortedVel = 0.0f;
						m_distortedPosition = 0.0f;
					}

					break;
				case DistortState.FALLING:
					if (init) {
						m_distortedVel = k_distortedFallVel;
					}

					if (m_distortedPosition >= k_distortAmount) {
						m_distortedPosition = k_distortAmount;
						m_distortState = DistortState.FALLEN;
					}

					break;

				case DistortState.FALLEN:
					if (init) {
						m_distortRestoreDelay = k_rainRestoreDelay;
						m_distortedVel = 0.0f;
					}
					
					m_distortRestoreDelay -= elapsedTime;
					if (m_distortRestoreDelay <= 0.0f) {
						m_distortState = DistortState.RISING;

						if (rainSound != null)
						{
							rainSound.Stop(AudioStopOptions.AsAuthored);
							rainSound = null;
						}
					}

					break;

				case DistortState.RISING:
					if (init) {
						m_distortedVel = k_distortedResetVel;
					}

					if (m_distortedPosition < 0.0f) {
						m_distortState = DistortState.NONE;
						m_distortedPosition = 0.0f;
					}
					break;
			}

			VertexColor = Color.Lerp(Color.White, Color.CornflowerBlue, MathUtils.Clamp(m_distortedPosition / k_distortAmount, 0.0f, 1.0f));
		}

		public override bool ShouldCull()
		{
			return m_dead;
		}
		

		public void Kill() {
			if (currentState == BALLOON_STATE.INVULNERABLE) return;

			if (isBalloonAlive)
			{
                currentState = BALLOON_STATE.DYING;
                if (downSound != null)
                {
                    downSound.Stop(AudioStopOptions.AsAuthored);
                    downSound = null;
                }
                if (upSound != null)
                {
                    upSound.Stop(AudioStopOptions.AsAuthored);
                    upSound = null;
                }
                if (leftSound != null)
                {
                    leftSound.Stop(AudioStopOptions.AsAuthored);
                    leftSound = null;

                }
                if (rightSound != null)
                {
                    rightSound.Stop(AudioStopOptions.AsAuthored);
                    rightSound = null;
                }
				Sound.PlayCue("pop");
				m_endExplosion.Effect.Trigger(Position);//same 
				anim.PlaySequence(animations[DEAD_ANIM_INDEX]);
				Environment.Camera.MoveSpeed = Vector2.Zero;

			}
		}
		/// <summary>
		/// Kill Balloon with no animation or sound effect to restart
		/// from a checkpoint.
		/// </summary>
        public void FastKill()
        {
            if (currentState != BALLOON_STATE.DYING)
            {
                currentState = BALLOON_STATE.DEAD;
                //Sound.StopAll();
            }
        }

		public void goToEndSequence(float delay)
		{
			descentDelay = delay;
			currentState = BALLOON_STATE.ENDING_SEQUENCE;
		}

		public void HitByRain() {
			if (m_distortState == DistortState.NONE) {
				m_distortState = DistortState.FALLING;
				rainSound = Sound.PlayCue("rain");
			} else {
				m_distortRestoreDelay = k_rainRestoreDelay;
				//if (m_distortState != DistortState.FALLING)
				//{
				//    if (rainSound != null)
				//    {
				//        rainSound.Stop(AudioStopOptions.AsAuthored);
				//        rainSound = null;
				//    }
				//}
			}
		}

		public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
		{
			// TODO: Explode!
			if (!(entB is Cloud || entB is Trigger)) { // Let cloud tell us what to do.
				Kill();
			}

			contact.Enabled = false;
			base.OnCollide(entB, contact);
		}
	}
}
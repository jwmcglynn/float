using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using ProjectMercury;



using Microsoft.Xna.Framework.Graphics;


namespace Sputnik.Game
{
	class Star : GameEntity
	{
		public float originalCameraPos;
		public float dropPostition;
		private Animation m_anim = new Animation();
		private Sequence m_blinking;
		private ParticleEntity m_magic_Trail;


		

		/***************************************************************************/

		// Debug constructor.
		public Star(GameEnvironment env)
			: base(env)
		{
			Initialize();
		}

		// Regular constructor.
		public Star(GameEnvironment env, SpawnPoint sp)
				: base(env, sp) {

			Position = sp.Position;
			Initialize();

			//Stardust = contentManager.Load<ParticleEffect>("ExplosionEffect");

			if (sp.Properties.ContainsKey("delay"))
			{
				string delay = sp.Properties["delay"];

				dropPostition = Convert.ToSingle(delay);

			}
			else
				dropPostition = 15;

			//EffectsAboveStar.("star");

		}

		private void Initialize()
		{
			m_magic_Trail = new ParticleEntity(Environment, "starTrail");
			m_magic_Trail.Zindex = 0.9f;
			AddChild(m_magic_Trail);

			originalCameraPos = Environment.Camera.Position.X; 
			float blinkOffTime = 0.5f;
			float blinkOnTime = 0.25f;

			m_blinking = new Sequence(Environment.contentManager);
			m_blinking.AddFrame("Star\\NStar1", blinkOffTime);
			m_blinking.AddFrame("Star\\NStar2", blinkOnTime);
			m_blinking.AddFrame("Star\\NStar3", blinkOffTime);
			m_blinking.AddFrame("Star\\NStar4", blinkOnTime);
			m_blinking.AddFrame("Star\\NStar5", blinkOffTime);
			m_blinking.AddFrame("Star\\NStar6", blinkOnTime);
			//Again
			m_blinking.AddFrame("Star\\NStar1", blinkOffTime);
			m_blinking.AddFrame("Star\\NStar2", blinkOnTime);
			m_blinking.AddFrame("Star\\NStar3", blinkOffTime);
			m_blinking.AddFrame("Star\\NStar4", blinkOnTime);
			m_blinking.AddFrame("Star\\NStar5", blinkOffTime);
			m_blinking.AddFrame("Star\\NStar6", blinkOnTime);

			Registration = new Vector2(75, 75);

			Zindex = ZSettings.SkyStar;
		}

		/***************************************************************************/

		enum State {
			Wait,
			Strobing,
			Falling
		};

		State m_curState = State.Wait;

		public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}

		public override void Update(float elapsedTime)
		{
			//float counter = 3.0f;// switchs between the differnt  star shapes
			switch (m_curState)
			{
				case State.Wait:
					// TODO: Wait here.

					if (Environment.Camera.Position.X - originalCameraPos > dropPostition)
					{ // TODO: Condition for next statetswitch here.
						m_curState = State.Strobing;
						m_anim.PlaySequence(m_blinking);
					}
					break;

				case State.Strobing:
					m_anim.Update(elapsedTime);
					Texture = m_anim.CurrentFrame;
					if (m_anim.Done)
					{
						m_curState = State.Falling;
						CreateCollisionBody(Environment.CollisionWorld, BodyType.Dynamic, CollisionFlags.FixedRotation);
						AddCollisionRectangle(new Vector2(14.0f, 80.0f), new Vector2(0.0f, -80.0f));


						LoadTexture(Environment.contentManager, "star_imagefalling");
						Scale = 1.0f;
						Zindex = ZSettings.FallingStar;
						Registration = new Vector2(198, 230);

						DesiredVelocity = new Vector2(0.0f, 450.0f);
					}	
					

					break;

				case State.Falling:
				//	m_magic_Trail.Effect.Trigger(Position);//; + new Vector2(110.0f, -5.0f));// particle effects postion
					m_magic_Trail.Effect.Trigger( new Vector2(Position.X, Position.Y-38));// particle effects postion	
				break;
				}


			base.Update(elapsedTime);
		}


	

		

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture)
		{

			return (entB is Environment || entB is Balloon);
		
			
		}
		public override void  OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
{
	if (entB is Environment)
		OnNextUpdate += () => Dispose();

	contact.Enabled = false;

 	 base.OnCollide(entB, contact);


}
		
		
		}

	}


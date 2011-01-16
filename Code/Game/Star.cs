﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

namespace Sputnik.Game
{
	class Star : GameEntity
	{
		public float originalCameraPos;
		public float dropPostition;
		private Animation m_anim = new Animation();
		private Sequence m_blinking;

		/***************************************************************************/

		// Debug constructor.
		public Star(GameEnvironment env)
			: base(env)
		{
			Initialize();
		}

		// Regular constructor.
		public Star(GameEnvironment env, SpawnPoint sp)
			: base(env, sp)
		{
			Initialize();
			Position = sp.Position;
			if (sp.Properties.ContainsKey("delay"))
			{
				string delay = sp.Properties["delay"];

				dropPostition = Convert.ToSingle(delay);

			}
			else
				dropPostition = 15;

			
		}

		private void Initialize()
		{
			originalCameraPos = Environment.Camera.Position.X; 
			float blinkOffTime = 1.0f;
			float blinkOnTime = 0.25f;

			m_blinking = new Sequence(Environment.contentManager);
			m_blinking.AddFrame("StarNormal-1", blinkOffTime);
			m_blinking.AddFrame("StarNormal-2", blinkOnTime);
			m_blinking.AddFrame("StarNormal-1", blinkOffTime);
			m_blinking.AddFrame("StarNormal-2", blinkOnTime);
			m_blinking.AddFrame("StarNormal-1", blinkOffTime);
			m_blinking.AddFrame("StarNormal-3", blinkOnTime);

			Registration = new Vector2(198, 230);
			Scale = 0.25f; // Reduce scale for temp art which is HUGE.
		}

		/***************************************************************************/

		enum State {
			Wait,
			Strobing,
			Falling
		};

		State m_curState = State.Wait;
		

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
						AddCollisionCircle(14, Vector2.Zero);
						
						
						LoadTexture(Environment.contentManager, "star_imagefalling");
						Scale = 1.0f;

						DesiredVelocity = new Vector2(0.0f, 200f);
					}

					break;

				case State.Falling:
					// TODO: Jeremy, emit particle effect here.
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

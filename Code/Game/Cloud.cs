using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sputnik.Game
{
    enum CLOUD_STATE {
        NORM, THUNDER, RAIN, HAIL, LIGHTNING, TRANSITION
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
		private CLOUD_STATE transitionState;
		private CLOUD_STATE transitionTarget;
		private Fixture m_rainFixture;

		private int m_neutralRung = 5;

		private Animation m_anim = new Animation();

		Sequence m_hail;
		Sequence m_rain;
		Sequence m_norm;
		Sequence m_thunder;
		Sequence m_lightning;


		Sequence m_h2r;
		Sequence m_r2n;
		Sequence m_n2t;
		Sequence m_t2l;

		Sequence m_r2h;
		Sequence m_n2r;
		Sequence m_t2n;
		Sequence m_l2t;
	
		
		public Cloud(GameEnvironment env) : base(env) {
			Initialize();
		}

		public Cloud(GameEnvironment env, SpawnPoint sp) : base(env, sp) {
            if (!sp.Properties.ContainsKey("neutral") || !int.TryParse(sp.Properties["neutral"], out m_neutralRung)) {
                m_neutralRung = 5; // Default value;
            }

			Position = sp.Position;
			Initialize();
		}
		
		public override void OnTempChange(float amount) {
			UpdateState((int) amount);

			base.OnTempChange(amount);
		}

		public void UpdateState(int temp) {
			// temp is 0 to 10.

			double d = (temp - m_neutralRung) / 2.0f;

			int diff = (int) Math.Ceiling((temp - m_neutralRung) / 2.0f); // Two temperatures per state.
			if (diff < -2) diff = -2;
			if (diff > 2) diff = 2;
			diff += 2; // Change from -2 -> 2 to 0 -> 4.

            switch (diff) {
                case 0:
                    transitionTarget = CLOUD_STATE.LIGHTNING;
                    break;
                case 1:
                    transitionTarget = CLOUD_STATE.THUNDER;
                    break;
                case 2:
                    transitionTarget = CLOUD_STATE.NORM;
                    break;
                case 3:
                    transitionTarget = CLOUD_STATE.RAIN;
                    break;
                case 4:
                    transitionTarget = CLOUD_STATE.HAIL;
                    break;
            }

			if (transitionTarget == currentState) { return; }
			CLOUD_STATE lastState = currentState;
			currentState = CLOUD_STATE.TRANSITION;
			switch (lastState)
			{
				case CLOUD_STATE.HAIL:
					m_anim.PlaySequence(m_h2r);
					break;
				case CLOUD_STATE.RAIN:
					if (transitionTarget == CLOUD_STATE.HAIL)
					{
						m_anim.PlaySequence(m_r2h);
					}
					else //transitionTarget == CLOUD_STATE.NORM
					{
						m_anim.PlaySequence(m_r2n);
					}
					break;
				case CLOUD_STATE.NORM:
					if (transitionTarget == CLOUD_STATE.RAIN)
					{
						m_anim.PlaySequence(m_n2r);
					}
					else //transitionTarget == CLOUD_STATE.NORM
					{
						m_anim.PlaySequence(m_n2t);
					}
					break;
				case CLOUD_STATE.THUNDER:
					if (transitionTarget == CLOUD_STATE.NORM)
					{
						m_anim.PlaySequence(m_t2n);
					}
					else //transitionTarget == CLOUD_STATE.NORM
					{
						m_anim.PlaySequence(m_t2l);
					}
					break;
				case CLOUD_STATE.LIGHTNING:
					m_anim.PlaySequence(m_l2t);
					break;
			}
        }
        private void Initialize()
        {
			float frameDelay = 0.2f;

			// Hail.
			m_hail = new Sequence(Environment.contentManager);
			m_hail.AddFrame("cloud\\CloudX3", float.PositiveInfinity);
			m_hail.Loop = true;

			// Rain.
			m_rain = new Sequence(Environment.contentManager);
			m_rain.AddFrame("cloud\\CloudX2", float.PositiveInfinity);

			m_rain.Loop = true;

			// Normal.
			m_norm = new Sequence(Environment.contentManager);
			m_norm.AddFrame("cloud\\CloudX1", float.PositiveInfinity);

			// Thunder.
			m_thunder = new Sequence(Environment.contentManager);
			m_thunder.AddFrame("cloud\\thunder1", frameDelay);
			m_thunder.AddFrame("cloud\\thunder2", frameDelay);
			m_thunder.AddFrame("cloud\\thunder3", frameDelay);
			m_thunder.Loop = true;

			// Lightning.
			m_lightning = new Sequence(Environment.contentManager);
			m_lightning.AddFrame("cloud\\Lighting1", frameDelay);
			m_lightning.AddFrame("cloud\\Lighting2", frameDelay);
			m_lightning.AddFrame("cloud\\Lighting3", frameDelay);
			m_lightning.Loop = true;

			/* TODO: INITIALIZE */
			m_h2r = new Sequence(Environment.contentManager);
			m_h2r.AddFrame("cloud\\SemiToBad3", frameDelay);
			m_h2r.AddFrame("cloud\\SemiToBad2", frameDelay);
			m_h2r.AddFrame("cloud\\SemiToBad1", frameDelay);
			m_h2r.Loop = false;


			m_r2n = new Sequence(Environment.contentManager);
			m_r2n.AddFrame("cloud\\NormSemi3", frameDelay);
			m_r2n.AddFrame("cloud\\NormSemi2", frameDelay);
			m_r2n.AddFrame("cloud\\NormSemi1", frameDelay);
			m_r2n.Loop = false;


			m_n2t = new Sequence(Environment.contentManager);
			m_n2t.AddFrame("cloud\\NormSemi1", frameDelay);
			m_n2t.AddFrame("cloud\\NormSemi2", frameDelay);
			m_n2t.AddFrame("cloud\\NormSemi3", frameDelay);
			m_n2t.Loop = false;


			m_t2l = new Sequence(Environment.contentManager);
			m_t2l.AddFrame("cloud\\SemiToBad1", frameDelay);
			m_t2l.AddFrame("cloud\\SemiToBad2", frameDelay);
			m_t2l.AddFrame("cloud\\SemiToBad3", frameDelay);
			m_t2l.Loop = false;




			m_r2h = new Sequence(Environment.contentManager);
			
			m_r2h.AddFrame("cloud\\SemiToBad1", frameDelay);
			m_r2h.AddFrame("cloud\\SemiToBad2", frameDelay);
			m_r2h.AddFrame("cloud\\SemiToBad3", frameDelay);
			m_r2h.Loop = false;


			m_n2r = new Sequence(Environment.contentManager);
			m_n2r.AddFrame("cloud\\NormSemi1", frameDelay);
			m_n2r.AddFrame("cloud\\NormSemi2", frameDelay);
			m_n2r.AddFrame("cloud\\NormSemi3", frameDelay);
			m_n2r.Loop = false;


			m_t2n = new Sequence(Environment.contentManager);
			m_t2n.AddFrame("cloud\\NormSemi3", frameDelay);
			m_t2n.AddFrame("cloud\\NormSemi2", frameDelay);
			m_t2n.AddFrame("cloud\\NormSemi1", frameDelay);
			m_t2n.Loop = false;


			m_l2t = new Sequence(Environment.contentManager);
			m_l2t.AddFrame("cloud\\SemiToBad3", frameDelay);
			m_l2t.AddFrame("cloud\\SemiToBad2", frameDelay);
			m_l2t.AddFrame("cloud\\SemiToBad1", frameDelay);
			m_l2t.Loop = false;
			




			UpdateState((int) Environment.Temperature);

            Registration = new Vector2(295.0f, 236.0f);

            CreateCollisionBody(Environment.CollisionWorld, BodyType.Kinematic, CollisionFlags.FixedRotation);

			Vector2 topleft = new Vector2(168.0f, 290.0f) - Registration;
			Vector2 bottomright = new Vector2(431.0f, 1016.0f) - Registration;
            m_rainFixture = AddCollisionRectangle((bottomright - topleft) * 0.5f, (topleft + bottomright) * 0.5f); 

			Zindex = ZSettings.Cloud;

			
			transitionState = CLOUD_STATE.NORM;
			transitionTarget = CLOUD_STATE.NORM;

            SnapToRung();
            //300 x 240
			AddCollisionCircle(130.0f, new Vector2(80.0f, 0.0f));
			AddCollisionCircle(130.0f, new Vector2(0.0f, 0.0f));
			AddCollisionCircle(130.0f, new Vector2(-80.0f, 0.0f));
        }

        public CLOUD_STATE StateOfCloud
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
			if (currentState == CLOUD_STATE.TRANSITION && m_anim.Done)
			{
				currentState = transitionTarget;
				switch (currentState)
				{
					case CLOUD_STATE.LIGHTNING:
						m_anim.PlaySequence(m_lightning);
						break;
					case CLOUD_STATE.THUNDER:
						m_anim.PlaySequence(m_thunder);
						break;
					case CLOUD_STATE.NORM:
						m_anim.PlaySequence(m_norm);
						break;
					case CLOUD_STATE.RAIN:
						m_anim.PlaySequence(m_rain);
						break;
					case CLOUD_STATE.HAIL:
						m_anim.PlaySequence(m_hail);
						break;
				}
			}

			m_anim.Update(elapsedTime);
			Texture = m_anim.CurrentFrame;

			base.Update(elapsedTime);
		}

		public override bool ShouldCollide(Entity entB, Fixture fixture, Fixture entBFixture) {
			return (entB is Balloon);
		}

        public override void OnCollide(Entity entB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (entB is Balloon)
            {
				Balloon b = (Balloon) entB;

				bool hitRain = (contact.FixtureA == m_rainFixture || contact.FixtureB == m_rainFixture);

                switch(currentState) {
					case CLOUD_STATE.LIGHTNING:
						if (!hitRain) OnNextUpdate += () => b.Kill();
                        Sound.PlayCue("thunder");
						break;
                    case CLOUD_STATE.HAIL:
						if (hitRain) OnNextUpdate += () => b.Kill();
						break;
                    case CLOUD_STATE.RAIN:
						if (hitRain) OnNextUpdate += () => b.HitByRain();
						break;
                }
			}

			contact.Enabled = false;
			base.OnCollide(entB, contact);
        }
    }
}

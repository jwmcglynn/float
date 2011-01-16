using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sputnik.Game;

namespace Sputnik.Menus
{
    class MainMenu : Menu
    {
        private Widget m_background; 

        private List<Widget> m_mainMenu = new List<Widget>();
        private List<Widget> m_difficultySelect = new List<Widget>();
        private List<Widget> m_credits = new List<Widget>();

		private List<Widget> m_instruction = new List<Widget>();

        private class TextButton : TextWidget   //changes when you higlght something 
        {
            public TextButton(Menu menu, string text)
                : base(menu, "font", text)
            {
                OnMouseOver += () => { VertexColor = Color.Aquamarine; };
                OnMouseOut += () => { VertexColor = Color.White; };
                OnMouseDown += () => { VertexColor = Color.Blue; };
            }
        }

        private class ImageButton : Widget
        {
            private Texture2D m_normal;
            private Texture2D m_over;
            public ImageButton(Menu menu, string normal, string over)
                : base(menu)
            {

                Texture = m_normal = menu.contentManager.Load<Texture2D>(normal);
                m_over = menu.contentManager.Load<Texture2D>(over);

                OnMouseOver += () => { Texture = m_over; };
                OnMouseOut += () => { Texture = m_normal; VertexColor = Color.White; };
                OnMouseDown += () => { Texture = m_over; VertexColor = Color.Gray; };
                OnMouseUp += () => { Texture = m_normal; };
            }
        }

        private void CreateButton(Widget widget, int height = 70)
        {
            widget.Registration = new Vector2(widget.Texture.Width * 0.5f, widget.Texture.Height * 0.5f);

            int width = (int)Math.Round((double)widget.Texture.Width);
            widget.CreateButton(new Rectangle(0, 0, width, height));

            widget.Zindex = 0.5f;
        }

        private Widget CreateSprite(string tex)
        {
            Widget widget = new Widget(this);

            widget.LoadTexture(contentManager, tex);
            widget.Registration = new Vector2(widget.Texture.Width * 0.5f, widget.Texture.Height * 0.5f);
            widget.Zindex = 0.5f;

            return widget;
        }

        public MainMenu(Controller ctrl)
            : base(ctrl)
        {

         //   Sound.StopAll(true);
         //   Sound.PlayCue("main_theme");
            Controller.IsMouseVisible = true;

            // Background.    
            m_background = new Widget(this);
            m_background.LoadTexture(contentManager, "sky-desktop");
            m_background.PositionPercent = new Vector2(0.5f, 0.5f);
            m_background.Zindex = 1.0f;
            m_background.Registration = new Vector2(m_background.Texture.Width, m_background.Texture.Height) * 0.5f;
            AddChild(m_background);

        

			Widget credit_picture = new Widget(this);
			credit_picture.LoadTexture(contentManager, "credits_image");
			credit_picture.PositionPercent = new Vector2(0.5f, 0.3f);
			credit_picture.Position = new Vector2(-75.0f, 0.0f);
			credit_picture.Zindex = 0.8f;
			credit_picture.Registration = new Vector2(375.0f, 230.0f);
			credit_picture.Visible = false;
			m_credits.Add(credit_picture);
			AddChild(credit_picture);

			  Widget instruction_picture = new Widget(this);
			  instruction_picture.LoadTexture(contentManager, "instructions_image");
			  instruction_picture.PositionPercent = new Vector2(0.5f, 0.3f);
			  instruction_picture.Position = new Vector2(-75.0f, 0.0f);
			  instruction_picture.Zindex = 0.8f;
			  instruction_picture.Registration = new Vector2(375.0f, 230.0f);
			  instruction_picture.Visible = false;
			  m_instruction.Add(instruction_picture);
			  AddChild(instruction_picture);
				  

            Vector2 k_buttonPos = new Vector2(0.5f, 0.5f);
            const float k_buttonSpacing = 60.0f;
            float ypos = 50.0f;

            ImageButton startLevel = new ImageButton(this, "main_start_level", "main_start_level");
            startLevel.PositionPercent = k_buttonPos;
            startLevel.Position = new Vector2(0.0f, ypos);
            CreateButton(startLevel);
            startLevel.OnActivate += () =>
            {
				GymEnvironment env = new GymEnvironment(Controller); /// change here also to load level one 
				
				            Controller.ChangeEnvironment(env);
            };
            m_mainMenu.Add(startLevel);
            AddChild(startLevel);

            ypos += k_buttonSpacing;

            ImageButton credits = new ImageButton(this, "main_credits", "main_credits");
            credits.PositionPercent = k_buttonPos;
            credits.Position = new Vector2(0.0f, ypos);
            CreateButton(credits);
            credits.OnActivate += () =>
            {
                ShowCredits();
            };
            m_mainMenu.Add(credits);
            AddChild(credits);

            ypos += k_buttonSpacing;

            ImageButton quit = new ImageButton(this, "main_quit", "main_quit");
            quit.PositionPercent = k_buttonPos;
            quit.Position = new Vector2(0.0f, ypos);
            CreateButton(quit);
            quit.OnActivate += () =>
            {
                Controller.Exit();
            };
            m_mainMenu.Add(quit);
            AddChild(quit);



			ypos += k_buttonSpacing;

			ImageButton instruction = new ImageButton(this, "MenuInstructions", "MenuInstructions");
			instruction.PositionPercent = k_buttonPos;
			instruction.Position = new Vector2(0.0f, ypos);
			CreateButton(instruction);
			instruction.OnActivate += () =>
			{
				ShowInstruction();
			};
			m_mainMenu.Add(instruction);
			AddChild(instruction);

            ////////////////////////////////////////////////////////////////////////////
            ///// Difficulty select.

     //      Widget sprite = CreateSprite("difficulty_bg");
    //        sprite.Visible = false;
   ////         sprite.PositionPercent = new Vector2(0.5f, 0.5f);
    //        sprite.Zindex = 0.2f;
   //         m_difficultySelect.Add(sprite);
   //         AddChild(sprite);

   ///         sprite = CreateSprite("choose_difficulty");
   //         sprite.Visible = false;
  //          sprite.PositionPercent = new Vector2(0.5f, 0.5f);
 //           sprite.Zindex = 0.1f;
//            m_difficultySelect.Add(sprite);
 //           AddChild(sprite);

   //         ImageButton diffNormal = new ImageButton(this, "diff_normal", "diff_normal1");
  //          CreateButton(diffNormal, 50);
  //          diffNormal.Visible = false;
 //           diffNormal.PositionPercent = new Vector2(0.5f, 0.5f);
 //           diffNormal.Position = new Vector2(0.0f, -20.0f);
 //           diffNormal.Zindex = 0.1f;
       //     diffNormal.OnActivate += () =>
  //          {
  //              Controller.ChangeEnvironment(new GymEnvironment(Controller)); /// this is where you load level one
 //           };
  //          m_difficultySelect.Add(diffNormal);
 //           AddChild(diffNormal);


           // ImageButton diffFrost = new ImageButton(this, "diff_frost", "diff_frost1");
         //   CreateButton(diffFrost, 50);
     //       diffFrost.Visible = false;
    //        diffFrost.PositionPercent = new Vector2(0.5f, 0.5f);
    //        diffFrost.Position = new Vector2(0.0f, 40.0f);
  //          diffFrost.Zindex = 0.1f;
  ////          diffFrost.OnActivate += () =>
  //          {
  //              GymEnvironment env = new GymEnvironment(Controller); /// change here also to load level one 
  //             // env.isFrostMode = true;
  //              Controller.ChangeEnvironment(env);
  //          };
     //       m_difficultySelect.Add(diffFrost);
        //    AddChild(diffFrost);

            ////////////////////////////////////////////////////////////////////////////
            ///// Create credits.

      //    sprite = CreateSprite("credits");
      //     sprite.Visible = false;
      //     sprite.PositionPercent = new Vector2(0.5f, 0.5f);
      //     m_credits.Add(sprite);
      //      AddChild(sprite);

            // TeamQ badge.
  //          sprite = CreateSprite("teamq");
  //          sprite.PositionPercent = new Vector2(0.5f, 0.225f);
 //           sprite.Visible = false;
 //           m_credits.Add(sprite);
//            AddChild(sprite);

   //         sprite = CreateSprite("programmers");
  //          sprite.Visible = false;
 //           sprite.PositionPercent = new Vector2(0.5f, 0.5f);
 //           m_credits.Add(sprite);
  //          AddChild(sprite);

  //          sprite = CreateSprite("programmer_names");
  //          sprite.Visible = false;
  //          sprite.PositionPercent = new Vector2(0.5f, 0.5f);
  //          m_credits.Add(sprite);
  //          AddChild(sprite);

   //         sprite = CreateSprite("art_sound");
  //          sprite.Visible = false;
  //          sprite.PositionPercent = new Vector2(0.5f, 0.5f);
  //          m_credits.Add(sprite);
   //         AddChild(sprite);

    //        sprite = CreateSprite("kayu");
  //          sprite.Visible = false;
 //           sprite.PositionPercent = new Vector2(0.5f, 0.5f);
 //           m_credits.Add(sprite);
//            AddChild(sprite);

  //          sprite = CreateSprite("special_thanks");
 //           sprite.Visible = false;
 //           sprite.PositionPercent = new Vector2(0.5f, 0.5f);
 //           m_credits.Add(sprite);
 //           AddChild(sprite);

  //          sprite = CreateSprite("special");
 //           sprite.Visible = false;
  //          sprite.PositionPercent = new Vector2(0.5f, 0.5f);
  //          m_credits.Add(sprite);
  //          AddChild(sprite);

            // Credits return button.
            Widget creditsReturn = new Widget(this);
           creditsReturn.Visible = false;
            creditsReturn.CreateButton(new Rectangle(0, 0, 5000, 5000));
            creditsReturn.Zindex = 0.1f;
            creditsReturn.PositionPercent = new Vector2(0.0f, 0.0f);
            creditsReturn.OnActivate += () =>
            {
                HideCredits();
            };
            m_credits.Add(creditsReturn);
            AddChild(creditsReturn);

			//instruction return button 

			Widget instructionReturn = new Widget(this);
			instructionReturn.Visible = false;
			instructionReturn.CreateButton(new Rectangle(0, 0, 5000, 5000));
			instructionReturn.Zindex = 0.1f;
			instructionReturn.PositionPercent = new Vector2(0.0f, 0.0f);
			instructionReturn.OnActivate += () =>
			{
				HideInstruction();
			};
			m_instruction.Add(instructionReturn);
			AddChild(instructionReturn);
        }

        public void ShowCredits()
        {
            m_mainMenu.ForEach(w =>
            {
                w.Visible = false;
            });

            m_credits.ForEach(w =>
            {
                w.Visible = true;
            });
        }

        public void HideCredits()
        {
            m_mainMenu.ForEach(w =>
            {
                w.Visible = true;
            });

            m_credits.ForEach(w =>
            {
                w.Visible = false;
            });
        }

		public void ShowInstruction()
		{
			m_mainMenu.ForEach(w =>     ///what is this for? 
			{
				w.Visible = false;
			});

			m_instruction.ForEach(w =>
			{
				w.Visible = true;
			});
		
		}


		public void HideInstruction()
		{
			m_mainMenu.ForEach(w =>
			{
				w.Visible = true;
			});

			m_instruction.ForEach(w =>
			{
				w.Visible = false;
			});
		}

        public void ShowDifficulty()
        {
            m_mainMenu.ForEach(w =>
            {
                w.Visible = false;
            });

            m_credits.ForEach(w =>
            {
                w.Visible = false;
            });

            m_background.Visible = true;

            m_difficultySelect.ForEach(w =>
            {
                w.Visible = true;
            });

        }

        public void HideDifficulty()
        {
            m_mainMenu.ForEach(w =>
            {
                w.Visible = true;
            });

            m_difficultySelect.ForEach(w =>
            {
                w.Visible = false;
            });

            m_credits.ForEach(w =>
            {
                w.Visible = false;
            });
        }

        public override void Update(float elapsedTime)
        {
            Controller.IsMouseVisible = true;

            base.Update(elapsedTime);
        }

        public override void Dispose()
        {
            Controller.IsMouseVisible = false;
            base.Dispose();
        }
    }
}
